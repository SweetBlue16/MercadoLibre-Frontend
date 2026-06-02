document.addEventListener('DOMContentLoaded', () => {
    const htmlElement = document.documentElement;
    const switchElement = document.getElementById('darkModeSwitch');
    const modoLuz = document.querySelector('.modo-luz');

    if (!switchElement || !modoLuz) {
        return;
    }

    const currentTheme = localStorage.getItem('bsTheme') || 'light';

    htmlElement.dataset.bsTheme = currentTheme;
    switchElement.checked = currentTheme === 'dark';

    switchElement.addEventListener('change', function () {
        const selectedTheme = this.checked ? 'dark' : 'light';

        htmlElement.dataset.bsTheme = selectedTheme;
        localStorage.setItem('bsTheme', selectedTheme);

        modoLuz.classList.toggle('bi-brightness-high', selectedTheme === 'light');
        modoLuz.classList.toggle('bi-moon-stars', selectedTheme === 'dark');
    });
});