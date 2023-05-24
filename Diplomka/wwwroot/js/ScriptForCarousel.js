$(document).ready(function () {
    var index = 0;

    setInterval(function () {
        var sliderItems = document.querySelectorAll(".slider-item");
        sliderItems[index].classList.remove("slider-item__active");

        if (index + 1 >= sliderItems.length) index = 0;
        else index += 1;

        sliderItems[index].classList.add("slider-item__active");
    }, 5000)
});