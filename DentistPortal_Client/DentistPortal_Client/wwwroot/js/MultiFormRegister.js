


const signUpButton = document.getElementById('signUp');
const form = document.getElementById('register-form');
const usernameInput = document.getElementById("username");
const firstNameInput = document.getElementById("first-name");
const lastNameInput = document.getElementById("last-name");
const emailInput = document.getElementById("email");
const passwordInput = document.getElementById("password");

const signInButton = document.getElementById('signIn');
//const container = document.getElementById('container');

//function checkInputs() {
//	if (usernameInput.value !== "" && emailInput.value !== "" && passwordInput.value !== "") {
//		signUpButton.style.display = "block";
//	} else {
//		signUpButton.style.display = "none";
//	}
//}
//usernameInput.addEventListener("input", checkInputs);
//emailInput.addEventListener("input", checkInputs);
//passwordInput.addEventListener("input", checkInputs);


signInButton.addEventListener('click', () => {
    container.classList.remove("right-panel-active");

});
signUpButton.addEventListener('click', () => {
    if (usernameInput.value === "" || emailInput.value === "" || passwordInput.value === "" || firstNameInput.value === "" || lastNameInput.value === "") {
        event.preventDefault()
        event.stopPropagation()
    }
    else {
        container.classList.add("right-panel-active");
    }
    form.classList.add('was-validated')
});


