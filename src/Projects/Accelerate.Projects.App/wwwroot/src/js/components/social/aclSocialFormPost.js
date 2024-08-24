import { mxContent, mxForm, mxEvent, } from '/src/js/mixins/index.js';

export default function (params) {
	return {
        ...mxEvent(params),
        ...mxForm(params),
        // PROPERTIES
        open: false,
        header: '', 
        actions: [],
        form: {},
        formatActions: [],
        settingFields: [
            'Tags',
            'Category',
            'Tags2',
            'Category2',
            'Visibility',
        ],
        modalFields: [],
        // INIT
        init() {
            this.setValues(params);
            this.render();
        },
        // GETTERS
        get settingsForm() {
            return {
                class: this.mxForm_formPadlessClass,
                title: 'Manage replies',
                label: 'Save',
                fields: this.modalFields,
                submit: (data) => { this.submitSettings(data) }
            }
        },
        // METHODS
        submitSettings(data){
            console.log('submitSettings');
            console.log(data);
            console.log(this.form.fields)
        },
        setValues(params) {
            this.mxContent_title = params.title;
            this.actions = params.actions;
            this.form = params.form;
            this.modalFields = this.filterFields(this.settingFields);
        },
        clearFields() {
            for(var i = 0; i < this.form.fields.length; i++) {
                this.form.fields[i].value = null;
                this.form.fields[i].updated = new Date().toISOString();
            } 
        },
        async submit() {
            this.loading = true;
            console.log(this.form)
            try {
                const formData = this._mxForm_GetFileFormData(this.form); 
                const response = this.$fetch.POST(this.form.action, formData, this.mxForm_FileFormHeaders, false);
                //this._mxEvent_Emit(this.$store.svcComments.svcComments_eventCreate, response)
                //this.clearFields();
                //console.log(this.$store.svcComments);
            } catch(e) {
                //console.log(e);
            }
            const dummyData = this._mxForm_GetFormData(this.form);
            const dummyItem = this.createDummyComment(dummyData);
            
            this._mxEvent_Emit(this.$store.svcComments.svcComments_eventCreate, dummyItem);
            this.clearFields();
            this.loading = false;
        },
        createDummyComment(data) {
            return { 
                id: data.id,
                profile: {
                    icon: 'calendar',
                    displayName: 'Deb peterson',
                    username: '@johnwes',
                    description: 'Blogger, thraser, skaterboi, etc',
                    date: '10/07/2024',
                    href: '#',
                    img: 'https://flowbite.s3.amazonaws.com/blocks/marketing-ui/avatars/roberta-casas.png',
                },
                settings: {},
                content: {
                    date: '2024-07-20T00:38:52.0421225Z',
                    text: data.text,
                    formats: data.formats,
                    media: [
                        {
                            src: "https://cdn.devdojo.com/images/june2023/mountains-01.jpeg",
                            id: "855c493e-e09d-4a82-ba84-5781a2af83a9",
                            name: "example-aaa.png",
                            type: "Image"
                        },
                    ]
                },
                replies: { },
                metrics: { },
                taxonomy: { },
                menu: [ 'CopyLink' ],
                actions: [ 'reply', 'quote', 'upvote', 'downvote', 'tag' ],
            };
        },
        indexOf(fieldName) {
            if (!this.form.fields) return -1;
            return this.form.fields
                .map(x => x.name)
                .indexOf(fieldName);;
        },
        updateFieldValue(field) {
            const index = this.indexOf(field.name);
            if (index == -1) return;
            this.form.fields[index].value = field.value;
        },
        toggleFieldVisibility(fieldName) {
            const index = this.indexOf(fieldName); 
            if (index == -1) return;
            this.form.fields[index].hidden = !this.form.fields[index].hidden;
        },
        toggleFieldModal(fieldName) {
            console.log(fieldName)
            this._mxEvent_Emit(this.getFieldEvent(fieldName), quoteFormData);
            //this.form.fields[index].hidden = !this.form.fields[index].hidden;
        },
        getFieldEvent(fieldName) {
            return `on:modal:${fieldName}`;
        },
        getField(fieldName) {
            const index = this.indexOf(fieldName); 
            this.form.fields[index];
        },
        switchTextField() {
            /*
            const textField = this.getField('Text');
            console.log(this.form.fields)
            const formatField = this.getField('Formats');
            console.log(textField)
            const block = {
                data: {
                    text: textField.value
                },
                type: 'paragraph'
            };
            formatField.value = [block];
            */
            this.toggleFieldVisibility('Text')
            this.toggleFieldVisibility('Formats')
        },
        filterFields(fieldNames) {
            const fields = this.form.fields
                .filter(x => fieldNames.indexOf(x.name) > -1)
                .map(x => { return { ...x } });

            const updatedFields = fields.map(x => {
                x.hidden = false;
                return x;
            })
            return updatedFields;
        },
        render() {
            const html = `
                <div>
                    <!--Fields-->
                    <div 
                        x-data="aclFormFieldset(form)" 
                        @onfieldchange.window="(ev) => { updateFieldValue(ev.detail) }"></div>
                    
                    <div class="w-full">
                        <div class="flex items-center justify-between px-3 py-2 border-b dark:border-gray-600">
                            <div class="flex flex-wrap items-center">
                                 
                                <div @click="toggleFieldVisibility('Image')" x-data="aclButton({ icon: 'image' })"></div>
                                <div @click="toggleFieldVisibility('Video')" x-data="aclButton({ icon: 'video' })"></div>
                                <div @click="toggleFieldVisibility('Mention')" x-data="aclButton({ icon: 'userPlus' })"></div>
                                <!--
                                <div @click="switchTextField" x-data="aclButton({ icon: 'documentText' })"></div>
                                <div @click="toggleFieldVisibility('Top')" x-data="aclButton({ icon: 'presentationChart' })"></div>
                                -->
                                <div @click="toggleFieldModal('Settings')" x-data="aclButton({ icon: 'cog' })"></div>
                                
                                <button type="button" data-tooltip-target="tooltip-fullscreen" class="p-2 text-gray-500 rounded cursor-pointer sm:ms-auto hover:text-gray-900 hover:bg-gray-100 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600">
                                    <svg class="w-4 h-4" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 19 19">
                                        <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 1h5m0 0v5m0-5-5 5M1.979 6V1H7m0 16.042H1.979V12M18 12v5.042h-5M13 12l5 5M2 1l5 5m0 6-5 5"/>
                                    </svg>
                                    <span class="sr-only">Full screen</span>
                                </button>
                            </div>
                            
                            <div class="flex flex-wrap items-center divide-gray-200 sm:divide-x sm:rtl:divide-x-reverse dark:divide-gray-600"> 
                                <button 
                                    @click="submit"
                                    class="end-0 text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-4 py-2 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800">
                                    <svg class="w-5 h-5" x-data="aclIconsSvg({ mxIcon_name: 'send' })"></svg>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div x-data="aclModalFormAjax({ 
                    event: getFieldEvent('Settings'), 
                    form: settingsForm })">
                </div>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      },
    }
}