import { mxContent, mxForm } from '/src/js/mixins/index.js';
import { 
    aclFieldInput,
    aclFieldTextarea,
    aclFieldContentEditable, 
    aclFieldEditorJs, 
    aclFieldSelect, 
    aclFieldSelectCheckbox,
    aclFieldFile,
    aclFieldSwitch
} from '/src/js/components/fields/index.js'

export default function (params) {
	return {
        ...mxContent(params),
        ...mxForm(params),
        // PROPERTIES 
        // INIT
        init() {
            this.setValues(params);
            this.render();
            /*
                this.$watch('mxForm_fields', (newVal, oldVal) => {
                    console.log(newVal);
                })
            */
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
            switch(fieldType)
            {
                case 'aclFieldInput':
                    return aclFieldInput(field);
                case 'aclFieldTextarea':
                    return aclFieldTextarea(field);
                case 'aclFieldSelect':
                    return aclFieldSelect(field);
                case 'aclFieldSelectCheckbox':
                    return aclFieldSelectCheckbox(field);
                case 'aclFieldContentEditable':
                    return aclFieldContentEditable(field);
                case 'aclFieldEditorJs':
                    return aclFieldEditorJs(field);
                case 'aclFieldFile':
                    return aclFieldFile(field);
                case 'aclFieldSwitch':
                    return aclFieldSwitch(field);
                default:
                    return aclFieldInput(field);
            }
        },
        // redundant
        renderField(field) {
            const component = field.component || 'aclFieldInput'
            return `${component}(field)`
        },
        getFieldKey(field) {
            const key = field.id || field.name;
            return `${key}:${field.updated}`;
        },
        render() {
            const html = `
                <template x-for="(field, i) in mxForm_fields" :key="getFieldKey(field)">
                    <div >
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