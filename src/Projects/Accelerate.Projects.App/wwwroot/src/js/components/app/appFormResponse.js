
import mxForm from '/src/js/mixins/mxForm.js';
export default function (data) {
    return {
        ...mxForm(data),
        // PROPERTIES
        loading: false,
        fields: [],
        item: null,
        label: 'Submit',
        quoteEvent: 'quote:post',
        tagStr: null,
        tags: [],
        tagFieldName: 'Tags',
        showTags: false,
        showText: false,
        showVideo: false,
        showImage: false,
        actionEvent: null,
        // INIT
        init() {
            this.tags = [];
            this.label = data.label;
            this.event = data.event;
            this.item = data.item;
            this.postbackType = data.postbackType
            this.fields = data.fields,
            this.actionEvent = data.actionEvent;
            this._mxForm_SetFieldVisibility(this.fields, this.tagFieldName, false)

            // On updates from cards
            // Move this and all content/post based logic to page level js instead
            this.$events.on(this.actionEvent, async (request) => {
                if (request.action == 'quote') {
                    // Don't do anything
                    console.log(request)
                    const item = request.item;
                    const field = this._mxForm_GetField(this.fields, 'QuoteIds');
                    if (!field) return;
                    let threadIds = field.value || []

                    const threadKey = item.threadId;
                    const index = threadIds.indexOf(threadKey);
                    if (index > -1) return;
                    if (index == -1) {
                        threadIds.push(threadKey)
                    }
                    field.value = threadIds;
                    field.items = threadIds;
                    this._mxForm_SetField(this.fields, field);
                }
            })

            this.setHtml(data)
        },
        // GETTERS
        get typeSelected() { return this.showImage || this.showVideo || this.showText },
        get tagField() { return this._mxForm_GetField(this.fields, this.tagFieldName) },
        // METHODS
        async submit(fields) {
            this.loading = true;
            const payload = {}
            fields.map(x => {
                payload[x.name] = x.value
                //if array, join values into str delimited list
                if (Array.isArray(x.value) && !x.isArray) {
                    payload[x.name] = x.value.join(',')
                }
                return payload
            })

            let response = await this.$fetch.POST(data.postbackUrl, payload);
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
        cancelTypes() {
            this.showText = false;
            this.showVideo = false;
            this.showImage = false;
        },
        addTag() {
            this.tags.push(this.tagStr);
            this.tagStr = null;
        },
        showTagField() {
            this.showTags = !this.showTags;
            this._mxForm_SetFieldVisibility(this.fields, this.tagFieldName, this.showTags)
        },
        setHtml(data) {
            // make ajax request
            const label = data.label || 'Submit'
            const html = `

        <article class="dense py-0 sticky">
          <progress x-show="loading"></progress>
          <!--Quotes-->
          <fieldset x-data="formFields({fields})"></fieldset>
         
          <fieldset role="group">
            <!--Types-->
            <button class="small secondary material-icons flat" x-show="!typeSelected" @click="showText = !showText" :disabled="loading">text_format</button>
            <button class="small secondary material-icons flat" x-show="!typeSelected" @click="showVideo = !showVideo" :disabled="loading">videocam</button>
            <button class="small secondary material-icons flat" x-show="!typeSelected" @click="showImage = !showImage" :disabled="loading">image</button>
            <!--Cancel-->
            <button class="small secondary material-icons flat" x-show="typeSelected" @click="cancelTypes" :disabled="loading">cancel</button>

            <!--Type formats-->
            <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">format_list_bulleted</button>
            <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">format_list_numbered</button>
            <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">link</button>
            <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">format_quote</button>
            <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">code</button>
            
            <input name="Tag" disabled type="text" placeholder="" />
            
            <button x-show="showTags == true" class="secondary material-icons flat" @click="showTagField" :disabled="loading">sell</button>
            <button x-show="showTags == false" class="secondary material-icons flat" @click="showTagField" :disabled="loading">cancel</button>
            
            <button class="" @click="await submit(fields)"  :disabled="loading">${label}</button>

          </fieldset>
         
        </article>
      `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}