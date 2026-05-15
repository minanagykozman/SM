let allEvents = [];
let eventModal;

$(document).ready(async function () {

    eventModal = new bootstrap.Modal(
        document.getElementById('eventModal')
    );

    // Date validation
    $('#eventStartDate').on('change', function () {

        const startVal = $(this).val();

        if (startVal) {

            $('#eventEndDate').attr('min', startVal);

            if ($('#eventEndDate').val() < startVal) {
                $('#eventEndDate').val(startVal);
            }
        }
    });

    await loadClassesDropDown(
        apiBaseUrl,
        'drpClassesManage',
        '#eventModal',
        true
    );

    await loadEvents();
});

// =========================================
// LOAD EVENTS
// =========================================

async function loadEvents() {

    showLoading();

    try {

        const response = await fetch(
            `${apiBaseUrl}/Events/GetEvents`,
            {
                method: "GET",
                credentials: "include"
            }
        );

        allEvents = await response.json();

        filterEvents();

    }
    catch (err) {

        console.error("Load failed", err);

    }
    finally {

        hideLoading();
    }
}

function filterEvents() {

    const name = $('#filterEventName').val().toLowerCase();

    const status = $('#filterStatus').val();

    const filtered = allEvents.filter(e => {

        const matchesName =
            e.eventName.toLowerCase().includes(name);

        const matchesStatus =
            status === 'all'
            || e.isActive.toString() === status;

        return matchesName && matchesStatus;
    });

    renderGrid(filtered);

    $('#counter').text(`(${filtered.length})`);
}

// =========================================
// RENDER GRID
// =========================================

function renderGrid(events) {

    const grid = document.getElementById('events-grid');

    grid.innerHTML = '';

    if (events.length === 0) {

        grid.innerHTML =
            '<div class="col-12 text-center py-5 text-muted">No events found.</div>';

        return;
    }

    events.forEach(event => {

        const col = document.createElement('div');

        col.className = 'col-12 col-md-6 col-lg-4';

        col.innerHTML = `
                    <div class="card h-100 shadow-sm border-0">
                        <div class="card-body">
                            <div class="d-flex justify-content-between mb-2">
                                <span class="badge ${event.isActive
                ? 'bg-success-subtle text-success border border-success-subtle'
                : 'bg-secondary-subtle text-secondary border border-secondary-subtle'} px-3">
                                    ${event.isActive ? 'Active' : 'Inactive'}
                                </span>
                                <div class="dropdown">
                                    <button class="btn btn-link btn-sm text-dark p-0"
                                            data-bs-toggle="dropdown">
                                        <i class="bi bi-three-dots-vertical"></i>
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end shadow-sm">
                                        <li>
                                            <button class="dropdown-item"
                                                    onclick="openEditModal(${event.eventID})">
                                                <i class="bi bi-pencil me-2"></i>Edit
                                            </button>
                                        </li>
                                        <li>
                                            <button class="dropdown-item text-danger"
                                                    onclick='deleteEvent(${JSON.stringify(event)})'>
                                                <i class="bi bi-trash me-2"></i>Delete
                                            </button>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                            <h5 class="card-title fw-bold text-primary mb-3">
                                ${event.eventName}
                            </h5>
                            <div class="small text-muted mb-1">
                                <i class="bi bi-calendar3 me-2"></i>
                                ${formatDate(event.eventStartDate)}
                            </div>
                            <div class="small text-muted">
                                <i class="bi bi-clock me-2"></i>
                                ${new Date(event.eventStartDate).toLocaleTimeString([], {
                    hour: '2-digit',
                    minute: '2-digit'
                })}
                            </div>
                        </div>
                    </div>
                `;
        grid.appendChild(col);
    });
}

// =========================================
// CREATE MODAL
// =========================================

function openCreateModal() {
    $('#eventForm')[0].reset();
    $('#drpClassesManage')
        .val(null)
        .trigger('change');
    $('#eventId').val('');
    $('#eventModalTitle').text('Create Event');
    $('#btnSaveEvent').text('Save Event');
    $('#isActive').prop('checked', true);
    eventModal.show();
}
// =========================================
// EDIT MODAL
// =========================================
async function openEditModal(eventID) {
    try {
        showLoading();
        $('#eventForm')[0].reset();
        $('#drpClassesManage')
            .val(null)
            .trigger('change');
        const response = await fetch(
            `${apiBaseUrl}/Events/GetEvent?eventID=${eventID}`,
            {
                method: "GET",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                }
            }
        );
        if (!response.ok) {
            throw new Error("Failed to load event");
        }
        const data = await response.json();
        $('#eventId').val(data.eventID);
        $('#eventName').val(data.eventName);
        $('#eventStartDate')
            .val(formatDateForInput(data.eventStartDate));
        $('#eventEndDate')
            .val(formatDateForInput(data.eventEndDate));
        $('#isActive')
            .prop('checked', data.isActive);
        const classIDs = data.classesIDs;
        $('#drpClassesManage').val(classIDs).trigger('change');
        $('#eventModalTitle').text('Edit Event');
        $('#btnSaveEvent').text('Update Event');
        eventModal.show();
    }
    catch (err) {
        console.error("Error loading event:", err);
        alert("Failed to load event");
    }
    finally {
        hideLoading();
    }
}
// =========================================
// SAVE EVENT
// =========================================

async function saveEvent() {
    const payload = {
        event: {
            eventID:
                parseInt($('#eventId').val()) || 0,
            eventName:
                $('#eventName').val(),
            eventStartDate:
                $('#eventStartDate').val(),
            eventEndDate:
                $('#eventEndDate').val(),
            isActive:
                $('#isActive').is(':checked')
        },
        classes:
            $('#drpClassesManage').val()
                ? $('#drpClassesManage')
                    .val()
                    .map(id => parseInt(id))
                : []
    };

    if (!payload.event.eventName
        || !payload.event.eventStartDate
        || payload.classes.length === 0) {

        alert("Please fill all required fields.");

        return;
    }

    try {

        const url =
            payload.event.eventID === 0
                ? "/Events/CreateEvent"
                : "/Events/UpdateEvent";

        const response = await fetch(
            `${apiBaseUrl}${url}`,
            {
                method: "POST",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload)
            }
        );

        if (!response.ok) {
            throw new Error("Save failed");
        }

        showSuccessToast("Event saved successfully");

        eventModal.hide();

        await loadEvents();

    }
    catch (err) {

        console.error(err);

        alert("Failed to save event");
    }
}

// =========================================
// DELETE EVENT
// =========================================

function deleteEvent(event) {

    showDeleteModal(
        'Delete Event',
        `Are you sure you want to delete "${event.eventName}"?`,
        event.eventID
    );
}

async function btnDelete_click() {

    let eventID = document.getElementById("ID").value;

    try {

        const request =
            `${apiBaseUrl}/Events/DeleteEvent`;

        const response = await fetch(request, {

            method: "POST",

            credentials: "include",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify(eventID)
        });

        if (!response.ok) {
            throw new Error("Event deletion failed");
        }

        closeDeleteModal();

        showSuccessToast("Event deleted successfully");

        await loadEvents();

    }
    catch (err) {

        console.error("Error deleting event:", err);
    }
}

// =========================================
// HELPERS
// =========================================

function formatDateForInput(dateString) {

    const date = new Date(dateString);

    const pad = (n) => n.toString().padStart(2, '0');

    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}