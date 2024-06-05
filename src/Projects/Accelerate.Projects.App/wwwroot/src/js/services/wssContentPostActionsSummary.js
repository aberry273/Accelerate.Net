import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'
import wssService from './wssService.js'
import {mxAlert, mxFetch, mxList, mxSearch } from '/src/js/mixins/index.js';
const wssContentPostActionsSummaryUpdate = 'wss:post:actionSummary';
export default function (settings) {
    return {
        // properties
        postbackUrl: 'wssContentPostActionsSummary.postbackUrl',
        queryUrl: 'wssContentPostActionsSummary.queryUrl',
        // mixins
        ...mxFetch(settings),
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

                this._mxEvents_Emit(wssContentPostActionsSummaryUpdate, data);
                // N ow use the contentService to hold actions
                //this.items = this.updateItems(this.items, data);
            })
        },
        /*
        // Custom logic
        async _wssContentActionsSummary_HandlePost(payload) {
            const url = `${this.postbackUrl}`
            const result = await this._mxFetch_Post(url, payload);
            // if successful, push into the array for immediate response
            // websocket will update it accordingly to remove if failure
            // or with proper data if successful
            if (result.status >= 200 < 300) {
                payload.animate = true;
                this._mxEvents_Emit(wssContentPostActionsSummaryUpdate, payload);
            }
        },
        */
    }
}