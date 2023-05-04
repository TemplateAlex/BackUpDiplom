const btnPopup = document.querySelector('.btn-action');
const iconClose = document.querySelector('.icon-close');
const wrapper = document.querySelector('.form-box-wrapper');

btnPopup.addEventListener('click', () => {
    
    wrapper.style.transform = "scale(1)";
});

iconClose.addEventListener('click', () => {
    wrapper.style.transform = "scale(0)";
    
});

