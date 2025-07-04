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
            loadMemberFundsPartial(this.fund.member.memberID, this.fundId);

        } catch (error) { console.error('Error loading funds history:', error); }
    }

    async loadMemberAidsHistory() {
        try {
            loadMemberAidsPartial(this.fund.member.memberID);
        } catch (error) { console.error('Error loading aids history:', error); }
    }

    async loadMemberAttendanceHistory() {
        try {
            loadMemberAttendancePartial(this.fund.member.memberID);
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
            statusBadge.className = `badge ${getFundStatusBadgeClass(this.fund.status)} fs-6 p-2 me-2`;
        }

        // Fields population
        document.getElementById('memberCode').value = this.fund.member?.code || '';
        document.getElementById('memberName').value = this.fund.member?.fullName || '';
        document.getElementById('memberMobile').value = this.fund.member?.mobile || '';
        document.getElementById('familyMembers').value = this.fund.member?.familyCount?.toString() || 'N/A';
        document.getElementById('requestType').value = this.fund.fundCategory || '';
        document.getElementById('requestedAmount').value = formatCurrency(this.fund.requestedAmount);
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

    parseNotes(notesString) {
        // Return an empty array if the input is null, empty, or just whitespace.
        if (!notesString || !notesString.trim()) {
            return [];
        }

        const notes = [];
        let currentNote = null;
        const lines = notesString.split('\n');
        const headerRegex = /^\[(.*?) - (.*?)\]:\s*(.*)$/;

        for (const line of lines) {
            const match = line.match(headerRegex);

            if (match) {
                // This line is a new note header.
                // First, if we were already building a note, save it to the array.
                if (currentNote) {
                    // Trim any trailing newlines from the previous note's content before saving.
                    currentNote.content = currentNote.content.trim();
                    notes.push(currentNote);
                }

                // Start a new note object from the captured regex groups.
                currentNote = {
                    timestamp: match[1].trim(), // e.g., "2024-07-03 10:30 AM"
                    author: match[2].trim(),    // e.g., "John Doe"
                    content: match[3].trim()     // The first line of the note's content
                };
            } else if (currentNote) {
                // This line is a continuation of the current note's content.
                // Append it to the existing content, preserving the multiline format.
                currentNote.content += '\n' + line;
            }
        }

        // After the loop, don't forget to push the very last note being built.
        if (currentNote) {
            currentNote.content = currentNote.content.trim();
            notes.push(currentNote);
        }

        return notes;
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
    