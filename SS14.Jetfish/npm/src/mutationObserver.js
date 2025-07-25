import {ContentImageConfigure} from "./contentImage";
import {HandleDateNode} from "./dateHandler";

export function init() {
    const config = { attributes: true, childList: true, subtree: true, attributeFilter: ["date", "date-full"] };

    const observer = new MutationObserver(mutation => {
        mutation.forEach((mutation) => {
            mutation.addedNodes.forEach(node => {
                if (!(node instanceof Element))
                    return;

                ContentImageConfigure();
                HandleDateNode(node);
            })
        })
    })

    observer.observe(document.body, config)
}
