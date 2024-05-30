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
        userId: '',
        parentId: '',
        searchUrl: '',

        async init() {
            const self = this;
            data = data != null ? data : {}
            this.item = data.item;
            this.items = data.items;
            this.userId = data.userId;
            this.filters = data.filters;
            this.searchUrl = data.searchUrl;

            component = data.component || component

            this.setHtml(data);
            await this.initSearch();
        },
        orderByDate(items) {
            items.sort(function (a, b) {
                return new Date(a.createdOn) - new Date(b.createdOn);
            });
        },
        async initSearch() {
            let queryData = this.filters || {}
            await this.$store.wssContentPosts.SearchByUrl(this.searchUrl, queryData, false);
            const items = this.$store.wssContentPosts.FilterPostsById(this.item.parentIds);
            this.orderByDate(items);
            this.items = items;
            //const results = await this.$store.wssContentPosts.SearchPosts(queryData, this.searchUrl);
            //this.items = results.posts;
        },
        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div class="">
                <template x-for="(item, i) in items" :key="item.id || i" >
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
                        <div class="line-background"></div>
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