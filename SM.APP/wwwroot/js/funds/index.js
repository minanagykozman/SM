(async function () {
    let allFunds = [];

    //Scanning code
    const scanButton = document.getElementById("scanMemberCodeBtn");
    const qrScannerContainer = document.getElementById("qrScannerContainer");
    const stopScanButton = document.getElementById("btnStopScan");
    const searchInput = document.getElementById("filterMemberCode");
    if (scanButton) {
        let html5QrCode;
        scanButton.addEventListener("click", function () {
            qrScannerContainer.classList.remove("d-none");
            html5QrCode = new Html5Qrcode("reader");
            html5QrCode.start(
                { facingMode: "environment" },
                { fps: 10, qrbox: { width: 250, height: 250 } },
                (decodedText) => {
                    searchInput.value = decodedText;
                    document.getElementById('applyFiltersBtn').click();
                    html5QrCode.stop();
                    qrScannerContainer.classList.add("d-none");
                },
                (errorMessage) => { console.warn("QR Code scan error: ", errorMessage); }
            ).catch((err) => { console.error("QR Code scanning failed: ", err); });
        });
        stopScanButton.addEventListener("click", function () {
            if (html5QrCode) { html5QrCode.stop(); }
            qrScannerContainer.classList.add("d-none");
        });
    }

    // --- ACTION FUNCTIONS ---
    function editFund(fund) {
        showEditModal(fund);
    }
    function updateFundStatus(fund) {
        showUpdateStatusModal(fund);
    }
    async function deleteFund(fund) {
        showDeleteFundModal(fund);
    }

    // *** NEW HELPER FUNCTION FOR STATUS BADGES ***
    function getStatusBadgeClass(statusName) {
        switch (statusName) {
            case 'Open':
                return 'bg-warning text-dark';
            case 'Approved':
                return 'bg-success';
            case 'Rejected':
                return 'bg-danger';
            case 'Delivered':
                return 'bg-info';
            default:
                return 'bg-secondary';
        }
    }

    async function loadServants() {
        try {
            const dropdown = $("#filterAssignedTo");
            const response = await fetch(`${apiBaseUrl}/Servants/GetServants`, { method: "GET", credentials: "include" });
            if (!response.ok) throw new Error("Failed to fetch servants.");
            const servants = await response.json();
            dropdown.empty();
            servants.forEach(s => dropdown.append($('<option>', { value: s.servantID, text: s.servantName })));
            dropdown.selectpicker('refresh');
        } catch (err) { console.error("Error loading servants:", err); }
    }

    async function loadFunds() {
        try {
            const response = await fetch(`${apiBaseUrl}/api/Fund/GetAllFunds`, { method: "GET", credentials: "include" });
            if (!response.ok) throw new Error("Failed to fetch funds.");
            allFunds = await response.json();
            applyFilters();
        } catch (err) {
            console.error("Error loading funds:", err);
            $("#fund-list").html('<div class="list-group-item text-center text-danger p-5">Failed to load fund requests.</div>');
        }
    }

    // --- RENDERING FUNCTIONS ---
    function renderCategoryPills(funds, activeCategory) {
        const categories = {};
        funds.forEach(fund => {
            const category = fund.fundCategory;
            if (!categories[category]) { categories[category] = { count: 0 }; }
            categories[category].count++;
        });
        const pillsContainer = $('#category-pills-container');
        pillsContainer.empty();
        const finalActiveCategory = (activeCategory && categories[activeCategory]) ? activeCategory : 'all';
        pillsContainer.append(`<li class="nav-item"><a class="nav-link ${finalActiveCategory === 'all' ? 'active' : ''}" href="#" data-category="all">All (${funds.length})</a></li>`);
        for (const categoryName in categories) {
            pillsContainer.append(`<li class="nav-item"><a class="nav-link ${finalActiveCategory === categoryName ? 'active' : ''}" href="#" data-category="${categoryName}">${categoryName} (${categories[categoryName].count})</a></li>`);
        }
    }

    function renderSummary(funds) {
        const totalRequests = funds.length;
        const totalAmount = funds.reduce((sum, fund) => sum + (fund.approvedAmount || 0), 0);
        $('#total-requests-count').text(totalRequests);
        $('#total-approved-amount').text(`${totalAmount.toLocaleString('en-US')} EGP`);
    }

    function renderFunds(funds) {
        const fundListContainer = $('#fund-list');
        fundListContainer.empty();
        if (funds.length === 0) {
            fundListContainer.html('<div class="list-group-item text-center p-5">No fund requests match your criteria.</div>');
            return;
        }
        funds.forEach(fund => {
            let deleteButtonHtml = '';
            if (fund.statusName === 'Open') {
                deleteButtonHtml = `<li><hr class="dropdown-divider"></li><li><button class="dropdown-item text-danger delete-btn" type="button"><i class="bi bi-trash me-2"></i>Delete</button></li>`;
            }

            // *** THE FIX: Call the helper function to get the correct badge class ***
            const statusBadgeClass = getStatusBadgeClass(fund.statusName);

            const fundItemHtml = `
                        <div class="list-group-item" data-fund-id="${fund.fundID}">
                            <div class="d-flex w-100 justify-content-between align-items-start">
                                <div class="me-3">
                                    <h5 class="mb-0">${fund.member.code}</h5>
                                    <small class="text-muted d-block mb-1">${fund.member.fullName}</small>
                                    <p class="mb-1 text-muted">${fund.requestDescription}</p>
                                    <small class="text-muted"><i class="bi bi-calendar-event"></i> ${formatDate(fund.requestDate)}</small>
                                </div>
                                <div class="d-flex align-items-center text-nowrap">
                                    <span class="badge bg-primary me-2">${fund.fundCategory}</span>
                                    <span class="badge ${statusBadgeClass} me-3">${fund.statusName}</span>
                                    <div class="dropdown">
                                        <button class="btn btn-sm btn-outline-secondary py-0 px-2" type="button" data-bs-toggle="dropdown"><i class="bi bi-three-dots-vertical"></i></button>
                                        <ul class="dropdown-menu dropdown-menu-end">
                                            <li><a class="dropdown-item" href="/Funds/Detail/${fund.fundID}"><i class="bi bi-eye me-2"></i>View</a></li>
                                            <li><button class="dropdown-item edit-btn" type="button"><i class="bi bi-pencil me-2"></i>Edit</button></li>
                                            <li><button class="dropdown-item update-status-btn" type="button"><i class="bi bi-check2-square me-2"></i>Update Status</button></li>
                                            ${deleteButtonHtml}
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
            fundListContainer.append(fundItemHtml);
        });
    }

    // --- FILTERING LOGIC ---
    function applyFilters() {
        const codeFilter = $('#filterMemberCode').val().toLowerCase();
        const assignedToFilter = $('#filterAssignedTo').val();
        const statusFilter = $('#filterStatus').val();
        let activeCategory = $('#category-pills-container .nav-link.active').data('category');
        let mainFilteredFunds = allFunds.filter(fund => {
            const codeMatch = !codeFilter || fund.member.code.toLowerCase().includes(codeFilter);
            const assignedToMatch = !assignedToFilter || fund.approverID == assignedToFilter;
            const statusMatch = statusFilter.length === 0 || statusFilter.includes(fund.statusName);
            return codeMatch && assignedToMatch && statusMatch;
        });
        renderCategoryPills(mainFilteredFunds, activeCategory);
        const finalActiveCategory = $('#category-pills-container .nav-link.active').data('category');
        let finalFilteredFunds = mainFilteredFunds;
        if (finalActiveCategory && finalActiveCategory !== 'all') {
            finalFilteredFunds = mainFilteredFunds.filter(fund => fund.fundCategory === finalActiveCategory);
        }
        renderFunds(finalFilteredFunds);
        renderSummary(finalFilteredFunds);
    }

    // --- EVENT HANDLERS ---
    $('#applyFiltersBtn').on('click', applyFilters);
    $('#clearFiltersBtn').on('click', function () {
        $('#filterMemberCode').val('');
        $('#filterAssignedTo').selectpicker('val', '');
        $('#filterStatus').selectpicker('val', '');
        $('#category-pills-container .nav-link').removeClass('active');
        $('#category-pills-container .nav-link[data-category="all"]').addClass('active');
        applyFilters();
    });
    $('#category-pills-container').on('click', '.nav-link', function (e) {
        e.preventDefault();
        $('#category-pills-container .nav-link').removeClass('active');
        $(this).addClass('active');
        applyFilters();
    });
    $('#fund-list').on('click', '.edit-btn, .update-status-btn, .delete-btn', function () {
        const fundId = $(this).closest('.list-group-item').data('fund-id');
        const fund = allFunds.find(f => f.fundID == fundId);
        if (!fund) return;
        if ($(this).hasClass('edit-btn')) editFund(fund);
        else if ($(this).hasClass('update-status-btn')) updateFundStatus(fund);
        else if ($(this).hasClass('delete-btn')) deleteFund(fund);
    });

    $(document).on('fundDataChanged', function () {
        console.log('Fund data changed, reloading list...');
        loadFunds();
    });

    // --- INITIAL PAGE LOAD ---
    $('.selectpicker').selectpicker();
    await loadServants();
    await loadFunds();
})();