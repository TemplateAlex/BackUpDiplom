var progressBar = document.querySelector(".progressBar")

document.addEventListener("scroll", function (event) {
    var windowScroll = document.documentElement.scrollTop;
    var windowHeight = document.documentElement.scrollHeight - document.documentElement.clientHeight;

    progressBar.style.width = (windowScroll / windowHeight) * 100 + "%";
});