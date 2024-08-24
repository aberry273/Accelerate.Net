export default function (data) {
    return {
        mxField_inputClass: 'block w-full px-4 py-4 mt-2 text-xl placeholder-gray-400 bg-gray-200 rounded-lg focus:outline-none focus:ring-4 focus:ring-blue-600 focus:ring-opacity-50',
        // PROPERTIES
        mxField_type: 'text',
        mxField_placeholder: null,
        mxField_cssClass: null,
        mxField_id: null,
        mxField_name: null,
        mxField_min: null,
        mxField_max: null,
        mxField_accept: '',
        mxField_items: [],
        mxField_rows: 1,
        mxField_class: null,
        mxField_disabled: null,
        mxField_value: null,
        mxField_data: null,
        mxField_required: null,
        mxField_readOnly: null,
        mxField_autocomplete: null,
        mxField_ariaInvalid: null,
        mxField_areaDescribedBy: null,
        mxField_pattern: null,
        init() {
        },
        // GETTERS  
        get mxField_inputInvalidClass() { return 'invalid:[&:not(:placeholder-shown):not(:focus)]:border-red-500 '},     
        get mxField_inputEmailRegex() { return '[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$' },
        // METHODS
        _mxField_onChange(data) {
            const field = {
                id: this.mxField_id,
                name: this.mxField_name,
                value: data || this.mxField_value
            }
            this.$dispatch('oninputchange', field)
        },
        _mxField_setValues(params) {
            this.mxField_type = params.type || 'text';
            this.mxField_placeholder = params.placeholder;
            this.mxField_cssClass = params.cssClass;
            this.mxField_id = params.id;
            this.mxField_name = params.name;
            this.mxField_min = params.min;
            this.mxField_max = params.max;
            this.mxField_accept = params.accept;
            this.mxField_disabled = params.disabled;
            this.mxField_class = params.class;
            this.mxField_hidden = params.hidden;
            this.mxField_value = params.value || '';
            this.mxField_data = params.data;
            this.mxField_required = params.required;
            this.mxField_readOnly = params.readOnly;
            this.mxField_autocomplete = params.autocomplete;
            this.mxField_ariaInvalid = params.ariaInvalid;
            this.mxField_areaDescribedBy = params.areaDescribedBy;
            this.mxField_pattern = params.pattern;
        },
		_mxField_GetFilePreview(file) {
			return (typeof file == 'string') ? file : URL.createObjectURL(file)
		},
        _mxField_ConvertItemStringToObject(x) {
            return {
                title: x,
                value: x,
                disabled: false
            } 
        },
    }
}