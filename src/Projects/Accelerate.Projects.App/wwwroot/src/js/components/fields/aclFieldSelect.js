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
        selectedItem: null,
        selectableItemActive: null,
        selectId: 'aclFieldSelect',
        selectKeydownValue: '',
        selectKeydownTimeout: 1000,
        selectKeydownClearTimeout: null,
        selectDropdownPosition: 'bottom',
        areItemsObject: true,

        // INIT
        init() {
            this.setValues(params);
            const item = this.mxField_items.filter(x => x.value == this.mxField_value)[0]
            this.selectedItem = item;
            this.render();

            this.$watch('selectOpen', () => {
                if (!this.selectedItem) {
                    //this.selectableItemActive = this.mxField_items[0];
                } else {
                    this.selectableItemActive = this.selectedItem;
                }
                setTimeout(() => {
                    this.selectScrollToActiveItem();
                }, 10);
                this.selectPositionUpdate();
                window.addEventListener('resize', (event) => { this.selectPositionUpdate(); });
            });
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

            this.areItemsObject = params.areItemsObject;

            if (this.areItemsObject === false) {
                params.items = params.items.map(x => {
                    return this._mxField_ConvertItemStringToObject(x)
                });
            }
            this.mxField_items = params.items;
            this.selectId = this.$id('select');
        },
        selectableItemIsActive(item) {
            return this.selectableItemActive && this.selectableItemActive.value == item.value;
        },
        electableItemActiveNext() {
            let index = this.mxField_items.indexOf(this.selectableItemActive);
            if (index < this.mxField_items.length - 1) {
                this.selectableItemActive = this.mxField_items[index + 1];
                this.selectScrollToActiveItem();
            }
        },
        selectableItemActivePrevious() {
            let index = this.mxField_items.indexOf(this.selectableItemActive);
            if (index > 0) {
                this.selectableItemActive = this.mxField_items[index - 1];
                this.selectScrollToActiveItem();
            }
        },
        selectScrollToActiveItem() {
            if (this.selectableItemActive) {
                this.activeElement = document.getElementById(this.selectableItemActive.value + '-' + this.selectId)
                let newScrollPos = (this.activeElement.offsetTop + this.activeElement.offsetHeight) - this.$refs.selectableItemsList.offsetHeight;
                if (newScrollPos > 0) {
                    this.$refs.selectableItemsList.scrollTop = newScrollPos;
                } else {
                    this.$refs.selectableItemsList.scrollTop = 0;
                }
            }
        },
        getKey(item, i) {
            return `${item.value}:${i}:${this.selectId}`
        },
        selectKeydown(event) {
            if (event.keyCode >= 65 && event.keyCode <= 90) {
                this.selectKeydownValue += event.key;
                selectedItemBestMatch = this.selectItemsFindBestMatch();
                if (selectedItemBestMatch) {
                    if (this.selectOpen) {
                        this.selectableItemActive = selectedItemBestMatch;
                        this.selectScrollToActiveItem();
                    } else {
                        this.selectedItem = this.selectableItemActive = selectedItemBestMatch;
                    }
                }

                if (this.selectKeydownValue != '') {
                    clearTimeout(this.selectKeydownClearTimeout);
                    this.selectKeydownClearTimeout = setTimeout(() => {
                        this.selectKeydownValue = '';
                    }, this.selectKeydownTimeout);
                }
            }
        },
        selectItemsFindBestMatch() {
            let typedValue = this.selectKeydownValue.toLowerCase();
            var bestMatch = null;
            var bestMatchIndex = -1;
            for (var i = 0; i < this.mxField_items.length; i++) {
                var title = this.mxField_items[i].key.toLowerCase();
                var index = title.indexOf(typedValue);
                if (index > -1 && (bestMatchIndex == -1 || index < bestMatchIndex) && !this.mxField_items[i].disabled) {
                    bestMatch = this.mxField_items[i];
                    bestMatchIndex = index;
                }
            }
            return bestMatch;
        },
        selectPositionUpdate() {
            let selectDropdownBottomPos = this.$refs.selectButton.getBoundingClientRect().top + this.$refs.selectButton.offsetHeight + parseInt(window.getComputedStyle(this.$refs.selectableItemsList).maxHeight);
            if (window.innerHeight < selectDropdownBottomPos) {
                this.selectDropdownPosition = 'top';
            } else {
                this.selectDropdownPosition = 'bottom';
            }
        },
        selectItem(item) {
            this.selectedItem = item;
            this.selectOpen = false;
            this.$refs.selectButton.focus();
            this.mxField_value = item.value;
            this._mxField_onChange()
        },
        itemSelected(item) {
            if (!this.selectedItem || !item) return;

            if (this.areItemsObject) {
                return this.selectedItem.value == item.value
            }
            else {
                return this.selectedItem == item;
            }
        },
        render() {
            const html = `
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
                    <span x-text="selectedItem ? selectedItem.key : 'Select Item'" class="truncate">Select Item</span>
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
                    
                    <template x-for="(item, i) in mxField_items" :key="getKey(item, i)">
                        <li 
                            @click="selectItem(item)"
                            :id="item.value + '-' + selectId"
                            :data-disabled="!!item.disabled"
                            :class="{ 'bg-neutral-100 text-gray-900' : selectableItemIsActive(item), '' : !selectableItemIsActive(item) }"
                            @mousemove="selectableItemActive=item"
                            class="text-xl px-4 py-4 relative flex items-center h-full py-2 pl-8 text-gray-700 cursor-default select-none data-[disabled]:opacity-50 data-[disabled]:pointer-events-none">
                            <svg x-show="itemSelected(item)" class="absolute left-0 w-4 h-4 ml-2 stroke-current text-neutral-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"></polyline></svg>
                            <span class="block font-medium truncate" x-text="item.key"></span>
                        </li>
                    </template>

                </ul>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
        },
    }
}