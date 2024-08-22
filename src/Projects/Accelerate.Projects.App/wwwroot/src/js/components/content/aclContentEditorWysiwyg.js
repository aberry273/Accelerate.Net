import { mxForm, mxEvents, mxFetch, mxModal, mxResponsive, mxCardPost } from '/src/js/mixins/index.js';

export default function aclContentEditorWysiwyg(data) {
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
        value: '',
        prefill: null,
        nodePosition: 0,
        onEmojiEvent: 'insert:wysiwyg:emoji',
        linkEvent: 'form:input:link',
        // for the context menu
        hasSelectedText: false,
        showPanel: true,
        showElementEditor: false,
        selectedformat: '',
        openFormatSelection: false,
        showUrlEditor: false,
        showRichTextMenu: false,
        urlText: '',
        queryText: '',
        userInput: null,
        results: [],
        contextNode: null,
        placeholder: null,
        //true if user clicks on element, false if user inputs via keyboard
        insert: false,
        init() {
            this.placeholder = data.placeholder;
            this.elementsEvent = data.elementsEvent;
            this.searchEvent = data.searchEvent;
            this.showRichText = data.showRichText;

           
            this.$watch('value', (newVal) => {
                this.$dispatch('change', newVal); 
            })

            this.$events.on('wysiwyg:clear', () => { this.clear(this) })

            var searchResultEvent = this.searchEvent + ":results";
            this.$events.on(searchResultEvent, (results) => {
                this.results = results;
            })

            this.$events.on(this.onEmojiEvent, (emoji) => {
                this.insertHtmlIntoText(emoji.char);
            })

            this.preventKeyboardShortcuts();

            console.log('pre-fill');
            console.log(this.prefill)

            this.$nextTick(() => {
                console.log(this.value)
                //if (this.value)
                {
                    console.log(this.value)
                    console.log('on: init:wysiwyg:value')
                    this.elements = this._mxCardPost_GetTextElementsFromEncodedText(this.value);
                    this.html = this._mxCardPost_ParseEncodedTextElements(this.value) || '';
                }

                this.load(self.data)
                this.editor = document.getElementById('editor')
                this.panel = document.getElementById('panel')
                this.userInput = document.getElementById('userInput')
            })
        },
        preventKeyboardShortcuts() {
            //Prevent bold, italics, underline shortcuts
            const self = this;
            document.onkeydown = function (e) {
                e = e || window.event;//Get event

                if (!e.ctrlKey) return;

                var code = e.which || e.keyCode;//Get key code

                switch (code) {
                    case 86: //ctrl+V or ctrl+v
                       e.preventDefault();
                       e.stopPropagation();
                       self.onPaste(e);
                       return;
                    case 66: //ctrl+B or ctrl+b
                    case 98:
                        e.preventDefault();
                        e.stopPropagation();
                        return;
                    case 73: //ctrl+I or ctrl+i
                    case 105:
                        e.preventDefault();
                        e.stopPropagation();
                        return;
                    case 85: //ctrl+U or ctrl+u
                    case 117:
                        e.preventDefault();
                        e.stopPropagation();
                        break;
                }
            };
        },
        insertHtmlIntoText(html) {
            this.setEditorCursorPosition((this.nodePosition || 0), this.editor);
      
            document.execCommand("insertHTML", false, html);

            this.value = this.convertHtmlToEncodedText();
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

            this.value = this.convertHtmlToEncodedText();
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
            this.$dispatch('elementsChange', this.elements)
            //this.$events.emit(this.elementsEvent, this.elements);
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
        onTouchEnd(ev) {
            this.hasSelectedText = (this.selectedText != null && this.selectedText.length > 0);
        },
        onMouseUp(ev) {
            this.hasSelectedText = (this.selectedText != null && this.selectedText.length > 0);
            console.log(this.hasSelectedText)
        }, 
        convertHtmlToEncodedText() {
            let encodedText = this.editor.innerHTML;
            encodedText = encodedText.replace(/&nbsp;/g, ' ');
            for (var i = 0; i < this.elements.length; i++) {
                encodedText = encodedText.replace(this.elements[i].formatted, this.elements[i].encoded);
            }
            return encodedText;
        },
        onPaste(e) { 
            e.preventDefault(); 
            var contentOnBlur = prompt('Paste text');
            // replace/strip HTML
            contentOnBlur = contentOnBlur.replace(/(<([^>]+)>)/ig, '');
            document.execCommand('insertText', false, contentOnBlur);
        },
        onKeyup(ev) {
            this.nodePosition = this.getCaretPosition(ev.target)
            this.contextNode = ev.target;
            /*
            case 86: //ctrl+V or ctrl+v
                        console.log(ev)
                        ev.preventDefault();
                        return;
                        */

            if (ev.key != '@' && ev.key != 'Shift') {
                this.insert = false;
            }
             

            this.value = this.convertHtmlToEncodedText();
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
            this.addLinkCard(ev.target.innerText);
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
        get selectedText() {
            return this.getSelectionText();
        },
        insertCodeIntoText(formatType) {
            this.editor.focus();
            this.showRichTextMenu = false;
            var selectedText = this.selectedText;
            if (!this.hasSelectedText && this.selectedText.length == 0) return;
            this.insertEncodedText(formatType, selectedText);
        },

        insertEncodedText(formatType, encodedText) {
            const format = this._mxCardPost_GetFormat(formatType);
            const element = this._mxCardPost_CreateElement(this.nodePosition, format, encodedText);

            this.addElement(element)
            this.performAction('insertHTML', element.formatted)
            this.value = this.convertHtmlToEncodedText();

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
                <nav x-show="showPanel">
                    <ul style="width:100%; max-width:100%">
                        <fieldset role="group" style="margin-bottom:0px">
                        <button x-show="!showInputEditors && showRichText" :disabled="!hasSelectedText" class="material-icons-round flat small px-0" @click="insertCodeIntoText('code')">code</button>
                        <button x-show="!showInputEditors && showRichText" :disabled="!hasSelectedText" class="material-icons-round flat small px-0" @click="insertCodeIntoText('quote')">format_quote</button>
                        <button x-show="!showInputEditors && showRichText" :disabled="!hasSelectedText" class="material-icons-round flat small px-0" @click="insertCodeIntoText('bold')">format_bold</button>
                        <button x-show="!showInputEditors && showRichText" :disabled="!hasSelectedText" class="material-icons-round flat small px-0" @click="insertCodeIntoText('italics')">format_italic</button>
                        
                        <!--Url input-->
                        <button x-show="!showUrlEditor && !showElementEditor" class="small material-icons-round flat small" @click="toggleUrlEditor()">link</button>
                        <button x-show="showUrlEditor && !showElementEditor" class="small material-icons-round flat small" @click="toggleUrlEditor()">cancel</button>

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
                        <button x-show="!showElementEditor && !showUrlEditor" class="material-icons-round flat small" @click="toggleUserEditor()">person_search</button>
                        <button x-show="showElementEditor && !showUrlEditor" class="small material-icons-round flat small" @click="toggleUserEditor()">cancel</button>
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
                                        <ul style="display: grid;list-style:none; text-align:left; " >
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
                        <div x-show="!showInputEditors" x-data="aclContentEmoji({ event: onEmojiEvent })"></div>
                        </fieldset>
                    </ul>
                </nav>
                <!--input-->
                <div   
                    id="editor"
                    contenteditable="true"
                    role="textbox"
                    x-html="html"
                    class="wysiwyg"
                    :placeholder="placeholder"
                    @click="($event) => onClick($event)"
                    @touchend="($event) => onTouchEnd($event)"
                    @mouseup="($event) => onMouseUp($event)"
                    @keyup="($event) => onKeyup($event)"
                    @keyup.shift.enter="($event) => onShiftEnter($event)"
                    @keyup.debounce="($event) => onKeyupDebounce($event)"
                    @keyup.@="($event) => toggleElementInput($event)">
                    @paste="($event) => onPaste($event)"
                </div> 
            </div>`
        },
    };
}