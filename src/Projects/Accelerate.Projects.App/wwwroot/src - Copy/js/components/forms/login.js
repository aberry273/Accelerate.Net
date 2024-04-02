export default function (data) {
	return {
    // PROPERTIES
    loading: false,
    fields: [
      {
        type: 'input',
        placeholder: 'Username',
        label: 'Username',
        autocomplete: null
      },
      {
        type: 'password',
        placeholder: 'Password',
        label: 'Password',
        autocomplete: null
      }
    ],
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
    setHtml(data) {
      // make ajax request
      this.$root.innerHTML = `
      <form>
        <fieldset>
          <template x-for="field in fields"> 
            <label>
              <template x-text="field.name"></template>
              <input
                :type="field.type"
                :name="field.name"
                :placeholder="field.placeholder"
                :autocomplete="field.autocomplete"
              />
            </label>
          </template>
        </fieldset>
        <input type="submit" value="Submit"
        />
      </form>
      `
    },
  }
}