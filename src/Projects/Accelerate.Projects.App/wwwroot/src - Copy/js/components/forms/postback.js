import input from './fields/input.js'

export default function (data) {
	return {
    // PROPERTIES
    loading: false,
    fields: [],
    method: data.method,
    // INIT
    init() {
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
    setHtml(data) {
      // make ajax request
      this.fields = data.fields || []
      this.$root.innerHTML = `
      <form method="${this.method}">
        <fieldset>
          <template x-for="(field, i) in fields" :key="field.name+i"> 
            <label x-html="renderField(field)"x-show="!field.hidden"></label>
          </template>
        </fieldset>
        <input type="submit" value="Submit"/>
      </form>
      `
    },
  }
}