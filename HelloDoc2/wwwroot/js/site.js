// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const buttons = document.querySelectorAll('.box');
buttons.forEach(button => {
    button.addEventListener('click', () => {
        buttons.forEach(otherButton => {
            otherButton.classList.remove('selected');

        });
        button.classList.add('selected');

    });
});
