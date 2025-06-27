class FundManager {
    constructor() {
        this.currentView = 'list';
        this.filters = {
            status: '',
            assignee: '',
            search: ''
        };
        
        // Fund Status Constants (matching C# enum values)
        this.STATUS = {
            OPEN: 0,
            APPROVED: 1,
            REJECTED: 2,
            DELIVERED: 3
        };
        
        this.CATEGORIES = {
            RENT: 'Rent',
            SHANTAT_BARAKA: 'ShantatBaraka',
            MEDICAL: 'Medical',
            SCHOOL_FEES: 'SchoolFees',
            OTHERS: 'Others'
        };
        
        this.init();
    }

    init() {
        this.bindEvents();
        this.updateFundCount();
        this.setupViewToggle();
    }

    bindEvents() {
        // View toggle buttons - using the correct IDs from the HTML
        $('#listView').on('change', () => {
            if ($('#listView').is(':checked')) {
                this.toggleView('list');
            }
        });
        $('#boardView').on('change', () => {
            if ($('#boardView').is(':checked')) {
                this.toggleView('board');
            }
        });

        // Filter controls
        $('#applyFiltersBtn').on('click', () => this.applyFilters());
        $('#clearFiltersBtn').on('click', () => this.clearFilters());

        // Create fund modal
        $('#createFundBtn').on('click', () => this.showCreateModal());
        $('#saveFundBtn').on('click', () => this.saveFund());

        // Form submit handler
        $('#createFundForm').on('submit', (e) => {
            e.preventDefault(); // Prevent default form submission
            this.saveFund(); // Call our custom save method
        });

        // Reset modal when dismissed
        $('#createFundModal').on('hidden.bs.modal', () => this.resetCreateModal());

        // Member lookup - modified to support dropdown search
        $('#memberCode').on('input', this.debounce((e) => this.searchMembers(e.target.value), 300));
        $('#memberCode').on('blur', (e) => {
            // Delay hiding dropdown to allow for click events
            setTimeout(() => this.hideSearchResults(), 200);
        });
        $('#memberCode').on('focus', () => {
            if ($('#memberCode').val().length >= 2) {
                this.searchMembers($('#memberCode').val());
            }
        });

        // Search and Scan buttons - using standardized QR scanner approach
        $('#btnSearchMember').on('click', () => this.searchMembersManually());
        
        // Use standardized QR scanner implementation
        const scanButton = document.getElementById("btnScanQR");
        const qrScannerContainer = document.getElementById("qrScannerContainer");
        const stopScanButton = document.getElementById("btnStopScan");
        
        if (scanButton) {
            let html5QrCode;
            scanButton.addEventListener("click", () => {
                qrScannerContainer.classList.remove("d-none");

                html5QrCode = new Html5Qrcode("reader");
                html5QrCode.start(
                    { facingMode: "environment" }, // Use back camera
                    {
                        fps: 10,
                        qrbox: { width: 250, height: 250 }
                    },
                    (decodedText) => {
                        // Set the decoded text and search for members
                        $('#memberCode').val(decodedText);
                        this.searchMembers(decodedText);

                        html5QrCode.stop();
                        qrScannerContainer.classList.add("d-none");
                    },
                    (errorMessage) => {
                        console.warn("QR Code scan error: ", errorMessage);
                    }
                ).catch((err) => {
                    console.error("QR Code scanning failed: ", err);
                    this.showError('Failed to start QR scanner. Please try again.');
                    qrScannerContainer.classList.add("d-none");
                });
            });
            
            // Stop scanning
            stopScanButton.addEventListener("click", () => {
                if (html5QrCode) {
                    html5QrCode.stop();
                }
                qrScannerContainer.classList.add("d-none");
            });
        }

        // Click outside to hide dropdown
        $(document).on('click', (e) => {
            if (!$(e.target).closest('#memberCode, #memberSearchResults').length) {
                this.hideSearchResults();
            }
        });

        // Form validation - removed automatic validation on change events
        // Validation now only happens on form submission
    }

    setupViewToggle() {
        const listView = document.getElementById('listViewContainer');
        const boardView = document.getElementById('boardViewContainer');
        
        if (this.currentView === 'list') {
            listView.style.display = 'block';
            boardView.style.display = 'none';
        } else {
            listView.style.display = 'none';
            boardView.style.display = 'block';
        }
    }

    toggleView(view) {
        this.currentView = view;
        this.setupViewToggle();
        
        // Load board data when switching to board view
        if (view === 'board') {
            this.loadBoardView();
        }
    }

    async loadBoardView() {
        try {
            this.showLoading(true);
            
            const params = new URLSearchParams();
            
            // Add all current filters to the board view request
            const assigneeFilter = document.getElementById('assigneeFilter');
            const statusFilter = document.getElementById('statusFilter');
            const searchInput = document.getElementById('searchTerm');
            
            if (assigneeFilter && assigneeFilter.value) {
                params.set('assigneeId', assigneeFilter.value);
            }
            
            if (statusFilter && statusFilter.value) {
                params.set('status', statusFilter.value);
            }
            
            if (searchInput && searchInput.value) {
                params.set('searchTerm', searchInput.value);
            }
            
            const url = `${apiBaseUrl}/api/Fund/status?${params}`;
            
            const response = await fetch(url, {
                credentials: 'include'
            });
            
            if (!response.ok) throw new Error('Failed to load board data');
            
            const data = await response.json();
            
            this.populateBoardColumns(data);
        } catch (error) {
            console.error('Error loading board view:', error);
            this.showError('Failed to load board view');
        } finally {
            this.showLoading(false);
        }
    }

    populateBoardColumns(data) {
        const openColumn = document.getElementById('openFunds');
        const approvedColumn = document.getElementById('approvedFunds');
        const completedColumn = document.getElementById('completedFunds');

        if (openColumn) {
            const openHtml = this.renderFundCards(data.open || [], 'open');
            openColumn.innerHTML = openHtml;
        } else {
            console.error('openFunds element not found');
        }
        
        if (approvedColumn) {
            const approvedHtml = this.renderFundCards(data.approved || [], 'approved');
            approvedColumn.innerHTML = approvedHtml;
        } else {
            console.error('approvedFunds element not found');
        }
        
        if (completedColumn) {
            const completedHtml = this.renderFundCards(data.completed || [], 'completed');
            completedColumn.innerHTML = completedHtml;
        } else {
            console.error('completedFunds element not found');
        }
    }

    renderFundCards(funds, columnType) {        
        if (!funds.length) {
            return '<div class="text-center text-muted py-3"><small>No funds in this status</small></div>';
        }

        // Map status numbers to names
        const statusMap = {
            0: 'Open',
            1: 'Approved',
            2: 'Rejected',
            3: 'Delivered'
        };

        try {
            const cards = funds.map(fund => {
                try {
                    const memberName = fund.member?.fullName || 'Unknown Member';
                    const description = fund.requestDescription || 'No description';
                    const amount = this.formatCurrency(fund.requestedAmount || 0);
                    const category = fund.fundCategory || 'Other';
                    const servantName = fund.servant?.servantName || 'Unassigned';
                    const fundId = fund.fundID || 0;
                    let statusBadge = '';
                    if (columnType === 'completed') {
                        // Map status number to string if needed
                        let statusText = fund.status;
                        if (typeof statusText === 'number') {
                            statusText = statusMap[statusText] || statusText;
                        }
                        let badgeClass = 'bg-secondary';
                        if (fund.status === this.STATUS.DELIVERED) badgeClass = 'bg-info';
                        if (fund.status === this.STATUS.REJECTED) badgeClass = 'bg-danger';
                        statusBadge = `<span class="badge ${badgeClass} ms-1">${statusText}</span>`;
                    }
                    return `
                    <div class="card mb-2 fund-card" onclick="window.location.href='/Funds/Detail/${fundId}'">
                        <div class="card-body p-3">
                            <h6 class="card-title mb-2">${memberName} ${statusBadge}</h6>
                            <p class="card-text small mb-2">${description}</p>
                            <div class="d-flex justify-content-between align-items-center">
                                <span class="badge bg-primary">${amount}</span>
                                <small class="text-muted">${category}</small>
                            </div>
                            <div class="mt-2">
                                <small class="text-muted">Assigned to: ${servantName}</small>
                            </div>
                        </div>
                    </div>
                `;
                } catch (cardError) {
                    console.error('Error rendering individual card:', cardError, fund);
                    return `<div class="alert alert-warning">Error rendering fund card</div>`;
                }
            });
            
            const result = cards.join('');
            return result;
        } catch (error) {
            console.error('Error in renderFundCards:', error);
            return '<div class="alert alert-danger">Error rendering fund cards</div>';
        }
    }

    applyFilters() {
        const statusFilter = document.getElementById('statusFilter');
        const assigneeFilter = document.getElementById('assigneeFilter');
        const searchInput = document.getElementById('searchTerm');

        // Update filter values
        this.filters.status = statusFilter?.value || '';
        this.filters.assignee = assigneeFilter?.value || '';
        this.filters.search = searchInput?.value || '';

        // Build query string
        const params = new URLSearchParams();
        if (this.filters.status) params.append('StatusFilter', this.filters.status);
        if (this.filters.assignee) params.append('AssigneeFilter', this.filters.assignee);
        if (this.filters.search) params.append('SearchTerm', this.filters.search);

        // Reload page with filters
        window.location.href = `${window.location.pathname}?${params}`;
    }

    clearFilters() {
        // Clear all filter inputs
        const statusFilter = document.getElementById('statusFilter');
        const assigneeFilter = document.getElementById('assigneeFilter');
        const searchInput = document.getElementById('searchTerm');

        if (statusFilter) statusFilter.value = '';
        if (assigneeFilter) assigneeFilter.value = '';
        if (searchInput) searchInput.value = '';

        // Reload page without filters
        window.location.href = window.location.pathname;
    }

    toggleMyFunds() {
        const params = new URLSearchParams(window.location.search);
        const currentAssignee = params.get('AssigneeFilter');
        
        if (currentAssignee) {
            // Remove the filter to show all funds
            params.delete('AssigneeFilter');
        } else {
            // Add current user filter (this would need to be set from server-side)
            // For now, we'll just reload to show the toggle effect
            params.set('myFunds', 'true');
        }
        
        window.location.href = `${window.location.pathname}?${params}`;
    }

    async searchMembers(query) {
        const searchResults = $('#memberSearchResults');
        
        if (!query || query.length < 2) {
            this.hideSearchResults();
            this.clearMemberSelection();
            return;
        }

        try {
            const response = await fetch(`${apiBaseUrl}/Member/SearchMembers?memberCode=${encodeURIComponent(query)}&firstName=&lastName=`, {
                credentials: 'include'
            });
            
            if (response.ok) {
                const members = await response.json();
                this.displaySearchResults(members || []);
            } else {
                this.hideSearchResults();
            }
        } catch (error) {
            console.error('Error searching members:', error);
            this.hideSearchResults();
        }
    }

    displaySearchResults(members) {
        const searchResults = $('#memberSearchResults');
        
        if (members.length === 0) {
            searchResults.html(`
                <div class="dropdown-item-text text-muted">
                    <i class="fas fa-exclamation-triangle me-2"></i>
                    No members found
                </div>
            `).addClass('show');
            return;
        }

        const resultsHtml = members.map(member => `
            <a href="#" class="dropdown-item member-option" data-member-id="${member.memberID}" 
               data-member-code="${member.code}" data-member-name="${member.unFirstName} ${member.unLastName}">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <strong>${member.unFirstName} ${member.unLastName}</strong><br>
                        <small class="text-muted">Code: ${member.code}</small>
                    </div>
                    <span class="badge bg-light text-dark">${member.cardStatusDisplay || 'N/A'}</span>
                </div>
            </a>
        `).join('');

        searchResults.html(resultsHtml).addClass('show');
        
        // Add click handlers for member selection
        searchResults.find('.member-option').on('click', (e) => {
            e.preventDefault();
            const memberOption = $(e.currentTarget);
            this.selectMember({
                memberID: memberOption.data('member-id'),
                code: memberOption.data('member-code'),
                name: memberOption.data('member-name')
            });
        });
    }

    selectMember(member) {
        $('#memberCode').val(member.code);
        $('#memberID').val(member.memberID);
        
        $('#memberInfo').html(`
            <div class="alert alert-success">
                <i class="fas fa-check-circle me-2"></i>
                <strong>${member.name}</strong><br>
                <small>Member Code: ${member.code}</small>
            </div>
        `).show();
        
        this.hideSearchResults();
    }

    clearMemberSelection() {
        $('#memberID').val('');
        $('#memberInfo').hide();
    }

    hideSearchResults() {
        $('#memberSearchResults').removeClass('show');
    }

    searchMembersManually() {
        const query = $('#memberCode').val().trim();
        if (query.length >= 1) {
            this.searchMembers(query);
        } else {
            this.showError('Please enter at least 1 character to search');
        }
    }

    showCreateModal() {
        // Reset form
        $('#createFundForm')[0].reset();
        $('#memberInfo').hide();
        $('#memberID').val('');
        this.hideSearchResults();
        this.clearMemberSelection();
        
        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('createFundModal'));
        modal.show();
    }

    resetCreateModal() {
        $('#createFundForm')[0].reset();
        $('#memberInfo').hide();
        $('#memberID').val('');
        this.hideSearchResults();
        this.clearMemberSelection();
        
        $('#validationErrors').empty();
        
        $('#createFundForm input, #createFundForm select, #createFundForm textarea').removeClass('is-invalid is-valid');
    }

    async saveFund() {
        if (!this.validateForm()) {
            return;
        }

        try {
            const requestedAmountValue = $('#requestedAmount').val();
            const formData = {
                memberID: parseInt($('#memberID').val()),
                requestDescription: $('#requestDescription').val(),
                servantID: parseInt($('#servantID').val()),
                requestedAmount: requestedAmountValue ? parseFloat(requestedAmountValue) : null,
                fundCategory: $('#fundCategory').val(),
                approverNotes: $('#approverNotes').val()
            };

            const response = await fetch(`${apiBaseUrl}/api/Fund`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
                credentials: 'include'
            });

            if (response.ok) {
                const result = await response.json();
                this.showSuccess('Fund request created successfully');
                
                // Close modal and reload page
                bootstrap.Modal.getInstance(document.getElementById('createFundModal')).hide();
                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            } else {
                throw new Error('Failed to create fund');
            }
        } catch (error) {
            console.error('Error creating fund:', error);
            this.showError('Failed to create fund request');
        }
    }

    validateForm() {
        const memberID = $('#memberID').val();
        const requestDescription = $('#requestDescription').val();
        const servantID = $('#servantID').val();
        const fundCategory = $('#fundCategory').val();

        if (!memberID) {
            this.showError('Please select a valid member');
            return false;
        }

        if (!requestDescription.trim()) {
            this.showError('Please enter a request description');
            return false;
        }

        if (!servantID) {
            this.showError('Please select an assignee');
            return false;
        }

        if (!fundCategory) {
            this.showError('Please select a category');
            return false;
        }

        return true;
    }

    updateFundCount() {
        const fundCount = document.querySelectorAll('#listViewContainer tbody tr:not([colspan])').length;
        const fundCountElement = document.getElementById('fundCount');
        if (fundCountElement) {
            fundCountElement.textContent = fundCount;
        }
    }

    formatCurrency(amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    }

    showLoading(show) {
        const spinner = document.getElementById('boardLoadingSpinner');
        if (spinner) {
            spinner.style.display = show ? 'block' : 'none';
        }
    }

    showSuccess(message) {
        this.showToast(message, 'success');
    }

    showError(message) {
        this.showToast(message, 'error');
    }

    showToast(message, type = 'info') {
        // Create toast container if it doesn't exist
        let toastContainer = document.getElementById('toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toast-container';
            toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
            toastContainer.style.zIndex = '1055';
            document.body.appendChild(toastContainer);
        }

        // Create toast element
        const toastId = 'toast-' + Date.now();
        const toastHtml = `
            <div id="${toastId}" class="toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'error' ? 'danger' : 'primary'} border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-triangle' : 'info-circle'} me-2"></i>
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;

        // Add toast to container
        toastContainer.insertAdjacentHTML('beforeend', toastHtml);

        // Initialize and show toast
        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement, {
            autohide: true,
            delay: type === 'error' ? 5000 : 3000 // Show errors longer
        });
        
        toast.show();

        // Remove toast element after it's hidden
        toastElement.addEventListener('hidden.bs.toast', () => {
            toastElement.remove();
        });
    }

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
}

// Initialize when DOM is ready
$(document).ready(function() {
    new FundManager();
}); 