export function setSetting(name, newSetting) {
    const oldSetting = sessionStorage.getItem(name);
    sessionStorage.setItem(name, newSetting);

    const event = new SettingChangedEvent(name, oldSetting, newSetting);
    window.dispatchEvent(event);

    console.debug(`Setting "${name}" changed from "${oldSetting}" to "${newSetting}"`);
}

export function getSetting(name) {
    return sessionStorage.getItem(name);
}

class SettingChangedEvent extends Event {
    constructor(name, oldValue, newValue) {
        super("settingchange");
        this._name = name;
        this._oldValue = oldValue;
        this._newValue = newValue;
    }

    get name() {
        return this._name;
    }

    get oldValue() {
        return this._oldValue;
    }

    get newValue() {
        return this._newValue;
    }
}

export const SettingAlwaysPlayAnimatedMedia = "SettingAlwaysPlayAnimatedMedia";
