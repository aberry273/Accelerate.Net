let component = `
    <div x-data="cardPost(
    {
      item: item,
    })"></div>
`
import { mxList, mxSearch, mxWebsockets, mxAlert } from '/src/js/mixins/index.js';
export default function (data) {
    return {
        // mixins
        ...mxList(data),
        ...mxSearch(data),
        //...mxAction(data),
        ...mxWebsockets(data),
        ...mxAlert(data),

        // PROPERTIES
        items: [],
        userId: '',
        searchUrl: '',
        filterEvent: '',
        actionEvent: '',
        itemEvent: '',
        quoteEvent: '',

        async init() {
            const self = this;
            data = data != null ? data : {}
            this.items = data.items;
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

            // On updates from filter
            this.$events.on(this.filterEvent, async (filterUpdates) => {
                //await this.$store.wssContentPosts.Search(filterUpdates, true);
                await this.initSearch(filterUpdates, true);
            })
            await this.initSearch(this.filters || {});

            this.setHtml(data);
        },
        async initSearch(queryData, replaceItems = false) {
            if (this.searchUrl) {
                await this.$store.wssContentPosts.SearchByUrl(this.searchUrl, queryData, replaceItems);
            }
            else {
                await this.$store.wssContentPosts.Search(queryData, replaceItems);
            }
        },
        get parentItems() {
            return this.$store.wssContentPosts.items.filter(x => x.parentId == null);
        },

        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div x-transition class="list">
              <template x-for="(item, i) in parentItems" :key="item.id || i" >
                <div x-data="cardPost({
                  item: item,
                  userId: userId,
                  actionEvent: actionEvent,
                  updateEvent: item.id,
                })"></div>
              </template>
              <template x-if="parentItems == null || parentItems.length == 0">
                <article>
                  <header><strong>No results!</strong></header>
                  It looks not there are no posts here yet, try create your own!
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