let allMembers = [];
let allClasses = [];
let selectedMemberIDs = new Set();

// DOM Elements
const elements = {
    mainContainer: document.getElementById('main-container'),
    refreshBtn: document.getElementById('refresh-btn'),
    codeSearchInput: document.getElementById('code-search-input'),
    // Advanced filter elements
    advancedFilterForm: document.getElementById('advanced-filter-form'),
    firstNameFilter: document.getElementById('first-name-filter'),
    lastNameFilter: document.getElementById('last-name-filter'),
    activeFilter: document.getElementById('active-filter'),
    baptisedFilter: document.getElementById('baptised-filter'),
    cardStatusFilter: document.getElementById('card-status-filter'),
    classFilter: document.getElementById('class-filter'),
    birthdateStartFilter: document.getElementById('birthdate-start-filter'),
    birthdateEndFilter: document.getElementById('birthdate-end-filter'),
    ageStartFilter: document.getElementById('age-start-filter'),
    ageEndFilter: document.getElementById('age-end-filter'),
    resetFiltersBtn: document.getElementById('reset-filters-btn'),
    applyFiltersBtn: document.getElementById('apply-filters-btn'),
    notInClassFilter: document.getElementById('not-in-class-filter'),
    classFilterOperator: document.getElementById('class-filter-operator'),
    // Bulk actions elements
    gridControls: document.getElementById('grid-controls'),
    bulkActionsDropdown: document.getElementById('bulk-actions-dropdown'),
    selectAllCheckbox: document.getElementById('select-all-checkbox'),
    selectedCount: document.getElementById('selected-count'),
    // Grid and main UI
    membersGrid: document.getElementById('members-grid'),
    counter: document.getElementById('counter'),
    searchTabs: null // Initialized later
};

/**
 * Initializes the page: fetches data, sets up event listeners.
 */
document.addEventListener("DOMContentLoaded", async () => {
    try {
        const codeSearchTabEl = document.getElementById('code-search-tab');
        showLoading();
        if (codeSearchTabEl) {
            elements.searchTabs = new bootstrap.Tab(codeSearchTabEl);
        }

        initializeScanning();
        await Promise.all([fetchMembers(), fetchClasses()]);
        setupEventListeners();
        toggleFormInputs(document.getElementById('filter-search-pane'), false);
        elements.codeSearchInput.focus();
    } catch (error) {
        console.error("Initialization failed:", error);
        showErrorMessage("Failed to load initial data. Please refresh the page.");
    } finally {
        hideLoading();
    }
});

/**
 * Fetches the full list of members from the API.
 */
async function fetchMembers() {
    const response = await fetch(`${apiBaseUrl}/Member/List`, {
        method: "GET",
        credentials: "include",
        headers: { "Content-Type": "application/json" }
    });
    if (!response.ok) throw new Error(`Failed to fetch members: ${response.statusText}`);
    allMembers = await response.json();
}

/**
 * Fetches the list of classes and defensively initializes the selectpicker.
 */
async function fetchClasses() {
    const response = await fetch(`${apiBaseUrl}/Meeting/GetServantClasses`, {
        method: "GET",
        credentials: "include",
        headers: { "Content-Type": "application/json" }
    });
    if (!response.ok) throw new Error(`Failed to fetch classes: ${response.statusText}`);
    allClasses = await response.json();

    // Clear any existing options before adding new ones
    elements.classFilter.innerHTML = '';

    allClasses.forEach(cls => {
        const option = new Option(cls.className, cls.classID);
        elements.classFilter.add(option);
    });

    // ✅ This is the key fix.
    // It checks if bootstrap-select has already been applied to this element.
    const isAlreadyInitialized = elements.classFilter.classList.contains('bs-select-hidden');

    if (isAlreadyInitialized) {
        // If it's already a selectpicker, just refresh its contents.
        $('#class-filter').selectpicker('refresh');
    } else {
        // Otherwise, initialize it for the first time.
        $('#class-filter').selectpicker();
    }
}

