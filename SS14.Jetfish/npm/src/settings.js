export function setSetting(name, newSetting) {
    let oldSetting = sessionStorage.getItem(name);
    if (oldSetting) {
        oldSetting = JSON.parse(oldSetting);
    }
    sessionStorage.setItem(name, JSON.stringify(newSetting));

    const event = new SettingChangedEvent(name, oldSetting, newSetting);
    window.dispatchEvent(event);

    console.debug(`Setting "${name}" changed from "${oldSetting}" to "${newSetting}"`);
}

export function getSetting(name, defaultValue) {
    const value = sessionStorage.getItem(name);
    if (value)
        return JSON.parse(value);
    else
        return defaultValue;
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
