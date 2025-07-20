import * as cm from './codemirrorInterop.js';
import * as dh from './dateHandler';
import {notifyEditorGone} from "./codemirrorInterop.js";

window.cmInterop = {
    initialize: cm.initializeEditor,
    destroy: cm.notifyEditorGone,
    getValue: cm.getValue,
    setValue: cm.setValue,
    debugAll: cm.getAll,
};

dh.init();
