import {showPanel, EditorView} from "@codemirror/view";

const template = () => `
<div class="editor-info-panel">
    <p>Help: <kbd>F1</kbd></p>
    <p style="flex-grow: 1"></p>
    <p>Tab Mode <span class="info-tab-mode-content"></span><kbd>Ctrl</kbd>+<kbd>M</kbd></p>
</div>
`

/**
 * @param {EditorView} view
 */
function InfoPanel(view)
{

    const panel = document.createRange().createContextualFragment(template());
    panel.querySelector(".info-tab-mode-content")
        .textContent = getTabFocusMode(view) === -1 ? "Captured" : "Browser";

    const dom = document.createElement("div");
    dom.appendChild(panel);

    return { dom, update(updated) {
            dom.querySelector(".info-tab-mode-content")
                .textContent = getTabFocusMode(view) === -1 ? "Captured" : "Browser";
        }};
}

function getTabFocusMode(view)
{
    return !!view.inputState && view.inputState.tabFocusMode;
}

export function createInfoPanel() {
    return showPanel.of(InfoPanel);
}
