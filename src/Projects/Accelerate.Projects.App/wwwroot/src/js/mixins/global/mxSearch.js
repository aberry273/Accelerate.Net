import { mxFetch } from '/src/js/mixins/index.js';
export default function (data) {
    return {
        ...mxFetch(),
        init() {
            this.$watch('mxSearch_Open', () => { })
        },
        // PROPERTIES
        fetchUrl: '/',
        mxSearch_Open: false,
        
        // GETTERS
        get mxModal_GetOpen() { return this.mxSearch_Open },
        
        // METHODS

        _mxSearch_GetFilters(filters) {
            let query = { ...this.presetFilters }
            const filterKeys = Object.keys(filters);
            for (var i = 0; i < filterKeys.length; i++) {
                const key = filterKeys[i];
                query[key] = filters[key];
            }
            return query;
        },
        /// Converts the data object into a list of filters
        _mxSearch_CreateSearchQuery(data, userId, page = 0, itemsPerPage = 1) {
            if (!data) return;
            let filters = [];
        
            const keys = Object.keys(data);
            for(let i = 0; i < keys.length; i++) {
                const key = keys[i];
                // if empty array, skip
                if (Array.isArray(data[key]) && data[key].length == 0) continue;

                const filter = {
                    name: key,
                    //Equals, NotEquals, Null, NotNull, GreaterThan, LessThan
                    Operator: 'Equals',
                    Condition: 'Filter',
                }
                if (Array.isArray(data[key])) {
                    filter.Values = data[key];
                }
                else {
                    filter.Value = data[key];
                }
                filters.push(filter)
            }
            let payload = {
                query: null,
                userId: userId,
                page: page,
                itemsPerPage: itemsPerPage,
                filters: filters,
            }
            return payload;
        },

        async _mxSearch_Post(fetchUrl, searchQuery) {
            if (!fetchUrl || !searchQuery) throw new DOMException("no url or data pased into _mxSearch_Post");
            return await this._mxFetch_Post(fetchUrl, searchQuery);
        },
    }
}