
import mxForm from '/src/js/mixins/mxForm.js';
import mxModal from '/src/js/mixins/mxModal.js';
import mxResponsive from '/src/js/mixins/mxResponsive.js';
export default function (data) {
    return {
        ...mxForm(data),
        ...mxModal(data),
        ...mxResponsive(data),
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
        textFieldName: 'Content',
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
            this.showTags = tagField = null ? !tagField.hidden : true
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
        get tagField() {
            return this._mxForm_GetField(this.fields, this.tagFieldName);
        },
        get imageField() {
            return this._mxForm_GetField(this.fields, this.imageFieldName);
        },
        get videoField() {
            return this._mxForm_GetField(this.fields, this.videoFieldName);
        },
        get textField() {
            return this._mxForm_GetField(this.fields, this.textFieldName);
        },
        get typeSelected() {
            return this.showImage || this.showVideo || this.showText
        },
        get isValid() {
            return (this.textField.value != null && this.textField.value.length > 0)
                || (this.videoField.value != null && this.videoField.value.length > 0)
                || (this.imageField.value != null && this.imageField.value.length > 0)
        },
        get tagField() { return this._mxForm_GetField(this.fields, this.tagFieldName) },
        // METHODS
        async submit(fields) {
            try {
                this.loading = true;

                const payload = this._mxForm_GetFileFormData({ fields: fields })

                const config = this.mxForm_HeadersMultiPart;
                const isJson = false
                let response = await this._mxForm_SubmitAjaxRequest(data.postbackUrl, payload, config, isJson);

                if (this.event) {
                    this.$dispatch(this.event, response)
                }
                this.$dispatch(this.localEvent, response)

                this.resetValues(fields);
            }
            catch (e) {

            }
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
            this.hideTagField(true);
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

            <article class="dense sticky">
                <progress x-show="loading"></progress>
                <!--Quotes-->
                <fieldset class="pa-2" x-data="formFields({fields})"></fieldset>
                
                <fieldset role="group">
                    <!--Toggle fields-->
                    <!--
                    <button class="small secondary material-icons flat" x-show="!typeSelected" @click="hideTextField(false)" :disabled="loading">text_format</button>
                    -->
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
                    
                    <button x-show="showTags == true" class="secondary material-icons flat" @click="hideTagField(false)" :disabled="loading">sell</button>
                    <button x-show="showTags == false" class="secondary material-icons flat" @click="hideTagField(true)" :disabled="loading">cancel</button>
                    
                    <button class="" @click="await submit(fields)"  :disabled="loading || !isValid">${label}</button>

                </fieldset> 
            </article>
        `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}