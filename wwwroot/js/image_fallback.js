document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('img[data-fallback-src]').forEach((image) => {
        image.addEventListener('error', () => {
            const fallbackSrc = image.dataset.fallbackSrc;

            if (!fallbackSrc || image.src.endsWith(fallbackSrc)) {
                return;
            }

            image.removeAttribute('data-fallback-src');
            image.src = fallbackSrc;
        });
    });
});