/**
 * Sets up all event listeners for the page.
 */
function setupEventListeners() {
    // Toggle search modes
    const tabs = document.querySelectorAll('#searchTabs button');
    tabs.forEach(tab => tab.addEventListener('shown.bs.tab', handleTabChange));

    // Search by Code (triggers on input)
    elements.codeSearchInput.addEventListener('input', handleCodeSearch);

    // Advanced Filters (triggers on button click)
    elements.applyFiltersBtn.addEventListener('click', applyAdvancedFilters);
    elements.resetFiltersBtn.addEventListener('click', resetAdvancedFilters);

    // Refresh button
    elements.refreshBtn.addEventListener('click', refreshData);

    // Class filter UI logic
    elements.classFilterOperator.addEventListener('change', () => {
        const label = document.getElementById('class-filter-operator-label');
        label.textContent = elements.classFilterOperator.checked ? 'Match ANY (OR)' : 'Match ALL (AND)';
    });
    elements.notInClassFilter.addEventListener('change', () => {
        const isChecked = elements.notInClassFilter.checked;
        if (isChecked) {
            $('#class-filter').selectpicker('val', ''); // Clear selection
            $('#class-filter').prop('disabled', true).selectpicker('refresh');
            elements.classFilterOperator.disabled = true;
        } else {
            $('#class-filter').prop('disabled', false).selectpicker('refresh');
            elements.classFilterOperator.disabled = false;
        }
    });
    $('#class-filter').on('change.bs.select', function () {
        const selectedClasses = $(this).val();
        if (selectedClasses && selectedClasses.length > 0) {
            elements.notInClassFilter.checked = false;
            elements.notInClassFilter.dispatchEvent(new Event('change'));
        }
    });

    // Delegated listener for member selection checkboxes
    elements.membersGrid.addEventListener('change', (event) => {
        if (event.target.classList.contains('member-select-checkbox')) {
            const memberId = parseInt(event.target.value, 10);
            if (event.target.checked) {
                selectedMemberIDs.add(memberId);
            } else {
                selectedMemberIDs.delete(memberId);
            }
            updateBulkActionsUI();
        }
    });

    // Listener for "Select All" checkbox
    elements.selectAllCheckbox.addEventListener('change', (event) => {
        const isChecked = event.target.checked;
        const visibleCheckboxes = elements.membersGrid.querySelectorAll('.member-select-checkbox');

        visibleCheckboxes.forEach(checkbox => {
            const memberId = parseInt(checkbox.value, 10);
            checkbox.checked = isChecked;
            if (isChecked) {
                selectedMemberIDs.add(memberId);
            } else {
                selectedMemberIDs.delete(memberId);
            }
        });
        updateBulkActionsUI();
    });

    // Corrected bulk action listener
    elements.gridControls.addEventListener('click', (event) => {
        const actionLink = event.target.closest('a[data-action]');
        if (!actionLink) {
            return;
        }

        event.preventDefault();
        const action = actionLink.dataset.action;
        const memberIds = Array.from(selectedMemberIDs);

        if (memberIds.length === 0) {
            alert("Please select at least one member.");
            return;
        }

        switch (action) {
            case 'print-card':
                bulkPrintCards(memberIds);
                break;
            case 'update-baptism':
                bulkUpdateBaptism(memberIds);
                break;
            case 'delete-members':
                bulkDeleteMembers(memberIds);
                break;
        }
    });
}

/**
 * Manages UI state when switching between search tabs.
 */
function handleTabChange(event) {
    const isCodeSearch = event.target.id === 'code-search-tab';
    toggleFormInputs(document.getElementById('code-search-pane'), isCodeSearch);
    toggleFormInputs(document.getElementById('filter-search-pane'), !isCodeSearch);
    populateMembersGrid([], true);
    elements.codeSearchInput.value = '';
    resetAdvancedFilters(false);
    if (isCodeSearch) {
        setTimeout(() => elements.codeSearchInput.focus(), 150);
    }
}

