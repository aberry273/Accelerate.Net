
import { mxForm, mxEvents, mxFetch, mxModal, mxResponsive, mxCardPost } from '/src/js/mixins/index.js';
 
const wysiwygUserSearchEvent = 'form:input:user';
const encoder = new TextEncoder();
export default function (data) {
    return {
        ...mxFetch(data),
        ...mxEvents(data),
        ...mxForm(data),
        ...mxModal(data),
        ...mxResponsive(data),
        ...mxCardPost(data),
        // PROPERTIES
        loading: false,
        fields: [],
        taxonomyFields: [],
        settingsFields: [],
        item: null,
        label: 'Submit', 
        tagStr: null,
        userId: null,
        tags: [],
        belowFold: false,
        quoteFieldName: 'QuotedIds',
        statusFieldName: 'Status',
        categoryFieldName: 'Category',
        charLimitFieldName: 'CharLimit',
        tagFieldName: 'Tags',
        imageFieldName: 'Images',
        videoFieldName: 'Videos',
        textFieldName: 'Content',
        validationMessage: '',
        originalYPosition: 0,
        formContainerElement: '#fixedPosition',
  
        formBottomOffset: 0,
        formTopOffset: 'initial',

        showTags: false,
        showText: false,
        showVideo: false,
        showImage: false,
        actionEvent: null,
        showFloatingPanel: false,
        fixed: false,
        fixTop: false,
        boundingRect: null,
        fetchMetadataUrl: null,
        searchUsersUrl: null,
        initYPos: 0,
        charLimit: 256,
        imageModal: 'upload-media-image-modal',
        // INIT
        init() {
            this.tags = [];
            this.label = data.label;
            this.event = data.event;
            this.item = data.item;
            this.userId = data.userId;
            this.postbackType = data.postbackType;
            this.postbackUrl = data.postbackUrl;
            this.fetchMetadataUrl = data.fetchMetadataUrl;
            this.searchUsersUrl = data.searchUsersUrl;
            this.fields = data.fields,
            this.actionEvent = data.actionEvent;
            this.fixTop = data.fixTop || false;

            this.taxonomyFields = this.getTaxonomyFields();
            this.settingsFields = this.getSettingsFields();

            var tagField = this._mxForm_GetField(this.fields, this.tagFieldName);
            this.showTags = tagField = null ? !tagField.hidden : true
            var imageField = this._mxForm_GetField(this.fields, this.imageFieldName);
            this.showImage = imageField != null ? !imageField.hidden : null
            var videoField = this._mxForm_GetField(this.fields, this.videoFieldName);
            this.showVideo = videoField != null ? !videoField.hidden : null

            const contentField = this._mxForm_GetField(this.fields, 'Content');
            this.charLimit = contentField.max || 256;
            // On updates from cards
            // Move this and all content/post based logic to mixin/geneirc logic
            this.$events.on(this.mxCardPost_quoteEvent, async (item) => {
                this.updateQuoteField(item);
                this.scrollToElement(item.threadId);
                this.highlightElement(item.threadId);
                this.showFloatingPanel = false;
                this.fixed = true;
                //if (this.belowFold) this.fixed = true;
            })
            // On updates from cards
            // Move this and all content/post based logic to mixin/generic logic
            this.$events.on(this.mxCardPost_replyEvent, async (item) => {
                this.updateReplyField(item);
                this.scrollToElement(item.threadId);
                this.highlightElement(item.threadId);
                this.showFloatingPanel = false;
                this.fixed = true;
                //if (this.belowFold) this.fixed = true;
                //this.formContainerElement = '#'+item.threadId
            }) 

            this.$events.on(this.mxCardPost_linkEvent, async (url) => {
                this.updateLinkField(url);
            })
            /*
            this.$events.on(this.mxCardPost_mentionsEvent, async (item) => {
                this.updateMentionsField(item);
            }) 
            */

            this.$events.on(this.mxCardPost_formatsEvent, async (item) => { 
                this.updateSettingsField(item);
            }) 
            this.$events.on(wysiwygUserSearchEvent, async (query) => {
                this.searchUsers(query);
            })
            this.setHtml(data);
        },
        // Content editable field
        //https://stackoverflow.com/questions/46000233/how-is-formatting-in-textarea-being-done

        // GETTERS
        get taxonomyFieldNames() {
            return [
                this.tagFieldName,
                this.categoryFieldName,
            ]
        },
        get settingsFieldNames() {
            return [
                this.statusFieldName,
                this.charLimitFieldName,
            ]
        },
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
        get quoteField() {
            return this._mxForm_GetField(this.fields, this.quoteFieldName);
        },
        get typeSelected() {
            return this.showImage || this.showVideo || this.showText
        },
        get inputAmount() {
            return this.textField != null && this.textField.value != null
                ? this.textField.value.length//encoder.encode(this.textField.value).byteLength
                : 0;
        },
        get characterCount() {
            return `${this.inputAmount} / ${this.charLimit}`;
        },
        get underTextLimit() {
            return this.inputAmount > 0 && this.inputAmount < this.charLimit;
        },
        scrollToElement(elementId) {
            const el = document.getElementById(elementId);
            const headerOffset = 75;
            const y = el.getBoundingClientRect().top + window.pageYOffset - headerOffset;
          
            window.scrollTo({
                top: y,
                left: 0,
                behavior: 'smooth'
            });
        },
        highlightElement(elementId) {
            const el = document.getElementById(elementId);
            var existingHighlights = document.getElementsByClassName('highlighted')[0];
            if (existingHighlights != null) {
                existingHighlights.classList.remove('highlighted');
            }

            el.classList.add('highlighted');

        },
        underMediaLimit() {
            const images = this.imageField.value != null ? this.imageField.value.length : 0;
            const videos = this.videoField.value != null ? this.videoField.value.length : 0;
    
            const total = images + videos
            if (total > 4) {
                this.validationMessage = "You can't add more than 4 images and videos";
            }
            else {
                this.validationMessage = ""
            }
            return total <= 4;
        },
        underQuoteLimit() {
            const total = this.quoteField.value != null ? this.quoteField.value.length : 0;
            if (total > 4) { 
                this.validationMessage = "You can't quote more than 4 posts"; 
            }
            else {
                this.validationMessage = ""
            }
            return total <= 4;
        },
        get isValid() {
            return (this.underTextLimit
                || this.underMediaLimit())
                && this.underQuoteLimit()
                && ((this.textField.value != null && this.textField.value.length > 0)
                || (this.videoField.value != null && this.videoField.value.length > 0)
                || (this.imageField.value != null && this.imageField.value.length > 0))
        },
        get tagField() { return this._mxForm_GetField(this.fields, this.tagFieldName) },

        get fixedStyle() {
            let style = 'display:block;margin-top: 55px;'
            if (this.mxResponsive_IsMobile)
                style += "left: 0; width: 100%;";
            style += `bottom: 0; top: initial`;
            return style;
        },
        getTaxonomyFields() {
            const fields = this.fields
                .filter(x => this.taxonomyFieldNames.indexOf(x.name) > -1)
                .map(x => { return { ...x } });

            const updatedFields = fields.map(x => {
                x.hidden = false;
                return x;
            })
            return updatedFields;
        },
        getSettingsFields() {
            const fields = this.fields
                .filter(x => this.settingsFieldNames.indexOf(x.name) > -1)
                .map(x => { return { ...x } });

            const updatedFields = fields.map(x => {
                x.hidden = false;
                return x;
            })
            return updatedFields;
        },
        updateQuoteField(item) {
            const field = this._mxForm_GetField(this.fields, 'QuotedIds');
            if (!field) return; 

            let quoteItems = field.items || []

            const quotedItem = this.createQuoteRequestItem(item);

            const quotedIds = quoteItems.map(x => x.quotedContentPostId);
            const index = quotedIds.indexOf(item.id)
            if (index > -1) return;

            quoteItems.push(quotedItem)

            field.value = quoteItems.map(x => x.quotedContentPostId);;
            field.items = quoteItems;
            this._mxForm_SetField(this.fields, field);
        },
        updateReplyField(item) {
            // Update parentId
            const parentIdField = this._mxForm_GetField(this.fields, 'ParentId');
            if (!parentIdField) return;

            parentIdField.value = item.id;
            this._mxForm_SetField(this.fields, parentIdField);
            // Update parentIds
            const parentIdsField = this._mxForm_GetField(this.fields, 'ParentIds');
            if (!parentIdsField) return;

            parentIdsField.value = `${parentIdsField.value},${item.id}`
            this._mxForm_SetField(this.fields, parentIdsField);
            // update replyto
            const replyToField = this._mxForm_GetField(this.fields, 'ReplyTo');
            if (!replyToField) return;

            replyToField.value = this.createReplyPostSummary(item);

            this._mxForm_SetField(this.fields, replyToField); 
        },
        async updateLinkField(url) {
            const linkField = this._mxForm_GetField(this.fields, 'LinkValue');
            if (!linkField) return;

            if (linkField.value != null) return;

            this.loading = true;

            const item = await this._mxFetch_Post(this.fetchMetadataUrl, { url })
            linkField.value = item;
            linkField.hidden = false;
            this._mxForm_SetField(this.fields, linkField);
            this.loading = false;

            //this.fixed = true;
        },
        async updateSettingsField(items) {
            const settingsField = this._mxForm_GetField(this.fields, 'Settings');
            if (!settingsField) return;

            var settings = settingsField.value || {};

            settings.formats = items;

            settingsField.value = settings;
            this._mxForm_SetField(this.fields, settingsField);
        },
        async searchUsers(queryText) {
            if (!queryText) return;
            this.loading = false;
            var request = {
                text: queryText,
                userId: this.userId,
                page: 0,
                itemPerPage: 8,
                filters: [],
                aggregates: []
            }
            const { users } = await this._mxFetch_Post(this.searchUsersUrl, request)
            const userItems = users.map(x => {
                return {
                    name: x.username,
                    id: x.id
                }
            })
            var resultsEvents = `${wysiwygUserSearchEvent}:results`;
            this.$events.emit(resultsEvents, userItems);
        },
        createReplyPostSummary(item) {
            const content = item.content.length > 64 ? item.content.slice(0, 64) + "..." : item.content;
            return `Reply to @${item.profile.username}: ${content} #${item.shortThreadId}`
        },
        createSingleLineQuotePost(item) {
            return `@${item.profile.username}: ${item.content.slice(0, 64)} #${item.shortThreadId}`
        },
        createQuoteRequestItem(item) {
            const quote = {
                partial: false,
                preview: this.createSingleLineQuotePost(item),
                quotedContentPostId: item.id,
                content: null,
                response: null
            }
            return quote;;
        },
        // 
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
        hideFloatingPanel(val) {
            this.showFloatingPanel = val;
        },
        setFieldVisibility(fieldName, val) {
            this._mxForm_SetFieldVisibility(this.fields, fieldName, val)
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
        // modal
        toggle() {
            this._mxModal_Toggle('formSettings');
        },
        applyAdditionalFieldUpdates() {
            //taxonomy
            for (var i = 0; i < this.fields.length; i++)
            {
                let field = this.taxonomyFields.filter(y => y.name == this.fields[i].name)[0];
                if (field == null) continue;
                this.fields[i].value = field.value;
            }
            //settings
            for (var i = 0; i < this.fields.length; i++) {
                let field = this.settingsFields.filter(y => y.name == this.fields[i].name)[0];
                if (field == null) continue;
                this.fields[i].value = field.value;
            }
            this.toggle();
        },
        setHtml(data) {
            // make ajax request
            //@scroll.window="fixed = isInPosition ? true : false"
            const label = data.label || 'Submit'
            const html = `
            <!--Additional settings-->
            <dialog id="formSettings">
                <article>
                  <header>
                    <button
                      aria-label="Close"
                      rel="prev"
                      data-target="formSettings"
                      @click="toggle"
                    ></button>
                    <h3 x-text="'Settings'"></h3>
                  </header>

                  <h4>Taxonomy</h4>
                  <div x-data="formFields({ fields: taxonomyFields })"></div>

                  <h4>Settings</h4>
                  <div x-data="formFields({ fields: settingsFields })"></div>

                  <footer>
                    <button @click="applyAdditionalFieldUpdates()">Update</button>
                  </footer>
                </article>
            </dialog>

            <span id="fixedPosition" @scroll.window="belowFold = (window.pageYOffset < 50) ? false: true"><span>
          
            <!--Floating button-->
            <button
                x-show="showFloatingPanel"
                @click="hideFloatingPanel(false)"
                class="material-icons-round round xsmall"
                style="y-index:1111; position: fixed; top: 65px; right: calc(var(--pico-spacing)*0.225);">
                chat
            </button>

            <!--Padded element for bottom fixed form-->
            <!--
            <div class="sticky" style="height: 200px; background: transparent;" x-show="!showFloatingPanel && fixed">
            </div>
            -->

            <!-- to update to teleporting between fixed/nonfixed elements-->
    
            <article
                id="form-element"
                class="form-post"
                :class="fixed ? 'floating bottom container dense sticky py-0' : 'dense sticky'"
                
                style="left:0; border: 1px solid #CCC; padding-left: 0; z-index:111; width: 100%;  margin-bottom:0px; padding-right: var(--pico-spacing);"
                :style="fixed ? fixedStyle : ''">
                    <progress x-show="loading"></progress>
                    <!--Quotes-->
                    <fieldset class="padded" x-data="formFields({fields})"></fieldset>

                    <div style="text-align:center" x-show="validationMessage">
                        <em x-text="validationMessage"></em>
                    </div>

                    <fieldset class="padded py-0 flat" role="group">
                        <button x-show="fixed" class="small secondary material-icons-round flat" @click="fixed = false">vertical_align_center</button>
                        <button x-show="!fixed" class="small secondary material-icons-round flat" @click="fixed = true">swap_vert</button>
                        <!--Toggle fields-->

                        <!--Video-->
                        <button class="small secondary material-icons-round flat" x-show="!showVideo" @click="hideVideoField(false)" :disabled="loading">videocam</button>
                        <button class="small secondary material-icons-round flat" x-show="showVideo" @click="hideVideoField(true)" :disabled="loading">cancel</button>

                        <!--Image-->
                        <button class="small secondary material-icons-round flat" x-show="!showImage" @click="hideImageField(false)" :disabled="loading">image</button>
                        <button class="small secondary material-icons-round flat" x-show="showImage" @click="hideImageField(true)" :disabled="loading">cancel</button>

                        <button class="small secondary material-icons-round flat" @click="toggle" :disabled="loading">settings</button>
                      
                        <button class="small flat" disabled><sub x-text="characterCount"></sub></button>
                        <button class="flat small primary" @click="await submit(fields)"  :disabled="loading || !isValid">${label}</button>
                    </fieldset> 
                </article>
               
            </template>
        `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}