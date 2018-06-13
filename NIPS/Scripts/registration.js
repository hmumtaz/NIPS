$(document).ready(function () {
    $('#linkClose').click(function () {
        $('#divError').hide('fade');
    });
    $('#register').click(function () {
        $.ajax({
            url: '/api/account/register',
            type: 'POST',
            data: {
                email: $('#email').val(),
                password: $('#password').val(),
                confirmPassword: $('#confirmPassword').val(),
            },
            success: function (response) {
                $.ajax({
                    url: '/api/UserRegistration',
                    type: 'POST',
                    data: {
                        firstName: $('#firstName').val(),
                        lastName: $('#lastName').val(),
                        slackID: $('#slackID').val(),
                        ID: response,
                        accessLevel: 0,
                        totalPoints: 0,
                        semesterPoints: 0,
                        weekPoints: 0,
                    },
                    success: function () {
                        $('#successModal').modal('show');
                    },
                    error: function (jqXHR) {
                        $('#divErrText').text(jqXHR.responseText);
                        $('#divError').show('fade');
                    }
                });
            },
            error: function (jqXHR) {
                $('#divErrText').text(jqXHR.responseText);
                $('#divError').show('fade');
            }
        });
    });
});