/* === CUSTOMER COMMON JS - Səbət, Hesab, Chat === */
var API_BASE = '/api';
var urlParams = new URLSearchParams(window.location.search);
var tableId = urlParams.get('tableId');
var isInRestaurant = !!tableId;
var cart = JSON.parse(sessionStorage.getItem('cart') || '[]');

// Səbət funksiyaları
function addToCart(productId, name, price, image) {
    var existing = cart.find(function (item) { return item.productId === productId; });
    if (existing) { if (existing.quantity >= 50) { alert('Maksimum 50 ədəd.'); return; } existing.quantity++; }
    else { if (cart.length >= 20) { alert('Səbətdə maksimum 20 fərqli məhsul ola bilər.'); return; } cart.push({ productId: productId, name: name, price: price, image: image, quantity: 1 }); }
    saveCart(); updateCartUI();
}
function removeFromCart(productId) { cart = cart.filter(function (item) { return item.productId !== productId; }); saveCart(); updateCartUI(); }
function saveCart() { sessionStorage.setItem('cart', JSON.stringify(cart)); }

function updateCartUI() {
    var totalItems = cart.reduce(function (sum, item) { return sum + item.quantity; }, 0);
    var totalPrice = cart.reduce(function (sum, item) { return sum + (item.price * item.quantity); }, 0);
    var cartCountEl = document.getElementById('cart-count');
    var orderBarCountEl = document.getElementById('order-bar-count');
    if (cartCountEl) cartCountEl.textContent = totalItems;
    if (orderBarCountEl) orderBarCountEl.textContent = totalItems;
    var totalText = '₼' + totalPrice.toFixed(2);
    var minicartTotalEl = document.getElementById('minicart-total');
    var orderBarTotalEl = document.getElementById('order-bar-total');
    if (minicartTotalEl) minicartTotalEl.innerHTML = totalText;
    if (orderBarTotalEl) orderBarTotalEl.innerHTML = totalText;

    var minicartItems = document.getElementById('minicart-items');
    if (minicartItems) {
        minicartItems.innerHTML = '';
        cart.forEach(function (item) {
            minicartItems.innerHTML +=
                '<li class="woocommerce-mini-cart-item mini_cart_item" style="display:flex;align-items:center;justify-content:space-between;padding:10px 0;border-bottom:1px solid #f0f0f0;">' +
                '  <div style="display:flex;align-items:center;gap:10px;flex:1;">' +
                '    <img src="' + item.image + '" style="width:50px;height:50px;object-fit:cover;border-radius:8px;" alt="' + item.name + '">' +
                '    <div>' +
                '      <div style="font-weight:600;font-size:14px;">' + item.name + '</div>' +
                '      <div style="color:#c8a97e;font-size:13px;">₼' + item.price.toFixed(2) + '</div>' +
                '    </div>' +
                '  </div>' +
                '  <div style="display:flex;align-items:center;gap:8px;">' +
                '    <button class="cart-qty-btn" data-id="' + item.productId + '" data-action="minus" style="width:28px;height:28px;border-radius:50%;border:1px solid #e0e0e0;background:#fff;cursor:pointer;font-size:16px;display:flex;align-items:center;justify-content:center;">−</button>' +
                '    <span style="font-weight:700;min-width:20px;text-align:center;">' + item.quantity + '</span>' +
                '    <button class="cart-qty-btn" data-id="' + item.productId + '" data-action="plus" style="width:28px;height:28px;border-radius:50%;border:1px solid #c8a97e;background:#c8a97e;color:#fff;cursor:pointer;font-size:16px;display:flex;align-items:center;justify-content:center;">+</button>' +
                '    <button class="remove-cart-item" data-id="' + item.productId + '" style="width:28px;height:28px;border-radius:50%;border:1px solid #e74c3c;background:#fff;color:#e74c3c;cursor:pointer;font-size:14px;display:flex;align-items:center;justify-content:center;margin-left:5px;">✕</button>' +
                '  </div>' +
                '</li>';
        });
    }

    var orderBar = document.getElementById('order-bar');
    if (orderBar) {
        if (isInRestaurant && totalItems > 0) { orderBar.style.display = 'flex'; }
        if (totalItems === 0) { orderBar.style.display = 'none'; }
    }
}

