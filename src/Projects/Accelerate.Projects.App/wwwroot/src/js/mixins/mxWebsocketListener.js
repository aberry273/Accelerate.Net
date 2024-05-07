export default function (data){
    return {
        // PROPERTIES
        mxWebsocketListener_Wss: null,
        userId: null,
        targetChannel: null,
        targetThread: null,
        items: [],
        // GETTERS

        async init() {
            this.mxWebsocketListener_Wss = data.wss;
            this.userId = data.userId;
            this.targetChannel = data.targetChannel;
            this.targetThread = data.targetThread;
            this.items = data.items || [];
            // Setup websocket listeners
            await this._mxWebsocketListener_InitWebsocketEvents(
                this.mxWebsocketListener_Wss,
                this.userId,
                this.targetChannel,
                this.targetThread,
            )
            this.$events.on(this.mxWebsocketListener_Wss.getMessageEvent(), async (e) => {
                const data = e.data;
                if (!data) return;
                this._mxWebsocketListener_UpdateItemUpdate(data);
            })
        },
        
        // METHODS
        async _mxWebsocketListener_InitWebsocketEvents(wssService, userId, targetChannel, targetThread) {
            this.userId = userId;

            // On websocket client initialized, send channel to server
            this.$events.on(wssService.getConnectedEvent(), async (ev) => {
                await wssService.connectUser(userId);

                if (targetThread) {
                    await wssService.connectThread(userId, targetThread);
                }
                if (targetChannel) {
                    await wssService.connectChannel(userId, targetChannel);
                }
            })
        },
        _mxWebsocketListener_UpdateItemUpdate(wssMessage) {
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
                this.$events.emit(item.id, item);
            }
            if (wssMessage.update == 'Deleted') {
                const index = this.items.map(x => x.id).indexOf(item.id);
                this.items.splice(index, 1);
            }
        },
    }
}