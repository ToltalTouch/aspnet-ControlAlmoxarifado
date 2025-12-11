(function () {
    // Carrega o HTML do partial e exibe o modal; também anexa o handler de submit
    async function loadEdit(id) {
        const res = await fetch('/Item/Edit/' + id, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        if (!res.ok) {
            console.error('Erro ao carregar partial Edit:', res.status);
            return;
        }
        const html = await res.text();
        showModalWithHtml(html);
    }

    function showModalWithHtml(html) {
        const container = document.getElementById('modalEditContent');
        if (!container) return console.error('modalEditContent não encontrado');
        container.innerHTML = html;

        const modalEl = document.getElementById('modalEdit');
        if (!modalEl) return console.error('modalEdit não encontrado');

        // foco no input quantidade quando o modal estiver visível
        modalEl.addEventListener('shown.bs.modal', function handler() {
            const input = container.querySelector('input[name="Quantidade"], input[asp-for="Quantidade"]');
            if (input) input.focus();
            modalEl.removeEventListener('shown.bs.modal', handler);
        });

        const modal = new bootstrap.Modal(modalEl);
        modal.show();

        // Anexa o handler de submit ao formulário recém-injetado
        const form = container.querySelector('form#editItemForm') || container.querySelector('form');
        if (!form) return;

        // remover listeners antigos (se houver) fazendo clone do form
        const newForm = form.cloneNode(true);
        form.parentNode.replaceChild(newForm, form);

        newForm.addEventListener('submit', async function (evt) {
            evt.preventDefault();
            const fd = new FormData(newForm);
            const post = await fetch(newForm.action || '/Item/Edit', {
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                body: fd
            });
            const contentType = post.headers.get('content-type') || '';
            if (contentType.includes('application/json')) {
                const json = await post.json();
                if (json.success) {
                    // atualiza apenas a célula de quantidade correspondente na tabela
                    const cell = document.querySelector(`[data-quantity-id="${json.id}"]`);
                    if (cell) {
                        cell.textContent = json.quantidade;
                    } else {
                        // fallback: recarrega a página se não encontrou a célula
                        window.location.reload();
                    }
                    modal.hide();
                }
            } else {
                // resposta HTML (validação) -> substituir conteúdo do modal e reanexar handlers
                const newHtml = await post.text();
                container.innerHTML = newHtml;
                // re-exibe (o modal já está aberto) e reanexa o handler recursivamente
                showModalWithHtml(newHtml);
            }
        });
    }

    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-edit');
        if (!btn) return;
        const id = btn.dataset.id;
        if (!id) return console.error('data-id não encontrado no botão .btn-edit');
        loadEdit(id);
    });
})();