// Hesab
var billOrders = [];
async function loadBill() {
    if (!tableId) return;
    try {
        var res = await fetch(API_BASE + '/Orders/table/' + tableId); var data = await res.json();
        if (!data.success || !data.data) return;
        billOrders = data.data.filter(function (o) { return o.status !== 5 && o.status !== 4; });
        var billItems = document.getElementById('bill-items');
        if (!billItems) return;
        billItems.innerHTML = '';
        var total = 0; var itemCount = 0;
        billOrders.forEach(function (order) {
            var st = order.status === 0 ? 'Gozleyir' : order.status === 1 ? 'Hazirlanir' : order.status === 2 ? 'Hazirdir' : 'Getirildi';
            billItems.innerHTML += '<div style="background:#f8f8f8;padding:8px 12px;margin:5px 0;border-radius:8px;font-size:12px;font-weight:600;display:flex;justify-content:space-between;"><span>#' + order.orderNumber + '</span><span style="color:#c8a97e;">' + st + '</span></div>';
            var items = order.orderItems || order.items || [];
            items.forEach(function (item) {
                var name = item.productName || (item.product ? item.product.name : 'Mehsul');
                var price = (item.unitPrice || 0) * (item.quantity || 1); total += price; itemCount += item.quantity || 1;
                billItems.innerHTML += '<div style="display:flex;justify-content:space-between;padding:5px 12px;font-size:13px;"><span>' + item.quantity + 'x ' + name + '</span><span style="font-weight:600;">₼' + price.toFixed(2) + '</span></div>';
            });
        });
        var billCountEl = document.getElementById('bill-count');
        var billTotalEl = document.getElementById('bill-total');
        if (billCountEl) billCountEl.textContent = itemCount;
        if (billTotalEl) billTotalEl.textContent = '₼' + total.toFixed(2);
        if (billOrders.length === 0) { billItems.innerHTML = '<p style="text-align:center;color:#999;padding:20px;">Aktiv sifaris yoxdur</p>'; }
    } catch (err) { console.error(err); }
}

// Chat
var chatConnection = null;
var orderConnection = null;
var unreadCount = 0;

function initSignalR() {
    if (!tableId || typeof signalR === 'undefined') return;

    orderConnection = new signalR.HubConnectionBuilder().withUrl("/hubs/order").withAutomaticReconnect().build();
    chatConnection = new signalR.HubConnectionBuilder().withUrl("/hubs/chat").withAutomaticReconnect().build();

    orderConnection.on("OrderStatusChanged", function (orderNumber, status) {
        var div = document.createElement("div");
        div.style.cssText = "position:fixed;top:20px;right:20px;background:#1cc88a;color:#fff;padding:15px 25px;border-radius:10px;z-index:9999;font-weight:600;box-shadow:0 5px 15px rgba(0,0,0,0.2);";
        div.textContent = "Sifaris #" + orderNumber + " - " + status;
        document.body.appendChild(div);
        setTimeout(function () { div.remove(); }, 5000);
        loadBill();
    });

    // Masa bağlandı — ödəniş tamamlandı, müştərini çıxart
    orderConnection.on("TableClosed", function (message) {
        sessionStorage.removeItem('cart');
        window.location.href = 'payment-success.html?tableId=' + tableId;
    });

    chatConnection.on("ReceiveMessage", function (tId, message, isFromCustomer) {
        if (!isFromCustomer) {
            var msgsDiv = document.getElementById('chat-msgs');
            if (!msgsDiv) return;
            if (msgsDiv.querySelector('p')) msgsDiv.innerHTML = '';
            msgsDiv.innerHTML += '<div style="max-width:80%;padding:10px 15px;border-radius:15px;background:#e8f4f8;color:#2d3436;align-self:flex-start;border-bottom-left-radius:5px;font-size:13px;"><div>' + message + '</div><div style="font-size:10px;opacity:0.7;margin-top:3px;">' + new Date().toLocaleTimeString('az', { hour: '2-digit', minute: '2-digit' }) + '</div></div>';
            msgsDiv.scrollTop = msgsDiv.scrollHeight;
            if ($('#chat-window').is(':hidden')) { unreadCount++; $('#chat-badge').text(unreadCount).show(); }
        }
    });

    orderConnection.start().then(function () { orderConnection.invoke("JoinTable", tableId); }).catch(function (err) { console.log("OrderHub error:", err); });
    chatConnection.start().then(function () { chatConnection.invoke("JoinTableChat", tableId); }).catch(function (err) { console.log("ChatHub error:", err); });
}

function sendChatMsg() {
    var input = document.getElementById('chat-msg-input');
    var text = input.value.trim();
    if (!text || !tableId || !chatConnection) return;
    chatConnection.invoke("SendMessageToWaiter", tableId, text).then(function () {
        var msgsDiv = document.getElementById('chat-msgs');
        if (msgsDiv.querySelector('p')) msgsDiv.innerHTML = '';
        msgsDiv.innerHTML += '<div style="max-width:80%;padding:10px 15px;border-radius:15px;background:#c8a97e;color:#fff;align-self:flex-end;border-bottom-right-radius:5px;font-size:13px;"><div>' + text + '</div><div style="font-size:10px;opacity:0.7;margin-top:3px;">' + new Date().toLocaleTimeString('az', { hour: '2-digit', minute: '2-digit' }) + '</div></div>';
        msgsDiv.scrollTop = msgsDiv.scrollHeight;
        input.value = '';
    }).catch(function () { alert('Mesaj gonderilemedi'); });
}

