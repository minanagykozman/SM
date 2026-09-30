let allMembers = [];
const urlParams = new URLSearchParams(window.location.search);
const showMembers = urlParams.get('showMembers') === 'true'; // Defaults to false if null
document.addEventListener("DOMContentLoaded", function () {
    const occurenceID = document.querySelector('input[name="classOccurenceID"]').value;


    const apiUrl = `${apiBaseUrl}/Meeting/get-meeting-data?classOccurenceID=${occurenceID}`;
    fetch(apiUrl, {
        method: "GET",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => response.json())
        .then(data => {
            document.getElementById("title").textContent = `${data.title}`;
            document.getElementById("counter").textContent = `(${data.count} Members Attended)`;

        })
        .catch(error => {
            console.error("Error fetching data:", error);
            alert("Failed to fetch data. Please try again.");
        });

    if (showMembers) {
        document.getElementById("membersSection").style.display = "block";
        loadClassOccurrenceMembers(occurenceID);
    }
});
function loadClassOccurrenceMembers(occurenceID) {
    showLoading();
    try {
        const membersUrl = `${apiBaseUrl}/Meeting/get-class-occurance-membres?occurenceID=${occurenceID}`;
        fetch(membersUrl, {
            method: "GET",
            credentials: "include",
            headers: { "Content-Type": "application/json" }
        })
            .then(response => response.json())
            .then(data => {
                allMembers = data;
                filterMembers();
            })
            .catch(error => {
                console.error("Error loading members:", error);
                throw error;
            });
    }
    catch (err) {
        console.error("Failed to submit member data:", err);
        showFailedToast("Failed to load data!");
    }
    finally {
        hideLoading();
    }
}

function renderMembers(members) {
    const container = document.getElementById("membersContainer");
    container.innerHTML = "";

    members.forEach(member => {
        const isPresent = member.present;
        const badgeClass = isPresent ? "bg-success" : "bg-danger";
        const badgeText = isPresent ? "Present" : "Absent";

        // Mobile dial block if mobile is not null
        let mobileHtml = "";
        if (member.mobile) {
            mobileHtml = `
                <div class="mt-2 text-center">
                    <a href="tel:${member.mobile}" class="btn btn-outline-success btn-sm w-100">
                        <i class="fa fa-phone"></i> ${member.mobile}
                    </a>
                </div>`;
        }

        const col = document.createElement("div");
        col.className = "col-md-4";
        col.innerHTML = `
            <div class="card p-3 shadow-sm h-100 position-relative">
    <div class="d-flex justify-content-between align-items-center">
        <div>
            <h5 class="text-primary mb-1">${member.fullName}</h5>
            <p class="mb-1 text-muted small">Code: ${member.code}</p>
        </div>
        <div class="d-flex align-items-center gap-2">
            <span class="badge ${badgeClass}">${badgeText}</span>
            <button class="btn btn-sm text-secondary toggle-attendance-btn p-0" data-code="${member.code}" data-action="${isPresent ? 'remove' : 'add'}" title=${isPresent ? 'Remove' : 'Add'}>
                <i class="fa ${isPresent ? 'fa-times-circle text-danger' : 'fa-check-circle text-success'} fa-2x"></i>
            </button>
        </div>
    </div>
    ${mobileHtml}
</div>
        `;
        container.appendChild(col);
    });

    // Attach event listeners to attendance toggle icons
    document.querySelectorAll(".toggle-attendance-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            const memberCode = this.getAttribute("data-code");
            const action = this.getAttribute("data-action");
            toggleMemberAttendance(memberCode, action);
        });
    });
}

function toggleMemberAttendance(memberCode, action) {
    const occurenceID = document.querySelector('input[name="classOccurenceID"]').value;
    showLoading();
    let apiUrl = "";
    let body = null;
    if (action == "add") {
        apiUrl = `${apiBaseUrl}/Meeting/TakeAttendance?classOccurenceID=${occurenceID}&memberCode=${encodeURIComponent(memberCode)}&forceRegister=false`;
        body = JSON.stringify({ classOccuranceID: occurenceID, memberCode: memberCode, forceRegister: true });
    }
    else {
        apiUrl = `${apiBaseUrl}/Meeting/remove-attendance?classOccurenceID=${occurenceID}&memberCode=${encodeURIComponent(memberCode)}`;
        body = JSON.stringify({ classOccuranceID: occurenceID, memberCode: memberCode });
    }

    fetch(apiUrl, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: body
    })
        .then(response => response.json())
        .then(data => {
            showSuccessToast("Attendance updated successfully!");
            // Refresh members list and counter
            loadClassOccurrenceMembers(occurenceID);
            document.getElementById("counter").textContent = `(${data} Members Attended)`;
        })
        .catch(error => {
            console.error("Error:", error);
            alert("Failed to update attendance.");
        });
    hideLoading();
}

