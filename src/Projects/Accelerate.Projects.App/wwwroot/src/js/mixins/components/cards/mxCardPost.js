const defaults = {}
export default function (data) {
    return {
        currentPage: 0,
        mxCardPost_thread: [],
        item: null,
        userId: null,
        updateEvent: '',
        mxCardPost_mentionEvent: 'action:post:mention',
        mxCardPost_quoteEvent: 'action:post:quote',
        mxCardPost_replyEvent: 'action:post:reply',
        mxCardPost_formatsEvent: 'action:post:formats',
        actionEvent: 'action:post', 
        modalEvent: 'action:post',
        redirectEvent: 'action:post',
        filterEvent: 'on:filter:posts',
        showMetadata: false,
        showReplies: false,
        articleClass: '',
        quotedPost: null,
        mxCardPost_TextFormats: [
            //everything until space
            {
                name: 'newline',
                key: '\\n',
                regex: new RegExp('\\n', 'gi'),
            },
            /*
            {
                name: 'link',
                regex: new RegExp('@l.(https?:\/\/[^\\s]+)', 'gi'),
            },
            */
            {
                name: 'post',
                key: '#p',
                regex: new RegExp('@[a-zA-Z0-9.+/]+#p', 'gim'),
                //regex: new RegExp('@p.[^\\s]+', 'gi'),
            },
            {
                name: 'user',
                key: '#u',
                regex: new RegExp('@[a-zA-Z0-9.-_=]+#u', 'gim'),
            },
            //everything until double space
            {
                name: 'quote',
                key: '#q',
                regex: new RegExp('@q.((?!\\s{2}).)*', 'gi'),
            },
            {
                name: 'italics',
                key: '#i',
                regex: new RegExp('@i.((?!\\s{2}).)*', 'gi'),
            },
            {
                name: 'bold',
                key: '#b',
                regex: new RegExp('@b.((?!\\s{2}).)*', 'gi'),
            },
            {
                name: 'code',
                key: '#c',
                regex: new RegExp('@c.((?!\\s{2}).)*', 'gi'),
            },
        ],
        _mxCardPost_init() {
            this.articleClass = data.class;
            this.item = data.item;
            this.userId = data.userId;
            this.updateEvent = data.updateEvent;
            this.mxCardPost_thread = this._mxCardPost_getThreadItems(data.item);
            this.currentPage = 0;

            this.$events.on(this.updateEvent, (item) => {
                if (this.item.id != item.id) return;
                this.item = item;
                this.mxCardPost_thread = this._mxCardPost_getThreadItems(item);
            });

        },
        _mxCardPost_getThreadItems(op) {
            // flatten parent and child hierarchy into single array
            if (op == null) return [];
            let postThreads = (op.threads == null) ? [] : op.threads;
            let thread = [op].concat(postThreads)
            return thread;
        },
        _mxCardPost_CreatePostActionPayload(action, item) {
            const payload = {
                userId: this.userId,
                contentPostId: item.id,
            }
            payload[action] = this._mxCardPost_userSelectedAction(action, item) ? false : true;
            return payload;
        },
        async _mxCardPost_reply(item) {
            this.$events.emit(this.mxCardPost_replyEvent, item)
        },
        async _mxCardPost_mention(item) {
            this.$events.emit(this.mxCardPost_mentionEvent, item)
        },
        async _mxCardPost_quote(item) {
            this.$events.emit(this.mxCardPost_quoteEvent, item)
        },
        async _mxCardPost_richtext(item) {
            this.$events.emit(this.mxCardPost_formatsEvent, item)
        },
        async _mxCardPost_action(action, item) {
            const payload = this._mxCardPost_CreatePostActionPayload(action, item);

            const result = await this.$store.wssContentPostActions._wssContentActions_HandlePost(payload);
        },
        _mxCardPost_GetFormat(name) {
            return this.mxCardPost_TextFormats.filter(x => x.name == name)[0]
        },
        _mxCardPost_NewLine(input) {
            return `<br/>`;
        },
        _mxCardPost_FormatUser(input) {
            return `<span contenteditable=\"false\" class=\"item u\">${ input }</span>`;
        },
        _mxCardPost_FormatPost(input) {
            return `<span contenteditable=\"false\" class=\"item p\">${ input }</span>`;
        },
        _mxCardPost_FormatHref(input) {
            return `<a class='item'>${input}</a>&nbsp;`;
        },
        _mxCardPost_FormatBold(input) {
            return `<strong>${input}</strong>&nbsp;`;
        },
        _mxCardPost_FormatItalics(input) {
            return `<em>${input}</em>&nbsp;`;
        },
        _mxCardPost_FormatCode(input) {
            let str = input.substring(3);
            return `<code>${input}</code>&nbsp;`;
        },
        _mxCardPost_FormatQuote(input) {
            return `<blockquote>${input}</blockquote>&nbsp;`;
        },
        _mxCardPost_FormatText(format, text) {
            switch (format.name) {
                case 'newline':
                    return this._mxCardPost_NewLine(text);
                case 'user':
                    return this._mxCardPost_FormatUser(text);
                case 'post':
                    return this._mxCardPost_FormatPost(text);
                //case 'link':
                    //return this._mxCardPost_FormatHref(text);
                case 'bold':
                    return this._mxCardPost_FormatBold(text);
                case 'italics':
                    return this._mxCardPost_FormatItalics(text);
                case 'quote':
                    return this._mxCardPost_FormatQuote(text);
                case 'code':
                    return this._mxCardPost_FormatCode(text);
                default:
                    return text
            }
        },
        _mxCardPost_GetTextElementsFromEncodedText(text) {
            let elements = [];
            if (!text) return;
            const self = this;
            // loop through each format type and fetch all instances of it based on the format regex
            for (var i = 0; i < this.mxCardPost_TextFormats.length; i++) {
                const format = this.mxCardPost_TextFormats[i];
                // return array of all instances of the regex pattern
                var links = text.match(format.regex)
                // if empty, skip
                if (links == null || links.length == 0) continue;
                // get the formatValues
                const formats = links.map(x => {
                    const index = text.indexOf(x);
                    const cleanedStr = x.slice(1, x.length - 2)
                    return self._mxCardPost_CreateElement(index, format, cleanedStr)
                })
                elements = elements.concat(formats)
            }
            return elements;
        },
        _mxCardPost_CreateElement(index, format, value) {
            return {
                start: index,
                end: index + value.length,
                value: value,
                format: format.name,
                formatted: this._mxCardPost_FormatText(format, value),
                encoded: this._mxCardPost_EncodeValue(format, value)
            }
        },
        _mxCardPost_EncodeValue(format, value) {
            return `@${value}${format.key}`;
            return format.key + value;
        },
        //Used by contentPost to render the encoded text into decoded HTML
        _mxCardPost_ParseEncodedTextElements(text) { 
            let elements = this._mxCardPost_GetTextElementsFromEncodedText(text);
            //relying on encoded text
            if (elements == null || elements.length == 0) return text;
            return this._mxCardPost_ProcessEncodedElements(text, elements);
        },
        //Used by contentPost to render the encoded text into decoded HTML
        _mxCardPost_ParseTextElements(item) {
            if (item.settings == null || item.settings.formats == null || item.settings.formats.length == 0) return item.content;
            //TODO: pick one or other method below
            //Not relying on encoded text
            let elements = item.settings.formats;
            return this._mxCardPost_ProcessSubstituteElements(item.content, elements);
        },

        _mxCardPost_InsertStringIntoString(string, substring, pos) {
            return string.slice(0, pos) + substring + string.slice(pos);
        },

        _mxCardPost_InsertStringIntoString(string, substring, start, end) {
            return string.slice(0, start) + substring + string.slice(end);
        },
        _mxCardPost_ProcessSubstituteElements(text, formattedElements) {
            for (var i = 0; i < formattedElements.length; i++) {
                const format = this._mxCardPost_GetFormat(formattedElements[i].format);
                const formatted = this._mxCardPost_FormatText(format, formattedElements[i].value);
                text = this._mxCardPost_InsertStringIntoString(text, formatted, formattedElements[i].start)
            }
            return text;
        },
        _mxCardPost_ProcessElementsWithEncodedValues(text, elements) {
            for (var i = 0; i < elements.length; i++) {
                text = this._mxCardPost_InsertStringIntoString(text, elements[i].encoded, elements[i].start, elements[i].end);
            }
            return text;
        },

        _mxCardPost_ProcessEncodedElements(text, elements) {
            for (var i = 0; i < elements.length; i++) {
                text = text.replace(elements[i].encoded, elements[i].formatted)
            }
            return text;
        },

        _mxCardPost_getQuotePost(id) {
            return this.$store.wssContentPosts.GetQuotePost(id);
        },
        _mxCardPost_redirectAction(action) {
            const ev = `redirect-${action}`;
            this.$events.emit(ev, this.mxCardPost_selectedPost)
        },
        _mxCardPost_modalAction(action, item) {
            const ev = `modal-${action}-post`;
            const requestType = action == 'delete' ? 'DELETE' : 'PUT';
            const payload = {
                // route to append to postbackUrl 
                postbackUrlRoute: item.id,
                // postback type
                postbackType: requestType,
                // content post item
                item: item,
            }
            this.$events.emit(ev, payload)
        },
        _mxCardPost_scrollTo(id) {
            const el = document.getElementById(id);
            el.scrollIntoView()
        },
        _mxCardPost_renderPost(post) {
            if (!post.content) return null;
            return cardRenderingText(post)
        },
        _mxCardPost_userSelectedAction(action, item) {
            // Retrieves the action based on the post actions
            //return this._mxContentActions_GetAction(item, action);
            return this.$store.wssContentPosts.CheckUserPostAction(item.id, this.userId, action);
        },
        _mxCardPost_cleanthreadId(post) {
            if (!post.threadId) return post.shortThreadId
            var ids = post.threadId.split('|')
            if (ids.length == 1) return post.shortThreadId
            // get the actual post id
            var last = ids.shift();
            // remove original post item
            //ids.shift();
            var shortenedIds = ids.map(x => this.getTinyThreadId(x));
            return shortenedIds.join('/') + "/" + post.shortThreadId;
        },
        _mxCardPost_getTinyThreadId(threadId) {
            return threadId.slice(0, 8).trim();
        },
        _mxCardPost_getImage(filePath) {
            if (this.imageWidth) {
                return filePath + '?w=' + this.imageWidth;
            }
            return filePath;
        },
        /*
        modalAction(action, data) {
            this.$events.emit(this.modalEvent, data)
        },
        */
        _mxCardPost_filterByThreadId(threadId) {
            const filters =
                [
                    {
                        name: 'Quotes',
                        values: [threadId]
                    }
                ]
            this.$events.emit(this.filterEvent, filters)
        },
        _mxCardPost_filterByTag(tag) {
            const filters =
                [
                    {
                        name: 'Tags',
                        values: [tag]
                    }
                ]
            this.$events.emit(this.filterEvent, filters)
        },
        _mxCardPost_load(html) {
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}