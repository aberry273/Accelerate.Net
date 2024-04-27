import { emit } from './utilities.js'
const wssEvent = 'wss:contentPostReviews';

export default function (settings) {
    return {
        socket: null,
        settings: {},
        client: null,
        connectionId: null,
        connectedEvent: 'connected',
        messageEvent: 'onmessage',
        async init(settings) {
            this.settings = settings;
            const self = this;
            this.client = new signalR.HubConnectionBuilder()
                .withUrl(this.settings.url)
                .configureLogging(signalR.LogLevel.Information)
                .build();

            this.client.onclose(async () => {
                await this.client.start();
            });

            // Start the connection.
            try {
                await this.client.start();
                this.connectionId = this.client.connection.connectionId;
                console.log('init')
                emit(this.settings.event, this.connectedEvent, this.connectionId);

                this.client.on("sendMessage", (user, message) => {
                    const received = `${user}: ${message}`;
                    console.log(received)
                    // Handle responses
                    const payload = {
                        user: user,
                        // {code, type, data}
                        data: message
                    }
                    emit(wssEvent, this.messageEvent, payload);

                });
            } catch (err) {
                console.log(err);
                //setTimeout(this.start, 5000);
            }
        },
        getMessageEvent() {
            return `${this.settings.event}:${this.messageEvent}`;
        },
        getConnectedEvent() {
            return `${this.settings.event}:${this.connectedEvent}`;
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
        /*
        async connectChannel(channelId) {
            await this.connectChannel(this.connectionId, this.user.id, channelId)
        },
        async connectThread(threadId) {
            await this.connectThread(this.connectionId, this.user.id, threadId)
        }
        */
    }
}