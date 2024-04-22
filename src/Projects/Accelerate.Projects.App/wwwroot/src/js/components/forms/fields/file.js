export default function (data) {
    return `
        <span x-data="{file: null}" class="file-field" :for="field.value ? '' : field.name">
            <template x-if="!field.value">
                <article class="file-picker">
                    <i aria-label="File" class="icon material-icons" >photo_camera</i>
                    <label>Select a file</label>
                </article>
            </template>
            <template x-if="field.value">
                <article class="padless">
                    <img :src="getFilePreview(field.value)" />
                    <label @click="(ev)=>{ ev.preventDefault(); field.value = null }">Cancel</label>
                </article>
            </template>
        </span>
        <input
            class="file-field"
            :id="field.name"
            :type="field.type"
            :name="field.name"
            :disabled="field.disabled == true"
            :aria-label="field.ariaLabel || field.label"
            :read-only="field.readonly"
            :role="field.role"
            :checked="field.checked"
            x-on:change="_mxForm_OnFieldChange(field, $event.target.files[0])"
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