import * as cm from './codemirrorInterop.js';
import * as ci from './contentImage';
import * as mutationObserver from './mutationObserver';
import * as settings from './settings';

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

window.settings = {
    setSetting: settings.setSetting,
}

mutationObserver.init();
ci.init();

console.log("Made with love <3, go play Space Station 14!")
