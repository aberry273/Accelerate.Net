import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'
import wssService from './wssService.js'
import { mxAlert, mxList, mxSearch } from '/src/js/mixins/index.js';

export default function (settings) {
    return {
        postbackUrl: 'wssContentPosts.postbackUrl',
        queryUrl: 'wssContentPosts.queryUrl',
        // mixins
        ...mxAlert(settings),
        ...mxList(settings),
        ...mxSearch(settings),
        // inherited
        ...wssService(settings),

        async init() {
            this.postbackUrl = settings.postbackUrl;
            this.queryUrl = settings.queryUrl;
            await this.initializeWssClient();
            await this.connectUser(settings.userId);

            // On updates from the websocket 
            this._mxEvents_On(this.getMessageEvent(), async (e) => {
                const data = e.data;
                if (!data) return;
                if (data.alert) this._mxAlert_AddAlert(data);
                this.updateItems(data);
            })
        },
        // Custom logic
        async Search(filters) {
            let query = this._mxList_GetFilters(filters);
            const postQuery = this._mxSearch_CreateSearchQuery(query);
            if (postQuery == null) return;
            const items = await this._mxSearch_Post(this.queryUrl, postQuery);
            this.setItems(items);
        },
    }
}