import {getSetting, SettingAlwaysPlayAnimatedMedia} from "./settings";

let shouldAlwaysPlay = false;

export function init() {
    shouldAlwaysPlay = getSetting(SettingAlwaysPlayAnimatedMedia, false);

    window.addEventListener("settingchange", (event) => {
        if (event.name !== SettingAlwaysPlayAnimatedMedia)
            return;

        const contentImages = document.querySelectorAll('[data-activate-on-hover=true]');
        const contentImageVideos = document.querySelectorAll('[data-play-on-hover=true]');

        if (event.newValue === true) {
            contentImages.forEach(image => ContentImageActivate({ target: image }));
            contentImageVideos.forEach(video => video.play().catch(_ => {}));
        } else {
            contentImages.forEach(image => ContentImageDeactivate({ target: image }));
            contentImageVideos.forEach(video => {
                video.autoplay = false;
                video.pause();
            });
        }

        shouldAlwaysPlay = getSetting(SettingAlwaysPlayAnimatedMedia, false);
    });
}

export function ContentImageConfigure()
{
    shouldAlwaysPlay = getSetting(SettingAlwaysPlayAnimatedMedia, false);

    const contentImages = document.querySelectorAll('[data-activate-on-hover=true]');
    for (const contentImage of contentImages)
    {
        if (shouldAlwaysPlay) {
            ContentImageActivate({ target: contentImage });
        }

        contentImage.addEventListener('mouseover', window.contentImage.activate, true);
        contentImage.addEventListener('mouseout', window.contentImage.deactivate, true);
    }

    const contentImageVideos = document.querySelectorAll('[data-play-on-hover=true]');
    for (const video of contentImageVideos)
    {
        if (shouldAlwaysPlay)
            video.play().catch(_ => {});

        video.addEventListener('mouseover', () => video.play().catch(_ => {}), true);
        video.addEventListener('mouseout', () => {
            if (shouldAlwaysPlay)
                return;

            video.pause()
        }, true);
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