/**
 * Enables or disables all form inputs within a given container.
 */
function toggleFormInputs(container, enable) {
    const inputs = container.querySelectorAll('input, select, button');
    inputs.forEach(input => {
        if (input.id === 'class-filter') {
            $(input).prop('disabled', !enable);
            setTimeout(() => $(input).selectpicker('refresh'), 50);
        } else {
            input.disabled = !enable;
        }
    });
}


/**
 * Re-fetches members and applies the current filter.
 */
async function refreshData() {
    showLoading();
    try {
        await fetchMembers();
        const activeTab = document.querySelector('#searchTabs .nav-link.active');
        if (activeTab && activeTab.id === 'code-search-tab') {
            handleCodeSearch();
        } else {
            applyAdvancedFilters();
        }
    } catch (error) {
        console.error("Failed to refresh data:", error);
        showErrorMessage("Could not refresh data from the server.");
    } finally {
        hideLoading();
    }
}

/**
 * Handles the logic for the "Search by Code" feature.
 */
function handleCodeSearch() {
    clearSelection();
    const searchTerm = elements.codeSearchInput.value.trim().toLowerCase();
    if (!searchTerm) {
        populateMembersGrid([], true);
        return;
    }
    const firstMatch = allMembers.find(m => (m.code && m.code.toLowerCase().includes(searchTerm)) || (m.unFileNumber && m.unFileNumber == searchTerm) || (m.unPersonalNumber && m.unPersonalNumber == searchTerm) || (m.imageReference && m.imageReference.toLowerCase().includes(searchTerm)));
    if (firstMatch) {
        const familyFileNumber = firstMatch.unFileNumber;
        const familyMembers = allMembers.filter(m => m.unFileNumber === familyFileNumber).sort((a, b) => b.age - a.age);
        populateMembersGrid(familyMembers);
    } else {
        populateMembersGrid([]);
    }
}

/**
 * Normalizes Arabic text by removing diacritics for smart search.
 */
function normalizeArabic(text) {
    if (!text) return "";
    return text.normalize("NFD").replace(/[\u064B-\u0652]/g, "");
}

/**
 * Intialize scanning component
 */
function initializeScanning() {
    const scanButton = document.getElementById("scanMemberCodeBtnSearch");
    const qrScannerContainer = document.getElementById("qrScannerContainerSearch");
    const stopScanButton = document.getElementById("btnStopScanSearch");
    const codeBtn = document.getElementById("code-search-input");
    if (scanButton) {
        let html5QrCode;
        scanButton.addEventListener("click", function () {
            qrScannerContainer.classList.remove("d-none");
            html5QrCode = new Html5Qrcode("reader");
            html5QrCode.start({
                facingMode: "environment"
            }, {
                fps: 10,
                qrbox: { width: 250, height: 250 }
            }, (decodedText) => {
                codeBtn.value = decodedText;
                html5QrCode.stop().then(() => {
                    qrScannerContainer.classList.add("d-none");
                    handleCodeSearch();
                });
            }, (errorMessage) => {
                // console.warn("QR Code scan error: ", errorMessage);
            }).catch((err) => {
                console.error("QR Code scanning failed: ", err);
            });
        });
        stopScanButton.addEventListener("click", function () {
            if (html5QrCode && html5QrCode.isScanning) {
                html5QrCode.stop().then(() => {
                    qrScannerContainer.classList.add("d-none");
                });
            }
        });
    }
}

/**
 * Applies all advanced filters together with AND logic.
 */
