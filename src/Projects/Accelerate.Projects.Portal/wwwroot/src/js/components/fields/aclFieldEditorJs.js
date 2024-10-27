import { mxField } from '/src/js/mixins/index.js';
 
const tools = {
    /**
     * Each Tool is a Plugin. Pass them via 'class' option with necessary settings {@link docs/tools.md}
     */
    header: {
      class: Header,
      inlineToolbar: ['link'],
      config: {
        placeholder: 'Header'
      },
      shortcut: 'CMD+SHIFT+H'
    },
    /**
     * Or pass class directly without any configuration
     */
    image: {
      class: SimpleImage,
      inlineToolbar: ['link'],
    },
    list: {
      class: List,
      inlineToolbar: true,
      shortcut: 'CMD+SHIFT+L'
    },
    checklist: {
      class: Checklist,
      inlineToolbar: true,
    },
    quote: {
      class: Quote,
      inlineToolbar: true,
      config: {
        quotePlaceholder: 'Enter a quote',
        captionPlaceholder: 'Quote\'s author',
      },
      shortcut: 'CMD+SHIFT+O'
    },
    warning: Warning,
    marker: {
      class:  Marker,
      shortcut: 'CMD+SHIFT+M'
    },
    code: {
      class:  CodeTool,
      shortcut: 'CMD+SHIFT+C'
    },
    delimiter: Delimiter,
    inlineCode: {
      class: InlineCode,
      shortcut: 'CMD+SHIFT+C'
    },
    linkTool: LinkTool,
    embed: Embed,
    table: {
      class: Table,
      inlineToolbar: true,
      shortcut: 'CMD+ALT+T'
    },
    /*
    mention: {
      class: MentionTool,
      config: {
        endpoint: 'http://localhost:3000/',
        queryParam: 'search'
      }
    }
    */
  };

const renderers = {
    h1: ({ children }) => `<h1 class="mb-4 text-4xl text-gray-900 md:text-5xl lg:text-6xl  ${sharedClasses}">${children}</h1>`,
    h2: ({ children }) => `<h1 class="mb-4 text-3xl text-gray-900 md:text-5xl lg:text-6xl ${sharedClasses}">${children}</h1>`,
    h3: ({ children }) => `<h3 class="text-3xl ${sharedClasses}">${children}</h3>`,
    h4: ({ children }) => `<h4 class="text-2xl ${sharedClasses}">${children}</h3>`,
    h5: ({ children }) => `<h5 class="text-xl ${sharedClasses}">${children}</h3>`,
    h6: ({ children }) => `<h6 class="text-large ${sharedClasses}">${children}</h3>`,
    p: ({ children }) => `<p class="my-4 text-lg ${bodyClasses} ${sharedClasses}">${children}</p>`,
    ul: ({ children }) => `<ul class="list-disc list-inside my-4 text-lg ${bodyClasses} ${sharedClasses}">${children}</ul>`,
    ol: ({ children }) => `<ol class="list-decimal list-inside my-4 text-lg ${bodyClasses} ${sharedClasses}">${children}</ol>`,
    li: ({ children }) => `<li class="my-2 text-lg ${bodyClasses} ${sharedClasses}">${children}</li>`,
    code: ({ children }) => `<code class="bg-gray-100 dark:bg-gray-800 rounded-md p-2 text-sm ${sharedClasses}">${children}</code>`,
    code_block: ({ children }) => `<pre class="bg-gray-100 dark:bg-gray-800 overflow-y-scroll rounded-md p-2 text-sm ${sharedClasses}">${children}</pre>`,
}

 // Parse this block in editorjs-html
 function customParser(block){
    return `<custom-tag> ${block.data.text} </custom-tag>`;
  }

  const edjsParser =  edjsHTML({custom: customParser});

export default function (params) {
	return {
        ...mxField(params),
        // PROPERTIES
        editor: null,
        id: '',
        fieldname: '',
        editorCss:  `
            .codex-editor__redactor {
                padding-bottom: 0 !important; /* This will override the inline style */
                border: none;
            }
            .ce-toolbar__content { max-width: 95%; padding-left: 8px; }
            .ce-block__content { padding-left: 8px; max-width: 95%; }
        `,
        editorReadonlyCss:  `
            .codex-editor__redactor {
                padding-bottom: 0 !important; /* This will override the inline style */
            }
            .ce-toolbar__content { display:none }
            .ce-block__content { padding-left: 0px; max-width: 100%; }
        `,
        readOnly: false,
        blocks: [],
        // INIT
        init() {
            // If no ID is set, use a default ID
            if (!params.id) params.id = this.defaultId;
            this._mxField_setValues(params);
            this.setValues(params);
            this.render();
            this.id = this.mxField_id;
            this.fieldname = this.mxField_name;
        },
        // GETTERS
        
        // METHODS
        setValues(params) {
            this.availableFormats = params.availableFormats;
            this.readOnly = params.readOnly;
        },
        onChange(data) {
            this.mxField_value = data;
            this._mxField_onChange(data);
        },
        render() {
            const html =  `
                <div class="w-full border border-gray-200 rounded-lg bg-gray-50 dark:bg-gray-700 dark:border-gray-600">
                    <div 
                        x-data="aclPluginEditorJs({ 
                            value: mxField_value, 
                            readOnly: false, 
                            id: mxField_id,
                            class: mxField_inputClass
                        })"
                        @onchange="(ev) => {onChange(ev.detail)}">
                    </div>
                </div>

                <style>
                    ${!this.readOnly ? this.editorCss : this.editorReadonlyCss}
                </style>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      },
    }
}
