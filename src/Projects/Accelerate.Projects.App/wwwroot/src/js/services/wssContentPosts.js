import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'
import wssService from './wssService.js'
import { mxAlert, mxList, mxSearch } from '/src/js/mixins/index.js';
const wssContentPostActionsUpdate = 'wss:post:action';
const wssContentPostActionsSummaryUpdate = 'wss:post:actionSummary';
const quoteEvent = 'action:post:quote';
export default function (settings) {
    return {
        postbackUrl: 'wssContentPosts.postbackUrl',
        queryUrl: 'wssContentPosts.queryUrl',
        actions: [],
        actionSummaries: [],
        quotedPosts: [],
        filters: {},
        //cachedFilters: {},
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

            // On update post from the websocket 
            this._mxEvents_On(this.getMessageEvent(), async (e) => {
                const msgData = e.data;
                if (!msgData) return;

                //this.items = this.updateItems(this.items, msgData);
            })
            // Listen for wssContentPostActionsUpdate
            this._mxEvents_On(wssContentPostActionsUpdate, async (data) => {
                if (!data) return;
                this.actions = this.updateItems(this.actions, data);
            })
            // Listen for wssContentPostActionsSummaryUpdate
            this._mxEvents_On(wssContentPostActionsSummaryUpdate, async (data) => {
                if (!data) return;
                this.actionSummaries = this.updateItems(this.actionSummaries, data);
            })
            // Listen of post quoting
            // Not used right now - to be used as state for editForm t
            /*
            this._mxEvents_On(quoteEvent, async (item) => {
                const threadKey = item.threadId;
                const index = this.quotes.indexOf(threadKey);
                if (index > -1) return;
                if (index == -1) {
                    this.quotes.push(threadKey)
                }  
            })
            */
        },
        // Custom logic
        async SearchPosts(filterUpdate, searchUrl) {
            this.filters = filterUpdate;
            const filters = filterUpdate != null ? filterUpdate.filters : [];
            let query = this._mxSearch_GetFilters(filters || []);
            const postQuery = this._mxSearch_CreateSearchQuery(query, this.userId);
            postQuery.sort = filterUpdate.sort;
            postQuery.sortBy = filterUpdate.sortBy;
            if (postQuery == null) return;
            return await this._mxSearch_Post(searchUrl || this.queryUrl, postQuery);
        },
        // Custom logic
        /*
        async Filter(filters) {
            const key = this.CreateFilterKey(filters);
            const result = await this.SearchPosts(filters)
            this.cachedFilters[key] = result;                        

            this.items = this.insertOrUpdateItems(this.items, result.posts);
            this.actions = this.insertOrUpdateItems(this.actions, result.actions);
        },
        */
        CreateFilterKey(filters) {
            let query = this._mxSearch_GetFilters(filters);
            const postQuery = this._mxSearch_CreateSearchQuery(query, this.userId);
            return JSON.stringify(postQuery);
        },
        // Searches for posts, items, quotes, summaries, only returns posts
        async SearchByUrl(searchUrl, filters, replace = false) {
            const result = await this.SearchPosts(filters, searchUrl)
            // cache in $store
            this.setSearchResults(result, replace);
            return result;
        },
        // Searches for posts, items, quotes, summaries, only returns posts
        async Search(filters, replace = false) {
            const result = await this.SearchPosts(filters)
            // cache in $store
            this.setSearchResults(result, replace);
            return result;
        },
        setSearchResults(result, replace = false) {
            if (replace) {
                this.items = result.posts;
                this.quotedPosts = result.quotedPosts;
                this.actions = result.actions;
                this.actionSummaries = result.actionSummaries;
            }
            else {
                this.items = this.insertOrUpdateItems(this.items, result.posts);
                this.quotedPosts = this.insertOrUpdateItems(this.items, result.quotedPosts);
                this.actions = this.insertOrUpdateItems(this.actions, result.actions);
                this.actionSummaries = this.insertOrUpdateItems(this.actionSummaries, result.actionSummaries);
            }
        },
        GetActionSummary(postId) {
            const summaries = this.actionSummaries.filter(x => x.contentPostId == postId);
            if (summaries == null || summaries.length == 0) return null;
            return summaries[0];
        },
        GetPostAction(postId, userId) {
            const actions = this.actions.filter(x => x.userId == userId && x.contentPostId == postId);
            if (actions == null || actions.length == 0) return null;
            return actions[0];
        },
        GetQuotePost(quotedPostId) {
            return this.quotedPosts.filter(x => x.id == quotedPostId)[0];
        },
        FilterPostsById(postIds) {
            return this.items.filter(x => postIds.indexOf(x.id) > -1);
        },
        CheckUserPostAction(postId, userId, actionType) {
            const action = this.GetPostAction(postId, userId);
            if (action == null) return false;
            return action[actionType];
            return action[actionType] === true;
        },
    }
}