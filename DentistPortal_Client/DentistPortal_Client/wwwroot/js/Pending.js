const slidePage = document.querySelector(".slide-page");
const nextBtnFirst = document.querySelector(".firstNext");
const prevBtnSec = document.querySelector(".prev-1");
const nextBtnSec = document.querySelector(".next-1");
const prevBtnThird = document.querySelector(".prev-2");
const nextBtnThird = document.querySelector(".next-2");
const prevBtnFourth = document.querySelector(".prev-3");
const submitBtn = document.querySelector(".submit");
const progressText = document.querySelectorAll(".step p");
const progressCheck = document.querySelectorAll(".step .check");
const bullet = document.querySelectorAll(".step .bullet");
let current = 1;

nextBtnFirst.addEventListener("click", function (event) {
    event.preventDefault();
    slidePage.style.marginLeft = "-25%";
    bullet[current - 1].classList.add("active");
    progressCheck[current - 1].classList.add("active");
    progressText[current - 1].classList.add("active");
    current += 1;
});

submitBtn.addEventListener("click", function () {
    bullet[current - 1].classList.add("active");
    progressCheck[current - 1].classList.add("active");
    progressText[current - 1].classList.add("active");
    current += 1;
    setTimeout(function () {
        alert("Your Form Successfully Signed up");
        location.reload();
    }, 800);
});

prevBtnSec.addEventListener("click", function (event) {
    event.preventDefault();
    slidePage.style.marginLeft = "0%";
    bullet[current - 2].classList.remove("active");
    progressCheck[current - 2].classList.remove("active");
    progressText[current - 2].classList.remove("active");
    current -= 1;
});


$('body').on('click', '.Submit', function () {
    debugger;
    //$(this).parent('div').find('.Submit').prop('disabled', (i, v) => !v)
    var userName = $('#username').val();
    var firstName = $('#firstname').val();
    var lastName = $('#lastname').val();
    var Password = $('#password').val();
    var Universityy = $('#university').val();
    var Levell = $('#level').val();
    var email = $('#email').val();
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
        Level: Levell,
        Email: email
    }
    $(this).toggleClass('edit Submit')
    if ($(this).text() == "Edit")
        $(this).text('Update')
    else
        $(this).text('Edit')
    debugger;
    $.ajax({
        type: "POST",
        url: "PendingPage?handler=Update",
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
