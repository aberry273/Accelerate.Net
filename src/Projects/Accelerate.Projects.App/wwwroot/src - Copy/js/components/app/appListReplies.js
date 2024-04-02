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
        userId: null,
        expandable: true,
        items: [],
        threadUrl: '/',
        targetThread: null,
        initialItems: [],
        init() {
            this.expandable = data.expandable;
            this.filterId = data.feed;
            this.userId = data.userId;
            this.reviewUrl = data.reviewUrl;
            this.sourceUrl = data.sourceUrl;
            this.reviewUrl = data.reviewUrl;
            this.threadUrl = data.threadUrl;
            this.fetchPostsUrl = data.fetchPostsUrl;

            this.fetchReviewsUrl = data.fetchReviewsUrl;
            this.targetThread = data.targetThread;
            //this.selectFeed(data.feed);
            this.initialItems = data.items;
            this.setHtml(data);
            const self = this;

            this.$events.on('action-reply', (item) => {
                self.selected = item;
            })
            this.$events.on('action-close', (item) => {
                self.selected = null;
            })
            this.$events.on('reply:created', (item) => {
                self.selected = null;
            })
        },
        isSelected(item) {
            return (this.selected != null && this.selected.id == item.id)
        },
        setHtml(data) {
            // make ajax request
            const html = `
          <div x-data="_contentPosts({
            items: initialItems,
            targetThread: targetThread,
              fetchPostsUrl: fetchPostsUrl,
              fetchReviewsUrl: fetchReviewsUrl,
            userId: userId,
          })">
          <template x-for="(post, i) in posts" :key="post.id+':'+post.updatedOn">
            <div>
              <div x-data="appCardPostReply(
                {
                  item: post,
                  userId: userId,
                  threadUrl: threadUrl,
                })"></div>
                <template x-if="selected != null && selected.id == post.id">
                  <article>
                    <i aria-label="Cancel" @click="selected = null" class="icon material-icons icon-click" rel="prev">close</i>
                    <div x-data="appFormResponse({
                      postbackUrl: 'https://localhost:7220/api/contentpost',
                      postbackType: 'POST',
                      event: 'reply:created',
                      fieldPlaceholder: 'Write a reply',
                      label: 'Reply'
                    })">
                    </div>
                  </article>
                </template>
              </div>
            </template>
            <template x-if="posts.length == 0">
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