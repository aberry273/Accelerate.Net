import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'

import { mxEvents } from '/src/js/mixins/index.js';

export default function (settings) {
    return {
        ...mxEvents(settings),
        items: [],
        socket: null,
        settings: {},
        client: null,
        connectionId: null,
        wssEvent: null,
        async init() {
            await this.initializeWssClient()
        },
        async initializeWssClient() {
            this.settings = settings;
            this.wssEvent = settings.wssEvent;
            const self = this;
            // Start the connection.
            try {
                this.client = await createClient(this.settings.url, this.wssEvent)
                await this.client.start();
                this.connectionId = this.client.connection.connectionId;
                emit(this.wssEvent, connectedEvent, this.client.connection.connectionId);
            } catch (err) {
                console.error(err);
                //setTimeout(createClient, 5000);
            }
        },
        setItems(items) {
            this.items = items;
        },
        updateItems(wssMessage) {
            var item = wssMessage.data;
            let emptyItems = false;
            if (this.items == null) {
                this.items = [];
                emptyItems = true;
            }
            if (wssMessage.update == 'Created') {
                const index = this.items.map(x => x.id).indexOf(item.id);
                if (index == -1) this.items.push(item);
                else this.items[index] = item
            }
            if (wssMessage.update == 'Updated') {
                const index = this.items.map(x => x.id).indexOf(item.id);
                this.items[index] = item
                this._mxEvents_Emit(item.id, item);
            }
            if (wssMessage.update == 'Deleted') {
                const index = this.items.map(x => x.id).indexOf(item.id);
                this.items.splice(index, 1);
            }
        },
        getMessageEvent() {
            return `${this.wssEvent}:${messageEvent}`;
        },
        getConnectedEvent() {
            return `${this.wssEvent}:${connectedEvent}`;
        },
        async connectUser(userId) {
            await this.client.invoke("UserRequest", this.connectionId, userId)
        },
        // Custom logic
    }
}