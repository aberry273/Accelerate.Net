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
            this.targetThread = data.targetThread;
            this.targetChannel = data.targetChannel;
            this.quoteEvent = data.quoteEvent;
            this.filters = data.filters;

            component = data.component || component
             
            // On updates from filter
            this.$events.on(this.filterEvent, async (filterUpdates) => {
                await this.search(filterUpdates);
            })
            await this.initSearch();

            this.setHtml(data);
        },
        async initSearch() {
         let queryData = this.filters || {}
          await this.search(queryData);
        },

        // METHODS
        async search(filters) {
            let query = this._mxList_GetFilters(filters);
            const postQuery = this._mxSearch_CreateSearchQuery(query);
            if (postQuery == null) return;
            const items = await this._mxSearch_Post(this.searchUrl, postQuery);

            this.$store.wssContentPosts.setItems(items);
        },
        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div x-transition>
              <template x-for="(item, i) in $store.wssContentPosts.items" :key="item.id || i" >
                <div x-data="cardPost({
                  item: item,
                  userId: userId,
                  actionEvent: actionEvent,
                  updateEvent: item.id,
                })"></div>
              </template>
              <template x-if="items == null || items.length == 0">
                <article>
                  <header><strong>No results!</strong></header>
                  No results could be found
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