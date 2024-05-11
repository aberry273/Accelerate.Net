import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'
import wssService from './wssService.js'
import { mxAlert, mxList, mxSearch } from '/src/js/mixins/index.js';
const wssContentPostActionsUpdate = 'wss:post:action';
export default function (settings) {
    return {
        postbackUrl: 'wssContentPosts.postbackUrl',
        queryUrl: 'wssContentPosts.queryUrl',
        actions: [],
        // mixins
        ...mxAlert(settings),
        ...mxList(settings),
        ...mxSearch(settings),
        // inherited
        ...wssService(settings),

        async init() {
            this.postbackUrl = settings.postbackUrl;
            this.queryUrl = settings.queryUrl;
            this.userId = settings.userId;
            await this.initializeWssClient();
            await this.connectUser(settings.userId);

            // On updates from the websocket 
            this._mxEvents_On(this.getMessageEvent(), async (e) => {
                const data = e.data;
                if (!data) return;
                if (data.alert) this._mxAlert_AddAlert(data);
                this.items = this.updateItems(this.items, data);
            })
            // Listen for wssContentPostActionsUpdate
            this._mxEvents_On(wssContentPostActionsUpdate, async (data) => {
                if (!data) return;
                this.actions.push(data);
            })
        },
        // Custom logic
        async Search(filters) {
            let query = this._mxList_GetFilters(filters);
            const postQuery = this._mxSearch_CreateSearchQuery(query, this.userId);
            if (postQuery == null) return;
            const result = await this._mxSearch_Post(this.queryUrl, postQuery);
            this.setItems(result.posts);
            this.actions = result.actions;
        },
        CheckUserPostAction(postId, userId, action) {
            const actions = this.actions.filter(x => x.userId == userId && x.contentPostId == postId);
            if (actions == null || actions.length == 0) return false;
            const result = actions[0];
            return result[action];
        },
    }
}