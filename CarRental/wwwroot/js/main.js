const menuToggle = document.querySelector('.menu-toggle');
const navMenu = document.querySelector('nav');

menuToggle.onclick = () => {
    // بنضيف ونشيل كلاس active من الـ nav والزرار نفسه
    menuToggle.classList.toggle('active');
    navMenu.classList.toggle('active');
};
