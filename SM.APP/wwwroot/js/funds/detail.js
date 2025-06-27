class FundDetailManager {
    constructor() {
        this.fundId = this.getFundIdFromUrl();
        this.fund = null;
        this.assignableServants = [];
        
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
        if (!this.fundId) {
            window.location.href = '/Funds';
            return;
        }
        
        this.bindEvents();
        this.loadFundDetails();
        this.loadAssignableServants();
    }

    getFundIdFromUrl() {
        const pathParts = window.location.pathname.split('/');
        return pathParts[pathParts.length - 1];
    }

    bindEvents() {
        // Action buttons
        document.getElementById('editFundBtn')?.addEventListener('click', (e) => {
            e.preventDefault();
            this.showEditModal();
        });
        document.getElementById('updateStatusBtn')?.addEventListener('click', (e) => {
            e.preventDefault();
            this.showUpdateStatusModal();
        });
        document.getElementById('addNotesBtn')?.addEventListener('click', (e) => {
            e.preventDefault();
            this.showAddNotesModal();
        });
        document.getElementById('addNoteBtn')?.addEventListener('click', (e) => {
            e.preventDefault();
            this.showAddNotesModal();
        });
        document.getElementById('deleteFundBtn')?.addEventListener('click', (e) => {
            e.preventDefault();
            this.deleteFund();
        });

        // Modal save buttons
        document.getElementById('saveEditBtn')?.addEventListener('click', () => this.saveFundEdit());
        document.getElementById('saveUpdateStatusBtn')?.addEventListener('click', () => this.updateFundStatus());
        document.getElementById('saveNotesBtn')?.addEventListener('click', () => this.saveNotes());

        // Status change handler
        document.getElementById('newStatus')?.addEventListener('change', (e) => {
            const approvedAmountGroup = document.getElementById('approvedAmountGroup');
            if (approvedAmountGroup) {
                approvedAmountGroup.style.display = e.target.value === 'Approved' ? 'block' : 'none';
            }
        });

        // Back button
        document.querySelector('.btn-outline-light')?.addEventListener('click', () => {
            window.location.href = '/Funds';
        });
    }

    async loadFundDetails() {
        try {
            this.showLoading(true);
            
            const response = await fetch(`${apiBaseUrl}/api/Fund/${this.fundId}`, {
                credentials: 'include'
            });
            if (!response.ok) {
                if (response.status === 404) {
                    this.showError('Fund not found');
                    setTimeout(() => window.location.href = '/Funds', 2000);
                    return;
                }
                throw new Error('Failed to load fund details');
            }

            this.fund = await response.json();
            this.populateFundDetails();
        } catch (error) {
            this.showError('Failed to load fund details');
        } finally {
            this.showLoading(false);
        }
    }

    async loadAssignableServants() {
        try {
            const response = await fetch(`${apiBaseUrl}/api/Fund/assignable-servants`, {
                credentials: 'include'
            });
            if (response.ok) {
                this.assignableServants = await response.json();
                this.populateAssigneeDropdowns();
            }
        } catch (error) {
            // Silently handle - this is not critical functionality
        }
    }

    populateFundDetails() {
        if (!this.fund) return;

        // Map status numbers to names
        const statusMap = {
            0: 'Open',
            1: 'Approved',
            2: 'Rejected',
            3: 'Delivered'
        };
        let statusText = this.fund.status;
        if (typeof statusText === 'number') {
            statusText = statusMap[statusText] || statusText;
        }

        // Header information
        document.getElementById('fundNumber').textContent = this.fund.fundID;
        document.getElementById('fundStatus').textContent = statusText;
        document.getElementById('fundStatus').className = `badge ${this.getStatusBadgeClass(statusText)}`;
        document.getElementById('creationDate').textContent = this.formatDate(this.fund.requestDate);

        // Fund information
        document.getElementById('memberName').textContent = this.fund.member?.fullName || 'Unknown Member';
        document.getElementById('memberCode').textContent = this.fund.member?.code || 'N/A';
        document.getElementById('requestDescription').textContent = this.fund.requestDescription || 'No description';
        document.getElementById('requestedAmount').textContent = this.formatCurrency(this.fund.requestedAmount);
        document.getElementById('approvedAmount').textContent = this.fund.approvedAmount ? 
            this.formatCurrency(this.fund.approvedAmount) : 'Not set';
        document.getElementById('fundCategory').textContent = this.fund.fundCategory || 'Other';
        document.getElementById('assignedTo').textContent = this.fund.servant?.servantName || 'Unassigned';

        // Timeline - Use numeric status values (1=Approved, 3=Delivered)
        document.getElementById('createdDate').textContent = this.formatDate(this.fund.requestDate);
        
        if (this.fund.status === this.STATUS.APPROVED || this.fund.status === this.STATUS.DELIVERED) {
            const approvedTimeline = document.getElementById('approvedTimeline');
            if (approvedTimeline) {
                approvedTimeline.style.display = 'block';
                document.getElementById('approverName').textContent = this.fund.approver?.servantName || 'Unknown';
                // Note: We'd need approval date from the backend
            }
        }

        if (this.fund.status === this.STATUS.DELIVERED) {
            const completedTimeline = document.getElementById('completedTimeline');
            if (completedTimeline) {
                completedTimeline.style.display = 'block';
                // Note: We'd need completion date from the backend
            }
        }

        // Notes
        this.populateNotes();

        // Update action buttons based on status and permissions
        this.updateActionButtons();
    }

    populateNotes() {
        const notesContainer = document.getElementById('notesContainer');
        if (!notesContainer) return;

        if (!this.fund.approverNotes || this.fund.approverNotes.trim() === '') {
            notesContainer.innerHTML = `
                <div class="text-center text-muted py-3">
                    <i class="fas fa-sticky-note fa-2x mb-2"></i>
                    <p>No notes available</p>
                </div>
            `;
            return;
        }

        // Parse notes (assuming they're in format: [timestamp - user]: note)
        const notes = this.parseNotes(this.fund.approverNotes);
        notesContainer.innerHTML = notes.map(note => `
            <div class="note-item mb-3 p-3 border rounded">
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <strong class="text-primary">${note.author}</strong>
                    <small class="text-muted">${note.timestamp}</small>
                </div>
                <p class="mb-0">${note.content}</p>
            </div>
        `).join('');
    }

    parseNotes(notesString) {
        const lines = notesString.split('\n');
        const notes = [];
        
        for (const line of lines) {
            const match = line.match(/^\[(.*?) - (.*?)\]: (.*)$/);
            if (match) {
                notes.push({
                    timestamp: match[1],
                    author: match[2],
                    content: match[3]
                });
            } else if (line.trim()) {
                // Fallback for notes without proper format
                notes.push({
                    timestamp: 'Unknown',
                    author: 'System',
                    content: line.trim()
                });
            }
        }
        
        return notes;
    }

    populateAssigneeDropdowns() {
        const editAssigneeSelect = document.getElementById('editAssigneeSelect');
        if (editAssigneeSelect && this.assignableServants) {
            editAssigneeSelect.innerHTML = '<option value="">Select Assignee</option>' +
                this.assignableServants.map(servant => 
                    `<option value="${servant.servantID}">${servant.servantName}</option>`
                ).join('');
        }
    }

    updateActionButtons() {
        const editBtn = document.getElementById('editFundBtn');
        const updateStatusBtn = document.getElementById('updateStatusBtn');
        const deleteBtn = document.getElementById('deleteFundBtn');

        // Only allow editing of open funds
        if (editBtn) {
            editBtn.style.display = this.fund.status === this.STATUS.OPEN ? 'block' : 'none';
        }

        // Only allow deletion of open funds (and only for admins - this would need server-side check)
        if (deleteBtn) {
            deleteBtn.style.display = this.fund.status === this.STATUS.OPEN ? 'block' : 'none';
        }
    }

    showEditModal() {
        if (!this.fund) return;

        // Populate edit form
        document.getElementById('editFundId').value = this.fund.fundID;
        document.getElementById('editAssigneeSelect').value = this.fund.servantID || '';
        document.getElementById('editFundCategory').value = this.fund.fundCategory || '';
        document.getElementById('editRequestDescription').value = this.fund.requestDescription || '';
        document.getElementById('editRequestAmount').value = this.fund.requestedAmount || '';

        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('editFundModal'));
        modal.show();
    }

    showUpdateStatusModal() {
        if (!this.fund) return;

        document.getElementById('statusFundId').value = this.fund.fundID;
        document.getElementById('newStatus').value = '';
        document.getElementById('newApprovedAmount').value = '';
        document.getElementById('statusNotes').value = '';
        
        // Hide approved amount initially
        document.getElementById('approvedAmountGroup').style.display = 'none';

        const modal = new bootstrap.Modal(document.getElementById('updateStatusModal'));
        modal.show();
    }

    showAddNotesModal() {
        if (!this.fund) return;

        document.getElementById('notesFundId').value = this.fund.fundID;
        document.getElementById('newNotes').value = '';

        const modal = new bootstrap.Modal(document.getElementById('addNotesModal'));
        modal.show();
    }

    async saveFundEdit() {
        try {
            const requestedAmountValue = document.getElementById('editRequestAmount').value;
            const formData = {
                fundID: parseInt(document.getElementById('editFundId').value),
                servantID: parseInt(document.getElementById('editAssigneeSelect').value),
                fundCategory: document.getElementById('editFundCategory').value,
                requestDescription: document.getElementById('editRequestDescription').value,
                requestedAmount: requestedAmountValue ? parseFloat(requestedAmountValue) : null,
                approverNotes: document.getElementById('editNotes').value
            };

            const response = await fetch(`${apiBaseUrl}/api/Fund/${this.fundId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error('Failed to update fund');
            }

            // Close modal and reload data
            bootstrap.Modal.getInstance(document.getElementById('editFundModal')).hide();
            this.showSuccess('Fund updated successfully');
            this.loadFundDetails();

        } catch (error) {
            this.showError('Failed to update fund');
        }
    }

    async updateFundStatus() {
        try {
            const formData = {
                fundID: parseInt(document.getElementById('statusFundId').value),
                status: document.getElementById('newStatus').value,
                approverNotes: document.getElementById('statusNotes').value,
                approvedAmount: document.getElementById('newApprovedAmount').value ? 
                    parseFloat(document.getElementById('newApprovedAmount').value) : null
            };

            const response = await fetch(`${apiBaseUrl}/api/Fund/${this.fundId}/status`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error('Failed to update status');
            }

            // Close modal and reload data
            bootstrap.Modal.getInstance(document.getElementById('updateStatusModal')).hide();
            this.showSuccess('Status updated successfully');
            this.loadFundDetails();

        } catch (error) {
            this.showError('Failed to update status');
        }
    }

    async saveNotes() {
        try {
            const formData = {
                fundID: parseInt(document.getElementById('notesFundId').value),
                approverNotes: document.getElementById('newNotes').value
            };

            const response = await fetch(`${apiBaseUrl}/api/Fund/${this.fundId}/notes`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error('Failed to add notes');
            }

            // Close modal and reload data
            bootstrap.Modal.getInstance(document.getElementById('addNotesModal')).hide();
            this.showSuccess('Notes added successfully');
            this.loadFundDetails();

        } catch (error) {
            this.showError('Failed to add notes');
        }
    }

    async deleteFund() {
        if (!confirm('Are you sure you want to delete this fund request? This action cannot be undone.')) {
            return;
        }

        try {
            const response = await fetch(`${apiBaseUrl}/api/Fund/${this.fundId}`, {
                method: 'DELETE',
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error('Failed to delete fund');
            }

            this.showSuccess('Fund deleted successfully');
            setTimeout(() => window.location.href = '/Funds', 1500);

        } catch (error) {
            this.showError('Failed to delete fund');
        }
    }

    getStatusBadgeClass(status) {
        const classes = {
            'Open': 'bg-warning text-dark',
            'Approved': 'bg-success',
            'Rejected': 'bg-danger',
            'Delivered': 'bg-info'
        };
        return classes[status] || 'bg-secondary';
    }

    formatDate(dateString) {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    formatCurrency(amount) {
        if (!amount && amount !== 0) return 'Not specified';
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    }

    showLoading(show) {
        const spinner = document.getElementById('loadingSpinner');
        const content = document.getElementById('fundContent');
        
        if (spinner) spinner.style.display = show ? 'block' : 'none';
        if (content) content.style.display = show ? 'none' : 'block';
    }

    showSuccess(message) {
        this.showAlert(message, 'success');
    }

    showError(message) {
        this.showAlert(message, 'danger');
    }

    showAlert(message, type) {
        const alert = document.createElement('div');
        alert.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        alert.style.top = '20px';
        alert.style.right = '20px';
        alert.style.zIndex = '9999';
        alert.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        document.body.appendChild(alert);

        // Auto-remove after 5 seconds
        setTimeout(() => {
            if (alert.parentNode) {
                alert.parentNode.removeChild(alert);
            }
        }, 5000);
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new FundDetailManager();
}); 