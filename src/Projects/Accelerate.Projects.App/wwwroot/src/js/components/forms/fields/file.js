export default function (data) {
    return ` 
        <span x-data="{file: null}" class="file-field" :for="field.id || field.name">
            <template x-if="!field.value || field.value.length == 0">
                <article class="file-picker">
                    <i aria-label="File" class="icon material-icons" x-text="field.icon || 'attach_file'"></i>
                    <label x-text="field.label || 'Upload file'"></label>
                </article>
            </template>
            <!--Single-->
            <template x-if="!field.multiple && field.value">
                <article class="padless">
                    <img :src="_mxForm_GetFilePreview(field.value)" />
                    <label @click="(ev) => { ev.preventDefault(); field.value = null }">Cancel</label>
                </article>
            </template>
            <!--multiple-->
            <template x-if="field.multiple && field.value">
                <div class="grid">
                    <div class="file-pickers">
                        <i aria-label="File" class="icon material-icons" x-text="field.icon || 'attach_file'"></i>
                        <label x-text="field.label || 'Upload file'"></label>
                    </div>
                    <template x-for="(file, i) in field.value">
                        <div class="padless">
                            <img :src="_mxForm_GetFilePreview(file)" />
                            <label @click="(ev)=>{ ev.preventDefault(); field.value.splice(i, 1); }">Remove</label>
                        </div>
                    </template>
                </div>
            </template>
        </span>
        <input
            class="file-field"
            :id="field.id || field.name"
            :type="field.type"
            :name="field.name"
            :disabled="field.disabled == true"
            :aria-label="field.ariaLabel || field.label"
            :read-only="field.readonly"
            :multiple="field.multiple"
            :role="field.role"
            hidden="true"
            :checked="field.checked"
            x-on:change="($event) => {
                if(!field.multiple) {
                    _mxForm_OnFieldChange(field, $event.target.files[0])
                }
                else {
                    if (field.value == null) field.value = [];
                    const updatedFiles = field.value.concat(Array.from($event.target.files))
                    _mxForm_OnFieldChange(field, updatedFiles)
                }
            }"
            :placeholder="field.placeholder"
            :autocomplete="field.autocomplete"
            :aria-invalid="field.ariaInvalid == true"
            :aria-describedby="field.id || field.name+i"
            :accept="field.accept || '.png'"
            ></input>
        <small 
            x-show="field.helper != null && field.helper.length > 0"
            :id="field.id || field.name+i" x-text="field.helper"></small>
        `
}