import input from './fields/input.js'
import textarea from './fields/textarea.js'

export default function (data) {
	return {
    // PROPERTIES
    loading: false,
    fields: [],
    label: 'Submit',
    loading: false,
    event: null,
    // INIT
    init() {
      this.label = data.label;
      this.event = data.event;
      this.setHtml(data)
    },
    // METHODS
    submitForm() {
    },
    filterPosts(feed) {
      this.loading = true;
      this.filtered = this.$store.feeds.items.filter(x => x.feed == feed);
      this.loading = false;
    },
    renderField(field) {
      if(field.type == 'textarea') return textarea(field)
      
      return input(field)
    },
    async submit(fields) {
      this.loading = true;
      const payload = {}
      fields.map(x => {
        payload[x.name] = x.value
        return payload
      })
      const response = this.$fetch.POST(data.postbackUrl, payload);
      if(this.event) {
        this.$dispatch(this.event, response)
      }
      this.loading = false;
    },
    setHtml(data) {
      // make ajax request
      const label = data.label || 'Submit'
      this.fields = data.fields || []
      this.$root.innerHTML = `
      <div>
        <progress x-show="loading"></progress>
        <fieldset>
          <template x-for="(field, i) in fields" :key="field.name+i"> 
            <label x-html="renderField(field)" x-show="!field.hidden"></label>
          </template>
        </fieldset>
        <button @click="await submit(fields)" :disabled="loading">${label}</button>
      </div>
      `
    },
  }
}