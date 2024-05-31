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
        loading: false,

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
            this.loading = true;

            if(this.items.length == 0) await this.initSearch();
             
            this.$nextTick(() => {
                const el = document.getElementById('ascendants');
                const el2 = document.getElementById(this.item.threadId);
                const top = window.pageYOffset + (el2.getBoundingClientRect().bottom - window.pageYOffset);
                window.scrollTo({
                    top: el2.getBoundingClientRect().y - 110,
                    left: 0,
                    behavior: 'instant'
                });

            })
            this.loading = false;
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
        },
        skipScroll(e) {
            const el = document.getElementById('ascendants');
            const el2 = document.getElementById(this.item.threadId);
            console.log(el.getBoundingClientRect());
            /*
            e.preventDefault();

                const el = document.getElementById('ascendants');
                const el2 = document.getElementById(this.item.threadId);
                const el3 = document.getElementById('lastLine');
                console.log(el);
                console.log(el.getBoundingClientRect());
                console.log(el2);
                console.log(el2.getBoundingClientRect());
                console.log(el3.getBoundingClientRect());
                document.getElementById(this.item.threadId).focus();
                */
                /*
                window.scrollTo({
                    top: el3.getBoundingClientRect().bottom - 75,
                    left: 0,
                    behavior: 'instant',
                });
                */
        },
        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div id="ascendants">
                <template x-for="(item, i) in items" :key="item.id || i" >
                    <div @scroll.window="skipScroll">
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
                        <div class="line-background" :id="i == items.length-1 ? 'lastLine' : null"></div>
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