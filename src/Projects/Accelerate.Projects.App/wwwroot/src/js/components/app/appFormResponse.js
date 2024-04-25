
import mxForm from '/src/js/mixins/mxForm.js';
import mxModal from '/src/js/mixins/mxModal.js';
export default function (data) {
    return {
        ...mxForm(data),
        ...mxModal(data),
        // PROPERTIES
        loading: false,
        fields: [],
        item: null,
        label: 'Submit',
        quoteEvent: 'quote:post',
        tagStr: null,
        tags: [],
        tagFieldName: 'Tags',
        imageFieldName: 'Images',
        videoFieldName: 'Videos',
        showTags: false,
        showText: false,
        showVideo: false,
        showImage: false,
        actionEvent: null,
        imageModal: 'upload-media-image-modal',
        // INIT
        init() {
            this.tags = [];
            this.label = data.label;
            this.event = data.event;
            this.item = data.item;
            this.postbackType = data.postbackType
            this.fields = data.fields,
                this.actionEvent = data.actionEvent;

            var tagField = this._mxForm_GetField(this.fields, this.tagFieldName);
            this.showTags = tagField = null ? !tagField.hidden : null
            var imageField = this._mxForm_GetField(this.fields, this.imageFieldName);
            this.showImage = imageField != null ? !imageField.hidden : null
            var videoField = this._mxForm_GetField(this.fields, this.videoFieldName);
            this.showVideo = videoField != null ? !videoField.hidden : null

            // On updates from cards
            // Move this and all content/post based logic to page level js instead
            this.$events.on(this.actionEvent, async (request) => {
                if (request.action == 'quote') {
                    // Don't do anything
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
        get typeSelected() {
            return this.showImage || this.showVideo || this.showText
        },
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
                if (fields[i].clearOnSubmit === true) {
                    fields[i].value = null;
                    fields[i].values = null;
                    fields[i].items = null;
                }
            }
        },
        format(type) {
        },
        cancelTypes() {
            this.hideTextField(true);
            this.hideImageField(true);
            this.hideVideoField(true);
        },
        addTag() {
            this.tags.push(this.tagStr);
            this.tagStr = null;
        },
        hideTagField(val) {
            this.showTags = val;
            this._mxForm_SetFieldVisibility(this.fields, this.tagFieldName, val)
        },
        hideTextField(val) {
            this.showText = !val;
            this._mxForm_SetFieldVisibility(this.fields, this.textFieldName, val)
        },
        hideImageField(val) {
            this.showImage = !val;
            this._mxForm_SetFieldVisibility(this.fields, this.imageFieldName, val)
        },
        hideVideoField(val) {
            this.showVideo = !val;
            this._mxForm_SetFieldVisibility(this.fields, this.videoFieldName, val)
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
                    <!--Toggle fields-->
                    <button class="small secondary material-icons flat" x-show="!typeSelected" @click="hideTextField(false)" :disabled="loading">text_format</button>
                    <button class="small secondary material-icons flat" x-show="!typeSelected" @click="hideVideoField(false)" :disabled="loading">videocam</button>
                    <button class="small secondary material-icons flat" x-show="!typeSelected" @click="hideImageField(false)" :disabled="loading">image</button>
                    <!--Cancel-->
                    <button class="small secondary material-icons flat" x-show="typeSelected" @click="cancelTypes" :disabled="loading">cancel</button>
                    
                    <!--Type formats-->
                    <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">format_list_bulleted</button>
                    <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">format_list_numbered</button>
                    <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">link</button>
                    <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">format_quote</button>
                    <button class="small secondary material-icons flat small" x-show="showText" @click="showText = !showText" :disabled="loading">code</button>
                    
                    <input name="Tag" disabled type="text" placeholder="" />
                    
                    <button x-show="showTags" class="secondary material-icons flat" @click="showTagField(!showTags)" :disabled="loading">sell</button>
                    <button x-show="!showTags" class="secondary material-icons flat" @click="showTagField(!showTags)" :disabled="loading">cancel</button>
                    
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