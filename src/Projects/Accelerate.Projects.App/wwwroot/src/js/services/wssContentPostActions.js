import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'
import wssService from './wssService.js'
import {mxAlert, mxFetch } from '/src/js/mixins/index.js';

export default function (settings) {
    return {
        // properties
        postbackUrl: 'wssContentServicePostActions.postbackUrl',
        // mixins
        ...mxFetch(settings),
        ...mxAlert(settings),
        // inherited
        ...wssService(settings),

        async init() {
            this.postbackUrl = settings.postbackUrl;
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
        async _wssContentActions_HandlePost(payload) {
            const url = `${this.postbackUrl}`
            const result = await this._mxFetch_Post(url, payload);
            // if successful, push into the array for immediate response
            // websocket will update it accordingly to remove if failure
            // or with proper data if successful
            if (result.status >= 200 < 300) {
                this.items.push(payload);
            }
        },
        _wssContentActions_CheckUserAction(postId, userId, action) {
            const actions = this.items.filter(x => x.userId == userId && x.contentPostId == postId);
            if (actions == null || actions.length == 0) return false;
            const result = actions[0];
            return result[action];
        },
    }
}