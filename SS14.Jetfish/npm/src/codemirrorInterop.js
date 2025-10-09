import { EditorView } from "codemirror";
import { indentWithTab, history, defaultKeymap, historyKeymap } from "@codemirror/commands";
import { markdown } from "@codemirror/lang-markdown";
import { EditorState } from '@codemirror/state';
import { highlightSelectionMatches, selectSelectionMatches, selectNextOccurrence } from '@codemirror/search';
import { indentOnInput, syntaxHighlighting, defaultHighlightStyle, bracketMatching } from '@codemirror/language';
import { highlightSpecialChars, drawSelection, dropCursor, keymap, placeholder  } from "@codemirror/view";
import { closeBrackets, autocompletion, closeBracketsKeymap, completionKeymap } from '@codemirror/autocomplete';
import { oneDark } from "@codemirror/theme-one-dark"
import { createInfoPanel } from "./editor/infoPanel";

let views = new Map();

/**
 * Toggles the tab focus mode while also causing a view state update so the info panel gets updated.
 * @param {EditorView} view
 * @returns {boolean}
 */
const customToggleTabFocusMode = view => {
    view.setTabFocusMode();
    // Figuring this out meant I had to dive into the codemirror source code. Great
    view.dispatch(view.state.update(view.state.update(), { userEvent: "input" }));
    return true;
};

const customKeymap = [
    { key: "Ctrl-m", mac: "Shift-Alt-m", run: customToggleTabFocusMode, preventDefault: true },
    { key: "Mod-Shift-l", run: selectSelectionMatches },
    { key: "Mod-d", run: selectNextOccurrence, preventDefault: true }
];

// TODO: Use a mutation observer to detect when the editor is removed from the DOM and destroy it
export function initializeEditor(domId) {
    const parent = document.getElementById(domId);
    if (!parent) {
        console.error("Failed to initialize editor, element not found: ", domId);
        return null;
    }

    let view = new EditorView({
        doc: "",
        extensions: [
            highlightSpecialChars(),
            history(),
            drawSelection(),
            dropCursor(),
            EditorState.allowMultipleSelections.of(true),
            indentOnInput(),
            syntaxHighlighting(defaultHighlightStyle, { fallback: true }),
            bracketMatching(),
            closeBrackets(),
            autocompletion(),
            highlightSelectionMatches(),
            markdown(),
            placeholder("Click to start typing... Markdown supported!"),
            oneDark,
            keymap.of([
                // Getting the keybind to work took so much effort wtf.
                // For the future: use the spread operator to add keymaps and the earlier in the list the higher the priority.
                ...customKeymap,
                ...closeBracketsKeymap,
                ...(defaultKeymap.filter(b => b.key !== "Ctrl-m")),
                ...historyKeymap,
                ...completionKeymap,
                indentWithTab
            ]),
            createInfoPanel()
        ],
        parent: parent,
    })

    // Prevent tab from being captured by default
    view.setTabFocusMode(true);

    views.set(domId, view);
    console.debug("Initialized Editor: ", domId, view);
}

export function notifyEditorGone(domId) {
    console.debug("Deleting Editor: ", domId);
    views.get(domId)?.destroy();
    views.delete(domId);
}

export function getValue(domId) {
    let value = views.get(domId)?.state?.doc?.toString();
    console.debug("[Get] Value: ", domId, value);
    return value;
}

export function setValue(domId, newValue) {
    console.debug("[Set] Value: ", domId, newValue);
    views.get(domId)?.dispatch({
        changes: { from: 0, to: views.get(domId).state.doc.length, insert: newValue }
    });
}

export function getAll() {
    let returnVal = [];
    for (let pair of views) {
        const [key, value] = pair;
        console.log(key + " = " + value);
        returnVal.push({
            key: key,
            value: value,
            textValue: value.state.doc.toString()
        });
    }

    return returnVal;
}
