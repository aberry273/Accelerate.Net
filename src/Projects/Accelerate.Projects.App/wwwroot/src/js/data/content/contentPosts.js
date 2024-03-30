export default function (data) {
    return {
        items: [],
        websocketEvent: 'wss:contentPosts:onmessage',
        async init() {
            this.items = data != null ? data.items : [];
            await this.setupWebsockets();
            const query = this.createQuery(data);
            await this.fetchItems(query);
        },
        async setupWebsockets() {
            // On websocket client initialized, send channel to server
            this.$events.on(this.$store.wssContentPosts.getConnectedEvent(), async (ev) => {
                await this.$store.wssContentPosts.connectUser(this.userId);

                if (this.targetThread) {
                    await this.$store.wssContentPosts.connectThread(this.userId, this.targetThread);
                }
                if (this.targetChannel) {
                    await this.$store.wssContentPosts.connectChannel(this.userId, this.targetChannel);
                }
            })
            // On updates from the websocket
            this.$events.on(this.$store.wssContentPosts.getMessageEvent(), async (e) => {
                const data = e.data;
                this.sendAlert(data);
                this.updateItems(data);
            })
        },
        createQuery(data) {
            const query = {};
            const filters = {};
            if (data.targetThread) {
                query.targetThread = data.targetThread;
                filters['targetThread'] = [data.targetThread];
            }
            let payload = {
                page: 0,
                itemsPerPage: 10,
                query: query,
                filters: filters,
            }
            return payload;
        },
        updateItems(wssMessage) {
            const item = wssMessage.data;
            if (wssMessage.update == 'Created')
                this.items.push(item);
            if (wssMessage.update == 'Updated') {
                const index = this.items.map(x => x.id).indexOf(item.id);
                this.items[index] = item
            }
            if (wssMessage.update == 'Deleted') {
                const index = this.items.map(x => x.id).indexOf(item.id);
                this.items.splice(index, 1);
            }
        },
        get posts() { return this.items },
        sendAlert(data) {
            const snackbarType = (data.code == 200) ? 'success' : 'error';
            const wasSuccess = (data.code == 200) ? 'successfully' : 'failed';
            const message = `${data.update} post ${wasSuccess}`
            const event = `snackbar-${snackbarType}`;
            this.$events.emit(event, { code: data.code, text: message });
        },
        setItems(items) {
            this.items = items
        },
        async fetchItems(query) {
            const results = await this.$fetch.POST(this.searchUrl, query);
            this.items = results;
        },
    }
}