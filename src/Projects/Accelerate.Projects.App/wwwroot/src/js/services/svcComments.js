import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'

import { mxEvent, mxFetch, mxService, mxToast } from '/src/js/mixins/index.js';

export default function (settings) {
    return {
        ...mxEvent(settings),
        ...mxFetch(settings),
        ...mxService(settings),
        ...mxToast(settings),
        // PROPERTIES
        svcComments_eventCreate: 'on:comment:create',
        svcComments_eventUpdate: 'on:comment:update',
        svcComments_eventDelete: 'on:comment:delete',
        svcComments_eventGet: 'on:comment:get',
        svcComments_eventCopyLink: 'on:comment:copylink',
        svcComments_eventQuote: 'on:comment:quote',
        svcComments_eventUpvote: 'on:upvote',
        svcComments_eventDownvote: 'on:comment:downvote',
        svcComments_eventDelete: 'on:comment:delete',
        svcComments_eventFlag: 'on:comment:flag',
        self: null,
        async init() {
            this.self = this;
            this._mxEvent_On(this.svcComments_eventCopyLink, (item) => {
                this._svcComments_copyLink(item, this);
                this._svcComments_sendAlert('copy', 'default', '')
            })
            this._mxEvent_On(this.svcComments_eventQuote, (item) => {
                this._svcComments_quote(item, this);
                this._svcComments_sendAlert('quote', 'default', '')
            })
            this._mxEvent_On(this.svcComments_eventUpvote, (item) => {
                this._svcComments_upvote(item, this);
                this._svcComments_sendAlert('upvote', 'default', '')
            })
            this._mxEvent_On(this.svcComments_eventDownvote, (item) => {
                this._svcComments_downvote(item, this);
                this._svcComments_sendAlert('downvote', 'default', '')
            })
            this._mxEvent_On(this.svcComments_eventFlag, (item) => {
                this._svcComments_flag(item, this);
                this._svcComments_sendAlert('flag', 'default', '')
            })
            this._mxEvent_On(this.svcComments_eventDelete, (item) => {
                this._svcComments_delete(item, this);
                this._svcComments_sendAlert('delete', 'default', '')
            })
        },
        // GETTERS  
        // METHODS
        async _svcComments_create(postbackUrl, comment) {
            await this._mxFetch_Post(
                postbackUrl, 
                filters
            );
        },
        async _svcComments_fetch(postbackUrl, filters) {
            await this._mxFetch_Post(
                postbackUrl, 
                filters
            );
        },
        _svcComments_setEvents(item, cb) {
            if (!this.mxEvent_event) return;
            const self = this;
            this.upvote(item, this);
            this.sendAlert('upvote', 'default', '')
        },
        _svcComments_sendAlert(title, type, description) {
            this._mxEvent_Emit(
                this.mxToast_event, 
                {
                    title: title,
                    type: type,
                    description: description
                }
            );
        },
        _svcComments_upvote(item) {
            console.log(item); 
        },
        _svcComments_downvote(item) {
            console.log(item);
        },
        _svcComments_quote(item) {
            console.log(item);
        },
        _svcComments_copyLink(item) {
            console.log(item);
        },
        _svcComments_flag(item) {
            console.log(item);
        },
        _svcComments_delete(item) {
            console.log(item);
        },
    }
}