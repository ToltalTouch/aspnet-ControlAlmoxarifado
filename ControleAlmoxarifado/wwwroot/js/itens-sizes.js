(function (){
    async function loadSizeForRow(rowId, container) {
        const itemName = container.dataset.item;
        if (!itemName) return;

        try {
            const res = await fetch('/Home/Sizes?itemName=' + encodeURIComponent(itemName), {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });
            if (!res.ok) {
                console.error('Erro ao carregar tamanhos:', res.status);
                // replace spinner with user-visible error
                try {
                    container.innerHTML = '<div class="text-danger">Erro ao carregar tamanhos. (status ' + res.status + ')</div>';
                } catch (e) { /* ignore */ }
                return;
            }

            const html = await res.text();
            container.innerHTML = html;
            container.dataset.loaded = 'true';
            console.debug('[itens-sizes] loaded sizes for', itemName, 'htmlLen=', html.length);
            // if a selected size is present on the container, auto-click it to load details
            const selSize = container.dataset.selectedSize;
            if (selSize && selSize.toString().trim() !== '') {
                const wanted = selSize.trim();
                // find the button with matching data-size (trim and match)
                const btn = Array.from(container.querySelectorAll('.size-select')).find(b => b.dataset.size && b.dataset.size.trim() === wanted);
                console.debug('[itens-sizes] auto-selecting size (direct) ', wanted, 'for item', itemName, 'btnFound=', !!btn);
                // ensure placeholder exists
                let detailsPlaceholder = container.querySelector('.size-details');
                if (!detailsPlaceholder) {
                    detailsPlaceholder = document.createElement('div');
                    detailsPlaceholder.className = 'size-details mt-2';
                    container.appendChild(detailsPlaceholder);
                }
                // directly load details instead of simulating a click to avoid delegation timing issues
                // call even if the button wasn't found, to handle cases where the button text/trim differs
                loadSizeDetails(container, wanted);
            }
        } catch (err) {
            console.error(err);
            try {
                container.innerHTML = '<div class="text-danger">Erro ao carregar tamanhos.</div>';
            } catch (e) { /* ignore */ }
        }
    }

    async function loadSizeDetails(container, size) {
        const itemName = container.dataset.item;
        if (!itemName || !size) return;

        const detailsPlaceholder = container.querySelector('.size-details');
        if (!detailsPlaceholder) return;
        detailsPlaceholder.innerHTML = '<div class="text-muted py-2">Carregando detalhes...</div>';
        console.debug('[itens-sizes] loading size details', { item: itemName, size: size, selectedGender: container.dataset.selectedGender });

        try {
            let url = '/Home/SizeDetails?itemName=' + encodeURIComponent(itemName) + '&size=' + encodeURIComponent(size);
            const gender = container.dataset.selectedGender;
            if (gender && gender.toString().trim() !== '') {
                url += '&gender=' + encodeURIComponent(gender);
            }
            const res = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            if (!res.ok) {
                console.error('[itens-sizes] SizeDetails response not OK', res.status, url);
                detailsPlaceholder.innerHTML = '<div class="text-danger">Erro ao carregar detalhes. (status ' + res.status + ')</div>';
                return;
            }

            const html = await res.text();
            // defensive: ensure we always replace the placeholder so the spinner doesn't persist
            if (!html || html.trim() === '') {
                console.debug('[itens-sizes] SizeDetails returned empty HTML for', { item: itemName, size });
                detailsPlaceholder.innerHTML = '<div class="text-muted">Nenhuma variante encontrada para este tamanho.</div>';
            } else {
                detailsPlaceholder.innerHTML = html;
                detailsPlaceholder.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }
            console.debug('[itens-sizes] injected size details htmlLen=', html.length, 'for', itemName, size);
        } catch (err) {
            console.error(err);
            detailsPlaceholder.innerHTML = '<div class="text-danger">Erro ao carregar detalhes.</div>';
        }
    }

    document.addEventListener('click', function (e) {
        // toggle sizes row when clicking item name
        const toggle = e.target.closest('.item-toggle');
        if (toggle) {
            const tr = toggle.closest('tr');
            if (!tr) return;
            const rowID = tr.dataset.itemId;
            const sizesRow = document.querySelector(`.sizes-row[data-parent-id="${rowID}"]`);
            if (!sizesRow) return;
            const container = sizesRow.querySelector('.sizes-container');

            if (container && container.dataset.loaded === 'true') {
                sizesRow.style.display = sizesRow.style.display === 'none' ? '' : 'none';
                return;
            }

            sizesRow.style.display = '';
            if (container) {
                container.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';
                loadSizeForRow(rowID, container);
            }

            return;
        }

        // click on a size button (delegated)
        const sizeBtn = e.target.closest('.size-select');
        if (sizeBtn) {
            try {
                const container = sizeBtn.closest('.sizes-container');
                console.debug('[itens-sizes] size button clicked', { sizeBtn, container });
                if (!container) {
                    console.warn('[itens-sizes] sizes-container not found for clicked button');
                    return;
                }
                let size = sizeBtn.dataset.size;

                // support special ALL value (case-insensitive)
                if (size && size.toUpperCase() === 'ALL') {
                    size = 'ALL';
                }

                // find or create placeholder for details
                let detailsPlaceholder = container.querySelector('.size-details');
                if (!detailsPlaceholder) {
                    console.debug('[itens-sizes] creating missing .size-details placeholder');
                    detailsPlaceholder = document.createElement('div');
                    detailsPlaceholder.className = 'size-details mt-2';
                    container.appendChild(detailsPlaceholder);
                }

                // clear any previous details and load new
                detailsPlaceholder.innerHTML = '';
                loadSizeDetails(container, size);
            } catch (err) {
                console.error('[itens-sizes] error handling size button click', err);
            }
            return;
        }
    });

    // Auto-expand logic placed inside the IIFE so it can access internal functions
    async function autoExpandOnce() {
        try {
            const form = document.querySelector('form');
            if (!form) return;

            const filterNames = ['category', 'size', 'gender', 'status', 'q'];
            const hasFilter = filterNames.some(name => {
                const el = form.querySelector(`[name="${name}"]`);
                return el && el.value && el.value.toString().trim() !== '';
            });

            if (!hasFilter) return;

            const rows = Array.from(document.querySelectorAll('tr[data-item-id]'));
            console.debug('[itens-sizes] filters detected on page load - expanding', rows.length, 'items');

            // process sequentially to avoid concurrent request bursts and timing issues
            async function expandSequentially(rowsList) {
                for (const tr of rowsList) {
                    try {
                        const rowID = tr.dataset.itemId;
                        const sizesRow = document.querySelector(`.sizes-row[data-parent-id="${rowID}"]`);
                        if (!sizesRow) continue;
                        const container = sizesRow.querySelector('.sizes-container');

                        sizesRow.style.display = '';
                        if (container && container.dataset.loaded !== 'true') {
                            container.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';
                            // wait for the loader to finish before moving to next
                            await loadSizeForRow(rowID, container);
                            // small pause to give browser time to render and to avoid hammering the server
                            await new Promise(r => setTimeout(r, 60));
                        }
                    } catch (err) {
                        console.error('[itens-sizes] error expanding row sequentially', err);
                    }
                }
            }

            // show overlay while expanding
            showItemsLoading();
            try {
                await expandSequentially(rows);
            } finally {
                hideItemsLoading();
            }
        } catch (err) {
            console.error('[itens-sizes] error auto-expanding items on load', err);
        }
    }

    // loading overlay helpers
    function showItemsLoading() {
        const area = document.getElementById('items-area');
        const overlay = document.getElementById('items-loading-overlay');
        if (!area || !overlay) return;
        area.classList.add('loading');
        overlay.style.display = 'flex';
        overlay.setAttribute('aria-hidden', 'false');
    }

    function hideItemsLoading() {
        const area = document.getElementById('items-area');
        const overlay = document.getElementById('items-loading-overlay');
        if (!area || !overlay) return;
        overlay.style.display = 'none';
        overlay.setAttribute('aria-hidden', 'true');
        area.classList.remove('loading');
    }

    // Run when DOM ready (or immediately if already ready), and schedule retries to handle timing issues
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', autoExpandOnce);
    } else {
        // if script loaded after DOMContentLoaded, run immediately
        setTimeout(autoExpandOnce, 50);
    }
    // show overlay when the main filter form is submitted
    try {
        const mainForm = document.querySelector('form');
        if (mainForm) {
            mainForm.addEventListener('submit', function () {
                showItemsLoading();
            });
        }
    } catch (e) {
        console.debug('[itens-sizes] could not attach submit listener', e);
    }
    // extra retries to handle late-rendered content/hot-reload scenarios
    setTimeout(autoExpandOnce, 300);
    setTimeout(autoExpandOnce, 1000);

    window.loadingOverlay = {
    show: function(id = 'items-loading-overlay') {
        const overlay = document.getElementById(id);
        const area = overlay?.closest('.items-area');
        if (!overlay) return;
        overlay.style.display = 'flex';
        overlay.setAttribute('aria-hidden', 'false');
        if (area) area.classList.add('loading');
    },
    hide: function(id = 'items-loading-overlay') {
        const overlay = document.getElementById(id);
        const area = overlay?.closest('.items-area');
        if (!overlay) return;
        overlay.style.display = 'none';
        overlay.setAttribute('aria-hidden', 'true');
        if (area) area.classList.remove('loading');
    }
    };
})();