function applyAdvancedFilters() {
    clearSelection();
    const filters = {
        firstName: normalizeArabic(elements.firstNameFilter.value.trim().toLowerCase()),
        lastName: normalizeArabic(elements.lastNameFilter.value.trim().toLowerCase()),
        isActive: elements.activeFilter.value,
        isBaptised: elements.baptisedFilter.value,
        cardStatus: elements.cardStatusFilter.value,
        classIDs: $('#class-filter').val() || [],
        birthStart: elements.birthdateStartFilter.value,
        birthEnd: elements.birthdateEndFilter.value,
        ageStart: parseInt(elements.ageStartFilter.value, 10) || null,
        ageEnd: parseInt(elements.ageEndFilter.value, 10) || null,
        isNotInAnyClass: elements.notInClassFilter.checked,
        classOperatorIsOr: elements.classFilterOperator.checked
    };
    const filteredMembers = allMembers.filter(member => {
        if (filters.firstName && (!member.unFirstName || !normalizeArabic(member.unFirstName.toLowerCase()).includes(filters.firstName))) return false;
        if (filters.lastName && (!member.unLastName || !normalizeArabic(member.unLastName.toLowerCase()).includes(filters.lastName))) return false;
        if (filters.isActive !== 'all' && member.isActive !== (filters.isActive === 'true')) return false;
        if (filters.isBaptised !== 'all' && member.baptised !== (filters.isBaptised === 'true')) return false;
        if (filters.cardStatus !== 'all' && member.cardStatus !== filters.cardStatus) return false;
        if (filters.isNotInAnyClass) {
            if (member.classesIDs && member.classesIDs.length > 0) return false;
        } else if (filters.classIDs.length > 0) {
            const memberClasses = member.classesIDs || [];
            if (filters.classOperatorIsOr) {
                if (!filters.classIDs.some(id => memberClasses.includes(parseInt(id, 10)))) return false;
            } else {
                if (!filters.classIDs.every(id => memberClasses.includes(parseInt(id, 10)))) return false;
            }
        }
        const birthDate = new Date(member.birthdate);
        if (filters.birthStart && birthDate < new Date(filters.birthStart)) return false;
        if (filters.birthEnd && birthDate > new Date(filters.birthEnd)) return false;
        if (filters.ageStart && member.age < filters.ageStart) return false;
        if (filters.ageEnd && member.age > filters.ageEnd) return false;
        return true;
    });
    populateMembersGrid(filteredMembers);
}

/**
 * Resets all advanced filters to their default state.
 */
function resetAdvancedFilters(triggerSearch = true) {
    clearSelection();
    elements.advancedFilterForm.reset();
    $('#class-filter').selectpicker('val', '');
    elements.notInClassFilter.disabled = false;
    $('#class-filter').prop('disabled', false).selectpicker('refresh');
    elements.classFilterOperator.disabled = false;
    document.getElementById('class-filter-operator-label').textContent = 'Match ANY (OR)';
    if (triggerSearch) {
        populateMembersGrid([], true);
    }
}

/**
 * Renders the member cards in the grid.
 */
