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
            this.mxField_label = params.label;
            this.mxField_required = params.required;
            this.mxField_readOnly = params.readOnly;
            this.mxField_autocomplete = params.autocomplete;
            this.mxField_ariaInvalid = params.ariaInvalid;
            this.mxField_areaDescribedBy = params.areaDescribedBy;
            this.mxField_pattern = params.pattern;
        },
        toggle() {
            this.mxField_value =! this.mxField_value;
            this._mxField_onChange();
        },
        render() {
            const html =  `
                <div class="flex items-center justify-start space-x-2 py-2">
                    <input   
                        class="peer"  
                        :id="mxField_id"
                        :name="mxField_name" 
                        :disabled="true"
                        :hidden="true"
                        :value="mxField_value"
                        x-model="mxField_value"  
                        :required="mxField_required" 
                        :aria-invalid="mxField_ariaInvalid"
                        :class="mxField_class" 
                        :aria-describedBy="mxField_areaDescribedBy || mxField_id" 
                    />
                    
                    <button 
                        x-ref="switchButton"
                        type="button" 
                        @click="toggle"
                        :class="mxField_value ? 'bg-blue-600' : 'bg-neutral-200'" 
                        class="relative inline-flex h-6 py-0.5 ml-4 focus:outline-none rounded-full w-10"
                        :class="mxField_cssClass"
                        x-cloak>
                        <span :class="mxField_value 
                            ? 'translate-x-[18px]' 
                            : 'translate-x-0.5'" 
                        class="w-5 h-5 duration-200 ease-in-out bg-white rounded-full shadow-md"></span>
                    </button>
                
                    <label @click="$refs.switchButton.click(); $refs.switchButton.focus()" :id="$id('switch')" 
                        :class="{ 'text-blue-600': mxField_value, 'text-gray-400': ! mxField_value }"
                        class="text-xl select-none"
                        x-cloak
                        x-text="mxField_label"> 
                    </label>
                </div>
                
                <span x-text="field.invalidText || 'Invalid input'" class="mt-2 hidden text-sm text-red-500 peer-[&:not(:placeholder-shown):not(:focus):invalid]:block">
                </span>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      },
    }
}