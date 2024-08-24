import { mxField } from '/src/js/mixins/index.js';

export default function (params) {
	return {
        ...mxField(params),
        // PROPERTIES
        type: '',
        value: null,
        placeholder: '',
        cssClass: '',
        selectOpen: false,
        selectedItem: '',
        selectableItemActive: null,
        selectId: 'aclFieldSelectCheckbox',
        selectKeydownValue: '',
        selectKeydownTimeout: 1000,
        selectKeydownClearTimeout: null,
        selectDropdownPosition: 'bottom',
        isObjectItems: true,
        
        // INIT
        init() {
            this.setValues(params);
            this.selectedItem = this.mxField_items.filter(x => x.value == this.mxField_value)[0]
            this.render();
        },
        // GETTERS
        get selectedItemText() {
            if(this.mxField_value == null || this.mxField_value.length == 0) return null;
            if(this.isObjectItems) {
                this.mxField_value.map(x => x.title).join(', ');
                return;
            }
            return this.mxField_value.join(', ');
        },
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

            this.isObjectItems = params.isObjectItems;
            
            if (this.isObjectItems === false) {
                params.items = params.items.map(x => {
                    return this._mxField_ConvertItemStringToObject(x)
                });
            }  
            this.mxField_items = params.items;
            this.selectId = this.$id('select');
        },
        selectItem(item) {
            let values = this.mxField_value || [];
            if (!Array.isArray(values)) {
                values = [];
            }
            const index = values.indexOf(item.value);
            if (index == -1) {
                values.push(item.value)
            } else {
                values.splice(index, 1);
            }
            this.mxField_value = values;
            this._mxField_onChange();
        },
        getItemLabel(item, i) {
            return item.value+'_'+i;
        },
        getCheckboxClass(item, i) {
            const label = this.getItemLabel(item, i);
            return `cursor-pointer peer-checked:[&_svg]:scale-100 block font-medium truncate text-neutral-600 peer-checked:text-blue-600 [&_svg]:scale-0 peer-checked:[&_.${label}]:border-blue-500 peer-checked:[&_.${label}]:bg-blue-500 select-none flex items-center space-x-2`
        },
        itemSelected(item) {
            if (this.mxField_value == null) return -1;
            const index = this.mxField_value.indexOf(item.value);
            return index > -1;
        }, 
        render() {
            const html =  `
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
                <button type="button" x-ref="selectButton" @click="selectOpen=!selectOpen"
                    :class="{ 'focus:ring-2 focus:ring-offset-2 focus:ring-neutral-400' : !selectOpen }"
                    class="relative text-xl px-4 py-4 mt-2 min-h-[38px] flex items-center justify-between w-full py-2 pl-3 pr-10 text-left placeholder-gray-400 bg-gray-200 border rounded-md shadow-sm cursor-default border-neutral-200/70 focus:outline-none text-sm">
                    <span 
                        x-text="selectedItemText ? selectedItemText : 'Select Item'" 
                            class="truncate">Select Item</span>
                    <span class="absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true" class="w-5 h-5 text-gray-400"><path fill-rule="evenodd" d="M10 3a.75.75 0 01.55.24l3.25 3.5a.75.75 0 11-1.1 1.02L10 4.852 7.3 7.76a.75.75 0 01-1.1-1.02l3.25-3.5A.75.75 0 0110 3zm-3.76 9.2a.75.75 0 011.06.04l2.7 2.908 2.7-2.908a.75.75 0 111.1 1.02l-3.25 3.5a.75.75 0 01-1.1 0l-3.25-3.5a.75.75 0 01.04-1.06z" clip-rule="evenodd"></path></svg>
                    </span>
                </button>
                
                <ul x-show="selectOpen"
                    x-ref="selectableItemsList"
                    @click.away="selectOpen = false"
                    x-transition:enter="transition ease-out duration-50"
                    x-transition:enter-start="opacity-0 -translate-y-1"
                    x-transition:enter-end="opacity-100"
                    :class="{ 'bottom-0 mb-10' : selectDropdownPosition == 'top', 'top-0 mt-6' : selectDropdownPosition == 'bottom' }"
                    class="absolute w-full z-10 py-1 mt-1 overflow-auto text-sm bg-white rounded-md shadow-md max-h-56 ring-1 ring-black ring-opacity-5 focus:outline-none"
                    x-cloak>
                    
                    <template x-for="(item, i) in mxField_items" :key="item.value">
                        <li @click="selectItem(item)"
                            :id="item.value + '-' + selectId"
                            :data-disabled="item.disabled"
                            :class="{ 'bg-neutral-100 text-gray-900' : itemSelected(item), '' : !itemSelected(item) }"
                            class="text-xl px-4  cursor-pointer relative flex items-center h-full py-2 pl-8 text-gray-700 cursor-default select-none data-[disabled]:opacity-50 data-[disabled]:pointer-events-none">
                            
                            <div class="flex items-center py-4" >
                                <div class="flex items-center h-5 w-full">
                                    <input 
                                        :name="getItemLabel(field, i)" 
                                        :id="getItemLabel(field, i)" 
                                        :value="itemSelected(item)" 
                                        type="checkbox" 
                                        class="flex items-center justify-center w-5 h-5 border-2 rounded custom-checkbox text-neutral-900"
                                        required>
                                    <span 
                                        :for="getItemLabel(field, i)" 
                                        class="pl-2" 
                                        x-text="item.title"></span>
                                </div>
                            </div>
                        </li>
                    </template>

                </ul>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      },
    }
}