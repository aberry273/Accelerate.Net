import { mxContent, mxForm } from '/src/js/mixins/index.js';

export default function (params) {
	return {
        ...mxContent(params),
        ...mxForm(params),
        // PROPERTIES
        header: '',
        form: {},
        // INIT
        init() {
            this.setValues(params);
            this.render();
        },
        // GETTERS
        // METHODS
        setValues(params) {
            this.mxContent_title = params.title;
            this.mxContent_subtitle = params.subtitle;
            this.mxContent_text = params.text;

            this.mxForm_fields = params.fields;
            this.mxForm_method = params.method;
            this.mxForm_action = params.action;
        },
        onChange(field) {
            console.log('change')
            console.log(field);
        },
        render() {
            const html = `
                <div :class="mxForm_formClass">
                    <h4 :class="mxContent_titleClass" x-text="mxContent_title"></h4>
                    <p :class="mxContent_textClass" x-text="mxContent_text"></p>
                    <form novalidate :method="mxForm_method" :action="mxForm_action" class="group relative w-full mt-10 space-y-8">
                        <!--Fields-->
                        <div x-data="aclFormFieldset({ fields: mxForm_fields })" @onChange="onChange"></div>
                        <!--Submit-->
                        <div class="relative">
                             <input role="button" type="submit" value="Submit" :class="mxForm_submitClass + ' '+ mxForm_submitInvalidClass" />
                        </div>
                    </form>
                </div>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      },
    }
}