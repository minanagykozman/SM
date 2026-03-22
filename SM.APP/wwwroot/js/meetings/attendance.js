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
            document.getElementById("counter").textContent = `(${data.count} Members)`;
            
        })
        .catch(error => {
            console.error("Error fetching data:", error);
            alert("Failed to fetch data. Please try again.");
        });
});

document.getElementById("btnCheck").addEventListener("click", function () {
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
            document.getElementById("counter").textContent = `(${data} Members)`;
        })
        .catch(error => {
            console.error("Error:", error);
            alert("Failed to add member. Please try again.");
        });
}