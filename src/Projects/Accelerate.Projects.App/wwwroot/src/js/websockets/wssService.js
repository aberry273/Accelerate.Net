import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'

export default function (settings) {
    return {
        socket: null,
        settings: {},
        client: null,
        connectionId: null,
        wssEvent: null,
        async init() {
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
        getMessageEvent() {
            return `${this.wssEvent}:${messageEvent}`;
        },
        getConnectedEvent() {
            return `${this.wssEvent}:${connectedEvent}`;
        },
        async connectUser(userId) {
            await this.client.invoke("UserRequest", this.connectionId, userId)
        },
        async connectChannel(userId, channelId) {
            await this.client.invoke("ChannelRequest", this.connectionId, userId, channelId)
        },
        async connectThread(userId, threadId) {
            await this.client.invoke("ThreadRequest", this.connectionId, userId, threadId)
        },
    }
}