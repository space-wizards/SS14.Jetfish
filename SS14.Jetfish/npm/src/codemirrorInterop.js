import { EditorView, minimalSetup } from "codemirror"
import { markdown } from "@codemirror/lang-markdown";
import { placeholder } from "@codemirror/view";

let views = new Map();

export function initializeEditor(domId) {
    const parent = document.getElementById(domId);
    if (!parent) {
        console.error("Failed to initialize editor, element not found: ", domId);
        return null;
    }

    let view = new EditorView({
        doc: "",
        extensions: [
            minimalSetup,
            markdown(),
            placeholder("Click to start typing... Markdown supported!")
        ],
        parent: parent
    })

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
