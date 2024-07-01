import { mxEvents } from '/src/js/mixins/index.js';

export default function header(data) {
    return {
        ...mxEvents(data),
        showDialog: false,
        queryText: null,
        results: [],
        topResults: [],
        event: 'select:emoji',
        init() {
            this.results = _emojiJson.slice(0, 24);
            this.event = data.event || 'select:emoji';
            this.topResults = [
                {
                    char: '👍',
                },
                {
                    char: '👎',
                },
                {
                    char: '❤️',
                },
                {
                    char: '☺️',
                },
                {
                    char: '😂',
                },
                {
                    char: '☹️',
                }
            ];
            this.$nextTick(() => {
                this.load(self.data)
            })
        },
        search() {
            if (!this.queryText) return;
            const results = _emojiJson.filter(x => x.name.indexOf(this.queryText) > -1);
            const max = results.length >= 24 ? 24 : results.length;
            this.results = results.slice(0, max)
        },
        select(emoji) {
            this.$events.emit(this.event, emoji);
            this.showDialog = false;
        },
        load(data) {
        this.$root.innerHTML = `
            <button @click="showDialog = !showDialog" class="material-icons flat small">add_reaction</button>
            <template x-if="showDialog">
                <div class="dropdown menu">
               
                    <article class="dropdownMenu padless">
                        <header class="padless py">
                            <input
                                id="userInput"
                                style="margin-bottom: 0px"
                                :change="search"
                                x-model="queryText"
                                :value="queryText"
                                placeholder="Search"
                            /> 
                        </header>
                        <!--Users Format-->
                        <div>
                            <!--User Search-->
                            <ul style="display: grid;list-style:none; text-align:left; " >
                                <li>
                                    <sup>Top results</sup>
                                </li>
                                 <div class="grid col-6">
                                    <template x-for="(item) in topResults">
                                        <span class="click" x-html="item.char" @click="select(item)"></span>
                                    </template>
                                </div>
                                <li><sup>Results</sup></li>
                                <div class="grid col-6">
                                    <template x-for="(item) in results">
                                        <span class="click" x-html="item.char" @click="select(item)"></span>
                                    </template>
                                </div>
                                <li x-show="results.length == 0">
                                    <sup>No results found</sup>
                                </li>
                            </ul>
                        </div>
                    </article>
                </div>
            </template>
        `;
      }
    };
  }