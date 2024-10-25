import { mxEvents } from '/src/js/mixins/index.js';

export default function header(data) {
    return {
        ...mxEvents(data),
        showDialog: false,
        queryText: null,
        results: [],
        topResults: [],
        selectIcon: false,
        icon: 'add_reaction',
        selectedIcon: '',
        event: 'select:emoji',
        currentPage: 0,
        perPage: 24,
        init() {
            this.results = _emojiJson.slice(0, 24);
            this.selectIcon = data.selectIcon;
            this.selectedIcon = data.selectedIcon;
            this.event = data.event || 'select:emoji';
            this.topResults = [
                {
                    char: '👍',
                    codes: '1F44D',
                },
                {
                    char: '👎',
                    codes: '1F44E',
                },
                {
                    char: '❤️',
                    codes: '2764 FE0F'
                },
                {
                    char: '🤣',
                    codes: '1F923'
                },
                {
                    char: '😂',
                    codes: '1F602'
                },
                {
                    char: '☹️',
                    codes: '1F923'
                }
            ];
            this.$nextTick(() => {
                this.load(self.data)
            })
        },
        get page() {
            return this.currentPage + 1
        },
        get pages() {
            return parseInt(_emojiJson.length / this.perPage);
        },
        get pageText() {
            return `${this.page} / ${this.pages}`;
        },
        search() {
            console.log(this.queryText)
            const results = this.queryText != null
                ? _emojiJson.filter(x => x.name.indexOf(this.queryText) > -1)
                : _emojiJson;

            const max = results.length >= this.perPage ? this.perPage : results.length;
            const pageIndex = this.currentPage * this.perPage;
            this.results = results.slice(pageIndex, pageIndex+max)
        },
        clear() {
            this.$events.emit(this.event, '');
            this.selectedIcon = null;
            this.showDialog = false;
        },
        select(emoji) {
            this.$events.emit(this.event, emoji);
            this.selectedIcon = emoji.char;
            this.showDialog = false;
        },
        lastPage() {
            if (this.currentPage == 0) return;
            this.currentPage--;
            this.search();
        },
        nextPage() {
            if (this.currentPage >= this.pages) return;
            this.currentPage++;
            this.search();
        },
        load(data) {
            this.$root.innerHTML = `
            <button @click="showDialog = !showDialog" class="flat small" style="padding:0px; padding-top:8px;">
               <i x-show="!selectIcon || !selectedIcon" class="material-icons" style="margin-top:4px;" x-text="icon"></i>
               <span x-show="selectIcon && selectedIcon" class="" x-html="selectedIcon"></span>
            </button>
            <template x-if="showDialog"  @click.outside="showDialog = false">
                <div class="dropdown menu">
                
                    <article class="dropdownMenu padless">
                        <header class="padless py">
                            <input
                                rel="icon"
                                id="searchInput"
                                style="margin-bottom: 0px"
                                :change="search"
                                :value="queryText"
                                placeholder="Search"
                            /> 
                        </header>
                        <!--Users Format-->
                        <div>
                            <!--User Search-->
                            <ul style="display: grid;list-style:none; text-align:left; " >
                                <li x-show="selectIcon && selectedIcon">
                                    <button rel="clearselected" @click="clear" class="flat small"><sup>Clear selected</sup></button>
                                </li>
                                <li>
                                    <sup>Favourites</sup>
                                </li>
                                 <div class="grid col-6">
                                    <template x-for="(item) in topResults">
                                        <span class="click" x-html="item.char" @click="select(item)"></span>
                                    </template>
                                </div>
                                <li><sup>Results</sup></li>
                                <div class="grid col-6">
                                    <template x-for="(item) in results">
                                        <span class="click emoji" :data-tooltip="item.name" x-html="item.char" @click="select(item)"></span>
                                    </template>
                                </div>
                                <li>
                                    <nav>
                                    <button :disabled="page > 1" class="xsmall flat material-icons click" @click="lastPage">chevron_left</button>
                                    <small x-text="pageText"></small>
                                    <button :disabled="page >= pages" class="xsmall flat material-icons click" @click="nextPage">chevron_right</button>
                                    </nav>
                                </li>
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