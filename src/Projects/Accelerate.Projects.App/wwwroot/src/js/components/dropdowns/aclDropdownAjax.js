
export default function (data) {
	return {
        data: null,
        queryText: '',
        searchEvent: 'null',
        event: 'null',
        open: false, 
        results: [],
        selected: '',
        init() {
            this.data = data;
            this.searchEvent = data.searchEvent;
            this.event = data.event;
            this.open = data.open;

            var searchResultEvent = this.searchEvent + ":results";
            this.$events.on(searchResultEvent, (results) => {
                this.results = results;
            })
            const self = this;
            this.$nextTick(() => {
                this.load(self.data)
            })

        },
        search() {
            this.$events.emit(this.searchEvent, this.queryText)
        },
        select(item) {
            this.open = false;
            this.selected = this.item;
            this.queryText = this.item.name;
            this.$events.emit(this.event, this.item)
        }, 
        close() {
            this.open = false;
        },
        load(data) {
        this.$root.innerHTML = `
            <details class="dropdown" :open="open" >
                <summary>Dropdown</summary>
                <ul style="list-style:none;">
                    <article class="padless">
                        <nav class="padless">
                            <ul>
                                <strong>Search</strong>
                            </ul>
                            <ul>
                                <button rel="prev" class="material-icons flat small"  @click="close()">close</button>
                            </ul>
                        </nav>
                         <div >
                            <input
                                :change="search"
                                x-model="queryText"
                                :value="queryText"
                                placeholder="username"
                            />
                        </div>
                        <ul style="list-style:none; text-align:left;" class="padded">
                            <strong>Results</strong>
                            <template x-for="(item) in results">
                                <div>
                                    <a href="#" @click.debounce="select(item)" x-text="item.name"></a>
                                </div>
                            </template>
                        </ul>
                        <ul x-show="results.length == 0">
                            No results found
                        </ul>
                    </article>
                </ul>
            </span>`
      },
      defaults() {
        this.load(defaults)
      }
    }
}