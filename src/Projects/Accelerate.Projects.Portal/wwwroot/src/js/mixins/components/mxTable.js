export default function (params) {
    return {
        // PROPERTIES
        mxTable_class: 'table-fixed',
        mxTable_event: 'on:table',
        mxTable_multiSelect: false,
        mxTable_headers: [],
        mxTable_items: [],
        mxTable_page: 0,
        mxTable_pageSize: 10,
        mxTable_total: 0,
        mxTable_headerTypeText: 'Text',
        mxTable_headerTypeLink: 'Link',
        mxTable_headerTypeNumber: 'Number',
        mxTable_headerTypeDate: 'Date',
        init() {
            this._mxTable_setValues(params);
        },
        // GETTERS  
        get mxTable_itemClass() { return '' },
        get mxTable_pageRangeText() { return `${this.mxTable_page + 1} - ${this.mxTable_pageSize}`; },
        get mxTable_totalResultsText() { return this.mxTable_total },
        // METHODS
        _mxTable_setValues(params) {
            if (!params) return;
            this.mxTable_event = params.event || this.mxTable_event;
            this.mxTable_headers = params.headers || [];
            this.mxTable_items = params.items || [];
            this.mxTable_class = params.class || '';
            this.mxTable_multiSelect = params.class || false;
            this.mxTable_page = params.page || 0;
            this.mxTable_pageSize = params.pageSize || 10;
            this.mxTable_pages = params.pages || 0;
        }
    }
}