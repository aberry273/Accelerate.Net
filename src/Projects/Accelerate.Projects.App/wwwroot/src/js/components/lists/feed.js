export default function (data) {
	return {
    // PROPERTIES
    filterId: '',
    filtered: [],
    loading: false,

    init() {
      this.filterId = data.feed;
      this.selectFeed(data.feed);
      this.setHtml(data)
      const self = this;
      this.$watch('$store.feedFilters.current', (val) => {
        this.selectFeed(val)
      })
    },
    selectFeed(feed) {
      this.filterPosts(feed);
    },
    filterPosts(feed) {
      this.loading = true;
      this.filtered = this.$store.feeds.items.filter(x => x.feed == feed);
      this.loading = false;
    },
    setHtml(data) {
      // make ajax request
      this.$root.innerHTML = `
        <div x-data="$data.feed" class="container feed" x-transition>
          <template x-for="post in filtered" :key="post.id">
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