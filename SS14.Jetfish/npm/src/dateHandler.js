export function HandleDateNode(node)
{
    let date = node.getAttribute("date");

    if (!date) {
        date = node.getAttribute("date-full");
        if (!date)
            return;

        let dateObj = new Date(date);
        node.innerHTML = dateObj.toLocaleString();

        return;
    }

    let dateObj = new Date(date);
    // TODO: Fancy date formatting (for example new comments should say "1 minute ago", "1 day ago" etc.)
    node.innerHTML = dateObj.toLocaleDateString();
}
