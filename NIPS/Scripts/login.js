$(document).ready(function () {
    $('#linkClose').click(function () {
        $('#divError').hide('fade');
    });
    $('#login').click(function () {
        $.ajax({
            url: '/token',
            type: 'POST',
            contentType: 'application/json',
            data: {
                username: $('#email').val(),
                password: $('#password').val(),
                grant_type: 'password',
            },
            success: function (response) {
                sessionStorage.setItem('accessToken', response.access_token);
                sessionStorage.setItem('userName', response.userName);
                window.location.href = "Home.html";
            },
            error: function (jqXHR) {
                $('#divErrText').text(jqXHR.responseText);
                $('#divError').show('fade');
            }
        });
    });
});