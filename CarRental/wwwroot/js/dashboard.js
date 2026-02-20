const navBar = document.querySelectorAll(".admin-menu a");
navBar.forEach(row => {
    row.addEventListener("click", (e) => {
        navBar.forEach(item => item.classList.remove("active"));
        row.classList.add("active")
    })
})