
import { mxForm, mxEvents, mxFetch, mxModal, mxResponsive, mxCardPost } from '/src/js/mixins/index.js';
 
const linkEvent = 'form:input:link';
const wysiwygUserSearchEvent = 'form:input:user';
export default function (data) {
    return {
        ...mxFetch(data),
        ...mxFetch(data),
        ...mxEvents(data),
        ...mxForm(data),
        ...mxModal(data),
        ...mxResponsive(data),
        ...mxCardPost(data),
        // PROPERTIES
        loading: false,
        fields: [],
        item: null,
        label: 'Submit', 
        tagStr: null,
        userId: null,
        tags: [],
        tagFieldName: 'Tags',
        imageFieldName: 'Images',
        videoFieldName: 'Videos',
        textFieldName: 'Content',
        originalYPosition: 0,
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
            })
            // On updates from cards
            // Move this and all content/post based logic to mixin/generic logic
            this.$events.on(this.mxCardPost_replyEvent, async (item) => {
                this.updateReplyField(item);
            }) 

            this.$events.on(linkEvent, async (url) => {
                this.updateLinkField(url);
            })

            this.$events.on(this.mxCardPost_mentionsEvent, async (item) => {
                this.updateMentionsField(item);
            }) 

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
        get inputAmount() {
            return this.textField != null && this.textField.value != null
                ? this.textField.value.length
                : 0;
        },
        get characterCount() {
            return `${this.inputAmount} / ${this.charLimit}`;
        },
        get underLimit() {
            return this.inputAmount < this.charLimit;
        },
        get isValid() {
            return this.underLimit
                && ((this.textField.value != null && this.textField.value.length > 0)
                || (this.videoField.value != null && this.videoField.value.length > 0)
                || (this.imageField.value != null && this.imageField.value.length > 0))
        },
        get tagField() { return this._mxForm_GetField(this.fields, this.tagFieldName) },

        get fixedStyle() {
            let style = 'display:block;margin-top: 55px;'
            if (this.mxResponsive_IsMobile)
                style += "left: 0; width: 100%;";
            style += "bottom: 0; top: initial;";
            return style;
        },
        updateQuoteField(item) {
            const field = this._mxForm_GetField(this.fields, 'QuotedItems');
            if (!field) return;

            let threadIds = field.value || []

            const quotedItem = this.createQuoteRequestItem(item);

            const quotedIds = threadIds.map(x => x.quotedContentPostId);
            const index = quotedIds.indexOf(item.id)
            if (index > -1) return;

            threadIds.push(quotedItem)

            field.value = threadIds;
            field.items = threadIds;
            this._mxForm_SetField(this.fields, field);
            this.showFloatingPanel = false;
        },
        updateMentionsField(item) {
            const field = this._mxForm_GetField(this.fields, 'Mentions');
            if (!field) return;

             
            this._mxForm_SetField(this.fields, field);
            this.showFloatingPanel = false;
        },
        updateReplyField(item) {
            const parentIdField = this._mxForm_GetField(this.fields, 'ParentId');
            if (!parentIdField) return;

            parentIdField.value = item.id;
            this._mxForm_SetField(this.fields, parentIdField);

            const replyToField = this._mxForm_GetField(this.fields, 'ReplyTo');
            if (!replyToField) return;

            replyToField.value = this.createReplyPostSummary(item);

            this._mxForm_SetField(this.fields, replyToField);
            this.showFloatingPanel = false;
            // If form is in scrollState, show
            //this.fixed = true;
        },
        async updateLinkField(url) {
            const linkField = this._mxForm_GetField(this.fields, 'Link');
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
                text: `*${queryText}*`,
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
            return `Reply to [${item.shortThreadId}]: ${content}`
        },
        createSingleLineQuotePost(item) {
            return `@${item.shortThreadId}: ${item.content.slice(0, 64)}`
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
            //@scroll.window="fixed = isInPosition ? true : false"
            const label = data.label || 'Submit'
            const html = `
            <span id="fixedPosition"><span>
            <!--Floating--> 
             
            <!--Floating button-->
            <button
                x-show="showFloatingPanel"
                @click="hideFloatingPanel(false)"
                class="material-icons round xsmall"
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
                :class="fixed ? 'floating bottom container dense sticky' : 'dense sticky'"
                
                style="left:0; border: 1px solid #CCC; padding-left: 0; z-index:111; width: 100%;  margin-bottom:0px; padding-right: var(--pico-spacing);"
                :style="fixed ? fixedStyle : ''"
>
                    <progress x-show="loading"></progress>
                    <!--Quotes-->
                    <fieldset class="padded" x-data="formFields({fields})"></fieldset>

                    <fieldset class="padded" role="group">
                        <button x-show="fixed" class="small secondary material-icons flat" @click="fixed = false">vertical_align_center</button>
                        <button x-show="!fixed" class="small secondary material-icons flat" @click="fixed = true">swap_vert</button>
                        <!--
                        <input class="flat" hide disabled type="text" placeholder="" />
                        -->
                        <!--Toggle fields-->
                    
                        <!--
                        <button class="small secondary material-icons flat" x-show="!showText" @click="hideTextField(false)" :disabled="loading">text_format</button>
                        <button class="small secondary material-icons flat" x-show="showText" @click="hideTextField(true)" :disabled="loading">cancel</button>
                        -->
                        <!--Video-->
                        <button class="small secondary material-icons flat" x-show="!showVideo" @click="hideVideoField(false)" :disabled="loading">videocam</button>
                        <button class="small secondary material-icons flat" x-show="showVideo" @click="hideVideoField(true)" :disabled="loading">cancel</button>

                        <!--Image-->
                        <button class="small secondary material-icons flat" x-show="!showImage" @click="hideImageField(false)" :disabled="loading">image</button>
                        <button class="small secondary material-icons flat" x-show="showImage" @click="hideImageField(true)" :disabled="loading">cancel</button>

                        
                        <details class="dropdown flat simple" style="margin-top:0px">
                            <summary role="button" class="material-icons secondary flat small">more_horiz</summary>
                            <ul dir="ltr" style="text-align:left">
                                <li x-show="showTags == true" @click="hideTagField(false)"><a href="#">
                                    <i class="material-icons flat small">sell</i>
                                    Tag
                                </a></li>
                                <li x-show="showTags == false" @click="hideTagField(true)"><a href="#">
                                    <i class="material-icons flat small">cancel</i>
                                    Tag
                                </a></li>
                            </ul>
                        </details>


                        <button class="small flat" disabled><sub x-text="characterCount"></sub></button>
                        <button class="flat primary" @click="await submit(fields)"  :disabled="loading || !isValid">${label}</button>

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