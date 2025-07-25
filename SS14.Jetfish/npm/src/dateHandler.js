export function HandleDateNode(node)
{
    // Empty strings are falsy in javascript
    let date = node.getAttribute("date") ?? node.getAttribute("data-full");
    if (!date)
        return;

    let dateObj = new Date(date);
    // TODO: Fancy date formatting (for example new comments should say "1 minute ago", "1 day ago" etc.)
    node.innerHTML = dateObj.toLocaleDateString();
}
