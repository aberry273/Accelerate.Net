import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'
import wssService from './wssService.js'
import { mxAlert, mxList, mxSearch } from '/src/js/mixins/index.js';
const wssContentPostActionsUpdate = 'wss:post:action';
const quoteEvent = 'action:post:quote';
export default function (settings) {
    return {
        postbackUrl: 'wssContentPosts.postbackUrl',
        queryUrl: 'wssContentPosts.queryUrl',
        actions: [],
        quotes: [],
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
                this.actions = this.updateItems(this.actions, data);
            })
            // Listen of post quoting
            this._mxEvents_On(quoteEvent, async (item) => {
                const threadKey = item.threadId;
                const index = this.quotes.indexOf(threadKey);
                if (index > -1) return;
                if (index == -1) {
                    this.quotes.push(threadKey)
                }  
            })

        },
        // Custom logic
        async SearchPosts(filters) {
            let query = this._mxList_GetFilters(filters);
            const postQuery = this._mxSearch_CreateSearchQuery(query, this.userId);
            if (postQuery == null) return;
            return await this._mxSearch_Post(this.queryUrl, postQuery);
        },
        // Custom logic
        async Search(filters) {
            const result = await this.SearchPosts(filters)
            this.setItems(result.posts);
            this.actions = result.actions;
        },
        GetPostAction(postId, userId) {
            const actions = this.actions.filter(x => x.userId == userId && x.contentPostId == postId);
            if (actions == null || actions.length == 0) return null;
            return actions[0];
        },
        CheckUserPostAction(postId, userId, actionType) {
            const action = this.GetPostAction(postId, userId);
            if (action == null) return false;
            return action[actionType] === true;
        },
    }
}