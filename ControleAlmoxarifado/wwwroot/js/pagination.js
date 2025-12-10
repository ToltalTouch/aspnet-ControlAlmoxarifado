(function (){
    function ResetFiltersHandler(e) {
        var btn = e.currentTarget;
        var form = btn.closest('form') || document.querySelector('form');
        if (!form) return;

        form.querySelectorAll('select, input[type="search"], input[type="text"]').forEach(function (el) {
            if (el.tagName === 'SELECT') el.selectedIndex = 0;
            else el.value = '';
        });

        var pageInput = form.querySelector('input[name="page"]');
        if (!pageInput) {
            pageInput = document.createElement('input');
            pageInput.type = 'hidden';
            pageInput.name = 'page';
            form.appendChild(pageInput);
        }

        pageInput.value = '1';
        form.submit();
    }

    document.addEventListener('DOMContentLoaded', function () {
        if (e.target && e.target.id === 'reset-filters-btn') {
            resetFiltersHandler(e);
        }
    });
})();