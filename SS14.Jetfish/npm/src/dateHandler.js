export function init() {
    const config = { attributes: true, childList: true, subtree: true, attributeFilter: ["date", "date-full"] };

    const observer = new MutationObserver(mutation => {
        mutation.forEach((mutation) => {
            if (mutation.type !== 'childList')
                return;

            mutation.addedNodes.forEach(node => {
                if (node instanceof Element) {
                    let date = node.getAttribute("date");

                    if (date == null || date == "") {
                        date = node.getAttribute("date-full");
                        if (date == null || date == "")
                            return;

                        let dateObj = new Date(date);
                        node.innerHTML = dateObj.toLocaleString();

                        return;
                    }

                    let dateObj = new Date(date);
                    // TODO: Fancy date formatting (for example new comments should say "1 minute ago", "1 day ago" etc.)
                    node.innerHTML = dateObj.toLocaleDateString();
                }
            })
        })
    })

    observer.observe(document.body, config)
}
