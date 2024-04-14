import { emit, createClient, connectedEvent, messageEvent } from './utilities.js'
const wssEvent = 'wss:contentPostReviews';

export default function (settings) {
    return {
        socket: null,
        settings: {},
        client: null,
        connectionId: null,
        async init() {
            this.settings = settings;
            const self = this;
            // Start the connection.
            try {
                this.client = await createClient(this.settings.url, wssEvent)
                await this.client.start();
                this.connectionId = this.client.connection.connectionId;
                emit(wssEvent, connectedEvent, this.client.connection.connectionId);
            } catch (err) {
                console.error(err);
                //setTimeout(createClient, 5000);
            }
        },
        getMessageEvent() {
            return `${wssEvent}:${messageEvent}`;
        },
        getConnectedEvent() {
            return `${wssEvent}:${connectedEvent}`;
        },
        async connectUser(userId) {
            console.log('connectUser ' + userId)
            await this.client.invoke("UserRequest", this.connectionId, userId)
        },
        async connectChannel(userId, channelId) {
            console.log('connectChannel ' + channelId)
            await this.client.invoke("ChannelRequest", this.connectionId, userId, channelId)
        },
        async connectThread(userId, threadId) {
            console.log('connectThread ' + threadId)
            await this.client.invoke("ThreadRequest", this.connectionId, userId, threadId)
        },
    }
}