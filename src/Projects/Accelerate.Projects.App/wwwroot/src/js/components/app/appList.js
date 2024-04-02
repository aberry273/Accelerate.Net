let component = `
    <div x-data="appCardPost(
    {
        item: post,
        userId: userId,
        threadUrl: threadUrl,
    })"></div>
`
export default function (data) {
    return {
        // PROPERTIES
        selected: null,
        filterId: '',
        filtered: [],
        loading: false,
        sourceUrl: '#',
        fetchPostsUrl: '#',
        fetchReviewsUrl: '#',
        targetThread: null,
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
            this.reviewUrl = data.reviewUrl;
            this.threadUrl = data.threadUrl;
            this.targetThread = data.targetThread;
            this.fetchPostsUrl = data.fetchPostsUrl;
            this.fetchReviewsUrl = data.fetchReviewsUrl;
            this.initialItems = data.items;
            component = data.component || component
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
            userId: userId,
            targetThread: targetThread,
            fetchPostsUrl: fetchPostsUrl,
            fetchReviewsUrl: fetchReviewsUrl,
          })">
          <template x-for="(post, i) in posts" :key="post.id+':'+post.updatedOn" >
            <div :id="post.id" x-data="appCardPost(
            {
                item: post,
                userId: userId,
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