import * as cm from './codemirrorInterop.js';
import * as dh from './dateHandler';
import * as ci from './contentImage';

window.cmInterop = {
    initialize: cm.initializeEditor,
    destroy: cm.notifyEditorGone,
    getValue: cm.getValue,
    setValue: cm.setValue,
    debugAll: cm.getAll,
};

window.contentImage = {
    activate: ci.ContentImageActivate,
    deactivate: ci.ContentImageDeactivate,
}

dh.init();
ci.init();

console.log("Made with love <3, go play Space Station 14!")
