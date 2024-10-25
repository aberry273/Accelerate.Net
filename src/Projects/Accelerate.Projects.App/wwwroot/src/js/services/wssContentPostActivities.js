import wssService from './wssService.js'
import {mxAlert, mxFetch, mxList, mxSearch } from '/src/js/mixins/index.js';
const wssContentPostActivityUpdate = 'wss:post:activity';
export default function (settings) {
    return {
        // properties
        postbackUrl: 'wssContentPostActivities.postbackUrl',
        queryUrl: 'wssContentPostActivities.queryUrl',
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
                this._mxAlert_AddAlert(data);
            })
        },
    }
}