// Sifariş ver (menyu səhifəsindən)
async function submitOrder() {
    if (cart.length === 0) { alert('Səbətiniz boşdur!'); return; }
    if (!tableId) { alert('Masa tapılmadı.'); return; }
    try {
        for (var i = 0; i < cart.length; i++) {
            await fetch(API_BASE + '/Basket', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ tableId: tableId, productId: cart[i].productId, quantity: cart[i].quantity }) });
        }
        var orderRes = await fetch(API_BASE + '/Orders', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ tableId: tableId }) });
        var orderData = await orderRes.json();
        if (orderData.success) {
            cart = []; saveCart(); updateCartUI();
            $('.tst-minicart-window').removeClass('tst-active');
            window.location.href = 'order-status.html?orderId=' + orderData.data.id + '&tableId=' + tableId;
        } else { alert(orderData.message || 'Xəta baş verdi.'); }
    } catch (err) { console.error(err); alert('Server xətası.'); }
}

// İnit - bütün səhifələrdə çağırılır
function initCustomerPage() {
    if (isInRestaurant) {
        $('#minicart-container').show();
        $('#nav-res-btn').hide();
        $('.tst-res-btn').hide();
        $('.tst-popup-bg').remove();
        var orderBar = document.getElementById('order-bar');
        if (orderBar) orderBar.style.display = 'flex';
        $('nav ul').html('<li><a href="index.html?tableId=' + tableId + '">Ana Səhifə</a></li><li><a href="menu.html?tableId=' + tableId + '">Menyu</a></li><li><a href="about.html?tableId=' + tableId + '">Haqqımızda</a></li><li><a href="contact.html?tableId=' + tableId + '">Əlaqə</a></li>');

        // Səhifədəki bütün daxili linkləri tableId ilə yenilə
        $('a[href]').each(function () {
            var href = $(this).attr('href');
            if (href && !href.startsWith('#') && !href.startsWith('http') && !href.startsWith('javascript') && href.indexOf('tableId') === -1) {
                if (href.indexOf('.html') !== -1) {
                    var separator = href.indexOf('?') !== -1 ? '&' : '?';
                    $(this).attr('href', href + separator + 'tableId=' + tableId);
                }
            }
        });

        // Hesab
        $('#bill-container').show();
        loadBill();
        setInterval(loadBill, 15000);
        $('#btn-show-bill').on('click', function (e) { e.preventDefault(); e.stopPropagation(); $('.tst-minicart-window').hide(); $('#bill-window').toggle(); });
        $('.tst-cart').on('click', function (e) { if (!$(this).is('#btn-show-bill')) { $('#bill-window').hide(); } });
        $('#btn-bill-pay').on('click', async function (e) {
            e.preventDefault();
            if (!billOrders || billOrders.length === 0) { alert('Sifaris yoxdur'); return; }
            try { var res = await fetch(API_BASE + '/Payments/online/' + billOrders[0].id, { method: 'POST', headers: { 'Content-Type': 'application/json' } }); var data = await res.json(); if (data.success && data.data && data.data.hppUrl) { window.location.href = data.data.hppUrl; } else { alert('Online odenis mumkun deyil.'); } } catch (err) { alert('Server xetasi'); }
        });
        $('#btn-bill-cash').on('click', function (e) {
            e.preventDefault();
            if (orderConnection) {
                orderConnection.invoke("NotifyTablePaymentRequest", tableId).catch(function () { });
            }
            alert('Kassir masanıza gələcək. Təşəkkürlər!');
        });

        // Chat
        $('#chat-float').show();
        $('#chat-btn').on('click', function () { unreadCount = 0; $('#chat-badge').hide(); });

        // Ofisiant çağır düyməsi
        if (!document.getElementById('btn-call-waiter')) {
            var callBtn = document.createElement('button');
            callBtn.id = 'btn-call-waiter';
            callBtn.innerHTML = '<i class="fas fa-bell"></i>';
            callBtn.style.cssText = 'width:55px;height:55px;border-radius:50%;background:#e74a3b;color:#fff;border:none;cursor:pointer;font-size:22px;box-shadow:0 5px 20px rgba(0,0,0,0.2);margin-bottom:10px;display:block;';
            callBtn.title = 'Ofisiantı çağır';
            var chatFloat = document.getElementById('chat-float');
            if (chatFloat) chatFloat.insertBefore(callBtn, chatFloat.firstChild);

            callBtn.addEventListener('click', function () {
                callBtn.disabled = true;
                callBtn.style.background = '#999';
                callBtn.innerHTML = '<i class="fas fa-check"></i>';
                if (chatConnection) {
                    chatConnection.invoke("SendMessageToWaiter", tableId, "⚡ Ofisiantı çağırıram!").catch(function () { });
                }
                var toast = document.createElement('div');
                toast.style.cssText = 'position:fixed;top:20px;right:20px;background:#e74a3b;color:#fff;padding:15px 25px;border-radius:10px;z-index:9999;font-weight:600;box-shadow:0 5px 15px rgba(0,0,0,0.2);';
                toast.textContent = 'Ofisiant çağırıldı! Gözləyin...';
                document.body.appendChild(toast);
                setTimeout(function () { toast.remove(); }, 3000);
                setTimeout(function () {
                    callBtn.disabled = false;
                    callBtn.style.background = '#e74a3b';
                    callBtn.innerHTML = '<i class="fas fa-bell"></i>';
                }, 15000);
            });
        }

        initSignalR();
    } else {
        // Rezervasiya düymələrini göstər və onclick bağla
        $('.tst-res-btn').attr('style', 'display:inline-block !important').each(function () {
            this.onclick = function (e) { e.preventDefault(); $('.tst-popup-bg').addClass('tst-active'); };
        });
        $('.tst-close-popup').each(function () {
            this.onclick = function () { $('.tst-popup-bg').removeClass('tst-active'); };
        });
    }

    updateCartUI();

    // Səbət qty kontrolları
    $(document).on('click', '.cart-qty-btn', function () {
        var productId = $(this).data('id');
        var action = $(this).data('action');
        var item = cart.find(function (i) { return i.productId === productId; });
        if (!item) return;
        if (action === 'plus') { if (item.quantity >= 50) { alert('Maksimum 50 ədəd.'); return; } item.quantity++; }
        else if (action === 'minus') { item.quantity--; if (item.quantity <= 0) { cart = cart.filter(function (i) { return i.productId !== productId; }); } }
        saveCart(); updateCartUI();
    });
    $(document).on('click', '.remove-cart-item', function () { removeFromCart($(this).data('id')); });

    // Sifariş ver düymələri
    $('#btn-checkout, #btn-order-bar').on('click', function () { submitOrder(); });

    // Rezervasiya form submit (bütün səhifələrdə)
    $('#reservation-form').on('submit', async function (e) {
        e.preventDefault();
        var name = $('#res-name').val().trim();
        var phone = $('#res-phone').val().trim();
        var email = $('#res-email').val().trim();
        var guests = $('#res-guests').val();
        var dateStr = $('#res-date').val();
        var time = $('#res-time').val();
        var note = $('#res-note').val().trim();

        if (!name || !phone || !guests || !dateStr || !time) { showResMessage('Bütün vacib sahələri doldurun.', false); return; }

        var dateParts = dateStr.split('.');
        var reservationDate = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
        var timeParts = time.split(':');
        reservationDate.setHours(parseInt(timeParts[0]), parseInt(timeParts[1]), 0);

        var dateFormatted = reservationDate.getFullYear() + '-' + String(reservationDate.getMonth() + 1).padStart(2, '0') + '-' + String(reservationDate.getDate()).padStart(2, '0') + 'T' + String(reservationDate.getHours()).padStart(2, '0') + ':' + String(reservationDate.getMinutes()).padStart(2, '0') + ':00';

        try {
            var guestCount = parseInt(guests);
            var response = await fetch(API_BASE + '/Reservations', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ customerName: name, customerPhone: phone, customerEmail: email || null, reservationDate: dateFormatted, guestCount: guestCount, note: note || null })
            });
            var data = await response.json();
            if (data.success) {
                showResMessage('Rezervasiya uğurla yaradıldı!', true);
                $('#reservation-form')[0].reset();
                if ($.fn.niceSelect) $('select').niceSelect('update');
            } else { showResMessage(data.message || 'Xəta baş verdi.', false); }
        } catch (err) { showResMessage('Server xətası.', false); }
    });
}

function showResMessage(text, isSuccess) {
    var msgEl = $('#reservation-message');
    var textEl = $('#reservation-message-text');
    textEl.text(text);
    textEl.css('color', isSuccess ? '#4CAF50' : '#f44336');
    msgEl.fadeIn();
    if (isSuccess) {
        setTimeout(function () { msgEl.fadeOut(); $('.tst-popup-bg').removeClass('tst-active'); }, 3000);
    }
}