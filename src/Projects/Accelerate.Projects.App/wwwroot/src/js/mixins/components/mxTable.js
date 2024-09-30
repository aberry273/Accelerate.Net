export default function (params) {
    return {
        // PROPERTIES
        mxTable_class: 'table-fixed',
        mxTable_event: 'on:table',
        mxTable_headers: [],
        mxTable_items: [],
        init() {
            this._mxTable_setValues(params);
        },
        // GETTERS  
        get mxTable_itemClass() { return '' },
        // METHODS
        _mxTable_setValues(params) {
            if (!params) return;
            this.mxTable_event = params.event || this.mxTable_event;
            this.mxTable_headers = params.headers || [];
            this.mxTable_items = params.items || [];
            this.mxTable_class = params.class || '';
        }
    }
}