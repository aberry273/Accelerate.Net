const edjsParser = edjsHTML();
export default function (params) {
	return {
        // PROPERTIES
        value: null,
        html: '',
        // INIT
        init() {
            this.setValues(params);
            this.render(); 
        },
        // GETTERS
        
        // METHODS
        setValues(params) { 
            this.value = Array.isArray(params.value) ? params.value : []; 
            const HTML = edjsParser.parseStrict({ blocks: this.value }); 
            if(HTML instanceof Error) throw HTML;
            this.html = HTML.join('');
        },
        render() {
            const html = `<div x-html="html"></div>`
            this.$nextTick(() => { this.$root.innerHTML = html });
      },
    }
}
