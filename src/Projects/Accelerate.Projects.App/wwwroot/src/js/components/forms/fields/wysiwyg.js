
 
export default function (data) {
    return ` 
        <div display="position:relative;" x-data="{
            val: null,
            init() {
                console.log('init wyswiyg');
                val = '${data.value}';
                
                console.log(field.value)
                this.$watch('val', (newVal) {
                    console.log(newVal)
                })
                //On aclDropDown user selecting an option
                /*
                this.$events.on('editor-wisyiwyg-plaintext', (val) => {
                    field.value = val;
                })
                this.$watch('field.value', (newVal, oldVal) => {
                    if(!newVal) {
                        //this.$events.emit('wysiwyg:clear')
                    }
                })
                */
                this.$events.on(mxCardPost.formatsEvent, (val) => {
                    field.items = val;
                })
            },
            onChange(ev) {
                console.log('onChange')
                field.value = ev.detail;
            },
        }">
            <span x-text="field.label"></span>
            <input
                :type="field.type"
                :name="field.name"
                :disabled="true"
                :hidden="false"
                :aria-label="field.ariaLabel || field.label"
                :value="field.value"
                x-model="field.value"
                :read-only="field.readonly"
                :role="field.role"
                :checked="field.checked"
                :placeholder="field.placeholder"
                :autocomplete="field.autocomplete"
                :aria-invalid="field.ariaInvalid == true"
                :aria-describedby="field.id || field.name+i"
                ></input>

            <div
                x-data="aclContentEditorWysiwyg({
                    placeholder: field.placeholder,
                    searchEvent: field.event,
                    showRichText: true,
                    value: '',
                    elementsEvent: mxCardPost_formatsEvent
                })"
                @change="(ev) => { field.value = ev.detail }"
                @itemsChange="(ev) => { field.items = ev.detail }">
            </div>
        </div>
    `
}