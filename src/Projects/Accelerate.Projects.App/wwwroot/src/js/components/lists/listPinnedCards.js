let component = `
    <div x-data="cardPost(
    {
      item: item,
    })"></div>
`
import { mxList, mxSearch, mxWebsockets, mxCardPost, mxAlert } from '/src/js/mixins/index.js';
export default function (data) {
    return {
        // mixins
        ...mxList(data),
        ...mxSearch(data),
        //...mxAction(data),
        ...mxWebsockets(data),
        ...mxAlert(data),
        ...mxCardPost(data),

        // PROPERTIES
        items: [],
        forceLoad: true,
        showPostReplies: [],
        showReplies: false,
        userId: '',
        searchUrl: '',
        filterEvent: '',
        actionEvent: '',
        itemEvent: '',
        quoteEvent: '',
        filterUpdates: {},

        async init() {
            const self = this;
            data = data != null ? data : {}
            this.forceLoad = data.forceLoad;
            this.items = data.items;
            this.showReplies = data.showReplies;
            this.filterEvent = data.filterEvent;
            this.actionEvent = data.actionEvent;
            this.itemEvent = data.itemEvent;
            this.searchUrl = data.searchUrl;
            this.userId = data.userId;
            this.threadId = data.threadId;
            this.channelId = data.channelId;
            this.quoteEvent = data.quoteEvent;
            this.filters = data.filters;

            component = data.component || component

            // On update post from the websocket 
            this._mxEvents_On(this.itemEvent, async (e) => {
                const msgData = e.data;
                if (!msgData) return;
                //check if item meeds filter criteria
                let filters = { ...this.filters, ...this.filterUpdates.filters }
                var itemMatchesFilters = this.filterItem(msgData.data, filters);
                //Only live update items if the current filters apply
                if (itemMatchesFilters) {
                    this.items = this.$store.wssContentPosts.updateItems(this.items, msgData);
                }
            })
            
            await this.initSearch();
            this.setHtml(data);
        },
        async initSearch(queryData) {
            const results = await this.$store.wssContentPosts.SearchPosts({}, this.searchUrl);
            this.items = results;
        },
        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div class="list">
                <template x-for="(pinnedPost, i) in items" :key="i">
                    <article class="outlined padless">
                        <!--Pin--> 
                        <div class="dense pin success" >
                            <nav class="mx">
                                <ul>
                                    <small>
                                        <span class="muted" style="padding-right:6px;">Pinned by: </span>
                                        <strong style="padding-right:6px;" x-text="pinnedPost.username"></strong>
                               
                                        <span x-show="pinnedPost.reason">
                                            '<span x-show="pinnedPost.reason" x-text="pinnedPost.reason"></span>'
                                        </span>
                                    </small>
                                </ul>
                                <ul>
                                    <small>
                                        <a class="muted" href="pinnedPost.href" x-text="pinnedPost.date"></a>
                                    </small>
                                </ul>
                            </nav>
                        </div>

                        <div x-data="cardPost({
                            item: pinnedPost.contentPost,
                            userId: userId,
                            actionEvent: actionEvent,
                            updateEvent: pinnedPost.contentPost.id,
                        })"></div>
                    </article>
                </template>
            </div>
            `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}