function populateMembersGrid(members, showInitialMessage = false) {
    elements.membersGrid.innerHTML = "";
    elements.counter.textContent = `(${members.length} Members)`;
    if (showInitialMessage) {
        elements.membersGrid.innerHTML = `<div class="col-12"><div class="alert alert-info text-center"><i class="bi bi-search me-2"></i>Please use a search method to find members.</div></div>`;
        elements.counter.textContent = '';
        elements.gridControls.classList.add('d-none');
        return;
    }
    if (members.length === 0) {
        elements.membersGrid.innerHTML = '<div class="col-12"><div class="alert alert-warning text-center">No members found matching your search criteria.</div></div>';
        elements.gridControls.classList.add('d-none');
        return;
    }
    elements.gridControls.classList.remove('d-none');
    members.forEach(member => {
        const isSelected = selectedMemberIDs.has(member.memberID);
        const memberCard = `<div class="col-xl-4 col-lg-6 col-12"><div class="card h-100 shadow-sm position-relative"><div class="position-absolute top-0 start-0 p-2" style="z-index: 10;"><input class="form-check-input member-select-checkbox" type="checkbox" value="${member.memberID}" ${isSelected ? 'checked' : ''}></div><div class="card-body"><div class="d-flex justify-content-between align-items-start"><h5 class="card-title mb-1 ms-4">${member.fullName || 'N/A'}</h5><div class="dropdown"><button class="btn btn-sm btn-light" type="button" data-bs-toggle="dropdown" aria-expanded="false"><i class="bi bi-three-dots-vertical"></i></button><ul class="dropdown-menu dropdown-menu-end"><li><a href="#" class="dropdown-item" onclick="showMemberDetailsModal(${member.memberID}, null)">View</a></li><li><a href="#" class="dropdown-item" onclick="showEditMemberModal(${member.memberID})">Edit</a></li><li><hr class="dropdown-divider"></li><li><a href="#" class="dropdown-item text-danger" onclick="showMemberDeleteModal(${member.memberID}, null)">Delete</a></li></ul></div></div><div class="card-text"><p class="mb-1"><small class="text-muted"><strong>Code:</strong> ${member.code || 'N/A'}</small></p><p class="mb-1"><strong>UN File #:</strong> ${member.unFileNumber || 'N/A'}</p><p class="mb-2"><strong>UN Personal #:</strong> ${member.unPersonalNumber || 'N/A'}</p><p class="mb-2"><strong>Card Prints #:</strong> ${member.cardDeliveryCount || 'N/A'}</p><span class="badge bg-primary me-2">${member.gender === 'M' ? 'Male' : 'Female'}</span><span class="badge ${member.baptised ? 'bg-success' : 'bg-secondary'} me-2">Baptised: ${member.baptised ? 'Yes' : 'No'}</span><span class="badge bg-info text-dark">${member.cardStatus || 'N/A'}</span>${member.mobile ? `<div class="mt-3 d-grid"><a href="tel:${member.mobile}" class="btn btn-outline-success btn-sm"><i class="bi bi-telephone-fill me-2"></i> ${member.mobile}</a></div>` : ''}</div></div></div></div>`;
        elements.membersGrid.insertAdjacentHTML('beforeend', memberCard);
    });
    updateBulkActionsUI();
}

/**
 * Shows or hides the bulk actions panel and updates counts.
 */
function updateBulkActionsUI() {
    const count = selectedMemberIDs.size;
    if (count > 0) {
        elements.bulkActionsDropdown.classList.remove('d-none');
        elements.selectedCount.textContent = count;
    } else {
        elements.bulkActionsDropdown.classList.add('d-none');
    }
    const visibleCheckboxes = elements.membersGrid.querySelectorAll('.member-select-checkbox');
    if (visibleCheckboxes.length > 0) {
        elements.selectAllCheckbox.checked = count > 0 && count === visibleCheckboxes.length;
        elements.selectAllCheckbox.indeterminate = count > 0 && count < visibleCheckboxes.length;
    } else {
        elements.selectAllCheckbox.checked = false;
        elements.selectAllCheckbox.indeterminate = false;
    }
}

/**
 * Clears all selected members and updates the UI.
 */
function clearSelection() {
    selectedMemberIDs.clear();
    updateBulkActionsUI();
}

// --- Bulk Action Functions (Placeholders) ---
function bulkPrintCards(memberIds) {
    console.log("Printing cards for member IDs:", memberIds);
    alert(`Printing cards for ${memberIds.length} members.`);
}

function bulkUpdateBaptism(memberIds) {
    console.log("Updating baptism status for member IDs:", memberIds);
    alert(`Updating baptism status for ${memberIds.length} members.`);
}

function bulkDeleteMembers(memberIds) {
    console.log("Deleting member IDs:", memberIds);
    if (confirm(`Are you sure you want to delete ${memberIds.length} members?`)) {
        alert('Deletion logic goes here.');
    }
}

function showErrorMessage(message) { alert(`ERROR: ${message}`); }
function showMemberDeleteModal(memberID, _) { alert(`Showing delete confirmation for member ID: ${memberID}`); }