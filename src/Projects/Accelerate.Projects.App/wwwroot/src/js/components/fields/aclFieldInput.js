import { mxField } from '/src/js/mixins/index.js';

export default function (params) {
	return {
        ...mxField(params),
        // PROPERTIES
        type: '',
        value: null,
        placeholder: '',
        cssClass: '',
        // INIT
        init() {
            this.setValues(params);
            this.render(); 
        },
        // GETTERS
        // METHODS
        setValues(params) {
            this.mxField_type = params.type || 'text';
            this.mxField_placeholder = params.placeholder;
            this.mxField_cssClass = params.cssClass;
            this.mxField_id = params.id;
            this.mxField_name = params.name;
            this.mxField_min = params.min;
            this.mxField_max = params.max;
            this.mxField_disabled = params.disabled;
            this.mxField_class = params.class;
            this.mxField_value = params.value;
            this.mxField_required = params.required;
            this.mxField_readOnly = params.readOnly;
            this.mxField_autocomplete = params.autocomplete;
            this.mxField_ariaInvalid = params.ariaInvalid;
            this.mxField_areaDescribedBy = params.areaDescribedBy;
            this.mxField_pattern = params.pattern;
        },
        render() {
            const html =  `
                <input 
                    :type="mxField_type"
                    :placeholder="mxField_placeholder"
                    class="peer" 
                    :class="mxField_cssClass || mxField_inputClass"
                    :id="mxField_id"
                    :name="mxField_name"
                    :min="mxField_min"
                    :max="mxField_max"
                    :disabled="mxField_disabled"
                    :value="mxField_value"
                    x-model="mxField_value"
                    :read-only="mxField_readOnly"
                    :checked="mxField_value"
                    :required="mxField_required"
                    :autocomplete="mxField_autocomplete"
                    :aria-invalid="mxField_ariaInvalid"
                    :class="mxField_class"
                    :pattern="mxField_pattern"
                    :aria-describedBy="mxField_areaDescribedBy || mxField_id"
                    data-primary="blue-600"
                    data-rounded="rounded-lg"
                    @change="_mxField_onChange"
                />
                <span x-text="field.invalidText || 'Invalid input'" class="mt-2 hidden text-sm text-red-500 peer-[&:not(:placeholder-shown):not(:focus):invalid]:block">
                </span>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      },
    }
}