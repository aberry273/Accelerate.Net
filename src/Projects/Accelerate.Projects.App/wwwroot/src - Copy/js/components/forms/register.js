export default function (data) {
	return {
    // PROPERTIES
    loading: false,
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
          <label>
            First name
            <input
              name="first_name"
              placeholder="First name"
              autocomplete="given-name"
            />
          </label>
          <label>
            Email
            <input
              type="email"
              name="email"
              placeholder="Email"
              autocomplete="email"
            />
          </label>
        </fieldset>
      
        <input
          type="submit"
          value="Subscribe"
        />
      </form>
      `
    },
  }
}