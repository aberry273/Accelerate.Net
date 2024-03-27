export default function (data) {
	return {
    // PROPERTIES
    selected: null,
    filterId: '',
    filtered: [],
    loading: false,
    sourceUrl: '#',
    expandable: true,
    items: [],
    threadUrl: '/',
    init() {
      this.expandable = data.expandable;
      this.filterId = data.feed;
      this.sourceUrl = data.sourceUrl;
      this.threadUrl = data.threadUrl;
      //this.selectFeed(data.feed);
      this.setHtml(data);
      const self = this;

      //DELETE THIS LATEr
      this.items = data.items;
      
      this.$watch('$store.feedFilters.current', async (val) => {
        await this.filterPosts(val)
      })

      // Listen for the event.
      window.addEventListener('expand',
        (e) => {
          const item = e.detail;
          self.selected = item;
        }, false);

      window.addEventListener('close',
        (e) => {
          self.selected = null;
      }, false);
    },
    async filterPosts(feed) {
      this.loading = true;
      await this.fetchItems();
      this.loading = false;
    },
    async fetchItems() {
        const results = await this.$fetch.GET(this.sourceUrl);
      /*
      const items = results.map(x => {
        return {
          id: x.id,
          userId: x.userId,
          username: 'null',
          profile: 'https://placehold.co/150x150',
          handle: `@${x.userId}`,
          updated: '5 minutes ago',
          content: x.content,
          feed: '',
          liked: false,
          agree: 0,
          disagree: 0,
          footer: 'footer',
        }
      })
      */
        this.filtered = results;
    },
    testFunction() {
      console.log('test');
    },
    setHtml(data) {
      // make ajax request
      const html = `
      <!--<div x-cloak x-on:post:created.window="await fetchItems" class="container feed" x-transition>
        -->  
      <div>
        <template x-for="(post, i) in filtered" :key="post.id+''+i">
          <div x-cloak x-data="appCardPost(
            {
              item: post,
              threadUrl: threadUrl,
            })"></div>
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
      this.$nextTick(() => {
        this.$root.innerHTML = html
      });
    },
  }
}