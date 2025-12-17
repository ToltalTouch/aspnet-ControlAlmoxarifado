(function () {
    'use strict';

    async function loadRemote(url) {
        try {
            const res = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            if (!res.ok) throw new Error('Erro ao carregar formulário: ' + res.status);
            const html = await res.text();
            const container = document.getElementById('modalEditContent');
            if (!container) throw new Error('Elemento #modalEditContent não encontrado');
            container.innerHTML = html;

            // se jquery unobtrusive validation está presente, reparse o conteúdo para ativar validação cliente
            if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive) {
                jQuery.validator.unobtrusive.parse(container);
            }

            const modalEl = document.getElementById('modalEdit');
            if (!modalEl) throw new Error('Modal #modalEdit não encontrado');
            const bsModal = new bootstrap.Modal(modalEl);
            bsModal.show();

            const form = container.querySelector('form');
            if (!form) return;

            // evitar múltiplos listeners removendo e substituindo o form por um clone
            const newForm = form.cloneNode(true);
            form.parentNode.replaceChild(newForm, form);

            newForm.addEventListener('submit', async function (e) {
                e.preventDefault();
                const fd = new FormData(newForm);
                const postUrl = newForm.action || url;
                const postRes = await fetch(postUrl, {
                    method: (newForm.method || 'POST'),
                    body: fd,
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                });

                const contentType = postRes.headers.get('content-type') || '';
                if (postRes.ok && contentType.includes('application/json')) {
                    const json = await postRes.json();
                    if (json && json.success) {
                        bsModal.hide();
                        // default: recarregar a página; pode ser alterado para atualização parcial
                        window.location.reload();
                        return;
                    }
                }

                // Se retornar HTML (erros de validação), substituir conteúdo e reanexar validação
                const respHtml = await postRes.text();
                container.innerHTML = respHtml;
                if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive) {
                    jQuery.validator.unobtrusive.parse(container);
                }
            });

        } catch (err) {
            console.error(err);
            alert('Não foi possível carregar o formulário.');
        }
    }

    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.open-remote-modal');
        if (!btn) return;
        e.preventDefault();
        const url = btn.getAttribute('data-url');
        if (!url) return;
        loadRemote(url);
    });

})();
