class FundDetailManager {
    constructor() {
        this.fundId = this.getFundIdFromUrl();
        this.fund = null;
        this.assignableServants = [];
        this.STATUS = { OPEN: 0, APPROVED: 1, REJECTED: 2, DELIVERED: 3 };
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
        ['editFundBtn', 'updateStatusBtn', 'addNoteBtn', 'deleteFundBtn'].forEach(id => {
            document.getElementById(id)?.addEventListener('click', (e) => {
                e.preventDefault();
                if (id.startsWith('edit')) this.showEditModal();
                if (id.startsWith('update')) this.showUpdateStatusModal();
                if (id.startsWith('add')) this.showAddNotesModal();
                if (id.startsWith('delete')) this.showDeleteFundModal();
            });
        });
    }

    async loadFundDetails() {
        try {
            this.showLoading(true);
            const response = await fetch(`${apiBaseUrl}/api/Fund/${this.fundId}`, { credentials: 'include' });
            if (!response.ok) {
                if (response.status === 404) this.showError('Fund not found');
                else throw new Error('Failed to load fund details');
                return;
            }
            this.fund = await response.json();
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
            await Promise.all([
                this.loadMemberFundsHistory(),
                this.loadMemberAidsHistory(),
                this.loadMemberAttendanceHistory(),
            ]);
        } catch (error) {
            console.error('Error loading member details:', error);
        }
    }

    async loadMemberFundsHistory() {
        try {
            const response = await fetch(`${apiBaseUrl}/Member/GetMemberFunds?memberID=${this.fund.member.memberID}`, { credentials: 'include' });
            if (response.ok) {
                const funds = await response.json();
                const filteredFunds = funds.filter(fund => parseInt(fund.fundID) !== parseInt(this.fundId));
                this.populateFundsHistory(filteredFunds);
            }
        } catch (error) { console.error('Error loading funds history:', error); }
    }

    async loadMemberAidsHistory() {
        try {
            const response = await fetch(`${apiBaseUrl}/Member/GetMemberAids?memberID=${this.fund.member.memberID}`, { credentials: 'include' });
            if (response.ok) {
                const aids = await response.json();
                this.populateAidsHistory(aids);
            }
        } catch (error) { console.error('Error loading aids history:', error); }
    }

    async loadMemberAttendanceHistory() {
        try {
            const response = await fetch(`${apiBaseUrl}/Member/GetMemberClassOverviews?memberID=${this.fund.member.memberID}`, { credentials: 'include' });
            if (response.ok) {
                const memberClasses = await response.json();
                this.populateAttendanceHistory(memberClasses);
            }
        } catch (error) { console.error('Error loading attendance history:', error); }
    }

    async loadAssignableServants() {
        try {
            const response = await fetch(`${apiBaseUrl}/api/Fund/assignable-servants`, { credentials: 'include' });
            if (response.ok) this.assignableServants = await response.json();
        } catch (error) { console.error('Error loading assignable servants:', error); }
    }

