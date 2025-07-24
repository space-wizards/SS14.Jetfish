export function ContentImageConfigure()
{
    const contentImages = document.querySelectorAll('[data-activate-on-hover=true]');
    for (const contentImage of contentImages)
    {
        contentImage.addEventListener('mouseover', () => {
            contentImage.setAttribute('src', contentImage.getAttribute('data-active-url'));

        }, true);

        contentImage.addEventListener('mouseout', () => {
            contentImage.setAttribute('src', contentImage.getAttribute('data-inactive-url'))

        }, true);
    }
}
