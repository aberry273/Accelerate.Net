
export default function (data) {
    return {
        // PROPERTIES
        loading: false,
        fields: [],
        item: null,
        label: 'Submit',
        tagStr: null,
        tags: [],
        showTags: false,
        // INIT
        init() {
            this.tags = [];
            this.label = data.label;
            this.event = data.event;
            this.item = data.item;
            this.postbackType = data.postbackType
            this.fields = data.fields,
                this.setHtml(data)
        },
        setFields(inputName, inputPlaceholder) {
            return [
                {
                    name: inputName || 'Content',
                    type: 'textarea',
                    placeholder: inputPlaceholder || 'Whats your update?',
                    autocomplete: null,
                    helper: '',
                    clearOnSubmit: true,
                },
                {
                    name: 'parentId',
                    type: 'input',
                    disabled: true,
                    hidden: true,
                    autocomplete: null,
                    helper: '',
                    value: this.item.id,
                },
                {
                    name: 'status',
                    type: 'input',
                    disabled: true,
                    hidden: true,
                    autocomplete: null,
                    helper: '',
                    value: this.item.status,
                },
                {
                    name: 'channelId',
                    type: 'input',
                    disabled: true,
                    hidden: true,
                    autocomplete: null,
                    helper: '',
                    value: this.item.channelId,
                },
                {
                    name: 'threadId',
                    type: 'input',
                    disabled: true,
                    hidden: true,
                    autocomplete: null,
                    helper: '',
                    value: this.item.threadId,
                },
                {
                    name: 'tags',
                    type: 'input',
                    disabled: true,
                    hidden: true,
                    autocomplete: null,
                    helper: '',
                    value: this.item.tags,
                },
                {
                    name: 'category',
                    type: 'input',
                    disabled: true,
                    hidden: true,
                    autocomplete: null,
                    helper: '',
                    value: this.item.category,
                },
                {
                    name: 'userId',
                    type: 'input',
                    disabled: true,
                    hidden: true,
                    autocomplete: null,
                    helper: '',
                    value: this.item.userId,
                },
            ]
        },

        async submit(fields) {
            this.loading = true;
            const payload = {}
            fields.map(x => {
                payload[x.name] = x.value
                return payload
            })
            // Set tags

            payload.tags = this.tags.join(',')
            let response = this.$fetch.POST(data.postbackUrl, payload);
            if (this.event) {
                this.$dispatch(this.event, response)
            }
            this.resetValues(fields);
            this.loading = false;
        },
        resetValues(fields) {
            for (var i = 0; i < fields.length; i++) {
                if (fields[i].clearOnSubmit)
                    fields[i].value = null;
            }
        },
        format(type) {

        },
        addTag() {
            this.tags.push(this.tagStr);
            this.tagStr = null;
        },
        setHtml(data) {
            // make ajax request
            const label = data.label || 'Submit'
            const html = `
        <div>
          <progress x-show="loading"></progress>
          <fieldset x-data="formFields({fields})"></fieldset>
          <footer align="right" style="text-align:right"> 

            <!-- Tags visible -->
            <fieldset role="group" x-show="showTags">
              <input name="Tag" type="text"  x-model="tagStr" placeholder="#tags" />
              <button class="small flat secondary material-icons" @click="addTag" :disabled="tagStr == null">add</button>
              <button class="small secondary outline material-icons" @click="showTags = !showTags"  :disabled="loading">chevron_right</button>
              <button class="small" @click="await submit(fields)" :disabled="loading">${label}</button>
            </fieldset>

            <!-- Tags hidden -->
            <fieldset role="group"  x-show="!showTags">
              <input name="Tag" disabled type="text" placeholder="" />
              
              <button class="small secondary material-icons outline" @click="showTags = !showTags" :disabled="loading">chevron_left</button>
              <button class="small" @click="await submit(fields)"  :disabled="loading">${label}</button>

            </fieldset>

            <!-- Tags -->
            <div class="grid" role="group" x-show="tags.length > 0" x-transition.scale.origin.top>
              <div class="container">
                <template x-for="(tag, i) in tags">
                  <button class="tag flat secondary small" x-text="tag" @click="tags.splice(i, 1)"></button>
                </template>
              </div>
            </div>
          </footer>
        </div>
      `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}