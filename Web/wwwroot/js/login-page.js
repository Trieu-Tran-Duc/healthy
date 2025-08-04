$(document).ready(function () {
    $('#loginForm').submit(function (e) {
        e.preventDefault(); 

        var email = $('#exampleInputEmail').val();
        var password = $('#exampleInputPassword').val();
        
        $.ajax({
            url: '/Login/Login',
            type: 'POST',
            data: {
                Email: email,
                Password: password
            },
            success: function (res) {
                if (res.success) {
                    window.location.href = res.redirectUrl;
                } else {
                    $('#errorMessage').removeClass('d-none').text(res.message);
                }
            },
            error: function () {
            }
        });
    });
});
