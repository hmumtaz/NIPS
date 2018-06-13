function deleteRow(row) {
    i = row.rowIndex;
    var thisLedgerID = document.getElementById('ledgerTable').rows[i].cells[0];
    $.ajax({
        url: 'api/users',
        method: 'DELETE',
        headers: {
            'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
        },
        data: {
            adminUserName: sessionStorage.getItem('userName'),
            ledgerID: thisLedgerID.innerHTML,
        },
        success: function (data) {
            document.getElementById('ledgerTable').deleteRow(i);
        },
        error: function (jqXHR) {
            $('#divErrText').text(jqXHR.responseText);
            $('#divError').show('fade');
        }
    });
}
$(document).ready(function () {
    if (sessionStorage.getItem('accessToken') == null) {
        window.location.href = 'Login.html';
    }

    $('errorModal').on('hidden.bs.modal', function () {
        window.location.href = 'Login.html';
    });

    $.ajax({
        url: 'api/users/GetUserPoints',
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
        },
        data: {
            userName: sessionStorage.getItem('userName'),
        },
        success: function (data) {
            var row = $('<tr><td>' + data.totalPoints + '</td><td>'
                + data.semesterPoints + '</td><td>'
                + data.weekPoints + '</td><td>'
                + data.weekRank + '</td><td>'
                + data.semesterRank + '</td></tr>');
            $('#ptsTable').append(row);
        },
        error: function (jqXHR) {
            $('#divErrText').text(jqXHR.responseText);
            $('#divError').show('fade');
        }
    });
    $.ajax({
        url: 'api/users/GetAccessLevel',
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
        },
        data: {
            userName: sessionStorage.getItem('userName')
        },
        success: function (accessLevel) {
            switch (accessLevel) {
                case 2:
                    $('#aPDiv').removeClass('hidden');
                case 1:
                    $('#giveDiv').removeClass('hidden');
                    $.ajax({
                        url: 'api/users',
                        method: 'GET',
                        headers: {
                            'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
                        },
                        success: function (data) {
                            $.each(data, function (index, value) {
                                var option = $('<option>' + value.LastName + "," + value.FirstName + '</option>');
                                $('#userList').append(option);
                            });
                        },
                        error: function (jqXHR) {
                            $('#divErrText').text(jqXHR.responseText);
                            $('#divError').show('fade');
                        }
                    });
                case 0:
                    break;
            }
        },
        error: function (jqXHR) {
            $('#divErrText').text(jqXHR.responseText);
            $('#divError').show('fade');
        }
    });
    $('#givePts').click(function () {
        $.ajax({
            url: 'api/users/GivePoints',
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },
            data: {
                giverID: sessionStorage.getItem('userName'),
                getterID: $('#userList').val(),
                amount: $('#amount').val(),
                reason: $('#reason').val(),
            },
            success: function () {
                $('#successModal').modal('show');
            },
            error: function (jqXHR) {
                $('#divErrText').text(jqXHR.responseText);
                $('#divError').show('fade');
            }
        });
    });
    $('#viewPtsTbl').click(function () {
        $.ajax({
            url: 'api/users/GetPointsTable',
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },
            data: {
                userName: sessionStorage.getItem('userName'),
            },
            success: function (data) {
                $('#deleteUserDiv').addClass("hidden");
                $("#ledgerDiv").addClass("hidden");
                $("#accessLvlDiv").addClass("hidden");
                $("#allPtsDiv").removeClass("hidden");
                $('#allPtsBody').empty();
                $.each(data, function (index, value) {
                    var row = $('<tr><td>' + value.LastName + ',' + value.FirstName + '</td><td>'
                        + value.TotalPoints + '</td><td>'
                        + value.SemesterPoints + '</td><td>'
                        + value.WeekPoints + '</td><td>'
                        + value.WeekRank + '</td><td>'
                        + value.SemesterRank + '</td></tr>');
                    $('#allPtsTable').append(row);
                    
                });
                $('#allPtsTable').DataTable();

            },
            error: function (jqXHR) {
                $('#divErrText').text(jqXHR.responseText);
                $('#divError').show('fade');
            }
        });
    });
    $('#viewLedger').click(function () {
        $.ajax({
            url: 'api/users/GetLedger',
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },
            data: {
                userName: sessionStorage.getItem('userName'),
            },
            success: function (data) {
                $('#deleteUserDiv').addClass("hidden");
                $("#allPtsDiv").addClass("hidden");
                $("#accessLvlDiv").addClass("hidden");
                $("#ledgerDiv").removeClass("hidden");
                $.each(data, function (index, value) {
                    $('#ledgerBody').empty();
                    var row = $('<tr><td>' + value.LedgerID + '</td><td>'
                        + value.GiverID + '</td><td>'
                        + value.GetterID + '</td><td>'
                        + value.Reason + '</td><td>'
                        + value.Amount + '</td><td>'
                        + '<input class="btn btn-danger" type="button" value="Delete" onclick="deleteRow(this.parentNode.parentNode)"/> </td</tr >');
                    $('#ledgerTable').append(row);
                    $('#ledgerTable').DataTable();
                });

            },
            error: function (jqXHR) {
                $('#divErrText').text(jqXHR.responseText);
                $('#divError').show('fade');
            }
        });
    });
    $('#viewChangeAccess').click(function () {
        $.ajax({
            url: 'api/users',
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },
            success: function (data) {
                $('#deleteUserDiv').addClass("hidden");
                $("#allPtsDiv").addClass("hidden");
                $("#ledgerDiv").addClass("hidden");
                $("#accessLvlDiv").removeClass("hidden");
                $.each(data, function (index, value) {
                    var option = $('<option>' + value.LastName + "," + value.FirstName + '</option>');
                    $('#accessLvlUserList').append(option);
                });
            },
            error: function (jqXHR) {
                $('#divErrText').text(jqXHR.responseText);
                $('#divError').show('fade');
            }
        });
    });
    $('#changeAccessLvl').click(function () {
        var accessValue = $('#accessLvls').val()
        var accessLevel = 0
        if (accessValue == "Admin") {
            accessLevel = 2
        } else if (accessValue == "Officer") {
            accessLevel = 1
        } else {
            accessLevel = 0
        };
        $.ajax({
            url: 'api/users/ChangeAccess',
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },
            data: {
                adminUserName: sessionStorage.getItem('userName'),
                brotherName: $('#accessLvlUserList').val(),
                newAccessLevel: accessLevel,
            },
            success: function () {

                $('#successModal').modal('show');
            },
            error: function (jqXHR) {
                $('#divErrText').text(jqXHR.responseText);
                $('#divError').show('fade');
            }
        });
    });
    $('#viewDeleteUser').click(function () {
        $.ajax({
            url: 'api/users',
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
            },
            success: function (data) {
                $('#deleteUserDiv').removeClass("hidden");
                $("#allPtsDiv").addClass("hidden");
                $("#ledgerDiv").addClass("hidden");
                $("#accessLvlDiv").addClass("hidden");
                $.each(data, function (index, value) {
                    var option = $('<option>' + value.LastName + "," + value.FirstName + '</option>');
                    $('#deleteUserList').append(option);
                });
            },
            error: function (jqXHR) {
                $('#divErrText').text(jqXHR.responseText);
                $('#divError').show('fade');
            }
        })
    });
    $('#deleteUser').click(function () {
        $.ajax({
            url: 'api/users/DeleteUser',
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken'),
            },
            data: {
                adminUserName: sessionStorage.getItem('userName'),
                deleteName: $('#deleteUserList').val(),
            },
            success: function () {

                $('#successModal').modal('show');
            },
            error: function (jqXHR) {
                $('#divErrText').text(jqXHR.responseText);
                $('#divError').show('fade');
            }
        });
    });
});
