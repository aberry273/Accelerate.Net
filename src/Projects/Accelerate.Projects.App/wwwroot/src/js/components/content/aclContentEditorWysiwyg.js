import { mxForm, mxEvents, mxFetch, mxModal, mxResponsive, mxCardPost } from '/src/js/mixins/index.js';

export default function header(data) {
    return {
        ...mxForm(data),
        ...mxEvents(data),
        ...mxCardPost(data),
        ...mxResponsive(data),
        selectedFormat: 'users',
        elements: [],
        elementsEvent: 'elementsEvent',
        editor: null,
        panel: null,
        showRichText: true,
        input: null,
        selectedItemEvent: 'nullevent',
        searchEvent: null,
        html: '', 
        nodePosition: 0,
        onEmojiEvent: 'insert:wysiwyg:emoji',
        linkEvent: 'form:input:link',
        // for the context menu
        showElementEditor: false,
        selectedformat: '',
        openFormatSelection: false,
        showUrlEditor: false,
        urlText: '',
        queryText: '',
        userInput: null,
        results: [],
        contextNode: null,
        //true if user clicks on element, false if user inputs via keyboard
        insert: false,
        init() {
            this.elementsEvent = data.elementsEvent;
            this.searchEvent = data.searchEvent;
            this.showRichText = data.showRichText;
            this.html = data.value || '';
             
            this.$nextTick(() => {
                this.load(self.data)
                this.editor = document.getElementById('editor')
                this.panel = document.getElementById('panel')
                this.userInput = document.getElementById('userInput')
            })
            this.$events.on('wysiwyg:clear', () => { this.clear(this) })

            var searchResultEvent = this.searchEvent + ":results";
            this.$events.on(searchResultEvent, (results) => {
                this.results = results;
            })

            this.$events.on(this.onEmojiEvent, (emoji) => {
                this.insertHtmlIntoText(emoji.char);
            })
        },
        insertHtmlIntoText(html) {
            this.setEditorCursorPosition((this.nodePosition || 0), this.editor);
      
            document.execCommand("insertHTML", false, html);

            this.convertHtmlToEncodedText();
            //move cursor to end of inserted element
            const pos = this.nodePosition + 1;
            this.setEditorCursorPosition(pos, this.editor);
            this.editor.focus();
        },
        insertIntoText(item) {
            let type = item.type;

            const format = this._mxCardPost_GetFormat(type);
            const element = this._mxCardPost_CreateElement(this.nodePosition, format, item.name);

            this.addElement(element);
            this.editor.focus();
            this.setEditorCursorPosition(this.nodePosition, this.editor);
            this.performCustomAction(element.formatted);

            this.convertHtmlToEncodedText();
            this.showElementEditor = false;
            //move cursor to end of inserted element
            const pos = this.nodePosition + element.value.length;
            this.setEditorCursorPosition(pos, this.editor);

            this.editor.focus();
        },
        clear(self) {
            const editor = document.getElementById('editor')
            editor.innerHTML = ''
        },
        addElement(element) {
            this.elements.push(element);
            this.$events.emit(this.elementsEvent, this.elements);
        },
        getSelectionText() {
            var text = "";
            if (window.getSelection) {
                text = window.getSelection().toString();
            } else if (document.selection && document.selection.type != "Control") {
                text = document.selection.createRange().text;
            }
            return text;
        },
        performCustomAction(html) {
            //to fix duplicate elements added
            //https://stackoverflow.com/questions/37008776/inserting-text-with-execcommand-causes-a-duplication-issue
            if (document.querySelectorAll('span[data-text="true"]').length === 0) {
                document.execCommand("insertHTML", false, html);
                document.execCommand("undo", false);
            } else {
                document.execCommand("insertHTML", false, html);
            }
        },
        performAction(command, val) {
            document.execCommand(command, false, val);
            this.editor.focus();
        },
        setEditorCursorPosition(pos, el) {
            //selecting element then adding characters causes the insertion to be in a random position
            // for contentedit field
            if (el.isContentEditable) {
                var range = document.createRange()
                var sel = window.getSelection()
                if (this.insert) { 
                    const node = el.childNodes[0];
                    range.setStart(node, node.length)
                    document.getSelection().collapse(el, pos)
                }
                else {
                    if (el.childNodes == null || el.childNodes.length == 0) {
                        const textnode = document.createTextNode("");
                        el.appendChild(textnode);
                    }
                    const node = el.childNodes[el.childNodes.length - 1];
                    range.setStart(node, 0)
                }
                range.collapse(true)

                sel.removeAllRanges()
                sel.addRange(range)

                //document.getSelection().collapse(el, pos)
                return
            }
        },
        toggleElementInput(ev) {
            if (this.showElementEditor) {
                this.showElementEditor = false;
            }
            else {
                //remove the @ character if entering the element editor
                this.performAction('delete');
                this.nodePosition = this.getCaretPosition(ev.target)

                this.showElementEditor = true;

                this.$nextTick(() => {
                    this.userInput.select();
                })
            }
        },
        //If the user presses @ on the search field, close and insert into the normal text
        returnToElementInput(ev) {
            if (ev.key != '@' && ev.key != 'Shift') {
                return;
            }
            this.showElementEditor = false;
            this.setEditorCursorPosition(this.nodePosition, this.editor);
            this.editor.focus();
            this.performAction('insertText', '@')
        },
        onClick(ev) {
            this.nodePosition = this.getCaretPosition(ev.target)
            //this.contextNode = ev.target;
            
            //Set insert to true if the cursor did not select the last position
            this.insert = this.nodePosition < this.editor.innerText.length;
            this.showElementEditor = false;
        }, 
        convertHtmlToEncodedText() {
            let encodedText = this.editor.innerHTML;
            for (var i = 0; i < this.elements.length; i++) {
                encodedText = encodedText.replace(this.elements[i].formatted, this.elements[i].encoded);
            }
            this.$events.emit('editor-wisyiwyg-plaintext', encodedText);
        },
        onPaste(e) {
            e.preventDefault();
            var contentOnBlur = (e.originalEvent || e).clipboardData.getData('text/plain') || prompt('Paste text');
            // replace/strip HTML
            contentOnBlur = contentOnBlur.replace(/(<([^>]+)>)/ig, '');
            document.execCommand('insertText', false, contentOnBlur);
        },
        onKeyup(ev) {
            this.nodePosition = this.getCaretPosition(ev.target)
            this.contextNode = ev.target;
            // If Block Quote or Code
            
            if (ev.key != '@' && ev.key != 'Shift') {
                this.insert = false;
            }

            this.convertHtmlToEncodedText();
        },
        addLinkCard(text) {
            if (!text) return;
            const hasUrl = this._mxForm_ValueHasUrl(text)
            if (hasUrl) {
                const value = this._mxForm_ValueGetUrl(text);
                this._mxEvents_Emit(this.mxCardPost_linkEvent, value)
            }
        },
        onKeyupDebounce(ev) {
            this.addLinkCard(ev.target.innerText)
        },
        getCaretPosition(target) {
            if (target.isContentEditable || document.designMode === 'on') {
                target.focus();
                const _range = document.getSelection().getRangeAt(0);
                const range = _range.cloneRange();
                const temp = document.createTextNode('\0');
                range.insertNode(temp);
                const caretposition = target.innerText.indexOf('\0');
                temp.parentNode.removeChild(temp);
                return caretposition;
            }
        },
        onShiftEnter() {
            
        },
        addLink() {
            this.editor.focus();
            this.setEditorCursorPosition(this.nodePosition, this.editor);
            this.insertEncodedText('link', this.urlText)

            this.urlText = null;
            this.showUrlEditor = false;
        },
        insertCodeIntoText(formatType) {
            var selectedText = this.getSelectionText();
            this.insertEncodedText(formatType, selectedText);
        },

        insertEncodedText(formatType, encodedText) {
            const format = this._mxCardPost_GetFormat(formatType);
            const element = this._mxCardPost_CreateElement(this.nodePosition, format, encodedText);

            this.addElement(element)
            this.performAction('insertHTML', element.formatted)
            this.convertHtmlToEncodedText();

            //move cursor to end of inserted element
            const pos = this.nodePosition + element.value.length;
            this.setEditorCursorPosition(pos, this.editor);

            this.editor.focus();
        },
        // element editor  
        validateUrl() {

        },
        search() {
            this.$events.emit(this.searchEvent, this.queryText)
        },
        select(item, type) {
            item.type = type;
            this.open = false;
            this.selected = item;
            this.queryText = item.name;

            this.showElementEditor = false;
            this.editor.focus();
            this.insertIntoText(item);
        }, 
        closeElementEditor() {
            this.showElementEditor = false;
        },
        selectFormat(type) {
            this.selectedFormat = type;
            this.openFormatSelection = false;
        },
        cancelLink() {
            this.urlText = null;
            this.showUrlEditor = false;
        },
        toggleUrlEditor() {
            this.showElementEditor = false;
            this.showUrlEditor = !this.showUrlEditor;
        },
        toggleUserEditor() {
            this.showUrlEditor = false;
            this.showElementEditor = !this.showElementEditor;
        },
        get showInputEditors() {
            return this.showUrlEditor || this.showElementEditor
        },
        load(data) {
            this.$root.innerHTML = `
            <div> 
                <!--Panel-->
                <nav>
                    <ul>
                    <!--
                        <span x-show="mxResponsive_IsMobile && showRichText">
                            <details class="dropdown flat simple" style="margin-top:0px">
                                <summary class="material-icons flat small">more_horiz</summary>
                              <ul dir="ltr" style="text-align:left">
                                <li @click="insertCodeIntoText('code')"><a href="javascript:;">
                                    <i class="material-icons flat small">code</i>
                                    Code
                                </a></li>
                                <li @click="insertCodeIntoText('quote')"><a href="javascript:;">
                                    <i class="material-icons flat small">format_quote</i>
                                    Quote
                                </a></li>
                                <li @click="insertCodeIntoText('bold')"><a href="javascript:;">
                                    <i class="material-icons flat small">format_bold</i>Bold
                                </a></li>
                                <li @click="insertCodeIntoText('italic')"><a href="javascript:;">
                                    <i class="material-icons flat small">format_italic</i>Bold
                                    Italics
                                </a></li>
                              </ul>
                            </details>
                        </span>
                        -->
                        <span x-show="showRichText"> 
                            <button class="material-icons flat small" @click="insertCodeIntoText('code')">code</button>
                            <button class="material-icons flat small" @click="insertCodeIntoText('quote')">format_quote</button>
                            <button class="material-icons flat small" @click="insertCodeIntoText('bold')">format_bold</button>
                            <button class="material-icons flat small" @click="insertCodeIntoText('italics')">format_italic</button>
                        </span>
                        <!--Url input-->
                        <button class="small material-icons flat small" :class="showUrlEditor ? 'secondary' : ''" @click="toggleUrlEditor()">link</button>

                        <!--User selector-->
                        <div x-show="showUrlEditor">
                            <input
                                id="urlInput"
                                style="margin-bottom: 0px"
                                :change="validateUrl"
                                x-model="urlText"
                                :value="urlText"
                                placeholder="url"
                            />
                        </div>
                        <button x-show="showUrlEditor"  class="material-icons flat small" @click="addLink">add</button>

                        <!--User selector-->
                        <button :class="showElementEditor ? 'secondary' : ''" class="material-icons flat small" @click="toggleUserEditor()">person_search</button>
                        <div x-show="showElementEditor">
                            <input
                                id="userInput"
                                style="margin-bottom: 0px"
                                :change="search"
                                x-model="queryText"
                                :value="queryText"
                                placeholder="username"
                                @keyup.@="($event) => returnToElementInput($event)"
                            />
                            <article class="dropdownMenu">
                                <!--Users Format-->
                                <template x-if="selectedFormat == 'users'">
                                    <div>
                                        <!--User Search-->
                                        <ul  style="display: grid;list-style:none; text-align:left; " >
                                            <li>Results</li>
                                            <template x-for="(item) in results">
                                                <li>
                                                    <a href="#" @click="select(item, 'user')" x-text="item.name"></a>
                                                </li>
                                            </template>
                                            <li x-show="results.length == 0">
                                                No results found
                                            </li>
                                        </ul>
                                    </div>
                                </template>
                                <!--  Link Formats-->
                            </article>
                        </div>

                        <!--Emoji picker-->
                        <div x-data="aclContentEmoji({ event: onEmojiEvent })"></div>
                    </ul> 
                    <ul>
                        <input style="width: 5px" class="flat"  disabled />
                    </ul>
                </nav>
                <!--input-->
                <div
                    id="editor"
                    contenteditable="true"
                    role="textbox"
                    x-html="html"
                    class="wysiwyg"
                    @paste="($event) => onPaste($event)"
                    @click="($event) => onClick($event)"
                    @keyup="($event) => onKeyup($event)"
                    @keyup.shift.enter="($event) => onShiftEnter($event)"
                    @keyup.debounce="($event) => onKeyupDebounce($event)"
                    @keyup.@="($event) => toggleElementInput($event)">
                </div> 
            </div>`
        },
    };
}