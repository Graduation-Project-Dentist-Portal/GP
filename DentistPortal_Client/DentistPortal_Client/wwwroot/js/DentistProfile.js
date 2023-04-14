$('body').on('click', '.Submit', function () {
    //debugger;
    $(this).parent('div').find('.Submit').prop('disabled', (i, v) => !v)
    var userName = $('#username').val();
    var firstName = $('#firstname').val();
    var lastName = $('#lastname').val();
    var Password = $('#password').val();
    var Universityy = $('#university').val();
    var Levell = $('#level').val();
    console.log(userName);
    console.log(firstName);
    console.log(lastName);
    console.log(Password);
    console.log(Universityy);
    console.log(Levell);
    var Obj = {
        Username: userName,
        FirstName: firstName,
        LastName: lastName,
        PasswordHash: Password,
        University: Universityy,
        Level: Levell
    }
    //debugger;
    $.ajax({
        type: "POST",
        url: "/DentistProfile?handler=Update",
        data: { obj: Obj },
        contentType: 'application/x-www-form-urlencoded',
        dataType: "json",
        headers:
        {
            "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (msg) {
            debugger;
            console.log(msg.status);
            console.log(msg.msg);
            Swal.fire({
                icon: msg.status,
                title: msg.msg,
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
    $(this).parent('div').find('.ForEdit').attr('disabled' , true);
});

$('body').on('click', '.edit', function () {
    debugger
    $(this).parent('div').find('.ForEdit').prop('disabled', (i, v) => !v)
    $(this).parent('div').find('.Submit').prop('disabled', (i, v) => !v)
});
