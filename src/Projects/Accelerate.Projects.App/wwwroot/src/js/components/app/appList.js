export default function (data) {
    return {
        // PROPERTIES
        selected: null,
        filterId: '',
        filtered: [],
        loading: false,
        sourceUrl: '#',
        searchUrl: '#',
        expandable: true,
        threadUrl: '/',
        initialItems: [],
        async init() {
            const self = this;
            this.expandable = data.expandable;
            this.filterId = data.feed;
            this.sourceUrl = data.sourceUrl;
            this.userId = data.userId;
            this.channel = data.channel;
            this.threadUrl = data.threadUrl;
            this.searchUrl = data.searchUrl;
            this.initialItems = data.items;
            //this.selectFeed(data.feed);
            this.setHtml(data);

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
        setHtml(data) {
            // make ajax request
            const html = `
      <div x-data="_contentPosts({
          items: initialItems,
          sourceUrl: sourceUrl,
          channel: channel,
          searchUrl: searchUrl,
        })">
        <template x-for="(post, i) in posts" :key="post.id+post.updatedOn" >
          <div x-data="appCardPost(
            {
              item: post,
              threadUrl: threadUrl,
            })"></div>
        </template> 
        <template x-if="posts == null || posts.length == 0">
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