export default function (data){
    return {
        // PROPERTIES
        actionUrl: '/',
        mxSearch_Open: false,
        
        // GETTERS
        get mxModal_GetOpen() { return this.mxSearch_Open },

        init() {
            this.$watch('mxSearch_Open', () => { })
            this.actionUrl = data.actionUrl;
        },
        
        // METHODS
        _mxAction_Init() {
            this.init();
        },
        async _mxAction_HandleActionPost(payload) {
            const url = `${data.actionUrl}`
            await this.$fetch.POST(url, payload);
        },
        async _mxAction_HandleActionPut(payload) {
            const url = `${data.actionUrl}`
            await this.$fetch.PUT(url, payload);
        },
        async _mxAction_HandleActionDelete(payload) {
            const url = `${data.actionUrl}`
            await this.$fetch.DELETE(url, payload);
        },
    }
}