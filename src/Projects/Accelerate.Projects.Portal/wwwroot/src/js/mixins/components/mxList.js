export default function (params) {
    return {
        // PROPERTIES
        mxList_items: [],
        init() {
            this.mxList_items = params.mxList_items;
        },
        // GETTERS  
        get mxList_itemClass() { return '' },
        // METHODS
    }
}