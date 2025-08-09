
//toggle menu
document.addEventListener("DOMContentLoaded", function () {
    const menuToggle = document.getElementById("menu-toggle");
    const sidebar = document.getElementById("sidebar");
    const closeBtn = document.getElementById("close-sidebar");

    if (menuToggle && sidebar && closeBtn) {
        
        menuToggle.addEventListener("click", function (e) {
            e.preventDefault();
            sidebar.classList.add("open");
        });

        closeBtn.addEventListener("click", function () {
            sidebar.classList.remove("open");
        });

        document.addEventListener("click", function (e) {
            const isClickInsideSidebar = sidebar.contains(e.target);
            const isClickOnToggle = menuToggle.contains(e.target);

            if (!isClickInsideSidebar && !isClickOnToggle) {
                sidebar.classList.remove("open");
            }
        });
    }
});

//scroll footer
window.addEventListener('scroll', function () {
    const footer = document.querySelector('.footer');
    if (window.scrollY > 300) {
        footer.classList.add('scroll-mode');
        return;
    } 

    footer.classList.remove('scroll-mode');
    return;
});

//button ontop
const btn = document.getElementById("btn-on-top");

window.addEventListener("scroll", function () {
    if (window.scrollY > 300) {
        btn.style.display = "block";
    } else {
        btn.style.display = "none";
    }
});

btn.addEventListener("click", function () {
    window.scrollTo({ top: 0, behavior: "smooth" });
});