    populateFundDetails() {
        if (!this.fund) return;

        const statusMap = { 0: 'Open', 1: 'Approved', 2: 'Rejected', 3: 'Delivered' };
        let statusText = typeof this.fund.status === 'number' ? statusMap[this.fund.status] : this.fund.status;

        const statusBadge = document.getElementById('requestStatusBadge');
        if (statusBadge) {
            statusBadge.textContent = statusText;
            statusBadge.className = `badge ${this.getFundStatusBadgeClass(this.fund.status)} fs-6 p-2 me-2`;
        }

        // Fields population
        document.getElementById('memberCode').value = this.fund.member?.code || '';
        document.getElementById('memberName').value = this.fund.member?.fullName || '';
        document.getElementById('memberMobile').value = this.fund.member?.mobile || '';
        document.getElementById('familyMembers').value = this.fund.member?.familyCount?.toString() || 'N/A';
        document.getElementById('requestType').value = this.fund.fundCategory || '';
        document.getElementById('requestedAmount').value = this.formatCurrency(this.fund.requestedAmount);
        document.getElementById('requestDescription').value = this.fund.requestDescription || '';
        document.getElementById('createdBy').value = this.fund.servant?.servantName || 'Unknown';
        document.getElementById('assignedTo').value = this.fund.approver?.servantName || 'Unassigned';
        document.getElementById('creationDate').value = formatDate(this.fund.requestDate);
        document.getElementById('lastUpdateDate').value = formatDate(this.fund.requestDate);
        document.getElementById('memberNotes').value = this.fund.member?.notes || '';
        document.getElementById('memberCreationDate').value = this.fund.member?.createdAt ? formatDate(this.fund.member.createdAt) : 'Not Set';
        this.populateFundNotes(this.fund.approverNotes);

        // Member Image
        const memberImage = document.getElementById('memberImage');
        const placeholder = document.getElementById('memberImagePlaceholder');

        if (memberImage && placeholder) {
            if (this.fund.member?.imageURL) {
                memberImage.src = this.fund.member.imageURL;
                memberImage.classList.remove('d-none');
                placeholder.classList.add('d-none');

                // It's still a good practice to handle loading errors
                memberImage.onerror = function () {
                    // If the image fails, hide it and show the placeholder instead
                    memberImage.classList.add('d-none');
                    placeholder.classList.remove('d-none');
                };
            } else {
                // If there's no URL, show the placeholder and hide the image
                placeholder.classList.remove('d-none');
                memberImage.classList.add('d-none');
            }
        }

        //Conditional Delete Button
        const deleteFundLi = document.getElementById('deleteFundLi');
        const deleteFundSeparator = document.getElementById('deleteFundSeparator');
        if (deleteFundLi) {
            deleteFundLi.style.display = (this.fund.status === this.STATUS.OPEN) ? 'block' : 'none';
            deleteFundSeparator.style.display = (this.fund.status === this.STATUS.OPEN) ? 'block' : 'none';
        }
    }

    populateFundNotes(notesString) {
        const notesContainer = document.getElementById('fundNotes');
        if (notesString && notesString.trim()) {
            notesContainer.innerHTML = this.parseNotes(notesString).map(note => `...`).join('');
        } else {
            notesContainer.innerHTML = '<p class="text-muted mb-0">No notes available</p>';
        }
    }

    parseNotes(notesString) { /* ... implementation ... */ return []; }

    populateFundsHistory(funds) {
        const fundsList = document.getElementById('fundsHistoryList');
        if (!funds || funds.length === 0) {
            fundsList.innerHTML = '<div class="list-group-item text-center text-muted">No funds history available</div>';
            return;
        }

        const self = this; // **FIX**: Preserve 'this' context
        funds.sort((a, b) => new Date(b.requestDate) - new Date(a.requestDate));
        fundsList.innerHTML = funds.map(function (fund) { // Use regular function with self
            return `
                        <div class="list-group-item list-group-item-action flex-column align-items-start">
                            <div class="d-flex w-100 justify-content-between">
                                <h5 class="mb-1">${fund.fundCategory || 'N/A'}</h5>
                                <span class="badge ${self.getFundStatusBadgeClass(fund.status)} fs-6">${self.getFundStatusText(fund.status)}</span>
                            </div>
                            <p class="mb-1">${fund.requestDescription || 'No description'}</p>
                            <div class="d-flex w-100 justify-content-between align-items-center mt-2">
                                <small class="text-muted"><i class="bi bi-calendar-event me-1"></i> ${formatDate(fund.requestDate)}</small>
                                <small><b>Requested:</b> ${self.formatCurrency(fund.requestedAmount)} | <b>Approved:</b> ${self.formatCurrency(fund.approvedAmount)}</small>
                            </div>
                        </div>`;
        }).join('');
    }

