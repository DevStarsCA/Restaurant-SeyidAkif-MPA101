/* === ADMIN COMMON JS === */

var API_BASE = '/api';

// Auth yoxla
function checkAuth() {
    var token = sessionStorage.getItem('token');
    if (!token) {
        window.location.href = 'login.html';
        return false;
    }
    // User adını göstər
    var user = JSON.parse(sessionStorage.getItem('user') || '{}');
    var nameEl = document.getElementById('user-name');
    if (nameEl && user.fullName) {
        nameEl.textContent = user.fullName;
    }
    return true;
}

// Auth ilə API sorğusu
async function authFetch(url, options) {
    var token = sessionStorage.getItem('token');
    if (!token) {
        window.location.href = 'login.html';
        return null;
    }

    var defaultOptions = {
        headers: {
            'Authorization': 'Bearer ' + token,
            'Content-Type': 'application/json'
        }
    };

    if (options) {
        defaultOptions = Object.assign(defaultOptions, options);
        if (options.headers) {
            defaultOptions.headers = Object.assign({
                'Authorization': 'Bearer ' + token
            }, options.headers);
        }
    }

    try {
        var response = await fetch(API_BASE + url, defaultOptions);

        if (response.status === 401) {
            sessionStorage.clear();
            window.location.href = 'login.html';
            return null;
        }

        return await response.json();
    } catch (err) {
        console.error('API Error:', err);
        return null;
    }
}

// FormData ilə API sorğusu (şəkil yükləmə üçün)
async function authFetchForm(url, formData, method) {
    var token = sessionStorage.getItem('token');
    if (!token) {
        window.location.href = 'login.html';
        return null;
    }

    try {
        var response = await fetch(API_BASE + url, {
            method: method || 'POST',
            headers: {
                'Authorization': 'Bearer ' + token
            },
            body: formData
        });

        if (response.status === 401) {
            sessionStorage.clear();
            window.location.href = 'login.html';
            return null;
        }

        return await response.json();
    } catch (err) {
        console.error('API Error:', err);
        return null;
    }
}

// Sidebar toggle
document.addEventListener('DOMContentLoaded', function () {
    var toggleBtn = document.getElementById('btn-toggle');
    var sidebar = document.getElementById('sidebar');
    var mainContent = document.getElementById('main-content');

    if (toggleBtn) {
        toggleBtn.addEventListener('click', function () {
            sidebar.classList.toggle('collapsed');
            mainContent.classList.toggle('expanded');
        });
    }

    // Logout
    var logoutBtn = document.getElementById('btn-logout');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function (e) {
            e.preventDefault();
            sessionStorage.clear();
            window.location.href = 'login.html';
        });
    }

    // Mobile sidebar
    if (window.innerWidth <= 992) {
        if (toggleBtn) {
            toggleBtn.addEventListener('click', function () {
                sidebar.classList.toggle('show');
            });
        }
    }
});

// Toast notification
function showToast(message, type) {
    var toast = document.createElement('div');
    toast.className = 'position-fixed top-0 end-0 m-3';
    toast.style.zIndex = '9999';

    var bgClass = type === 'success' ? 'bg-success' : type === 'error' ? 'bg-danger' : 'bg-info';

    toast.innerHTML =
        '<div class="toast show align-items-center text-white ' + bgClass + ' border-0" role="alert">' +
        '  <div class="d-flex">' +
        '    <div class="toast-body">' + message + '</div>' +
        '    <button type="button" class="btn-close btn-close-white me-2 m-auto" onclick="this.closest(\'.position-fixed\').remove()"></button>' +
        '  </div>' +
        '</div>';

    document.body.appendChild(toast);
    setTimeout(function () { toast.remove(); }, 3000);
}

// Tarix format
function formatDate(dateStr) {
    var d = new Date(dateStr);
    return d.toLocaleDateString('az', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

function formatDateTime(dateStr) {
    if (!dateStr) return '';
    var parts = dateStr.replace('T', ' ').replace('Z', '').split(/[- :]/);
    var d = parts[2] + '.' + parts[1] + '.' + parts[0] + ' ' + parts[3] + ':' + parts[4];
    return d;
}

// Confirm dialog
function confirmAction(message) {
    return confirm(message || 'Əminsiniz?');
}

// Status badge
function getOrderStatusBadge(status) {
    var map = {
        0: '<span class="badge bg-warning">Gözləyir</span>',
        1: '<span class="badge bg-info">Hazırlanır</span>',
        2: '<span class="badge bg-primary">Hazırdır</span>',
        3: '<span class="badge bg-success">Verildi</span>',
        4: '<span class="badge bg-secondary">Tamamlandı</span>',
        5: '<span class="badge bg-danger">Ləğv</span>'
    };
    return map[status] || '<span class="badge bg-secondary">Naməlum</span>';
}

function getTableStatusBadge(status) {
    var map = {
        0: '<span class="badge bg-success">Boş</span>',
        1: '<span class="badge bg-danger">Dolu</span>',
        2: '<span class="badge bg-warning">Rezerv</span>'
    };
    return map[status] || '<span class="badge bg-secondary">Naməlum</span>';
}

function getReservationStatusBadge(status) {
    var map = {
        0: '<span class="badge bg-warning">Gözləyir</span>',
        1: '<span class="badge bg-success">Təsdiqləndi</span>',
        2: '<span class="badge bg-danger">Ləğv</span>'
    };
    return map[status] || '<span class="badge bg-secondary">Naməlum</span>';
}