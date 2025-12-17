(function (){
    async function loadModal(url){
        const res = await fetch(url, {headers: { 'X-Requested-With': 'XMLHttpRequest' }});
        const html = await res.text();
        document.getElementById('modalNewCotent').innerHTML = html;

        const modalEl = document.getElementById('modalNewItem');
        const bsModal = new bootstrap.Modal(modalEl);
        bsModal.show();

        const form = modalEl.querySelector('form');
        if (!form) return;

        form.addEventListener('submit', async function (e) {
            e.preventDefault();
            const formData = new FormData(form);
            const postUrl = form.action || url;
            const postRes = await fetch(postUrl, {
                method: form.method || 'POST',
                body: formData,
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            const contentType = postRes.headers.get('content-type') || '';
            if (postRes.ok && contentType.includes('application/json')) {
                bsModal.hide();
                window.location.reload();
                return;
            }

            const responseHtml = await postRes.text();
            document.getElementById('modalNewCotent').innerHTML = responseHtml;
        });
    }

    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.open-remote-modal');
        if (!btn) return;
        const url = btn.getAttribute('data-url');
        if (!url) return;
        loadModal(url);
    });
})();