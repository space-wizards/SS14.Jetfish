import * as cm from './codemirrorInterop.js';
import * as ci from './contentImage';
import * as mutationObserver from './mutationObserver';

window.cmInterop = {
    initialize: cm.initializeEditor,
    destroy: cm.notifyEditorGone,
    getValue: cm.getValue,
    setValue: cm.setValue,
    debugAll: cm.getAll,
};

window.contentImage = {
    configure: ci.ContentImageConfigure,
    activate: ci.ContentImageActivate,
    deactivate: ci.ContentImageDeactivate,
}

mutationObserver.init();

console.log("Made with love <3, go play Space Station 14!")