// Filter Event Listeners
document.getElementById("memberFilterSearch")?.addEventListener("input", filterMembers);
document.getElementById("filterAttendanceStatus")?.addEventListener("change", filterMembers);
document.getElementById("filterActiveStatus")?.addEventListener("change", filterMembers);
document.getElementById("filterBaptismStatus")?.addEventListener("change", filterMembers);

function filterMembers() {
    const searchText = document.getElementById("memberFilterSearch").value.toLowerCase();
    const attendanceVal = document.getElementById("filterAttendanceStatus").value;
    const activeVal = document.getElementById("filterActiveStatus").value;
    const baptismVal = document.getElementById("filterBaptismStatus").value;
    const filtered = allMembers.filter(m => {
        const matchesSearch = !searchText ||
            m.fullName.toLowerCase().includes(searchText) ||
            m.code.toLowerCase().includes(searchText) ||
            (m.mobile && m.mobile.includes(searchText));

        const matchesAttendance = attendanceVal === 'all' ||
            (attendanceVal === 'present' && m.present) ||
            (attendanceVal === 'absent' && !m.present);

        const matchesActive = activeVal === 'all' ||
            (activeVal === 'active' && m.isActive) ||
            (activeVal === 'inactive' && !m.isActive);

        const matchesBaptism = baptismVal === 'all' ||
            (baptismVal === 'baptized' && m.isBaptized) ||
            (baptismVal === 'not_baptized' && !m.isBaptized);

        return matchesSearch && matchesAttendance && matchesActive && matchesBaptism;
    });

    renderMembers(filtered);
}

document.getElementById("btnCheck").addEventListener("click", function () {
    showLoading();
    const searchValue = document.getElementById("SearchString").value.trim();
    if (!searchValue) {
        alert("Please enter a search value.");
        return;
    }
    const occurenceID = document.querySelector('input[name="classOccurenceID"]').value;

    const apiUrl = `${apiBaseUrl}/Meeting/CheckAttendance?classOccurenceID=${occurenceID}&memberCode=${encodeURIComponent(searchValue)}`;
    fetch(apiUrl, {
        method: "GET",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => response.json())
        .then(data => {
            populateModal(data); // Function to fill modal with API response
        })
        .catch(error => {
            console.error("Error fetching data:", error);
            alert("Failed to fetch data. Please try again.");
        });
    hideLoading();
});

function populateModal(data) {
    $("#divTeam").hide();
    $("#divBus").hide();
    var searchInput = document.getElementById("SearchString");
    switch (data.attendanceStatus) {
        case 0:
            document.getElementById("MemberStatusAlert").value = "Member not registered in class";
            $("#divStatus").hide();
            $("#divStatusAlert").show();
            document.getElementById("MemberCode").value = data.member.code;
            document.getElementById("MemberName").value = data.member.fullName;
            document.getElementById("MemberAge").value = data.member.age;
            document.getElementById("CardStatus").value = data.member.cardStatus;
            document.querySelector('input[name="MemberCode"]').value = data.member.code;
            $("#MemberDataBody").show();
            $("#submitBtn").show();
            showModal();
            break;
        case 1:
            showFailedToast("Member not found");
            searchInput.value = "";
            searchInput.focus();
            break;
        case 2:
            showFailedToast("Meeting not found");
            searchInput.value = "";
            searchInput.focus();
            break;
        case 3:
            showFailedToast("Member already attended");

            searchInput.value = "";
            searchInput.focus();
            break;
        case 4:
        case 5:
            document.querySelector('input[name="MemberCode"]').value = data.member.code;
            takeAttendance(false);
            searchInput.value = "";
            searchInput.focus();
            break;
    }
}

document.getElementById("submitBtn").addEventListener("click", function () {
    closeModal();
    takeAttendance(true);
    var searchInput = document.getElementById("SearchString");
    searchInput.value = "";
    searchInput.focus();

});

function takeAttendance(forceRegister) {
    const occurenceID = document.querySelector('input[name="classOccurenceID"]').value;
    const memberCode = document.querySelector('input[name="MemberCode"]').value;
    const apiUrl = `${apiBaseUrl}/Meeting/TakeAttendance?classOccurenceID=${occurenceID}&memberCode=${encodeURIComponent(memberCode)}&forceRegister=${forceRegister}`;
    fetch(apiUrl, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ classOccuranceID: occurenceID, memberCode: memberCode, forceRegister: forceRegister })
    })
        .then(response => response.json())
        .then(data => {
            showSuccessToast("Attendance added successfully!");
            document.getElementById("counter").textContent = `(${data} Members Attended)`;
            if (showMembers)
                loadClassOccurrenceMembers(occurenceID);
        })
        .catch(error => {
            console.error("Error:", error);
            alert("Failed to add member. Please try again.");
        });
}