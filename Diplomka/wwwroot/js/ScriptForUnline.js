document.querySelectorAll(".underline-text__wrapper").forEach(elem => {
    elem.addEventListener("mousemove", function () {
        this.querySelector(".unline").style.width = 100 + "%";
    });
    elem.addEventListener("mouseout", function () {
        this.querySelector(".unline").style.width = 0 + "%";
    });
});