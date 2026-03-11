/* === AI ASSISTANT WIDGET === */
(function () {
    if (document.getElementById('ai-widget')) return;

    var isSending = false;

    var widgetHTML = '<div id="ai-widget" style="position:fixed;bottom:20px;left:20px;z-index:9998;">' +
        '<div id="ai-chat-window" style="display:none;width:340px;background:#fff;border-radius:15px;box-shadow:0 10px 40px rgba(0,0,0,0.2);margin-bottom:10px;overflow:hidden;">' +
        '  <div style="background:#1a1a2e;color:#fff;padding:15px 20px;display:flex;justify-content:space-between;align-items:center;">' +
        '    <div style="display:flex;align-items:center;gap:10px;"><span style="font-size:20px;">🤖</span><div><strong style="font-size:14px;">RestoBot</strong><div style="font-size:11px;color:#c8a97e;">AI Köməkçi</div></div></div>' +
        '    <button id="ai-close" style="background:none;border:none;color:#fff;font-size:18px;cursor:pointer;">✕</button>' +
        '  </div>' +
        '  <div id="ai-messages" style="height:300px;overflow-y:auto;padding:15px;display:flex;flex-direction:column;gap:10px;background:#f8f9fa;">' +
        '    <div style="max-width:85%;padding:10px 14px;border-radius:12px;background:#1a1a2e;color:#fff;font-size:13px;align-self:flex-start;border-bottom-left-radius:4px;">Salam! Mən RestoBot, sizə menyumuz, iş saatlarımız və rezervasiya haqqında kömək edə bilərəm. Nə sormaq istəyirsiniz?</div>' +
        '  </div>' +
        '  <div style="padding:10px;border-top:1px solid #eee;display:flex;gap:8px;">' +
        '    <input id="ai-input" type="text" placeholder="Sualınızı yazın..." style="flex:1;padding:10px 14px;border:1px solid #e0e0e0;border-radius:20px;font-size:13px;outline:none;" />' +
        '    <button id="ai-send" style="width:40px;height:40px;border-radius:50%;background:#c8a97e;color:#fff;border:none;cursor:pointer;font-size:16px;display:flex;align-items:center;justify-content:center;">➤</button>' +
        '  </div>' +
        '</div>' +
        '<button id="ai-toggle" style="width:55px;height:55px;border-radius:50%;background:#1a1a2e;color:#fff;border:none;cursor:pointer;font-size:24px;box-shadow:0 5px 20px rgba(0,0,0,0.3);display:flex;align-items:center;justify-content:center;">🤖</button>' +
        '</div>';

    document.body.insertAdjacentHTML('beforeend', widgetHTML);

    var chatWindow = document.getElementById('ai-chat-window');
    var toggleBtn = document.getElementById('ai-toggle');
    var closeBtn = document.getElementById('ai-close');
    var sendBtn = document.getElementById('ai-send');
    var input = document.getElementById('ai-input');
    var messages = document.getElementById('ai-messages');

    toggleBtn.addEventListener('click', function () {
        chatWindow.style.display = chatWindow.style.display === 'none' ? 'block' : 'none';
        if (chatWindow.style.display === 'block') input.focus();
    });

    closeBtn.addEventListener('click', function () {
        chatWindow.style.display = 'none';
    });

    sendBtn.addEventListener('click', function () {
        sendMessage();
    });

    input.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') sendMessage();
    });

    function addMessage(text, isUser) {
        var div = document.createElement('div');
        div.style.cssText =
            'max-width:85%;padding:10px 14px;border-radius:12px;font-size:13px;line-height:1.5;word-wrap:break-word;' +
            (isUser
                ? 'background:#c8a97e;color:#fff;align-self:flex-end;border-bottom-right-radius:4px;'
                : 'background:#1a1a2e;color:#fff;align-self:flex-start;border-bottom-left-radius:4px;');

        div.textContent = text;
        messages.appendChild(div);
        messages.scrollTop = messages.scrollHeight;
        return div;
    }

    function addTyping() {
        var div = document.createElement('div');
        div.id = 'ai-typing';
        div.style.cssText = 'max-width:85%;padding:10px 14px;border-radius:12px;font-size:13px;background:#1a1a2e;color:#c8a97e;align-self:flex-start;border-bottom-left-radius:4px;';
        div.textContent = 'Yazır...';
        messages.appendChild(div);
        messages.scrollTop = messages.scrollHeight;
    }

    function removeTyping() {
        var typing = document.getElementById('ai-typing');
        if (typing) typing.remove();
    }

    async function sendMessage() {
        if (isSending) return;

        var text = input.value.trim();
        if (!text) return;

        isSending = true;
        addMessage(text, true);
        input.value = '';
        addTyping();
        sendBtn.disabled = true;
        input.disabled = true;

        try {
            var res = await fetch('/api/AiAssistant/chat', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ message: text })
            });

            var raw = await res.text();
            var data = raw ? JSON.parse(raw) : null;

            removeTyping();

            if (res.ok && data && data.success) {
                addMessage(data.reply, false);
            } else {
                addMessage((data && data.message) || 'Xəta baş verdi. Bir az sonra yenidən cəhd edin.', false);
            }
        } catch (err) {
            removeTyping();
            addMessage('Server xətası. Bir az sonra yenidən cəhd edin.', false);
        } finally {
            isSending = false;
            sendBtn.disabled = false;
            input.disabled = false;
            input.focus();
        }
    }
})();