

export default function (data) {
    return ` 
        <div display="position:relative;" x-data="{
            linkEvent: 'form:input:link',
            showRender: false,
            value: '',
            wysiwyg: null,
            init() {
                //On aclDropDown user selecting an option
                this.$events.on('editor-wisyiwyg-plaintext', (val) => {
                    field.value = val;
                })
                this.$events.on(mxCardPost.formatsEvent, (val) => {
                    field.items = val;
                })
                this.$watch('field.value', (newVal) => {
                    if(!newVal) {
                        this.$events.emit('wysiwyg:clear')
                    }
                })
            },  
        }">
            <span x-text="field.label"></span>
            <input
                :type="field.type"
                :name="field.name"
                :disabled="true"
                :hidden="true"
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
                ></textarea>
          
            <div x-data="aclContentEditorWysiwyg({
                searchEvent: field.event,
                showRichText: false,
                value: value,
                elementsEvent: mxCardPost_formatsEvent
            })"></div>
        </div>
    `
}