let allClasses = [];
document.addEventListener("DOMContentLoaded", async function () {
    await fetchClasses();

    // Attach filter event listeners
    const filters = ['filterClassName', 'filterGender', 'filterStatus', 'filterYear'];
    filters.forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.addEventListener('input', applyFilters);
            el.addEventListener('change', applyFilters);
        }
    });
});
// Converts ISO to YYYY-MM-DD for input type="date"
function toInputDate(isoString) {
    if (!isoString) return '';
    return new Date(isoString).toISOString().split('T')[0];
}

// --- API Calls ---

async function fetchClasses() {
    // Assuming this endpoint exists to list classes
    const url = `${apiBaseUrl}/Meeting/GetServantClasses`;

    if (typeof showLoading === 'function') showLoading();

    try {
        const response = await fetch(url, {
            method: "GET",
            credentials: 'include',
            headers: { "Content-Type": "application/json" }
        });

        if (response.ok) {
            allClasses = await response.json();
            populateYearFilter();
            applyFilters();
        } else {
            const grid = document.getElementById('classes-grid');
            if (grid) grid.innerHTML = `<div class="col-12"><div class="alert alert-danger">Failed to load classes.</div></div>`;
        }
    } catch (error) {
        console.error("Error fetching classes:", error);
    } finally {
        if (typeof hideLoading === 'function') hideLoading();
    }
}

function populateYearFilter() {
    const yearSelect = document.getElementById('filterYear');
    if (!yearSelect) return;

    // Extract unique years
    const years = [...new Set(allClasses.map(c => c.year))].filter(y => y).sort((a, b) => b - a);

    // Keep the "All Years" option, remove the rest
    yearSelect.innerHTML = '<option value="">All Years</option>';

    years.forEach(y => {
        yearSelect.insertAdjacentHTML('beforeend', `<option value="${y}">${y}</option>`);
    });
}

// --- Filtering & Rendering ---

function applyFilters() {
    const nameTxt = document.getElementById('filterClassName').value.toLowerCase();
    const gender = document.getElementById('filterGender').value;
    const status = document.getElementById('filterStatus').value;
    const year = document.getElementById('filterYear').value;

    const filtered = allClasses.filter(c => {
        const matchName = !nameTxt || (c.className && c.className.toLowerCase().includes(nameTxt));
        const matchGender = !gender || c.gender === gender;
        const matchStatus = !status || c.isActive.toString() === status;
        const matchYear = !year || c.year.toString() === year;

        return matchName && matchGender && matchStatus && matchYear;
    });

    renderGrid(filtered);
}

