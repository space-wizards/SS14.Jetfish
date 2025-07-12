import * as cm from './codemirrorInterop.js';

window.cmInterop = {
    initialize: cm.initializeEditor,
    getValue: cm.getValue,
    setValue: cm.setValue
};
