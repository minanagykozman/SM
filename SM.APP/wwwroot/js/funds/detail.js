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
        this.setupFundDataListener();
    }

    getFundIdFromUrl() {
        const pathParts = window.location.pathname.split('/');
        return pathParts[pathParts.length - 1];
    }

    bindEvents() {
        // Action buttons - Desktop
        document.getElementById('editFundBtn')?.addEventListener('click', (e) => {
            e.preventDefault();
            console.log('Edit button clicked');
            this.showEditModal();
        });
        document.getElementById('updateStatusBtn')?.addEventListener('click', (e) => {
            e.preventDefault();
            console.log('Update status button clicked');
            // Use the global function from the partial
            if (typeof showUpdateStatusModal === 'function') {
                showUpdateStatusModal(this.fund);
            } else {
                this.showUpdateStatusModal();
            }
        });
        document.getElementById('addNoteBtn')?.addEventListener('click', (e) => {
            e.preventDefault();
            console.log('Add note button clicked');
            // Use the global function from the partial
            if (typeof showAddNotesModal === 'function') {
                showAddNotesModal(this.fund);
            } else {
                this.showAddNotesModal();
            }
        });
        document.getElementById('deleteFundBtn')?.addEventListener('click', (e) => {
            e.preventDefault();
            console.log('Delete button clicked');
            // Use the global function from the partial
            if (typeof showDeleteFundModal === 'function') {
                showDeleteFundModal(this.fund);
            } else {
                this.handleDeleteFund();
            }
        });

        // Action buttons - Mobile
        document.getElementById('editFundBtnMobile')?.addEventListener('click', (e) => {
            e.preventDefault();
            console.log('Edit button clicked (mobile)');
            this.showEditModal();
        });
        document.getElementById('updateStatusBtnMobile')?.addEventListener('click', (e) => {
            e.preventDefault();
            console.log('Update status button clicked (mobile)');
            // Use the global function from the partial
            if (typeof showUpdateStatusModal === 'function') {
                showUpdateStatusModal(this.fund);
            } else {
                this.showUpdateStatusModal();
            }
        });
        document.getElementById('addNoteBtnMobile')?.addEventListener('click', (e) => {
            e.preventDefault();
            console.log('Add note button clicked (mobile)');
            // Use the global function from the partial
            if (typeof showAddNotesModal === 'function') {
                showAddNotesModal(this.fund);
            } else {
                this.showAddNotesModal();
            }
        });
        document.getElementById('deleteFundBtnMobile')?.addEventListener('click', (e) => {
            e.preventDefault();
            console.log('Delete button clicked (mobile)');
            // Use the global function from the partial
            if (typeof showDeleteFundModal === 'function') {
                showDeleteFundModal(this.fund);
            } else {
                this.handleDeleteFund();
            }
        });

        // Modal save buttons - these are now handled by the partials
        // document.getElementById('saveEditBtn')?.addEventListener('click', () => this.saveFundEdit());
        // document.getElementById('saveUpdateStatusBtn')?.addEventListener('click', () => this.updateFundStatus());
        // document.getElementById('saveNotesBtn')?.addEventListener('click', () => this.saveNotes());

        // Status change handler - this is now handled by the partial
        // document.getElementById('newStatus')?.addEventListener('change', (e) => {
        //     const approvedAmountGroup = document.getElementById('approvedAmountGroup');
        //     if (approvedAmountGroup) {
        //         approvedAmountGroup.style.display = e.target.value === 'Approved' ? 'block' : 'none';
        //     }
        // });

        // Back button
        document.querySelector('.btn-outline-secondary')?.addEventListener('click', () => {
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
            console.log('Fund data loaded:', this.fund);
            this.populateFundDetails();
            await this.loadMemberDetails();
        } catch (error) {
            console.error('Error loading fund details:', error);
            this.showError('Failed to load fund details');
        } finally {
            this.showLoading(false);
        }
    }

    async loadMemberDetails() {
        if (!this.fund?.member?.memberID) return;
        
        try {
            // Load member history data in parallel
            await Promise.all([
                this.loadMemberFundsHistory(),
                this.loadMemberAidsHistory(),
                this.loadMemberAttendanceHistory(),
                this.loadFamilyMemberCount()
            ]);
        } catch (error) {
            console.error('Error loading member details:', error);
        }
    }

    async loadMemberFundsHistory() {
        try {
            const response = await fetch(`${apiBaseUrl}/Member/GetMemberFunds?memberID=${this.fund.member.memberID}`, {
                credentials: 'include'
            });
            if (response.ok) {
                const funds = await response.json();
                // Filter out the current fund - ensure both values are compared as integers
                const filteredFunds = funds.filter(fund => parseInt(fund.fundID) !== parseInt(this.fundId));
                this.populateFundsHistory(filteredFunds);
            }
        } catch (error) {
            console.error('Error loading funds history:', error);
        }
    }

    async loadMemberAidsHistory() {
        try {
            const response = await fetch(`${apiBaseUrl}/Member/GetMemberAids?memberID=${this.fund.member.memberID}`, {
                credentials: 'include'
            });
            if (response.ok) {
                const aids = await response.json();
                this.populateAidsHistory(aids);
            }
        } catch (error) {
            console.error('Error loading aids history:', error);
        }
    }

    async loadMemberAttendanceHistory() {
        try {
            const response = await fetch(`${apiBaseUrl}/Member/GetAttendanceHistory?memberID=${this.fund.member.memberID}`, {
                credentials: 'include'
            });
            if (response.ok) {
                const attendance = await response.json();
                this.populateAttendanceHistory(attendance);
            }
        } catch (error) {
            console.error('Error loading attendance history:', error);
        }
    }

    async loadFamilyMemberCount() {
        try {
            if (this.fund.member?.familyCount !== undefined) {
                console.log('Family member count from fund data:', this.fund.member.familyCount);
                document.getElementById('familyMembers').value = this.fund.member.familyCount.toString();
            } else {
                console.log('No family count available in fund data');
                document.getElementById('familyMembers').value = 'N/A';
            }
        } catch (error) {
            console.error('Error setting family member count:', error);
            document.getElementById('familyMembers').value = 'N/A';
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

        // Map status numbers to names for display
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

        // Fund Details Tab - using correct MemberFund properties
        const statusBadge = document.getElementById('requestStatusBadge');
        if (statusBadge) {
            statusBadge.textContent = statusText;
            statusBadge.className = `badge ${this.getStatusBadgeClass(this.fund.status)}`;
        }
        
        // Update Fund ID title
        const fundIdTitle = document.getElementById('fundIdTitle');
        if (fundIdTitle) {
            fundIdTitle.textContent = `Fund ${this.fund.fundID}`;
        }
        
        document.getElementById('memberCode').value = this.fund.member?.code || '';
        
        // Use correct Member property names
        let memberName = this.fund.member?.fullName || 
            `${this.fund.member?.unFirstName || ''} ${this.fund.member?.unLastName || ''}`.trim() ||
            this.fund.member?.baptismName || '';
        document.getElementById('memberName').value = memberName;
        
        document.getElementById('memberMobile').value = this.fund.member?.mobile || '';
        document.getElementById('requestType').value = this.fund.fundCategory || '';
        document.getElementById('requestedAmount').value = this.fund.requestedAmount ? 
            this.formatCurrency(this.fund.requestedAmount) : '';
        document.getElementById('requestDescription').value = this.fund.requestDescription || '';
        document.getElementById('createdBy').value = this.fund.servant?.servantName || 'Unknown';
        document.getElementById('assignedTo').value = this.fund.approver?.servantName || 'Unassigned';
        document.getElementById('creationDate').value = this.formatDate(this.fund.requestDate);
        
        // For last update, use requestDate as fallback since there's no updatedAt in MemberFund
        document.getElementById('lastUpdateDate').value = this.formatDate(this.fund.requestDate);
        
        // Member Notes
        document.getElementById('memberNotes').value = this.fund.member?.notes || '';
        
        // Fund Notes (ApproverNotes)
        const notesContainer = document.getElementById('fundNotes');
        if (notesContainer) {
            if (this.fund.approverNotes && this.fund.approverNotes.trim()) {
                // Parse notes (assuming they're in format: [timestamp - user]: note)
                const notes = this.parseNotes(this.fund.approverNotes);
                notesContainer.innerHTML = notes.map(note => `
                    <div class="note-item border-bottom pb-2 mb-2">
                        <div class="d-flex justify-content-between">
                            <strong>${note.author}</strong>
                            <small class="text-muted">${note.timestamp}</small>
                        </div>
                        <p class="mb-0" style="white-space: pre-wrap;">${note.content.replace(/\n/g, '<br>')}</p>
                    </div>
                `).join('');
            } else {
                notesContainer.innerHTML = '<p class="text-muted mb-0">No notes available</p>';
            }
        }

        // Member Image
        const memberImage = document.getElementById('memberImage');
        const memberImagePlaceholder = document.getElementById('memberImagePlaceholder');
        if (memberImage && memberImagePlaceholder) {
            if (this.fund.member?.imageURL) {
                memberImage.src = this.fund.member.imageURL;
                memberImage.style.display = 'block';
                memberImagePlaceholder.style.display = 'none';
            } else {
                memberImage.style.display = 'none';
                memberImagePlaceholder.style.display = 'block';
            }
        }

        // Member Details Tab
        document.getElementById('memberCreationDate').value = this.fund.member?.createdAt ? 
            this.formatDate(this.fund.member.createdAt) : 'Not Set';
        
        // Update form elements for editing
        this.updateEditForm();
    }

    parseNotes(notesString) {
        const lines = notesString.split('\n');
        const notes = [];
        let currentNote = null;
        
        for (const line of lines) {
            const match = line.match(/^\[(.*?) - (.*?)\]: (.*)$/);
            if (match) {
                // Save previous note if exists
                if (currentNote) {
                    notes.push(currentNote);
                }
                // Start new note
                currentNote = {
                    timestamp: match[1],
                    author: match[2],
                    content: match[3]
                };
            } else if (line.trim()) {
                if (currentNote) {
                    // Add line to current note content
                    currentNote.content += '\n' + line.trim();
                } else {
                    // Fallback for notes without proper format
                    currentNote = {
                        timestamp: 'Unknown',
                        author: 'System',
                        content: line.trim()
                    };
                }
            }
        }
        
        // Don't forget to add the last note
        if (currentNote) {
            notes.push(currentNote);
        }
        
        return notes;
    }

    populateFundsHistory(funds) {
        const tbody = document.getElementById('fundsHistoryBody');
        if (!tbody) return;

        if (funds && funds.length > 0) {
            // Sort funds by date descending
            funds.sort((a, b) => new Date(b.requestDate) - new Date(a.requestDate));
            
            tbody.innerHTML = funds.map(fund => `
                <tr>
                    <td>${fund.requestDate ? new Date(fund.requestDate).toLocaleDateString() : ''}</td>
                    <td>${fund.fundCategory || ''}</td>
                    <td>${fund.requestDescription || ''}</td>
                    <td>
                        <span class="badge ${this.getStatusBadgeClass(fund.status)}">${this.getFundStatusText(fund.status)}</span>
                    </td>
                    <td>${fund.requestedAmount || ''}</td>
                    <td>${fund.approvedAmount || ''}</td>
                </tr>
            `).join('');
        } else {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No funds history available</td></tr>';
        }
    }

    populateAidsHistory(aids) {
        const tbody = document.getElementById('aidsHistoryBody');
        if (!tbody) return;

        if (aids && aids.length > 0) {
            tbody.innerHTML = aids.map(aid => `
                <tr>
                    <td>${aid.aid?.aidName || ''}</td>
                    <td>${aid.timeStamp ? new Date(aid.timeStamp).toLocaleDateString() : ''}</td>
                </tr>
            `).join('');
        } else {
            tbody.innerHTML = '<tr><td colspan="2" class="text-center text-muted">No aids history available</td></tr>';
        }
    }

    populateAttendanceHistory(attendance) {
        const tbody = document.getElementById('attendanceHistoryBody');
        if (!tbody) return;

        if (attendance && attendance.length > 0) {
            tbody.innerHTML = attendance.map(record => `
                <tr>
                    <td>${record.classOccurrence?.className || ''}</td>
                    <td>${record.timeStamp ? new Date(record.timeStamp).toLocaleDateString() : ''}</td>
                    <td>
                        <span class="badge bg-success">Present</span>
                    </td>
                </tr>
            `).join('');
        } else {
            tbody.innerHTML = '<tr><td colspan="3" class="text-center text-muted">No attendance history available</td></tr>';
        }
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

    updateEditForm() {
        if (!this.fund) return;

        // Update edit modal with current fund data - using correct MemberFund properties
        const editForm = document.getElementById('editFundForm');
        if (editForm) {
            const descriptionField = editForm.querySelector('#editRequestDescription');
            const amountField = editForm.querySelector('#editRequestedAmount');
            const categoryField = editForm.querySelector('#editFundCategory');

            if (descriptionField) descriptionField.value = this.fund.requestDescription || '';
            if (amountField) amountField.value = this.fund.requestedAmount || '';
            if (categoryField) categoryField.value = this.fund.fundCategory || '';
        }

        // Update status modal with current status
        const statusForm = document.getElementById('updateStatusForm');
        if (statusForm) {
            const statusField = statusForm.querySelector('#newStatus');
            const notesField = statusForm.querySelector('#statusNotes');
            
            // Convert numeric status to string for the dropdown
            let statusValue = '';
            if (typeof this.fund.status === 'number') {
                const statusMap = { 0: 'Open', 1: 'Approved', 2: 'Rejected', 3: 'Delivered' };
                statusValue = statusMap[this.fund.status] || '';
            } else {
                statusValue = this.fund.status || '';
            }
            
            if (statusField) statusField.value = statusValue;
            if (notesField) notesField.value = '';
        }
    }

    showEditModal(fund) {
        const fundData = fund || this.fund;
        if (!fundData) return;

        // Populate edit form
        document.getElementById('editFundId').value = fundData.fundID;
        document.getElementById('editAssigneeSelect').value = fundData.servantID || '';
        document.getElementById('editFundCategory').value = fundData.fundCategory || '';
        document.getElementById('editRequestDescription').value = fundData.requestDescription || '';
        document.getElementById('editRequestAmount').value = fundData.requestedAmount || '';

        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('editFundModal'));
        modal.show();
    }

    // showUpdateStatusModal - now handled by _FundUpdateStatusPartial
    // showAddNotesModal - now handled by _FundAddNotesPartial  
    // showDeleteFundModal - now handled by _DeleteFundModalPartial

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
            
            // Trigger fund data changed event
            $(document).trigger('fundDataChanged');

        } catch (error) {
            this.showError('Failed to update fund');
        }
    }

    // updateFundStatus API method - now handled by _FundUpdateStatusPartial
    // async updateFundStatus() {
    //     try {
    //         const formData = {
    //             fundID: parseInt(document.getElementById('statusFundId').value),
    //             status: parseInt(document.getElementById('newStatus').value),
    //             approverNotes: document.getElementById('statusNotes').value
    //         };

    //         const response = await fetch(`${apiBaseUrl}/api/Fund/${this.fundId}/status`, {
    //             method: 'PUT',
    //             headers: {
    //                 'Content-Type': 'application/json',
    //             },
    //             body: JSON.stringify(formData),
    //             credentials: 'include'
    //         });

    //         if (!response.ok) {
    //             throw new Error('Failed to update status');
    //         }

    //         // Close modal and reload data
    //         bootstrap.Modal.getInstance(document.getElementById('updateStatusModal')).hide();
    //         this.showSuccess('Status updated successfully');
    //         this.loadFundDetails();
            
    //         // Trigger fund data changed event
    //         $(document).trigger('fundDataChanged');

    //     } catch (error) {
    //         this.showError('Failed to update status');
    //     }
    // }

    // saveNotes API method - now handled by _FundAddNotesPartial
    // async saveNotes() {
    //     try {
    //         const formData = {
    //             fundID: parseInt(document.getElementById('notesFundId').value),
    //             approverNotes: document.getElementById('newNotes').value
    //         };

    //         const response = await fetch(`${apiBaseUrl}/api/Fund/${this.fundId}/notes`, {
    //             method: 'POST',
    //             headers: {
    //                 'Content-Type': 'application/json',
    //             },
    //             body: JSON.stringify(formData),
    //             credentials: 'include'
    //         });

    //         if (!response.ok) {
    //             throw new Error('Failed to add notes');
    //         }

    //         // Close modal and reload data
    //         bootstrap.Modal.getInstance(document.getElementById('addNotesModal')).hide();
    //         this.showSuccess('Notes added successfully');
    //         this.loadFundDetails();
            
    //         // Trigger fund data changed event
    //         $(document).trigger('fundDataChanged');

    //     } catch (error) {
    //         this.showError('Failed to add notes');
    //     }
    // }

    getFundStatusText(status) {
        const statusMap = {
            0: 'Open',
            1: 'Approved', 
            2: 'Rejected',
            3: 'Delivered'
        };
        return statusMap[status] || status;
    }

    getFundStatusBadgeClass(status) {
        switch (status) {
            case 0: // Open
                return 'bg-warning text-dark';
            case 1: // Approved
                return 'bg-success';
            case 2: // Rejected
                return 'bg-danger';
            case 3: // Delivered
                return 'bg-info';
            default:
                return 'bg-secondary';
        }
    }

    getStatusBadgeClass(status) {
        // Generic method for other status displays
        if (typeof status === 'string') {
            switch (status.toLowerCase()) {
                case 'open':
                    return 'bg-warning text-dark';
                case 'approved':
                    return 'bg-success';
                case 'rejected':
                    return 'bg-danger';
                case 'delivered':
                    return 'bg-info';
                case 'pending':
                    return 'bg-warning text-dark';
                case 'in progress':
                    return 'bg-primary';
                default:
                    return 'bg-secondary';
            }
        }
        return this.getFundStatusBadgeClass(status);
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

    // Action functions as requested
    editFund(fund) {
        this.showEditModal(fund);
    }

    updateFundStatus(fund) {
        // Use the global function from the partial
        if (typeof showUpdateStatusModal === 'function') {
            showUpdateStatusModal(fund);
        } else {
            this.showUpdateStatusModal(fund);
        }
    }

    async deleteFund(fund) {
        // Use the global function from the partial
        if (typeof showDeleteFundModal === 'function') {
            showDeleteFundModal(fund);
        } else {
            this.showDeleteFundModal(fund);
        }
    }

    // Setup event listener for fund data changes
    setupFundDataListener() {
        $(document).on('fundDataChanged', () => {
            console.log('Fund data changed, reloading fund details...');
            this.loadFundDetails();
        });
    }

    // Handle delete button click from UI
    handleDeleteFund() {
        console.log('handleDeleteFund called');
        this.showDeleteFundModal();
    }

    // Delete fund API call
    async performDeleteFund() {
        console.log('performDeleteFund called');
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
            
            // Trigger fund data changed event
            $(document).trigger('fundDataChanged');
            
            setTimeout(() => window.location.href = '/Funds', 1500);

        } catch (error) {
            console.error('Delete fund error:', error);
            this.showError('Failed to delete fund');
        }
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new FundDetailManager();
}); 