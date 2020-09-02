function CollapsibleFilter() {
    var left = document.getElementById("filter");
    if (left.style.left !== "5px") {
        left.style.left = "5px";
    } else {
        left.style.left = "-480px";
    }
}

