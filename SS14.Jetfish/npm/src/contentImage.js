import {getSetting, SettingAlwaysPlayAnimatedMedia} from "./settings";

let shouldAlwaysPlay = false;

export function init() {
    shouldAlwaysPlay = getSetting("SettingAlwaysPlayAnimatedMedia");

    window.addEventListener("settingchange", (event) => {
        if (event.name !== SettingAlwaysPlayAnimatedMedia)
            return;

        const contentImages = document.querySelectorAll('[data-activate-on-hover=true]');

        if (event.newValue === true) {
            contentImages.forEach(image => ContentImageActivate({ target: image }));
        } else {
            contentImages.forEach(image => ContentImageDeactivate({ target: image }));
        }
    });
}

export function ContentImageConfigure()
{
    shouldAlwaysPlay = getSetting("SettingAlwaysPlayAnimatedMedia");

    const contentImages = document.querySelectorAll('[data-activate-on-hover=true]');
    for (const contentImage of contentImages)
    {
        if (shouldAlwaysPlay) {
            ContentImageActivate({ target: contentImage });
        }

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
    if (shouldAlwaysPlay)
        return;

    const image = e.target;
    image.setAttribute('src', image.getAttribute('data-inactive-url'))
}
