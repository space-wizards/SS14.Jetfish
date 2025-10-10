import {showPanel, EditorView, keymap} from "@codemirror/view";
import {StateField, StateEffect} from "@codemirror/state"

const template = () => `
    <div class="editor-help-panel">
        <div class="help-column">
            <div class="help-row"><p>Select all</p><p><kbd>Ctrl</kbd>+<kbd>A</kbd></p></div>
            <div class="help-row"><p>Select line</p><p><kbd>Alt</kbd>+<kbd>L</kbd></p></div>
            <div class="help-row"><p>Simplify selection</p><p><kbd>Esc</kbd></p></div>
            <div class="help-row"><p>Add a cursor to the next matching selection</p><p><kbd>Ctrl</kbd>+<kbd>D</kbd></p></div>
            <div class="help-row"><p>Add a cursor to all matching selections</p><p><kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>L</kbd></p></div>
        </div>
        <div class="help-column">
            <div class="help-row"><p>Add a cursor above the current selections</p><p><kbd>Ctrl</kbd>+<kbd>Alt</kbd>+<kbd>↑</kbd></p></div>
            <div class="help-row"><p>Add a cursor below the current selections</p><p><kbd>Ctrl</kbd>+<kbd>Alt</kbd>+<kbd>↓</kbd></p></div>
            <div class="help-row"><p>Insert blank line</p><p><kbd>Ctrl</kbd>+<kbd>Enter</kbd></p></div>
            <div class="help-row"><p>Move line up</p><p><kbd>Alt</kbd>+<kbd>↑</kbd></p></div>
            <div class="help-row"><p>Move line down</p><p><kbd>Alt</kbd>+<kbd>↓</kbd></p></div>
        </div>
        <div class="help-column">
            <div class="help-row"><p>Toggle capturing tab</p><p><kbd>Ctrl</kbd>+<kbd>M</kbd></p></div>
            <div class="help-row"><p>Start autocompletion (mentions etc.)</p><p><kbd>Ctrl</kbd>+<kbd>Space</kbd></p></div>
            <div class="help-row"><p>Move to document start</p><p><kbd>Ctrl</kbd>+<kbd>Home</kbd></p></div>
            <div class="help-row"><p>Move to document end</p><p><kbd>Ctrl</kbd>+<kbd>End</kbd></p></div>
        </div>
    </div>
`;

const toggleHelp = StateEffect.define({});

const helpPanelState = StateField.define({
    create: () => false,
    update(value, tr) {
        for (let e of tr.effects) if (e.is(toggleHelp)) value = e.value
        return value
    },
    provide: f => showPanel.from(f, on => on ? HelpPanel : null)
});


const helpKeymap = [{
    key: "F1",
    run(view) {
        view.dispatch({
            effects: toggleHelp.of(!view.state.field(helpPanelState))
        })
        return true
    }
}];


/**
 * @param {EditorView} view
 */
function HelpPanel(view) {

    const panel = document.createRange().createContextualFragment(template());
    const dom = document.createElement("div");
    dom.appendChild(panel);

    return { top: true, dom };
}

export const helpPanelExtension = (() => [helpPanelState, keymap.of(helpKeymap)])();
