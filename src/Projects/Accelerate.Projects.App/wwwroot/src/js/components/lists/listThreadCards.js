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
        showPostReplies: [],
        userId: '',
        searchUrl: '',
        filterEvent: '',
        actionEvent: '',
        itemEvent: '',
        parentId: '',
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
            //this.threadId = data.threadId;
            this.parentId = data.parentId;
            this.channelId = data.channelId;
            this.quoteEvent = data.quoteEvent;
            this.filters = data.filters;

            component = data.component || component
             
            // On updates from filter
            this.$events.on(this.filterEvent, async (filterUpdates) => {
                // If not filters are applied, use default filters
                if (!this.hasFiltersApplied(filterUpdates)) {
                    await this.initSearch();
                }
                else {
                    await this.$store.wssContentPosts.SearchByUrl(this.searchUrl, filterUpdates, true);
                }
                
            })

            await this.initSearch();

            this.setHtml(data);
        },
        hasFiltersApplied(filters) {
            const keys = Object.keys(filters);
            for (let i = 0; i < keys.length; i++) {
                if (filters[keys[i]].length > 0) {
                    return true;
                }
            }
            return false;
        },
        async initSearch() {
            let queryData = this.filters || {} 
            await this.$store.wssContentPosts.SearchByUrl(this.searchUrl, queryData);
        },

        replies(post) {
            return this._mxCardPost_getActionSummary(post.id).replies
        },

        get threadItems() {
            return this.$store.wssContentPosts.items.filter(x => x.parentId == this.parentId);
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
            <div class="list">
              <template x-for="(item, i) in threadItems" :key="item.id || i" >
                    <div>
                        <div x-data="cardPost({
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

                        <div x-show="replies(item) > 0 && !showReplies(item)">
                            <a class="line child click" @click="toggleReplies(item)">
                                <small class="pl">
                                    <small>
                                        <span>Show replies</span>
                                    </small>
                                </small>
                            </a>
                        </div>

                        <!-- Replies -->
                        <template x-if="showReplies(item)">
                            <div>
                               <a class="line child click" @click="toggleReplies(item)">
                                    <small class="pl">
                                        <small>
                                            <span>Hide replies</span>
                                        </small>
                                    </small>
                                </a>

                                <div class="line child mt-0"
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
              <template x-if="threadItems == null || threadItems.length == 0">
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