export default function (data) {
	return {
    // PROPERTIES
    filterId: '',
    filtered: [],
    loading: false,
    sourceUrl: '#',
    init() {
      this.filterId = data.feed;
      this.sourceUrl = data.sourceUrl;
      //this.selectFeed(data.feed);
      this.setHtml(data)
      const self = this;
      this.$watch('$store.feedFilters.current', async (val) => {
        await this.filterPosts(val)
      })
    },
    async filterPosts(feed) {
      this.loading = true;
      await this.fetchItems();
      this.loading = false;
    },
    async fetchItems() {
      const results = await this.$fetch.GET(this.sourceUrl);
      const items = results.map(x => {
        return {
          id: x.id,
          username: x.userId,
          profile: 'https://placehold.co/150x150',
          handle: `@${x.userId}`,
          updated: '5 minutes ago',
          content: `<p>${x.content}</p>`,
          feed: '',
          liked: false,
          agree: 0,
          disagree: 0,
          footer: 'footer',
        }
      })
      this.filtered = items;
    },
    setHtml(data) {
      // make ajax request
      this.$root.innerHTML = `
        <div x-data="$data.feed" x-on:post:created.window="await fetchItems" class="container feed" x-transition>
          <template x-for="(post, i) in filtered" :key="post.id+''+i">
            <div x-data="card(post)"></div>
          </template>
          <template x-if="filtered.length == 0">
            <article>
              <header><strong>No results!</strong></header>
              No posts could be found
            </article>
          </template>
          <template x-if="loading">
            <article aria-busy="true"></article>
          </template>
        </div>
      `
    },
  }
}