    populateAidsHistory(aids) {
        const aidsList = document.getElementById('aidsHistoryList');
        if (!aidsList) return;

        if (aids && aids.length > 0) {
            // **FIXED**: The logic was inadvertently removed in the previous update. This is the corrected version.
            aidsList.innerHTML = aids.map(aid => `
            <div class="list-group-item d-flex justify-content-between align-items-center">
                <span>${aid.aid?.aidName || 'N/A'}</span>
                <small class="text-muted fw-bold">${formatDate(aid.timeStamp)}</small>
            </div>
        `).join('');
        } else {
            aidsList.innerHTML = '<div class="list-group-item text-center text-muted"><i class="bi bi-bandaid fa-lg me-2"></i>No aids history available</div>';
        }
    }

    populateAttendanceHistory(memberClasses) {
        const attendanceList = document.getElementById('attendanceHistoryList');
        if (!memberClasses || memberClasses.length === 0) {
            attendanceList.innerHTML = '<div class="list-group-item text-center text-muted">No class attendance data available</div>';
            return;
        }

        // No context issue here, but keeping the pattern for consistency
        const self = this;
        attendanceList.innerHTML = memberClasses.map(function (classData) {
            let percentage = 0, badgeClass = 'bg-secondary', attendanceText = 'N/A';
            if (classData.attendance && classData.attendance.includes('/')) {
                const [attended, total] = classData.attendance.split('/').map(n => parseInt(n));
                if (total > 0) {
                    percentage = Math.round((attended / total) * 100);
                    if (percentage >= 80) badgeClass = 'bg-success';
                    else if (percentage >= 50) badgeClass = 'bg-warning text-dark';
                    else badgeClass = 'bg-danger';
                }
                attendanceText = classData.attendance;
            }
            const lastPresent = classData.lastPresentDate ? formatDate(classData.lastPresentDate) : 'Never';
            return `
                        <div class="list-group-item">
                            <div class="d-flex w-100 justify-content-between mb-1">
                                <h6 class="mb-0">${classData.className || ''}</h6>
                                <span class="badge bg-light text-dark align-self-center fs-6 fw-bold">${attendanceText}</span>
                            </div>
                            <div class="progress mb-1" style="height: 20px;">
                                <div class="progress-bar ${badgeClass}" role="progressbar" style="width: ${percentage}%;" aria-valuenow="${percentage}" aria-valuemin="0" aria-valuemax="100">${percentage}%</div>
                            </div>
                            <small class="text-muted">Last Attended: ${lastPresent}</small>
                        </div>`;
        }).join('');
    }

    showEditModal() {
        if (typeof showEditModal === 'function') showEditModal(this.fund);
    }

    showUpdateStatusModal() {
        if (typeof showUpdateStatusModal === 'function') showUpdateStatusModal(this.fund);
    }

    showAddNotesModal() {
        if (typeof showAddNotesModal === 'function') showAddNotesModal(this.fund);
    }

    showDeleteFundModal() {
        if (typeof showDeleteFundModal === 'function') showDeleteFundModal(this.fund);
    }

    getFundStatusText(status) {
        const statusMap = { 0: 'Open', 1: 'Approved', 2: 'Rejected', 3: 'Delivered' };
        return statusMap[status] || status.toString();
    }

    getFundStatusBadgeClass(status) {
        switch (status) {
            case 0: return 'bg-warning text-dark';
            case 1: return 'bg-success';
            case 2: return 'bg-danger';
            case 3: return 'bg-info';
            default: return 'bg-secondary';
        }
    }

    formatCurrency(amount) {
        if (amount === null || amount === undefined) return 'N/A';
        return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'EGP' }).format(amount);
    }

    showLoading(show) {
        document.getElementById('loadingSpinner').style.display = show ? 'block' : 'none';
        document.getElementById('fundContent').style.display = show ? 'none' : 'block';
    }

    showError(message) { if (typeof showToast === 'function') showToast(message, 'Error'); }

    setupFundDataListener() {
        $(document).on('fundDataChanged', () => { this.loadFundDetails(); });
    }
}

document.addEventListener('DOMContentLoaded', () => { new FundDetailManager(); });
    