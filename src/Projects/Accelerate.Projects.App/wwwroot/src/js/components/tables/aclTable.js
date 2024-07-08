import { aclTableFieldLink, aclTableFieldText, aclTableFieldDate } from './fields/index.js'
import { mxDate } from '/src/js/mixins/index.js';

const dummyData = {
    headers: [
        {
            label: 'Header 1',
            name: 'Header1'
        },
        {
            label: 'Header 2',
            name: 'Header2'
        },
    ],
    rows: [
        {
            value: '1234',
        },
        {
            value: '2345',
        }
    ]
}
export default function (data) {
    return {
        ...mxDate(data),
        data: null,
        results: [],
        event: 'acl:table',
        selected: '',
        itemsPerPage: 10,
        pages: 0,
        currentPage: 0,
        postbackUrl: "/",
        table: {
            headers: [],
            rows: []
        },
        results: [],
        init() {
            this.data = data;
            this.searchEvent = data.searchEvent;
            this.event = data.event;
            this.table = data.table || {
                headers: [],
                rows: []
            };
            this.results = this.table.rows;

            this.itemsPerPage = data.table.itemsPerPage;
            this.pages = data.table.pages;
            this.currentPage = data.table.currentPage;
            this.postbackUrl = data.postbackUrl;

            this.$events.on(this.event, (results) => {
                this.results = results;
            })
            const self = this;
            this.$nextTick(() => {
                this.load(self.data)
            })

        },
        get pageRange() {
            return this.pages + 1
            const total = this.currentPage + 5;
            if (total > this.pages) return this.pages - this.currentPage;
            return total;
        },
        get headerKeys() {
            if (this.table == null || this.table.headers == null) return [];
            return this.table.headers.map(x => x.name);
        },
        search() {
            this.$events.emit(this.searchEvent, this.queryText)
        },
        select(item) {
        }, 
        close() {
            this.open = false;
        },
        async selectPrevious() {
            this.currentPage--;
            const payload = {
                itemsPerPage: this.itemsPerPage,
                currentPage: this.currentPage,
            }
            this.results = await this.$fetch.POST(this.postbackUrl, payload);
        },
        async selectNext() {
            this.currentPage++;
            const payload = {
                itemsPerPage: this.itemsPerPage,
                currentPage: this.currentPage,
            }
            this.results = await this.$fetch.POST(this.postbackUrl, payload);
        },
        async selectPage(page) {
            this.currentPage = page-1;
            const payload = {
                itemsPerPage: this.itemsPerPage,
                currentPage: this.currentPage,
            }
            this.results = await this.$fetch.POST(this.postbackUrl, payload);
        },
        getHeader(headerKey) {
            return this.table.headers.filter(x => x.name == headerKey)[0]
        },
        renderField(headerKey, value) {
            var header = this.getHeader(headerKey);
            if (header == null) return aclTableFieldText(value);
            if (header.headerType == 'Link') return aclTableFieldLink(value)
            if (header.headerType == 'Date') return aclTableFieldDate(value)
            return aclTableFieldText(value)
        },
        load(data) {
        this.$root.innerHTML = `
            <div class="overflow-auto">
              <table>
                <thead>
                    <tr>
                        <template x-for="header in table.headers">
                            <th x-text="header.label || header.name"></th>
                        </template>
                    </tr>
                </thead>
                <tbody>
                    <template x-for="row in results">
                        <tr>
                            <template x-for="key in headerKeys">
                                <td>
                                    <span x-show="row[key]" x-html="renderField(key, row[key])"></span>
                                </td>
                            </template>
                        </tr>
                    </template>
                </tbody>
              </table>
                <!--Pagination-->
                <nav>
                    <button aria-label="Previous"
                        :disabled="currentPage == 0"
                        @click="selectPrevious"
                        class="round flat xsmall primary material-icons"
                        rel="next">chevron_left</button>

                    <template x-for="i in pageRange">
                        <button
                            @click="selectPage(i)"
                            class="round flat xsmall"
                            :class="(currentPage+1) == i ? 'primary' : ''"
                            x-text="i"></button>
                    </template>

                    <button aria-label="Next"
                        :disabled="currentPage >= pages"
                        @click="selectNext"
                        class="round flat xsmall primary material-icons"
                        rel="next">chevron_right</button>
                </nav>
            </div>
        `
      },
      defaults() {
        this.load(defaults)
      }
    }
}