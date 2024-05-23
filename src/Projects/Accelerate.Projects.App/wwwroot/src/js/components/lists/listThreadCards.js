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
        showPostReplies: [],
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
                await this.$store.wssContentPosts.Search(filterUpdates);
            })
            await this.initSearch();

            this.setHtml(data);
        },
        async initSearch() {
            let queryData = this.filters || {}
            await this.$store.wssContentPosts.Search(queryData);
        },

        toggleReplies(post) {
            const index = this.showPostReplies.indexOf(post.id);

            if (index == -1) {
                this.showPostReplies.push(post.id)
            }
            else {
                this.showPostReplies.splice(index, 1);
            }
        },

        showReplies(post) {
            const index = this.showPostReplies.indexOf(post.id);
            return index > -1;
        },

        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div x-transition>
              <template x-for="(item, i) in $store.wssContentPosts.items" :key="item.id || i" >
                    <div>
                        <div x-data="cardPostThread({
                            item: item,
                            userId: userId,
                            actionEvent: actionEvent,
                            updateEvent: item.id,
                             
                            searchUrl: searchUrl,
                            filterEvent: filterEvent,
                            actionEvent: actionEvent,
                            itemEvent: $store.wssContentPosts.getMessageEvent(),
                            parentId: item.id,
                            filters: {
                                parentId: [item.id]
                            },
                            
                        })"></div>

                        <a class="line click" @click="toggleReplies(item)" x-show="item.replies > 1 && !showReplies(item)">
                            <small class="pl">
                                <small>
                                    <span>View replies</span>
                                </small>
                            </small>
                        </a>
                         
                        <!-- Replies -->
                        <template x-if="showReplies(item)">
                            <div>
                               <a class="line click" @click="toggleReplies(item)">
                                    <small class="pl">
                                        <small>
                                            <span>Hide replies</span>
                                        </small>
                                    </small>
                                </a>

                                <div class="line primary mt-0"
                                    x-data="listThreadRepliesCards( {
                                    searchUrl: searchUrl,
                                    filterEvent: filterEvent,
                                    actionEvent: actionEvent,
                                    itemEvent: $store.wssContentPosts.getMessageEvent(),
                                    parentId: item.id,
                                    filters: {
                                        parentId: [item.id]
                                    },
                                    userId: userId,
                                })"></div>
                                <hr />
                            </div>
                        </template>

                    </div>
              </template>
              <template x-if="$store.wssContentPosts.items == null || $store.wssContentPosts.items.length == 0">
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