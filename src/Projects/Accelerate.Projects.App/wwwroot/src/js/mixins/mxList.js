export default function (data){
    return {
        init() { 
            this.filterEvent = data.filterEvent || 'filters:posts';
            this.presetFilters = data.filters;

            this.$watch('open', () => { })
        },
        // PROPERTIES
        presetFilters: {},
        filterEvent: null,
        fetchUrl: '/',
        // GETTERS
        get mxModal_Test() { return true },
        // METHODS 
    }
}