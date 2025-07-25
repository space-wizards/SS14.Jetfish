export function ContentImageConfigure()
{
    const contentImages = document.querySelectorAll('[data-activate-on-hover=true]');
    for (const contentImage of contentImages)
    {
        contentImage.addEventListener('mouseover', window.contentImage.activate, true);
        contentImage.addEventListener('mouseout', window.contentImage.deactivate, true);
    }
}

export function ContentImageActivate(e)
{
    const image = e.target;
    image.setAttribute('src', image.getAttribute('data-active-url'));
}

export function ContentImageDeactivate(e)
{
    const image = e.target;
    image.setAttribute('src', image.getAttribute('data-inactive-url'))
}
