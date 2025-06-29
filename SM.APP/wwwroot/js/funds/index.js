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

        // Member lookup
        $('#memberCode').on('input', this.debounce((e) => this.lookupMember(e.target.value), 300));
        $('#memberCode').on('blur', (e) => this.validateMember(e.target.value));

        // Form validation
        $('#createFundForm input, #createFundForm select, #createFundForm textarea').on('change', () => {
            this.validateForm();
        });
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
            // Add any current filters to the board view request
            const assigneeFilter = document.getElementById('assigneeFilter');
            if (assigneeFilter && assigneeFilter.value) {
                params.set('assigneeId', assigneeFilter.value);
            }
            
            const url = `${apiBaseUrl}/api/Fund/by-status?${params}`;
            
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

    async lookupMember(memberCode) {
        const memberInfo = $('#memberInfo');
        
        if (!memberCode || memberCode.length < 2) {
            memberInfo.hide();
            return;
        }

        try {
            const response = await fetch(`${apiBaseUrl}/Member/GetMemberByCode?memberCode=${encodeURIComponent(memberCode)}`, {
                credentials: 'include'
            });
            if (response.ok) {
                const member = await response.json();
                if (member) {
                    memberInfo.html(`
                        <div class="alert alert-success">
                            <strong>${member.unFirstName} ${member.unLastName}</strong><br>
                            <small>Member Code: ${member.code}</small>
                        </div>
                    `).show();
                    
                    // Store member ID for form submission
                    $('#memberID').val(member.memberID);
                } else {
                    memberInfo.html(`
                        <div class="alert alert-warning">
                            Member not found
                        </div>
                    `).show();
                    $('#memberID').val('');
                }
            }
        } catch (error) {
            console.error('Error looking up member:', error);
            memberInfo.html(`
                <div class="alert alert-danger">
                    Error looking up member
                </div>
            `).show();
        }
    }

    validateMember(memberCode) {
        const memberInfo = $('#memberInfo');
        const memberID = $('#memberID').val();
        
        if (memberCode && !memberID) {
            memberInfo.html(`
                <div class="alert alert-danger">
                    Please select a valid member
                </div>
            `).show();
        }
    }

    showCreateModal() {
        // Reset form
        $('#createFundForm')[0].reset();
        $('#memberInfo').hide();
        $('#memberID').val('');
        
        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('createFundModal'));
        modal.show();
    }

    async saveFund() {
        if (!this.validateForm()) {
            return;
        }

        try {
            const formData = {
                memberID: parseInt($('#memberID').val()),
                requestDescription: $('#requestDescription').val(),
                servantID: parseInt($('#servantID').val()),
                requestedAmount: parseFloat($('#requestedAmount').val()),
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
        const requestedAmount = $('#requestedAmount').val();

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

        if (!requestedAmount || parseFloat(requestedAmount) <= 0) {
            this.showError('Please enter a valid amount');
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