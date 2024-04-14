let component = `
    <div x-data="cardPost(
    {
      item: item,
    })"></div>
`
import mxList from '/src/js/mixins/mxList.js';
import mxSearch from '/src/js/mixins/mxSearch.js';
import mxAction from '/src/js/mixins/mxAction.js';
import mxWebsockets from '/src/js/mixins/mxWebsockets.js';
import mxAlert from '/src/js/mixins/mxAlert.js';

export default function (data) {
    return {
        // mixins
        ...mxList(data),
        ...mxSearch(data),
        ...mxAction(data),
        ...mxWebsockets(data),
        ...mxAlert(data),

        // PROPERTIES
        items: [],
        userId: '',
        searchUrl: '',
        filterEvent: '',
        actionEvent: '',
        itemEvent: '',

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

            component = data.component || component
             
            // Setup websocket listeners
            await this._mxWebsockets_InitWebsocketEvents(
                this.$store.wssContentPosts,
                this.userId,
                this.targetChannel,
                this.targetThread,
            )
            await this._mxWebsockets_InitWebsocketEvents(
                this.$store.wssContentPostReviews,
                this.userId,
                this.targetChannel,
                this.targetThread,
            )

            // On updates from the websocket
            this.$events.on(this.$store.wssContentPosts.getMessageEvent(), async (e) => {
                const data = e.data;
                if (!data) return;
                if (data.alert) this._mxAlert_AddAlert(data);
                this.updateItemUpdate(data);
            })

            // On updates from cards
            this.$events.on(this.actionEvent, async (request) => {
                const payload = this.CreatePostActivity(request);
                await this._mxAction_HandleActionPost(payload);
            })

            // On updates from filter
            this.$events.on(this.filterEvent, async (filterUpdates) => {
                await this.search(filterUpdates);
            })

            let queryData = {}
            if (data.parentId) queryData.parentId = [data.parentId]
            if (data.targetChannel) queryData.targetChannel = [data.targetChannel]

            await this.search(queryData);

            this.setHtml(data);
        },
        // METHODS
        async search(filters) {
            let query = this._mxList_GetFilters(filters);
            const postQuery = this._mxSearch_CreateSearchQuery(query);
            if (postQuery == null) return;
            this.items = await this._mxSearch_Post(this.searchUrl, postQuery);
        },

        CreatePostActivity(request) {
            return {
                userId: request.userId,
                contentPostId: request.item.id,
                action: request.action,
                value: null,
            }
        },

        updateItemUpdate(wssMessage) {
            var item = wssMessage.data;
            if (this.items == null) this.items = [];
            if (wssMessage.update == 'Created') {
                const index = this.items.map(x => x.id).indexOf(item.id);
                if (index == -1) this.items.push(item);
                else this.items[index] = item
            }
            if (wssMessage.update == 'Updated') {
                const index = this.items.map(x => x.id).indexOf(item.id);
                this.items[index] = item
            }
            if (wssMessage.update == 'Deleted') {
                const index = this.items.map(x => x.id).indexOf(item.id);
                this.items.splice(index, 1);
            }
        },
        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div x-transition>
              <template x-for="(item, i) in items" :key="item.id+item.updatedOn || i" >
                <div x-data="cardPost({
                  item: item,
                  userId: userId,
                  updateEvent: actionEvent,
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