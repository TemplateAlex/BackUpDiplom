$(document).ready(function () {
    document.querySelectorAll(".progress-ring__circle").forEach(item => {
        var radius = item.r.baseVal.value;
        var circleLenght = 2 * Math.PI * radius;
        item.style.strokeDasharray = `${circleLenght} ${circleLenght}`;
        item.style.strokeDashoffset = circleLenght
        var percent = item.parentElement.parentElement.parentElement.childNodes[3].childNodes[1].textContent;
        var percentNumber = Number(percent.substring(0, percent.length - 1));
        var offset = circleLenght - percentNumber / 100 * circleLenght;
        item.style.strokeDashoffset = offset;

        if (percentNumber < 40) {
            item.style.stroke = "#FF0000";
        }
        else if (percentNumber < 60) {
            item.style.stroke = "#FFA500";
        }
        else if (percentNumber < 80) {
            item.style.stroke = "#FFFF00"
        }
        else {
            item.style.stroke = "#00FF00";
        }

    });
});