export function ContentImageActivate(e)
{
    const image = e.target;
    console.log('activate', e);
}

export function ContentImageDeactivate(e)
{
    const image = e.target;
    console.log('deactivate',e);
}

export function init() {
    window.addEventListener('load', () =>
    {
        console.log('setting up images');
        const contentImages = document.querySelectorAll('[data-active-url]');
        console.log(contentImages);
        for (const contentImage of contentImages)
        {
            console.log(contentImage);
            contentImage.addEventListener('mouseover', () =>
            {
                contentImage.setAttribute('data-inactive-url', contentImage.getAttribute('src'));
                contentImage.setAttribute('src', contentImage.getAttribute('data-active-url'));

            });

            contentImage.addEventListener('mouseout', () =>
            {
                const inactiveUrl = contentImage.getAttribute('data-inactive-url');
                contentImage.removeAttribute('data-inactive-url');
                contentImage.setAttribute('src', inactiveUrl)

            });

            /*
            console.log('hovering');

                if (inactiveUrl)
                {
                    contentImage.removeAttribute('data-inactive-url');
                    contentImage.setAttribute('src', inactiveUrl)
                }
                else
                {

                }
             */
        }
    });
}
