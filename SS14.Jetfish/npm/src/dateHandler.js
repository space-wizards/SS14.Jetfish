import {ContentImageConfigure} from "./contentImage";

export function init() {
    const config = { attributes: true, childList: true, subtree: true, attributeFilter: ["date", "date-full"] };

    const observer = new MutationObserver(mutation => {
        mutation.forEach((mutation) => {
            mutation.addedNodes.forEach(node => {
                if (!(node instanceof Element))
                    return;

                ContentImageConfigure();

                // Empty strings are falsy in javascript
                let date = node.getAttribute("date") ?? node.getAttribute("data-date");
                if (!date)
                    return;

                let dateObj = new Date(date);
                // TODO: Fancy date formatting (for example new comments should say "1 minute ago", "1 day ago" etc.)
                node.innerHTML = dateObj.toLocaleDateString();

            })
        })
    })

    observer.observe(document.body, config)
}
