import * as cm from './codemirrorInterop.js';
import * as dh from './dateHandler';

window.cmInterop = {
    initialize: cm.initializeEditor,
    destroy: cm.notifyEditorGone,
    getValue: cm.getValue,
    setValue: cm.setValue,
    debugAll: cm.getAll,
};

dh.init();

console.log("Made with love <3, go play Space Station 14!")
