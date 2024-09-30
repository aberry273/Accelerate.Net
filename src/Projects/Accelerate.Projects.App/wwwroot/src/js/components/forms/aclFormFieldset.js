import { mxContent, mxForm } from '/src/js/mixins/index.js';

import {
    aclFieldInput,
} from '/src/js/components/fields/index.js'

import * as fields from '/src/js/components/fields/index.js'
/*
Object.keys(fields).forEach(svc => {
    let settings = wssSettings.filter(x => x.serviceName == svc)[0]
    if (settings != null) {
        let data = fields[svc](settings);
        alpinejs.store(svc, data);
    }
});
*/
export default function (params) {
    return {
        ...mxContent(params),
        ...mxForm(params),
        // PROPERTIES 
        // INIT
        init() {
            this.setValues(params);
            this.render();
        },
        // GETTERS
        // METHODS
        setValues(params) {
            this.mxForm_fields = params.fields;
        },
        onFieldChange(field) {
            this.$dispatch('onfieldchange', field)
        },
        getFieldComponent(field) {
            const fieldType = field.component || field.type;
            const fieldComponent = fields[fieldType];
            return fieldComponent != null ? fieldComponent(field) : aclFieldInput(field)
        },
        getFieldKey(field, i) {
            const key = field.id || field.name;
            return `${key}:${i}${field.updated}`;
        },
        render() {
            const html = `
                <template x-for="(field, i) in mxForm_fields" :key="getFieldKey(field, i)">
                    <div>
                        <label x-cloak :for="field.id || field.name" class="relative" x-show="!field.hidden">
                            <span x-show="field.label && field.component != 'aclFieldSwitch'" class="font-medium text-gray-900" x-text="field.label"></span>
                            <div x-data="getFieldComponent(field)" @oninputchange="(ev) => { onFieldChange(ev.detail) }"></div>
                            <div x-show="field.helperText != null && field.helperText.length > 0">
                                <small x-text="field.helperText"></small>
                            </div>
                        </label>
                    </div>
                </template>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
        },
    }
}