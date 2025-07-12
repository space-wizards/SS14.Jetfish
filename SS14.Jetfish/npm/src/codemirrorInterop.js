import { EditorView, minimalSetup } from "codemirror"
import { markdown } from "@codemirror/lang-markdown";

let view = null;

export function initializeEditor(domId) {
    const parent = document.getElementById(domId);
    if (!parent) {
        console.error("DOM element not found:", domId);
        return;
    }

    view = new EditorView({
        doc: "Nik is a cat!",
        extensions: [
            minimalSetup,
            markdown()
        ],
        parent: parent
    })
}

export function getValue() {
    return view?.state?.doc?.toString();
}

export function setValue(newValue) {
    if (view) {
        view.dispatch({
            changes: { from: 0, to: view.state.doc.length, insert: newValue }
        });
    }
}
