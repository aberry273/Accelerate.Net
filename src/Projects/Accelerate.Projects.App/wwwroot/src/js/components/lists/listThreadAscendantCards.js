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
        item: {},
        items: [],
        results: {},
        userId: '',
        parentId: '',
        searchUrl: '',
        loading: false,
        lastScrollHeight: 0,
        inPosition: false,

        async init() {
            const self = this;
            data = data != null ? data : {}
            this.item = data.item;
            this.results = data.results;
            this.$store.wssContentPosts.setSearchResults(this.results);
            this.items = this.results.posts;
            this.userId = data.userId;
            this.filters = data.filters;
            this.searchUrl = data.searchUrl;

            component = data.component || component
            this.setHtml(data);
        },
        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div id="ascendants">
                <template x-for="(item, i) in items" :key="item.id">
                    <div>
                        <div x-data="cardPost({
                            item: item,
                            userId: userId,
                            actionEvent: actionEvent,
                            updateEvent: item.id,
                            filterEvent: filterEvent,
                            actionEvent: actionEvent,
                            itemEvent: $store.wssContentPosts.getMessageEvent(),
                            parentId: item.id
                        })"></div>
                        <div
                            x-show="i < items.length-1"
                            :id="i == items.length-2 ? 'parentline' : null"
                            class="line-background" ></div>
                    </div>
                </template>
            </div>
            `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}