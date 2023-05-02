$('body').on('click', '.Submit', function () {
    //debugger;
    //$(this).parent('div').find('.Submit').prop('disabled', (i, v) => !v)
    var userName = $('#username').val();
    var firstName = $('#firstname').val();
    var lastName = $('#lastname').val();
    var Password = $('#password').val();
    console.log(userName);
    console.log(firstName);
    console.log(lastName);
    console.log(Password);
    var Obj= {
        Username: userName,
        FirstName: firstName,
        LastName: lastName,
        PasswordHash: Password
    }
    $(this).toggleClass('edit Submit')
    if ($(this).text() == "Edit")
        $(this).text('Update')
    else
        $(this).text('Edit')
    //debugger;
    $.ajax({
        type: "POST",
        url: "/PatientProfile?handler=Update",
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
    $(this).parent('div').find('.ForEdit').attr('disabled', true)
});

$('body').on('click', '.edit', function () {        
    $(this).parent('div').find('.ForEdit').removeAttr('disabled')
    //$(this).parent('div').find('.Submit').prop('disabled', (i, v) => !v)
    $(this).toggleClass('edit Submit')
    if ($(this).text() == "Edit")
        $(this).text('Update')
    else
        $(this).text('Edit')
});
