(function () {
    function doCleanFilters() {
        const btn = document.getElementById('resetFilters');
        const form = btn?.closest('form') || document.querySelector('form');
        if (!form) return;

        form.querySelectorAll('select, input[type="search"], input[type="text"]').forEach(e => {
            if (e.tagName === 'SELECT') e.selectedIndex = 0;
            else e.value = '';
        });

        let pageInput = form.querySelector('input[name="page"]');
        if (!pageInput) {
            pageInput = document.createElement('input');
            pageInput.type = 'hidden';
            pageInput.name = 'page';
            form.appendChild(pageInput);
        }
        pageInput.value = '1';
        form.submit();
    }

    function init() {
        const btn = document.getElementById('resetFilters');
        btn?.addEventListener('click', function (e) {
            e.preventDefault();
            doCleanFilters();
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
    window.cleanFilters = doCleanFilters;
})();