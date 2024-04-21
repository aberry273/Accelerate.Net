let component = `
    <div x-data="cardPost(
    {
      item: item,
    })"></div>
`
import mxList from '/src/js/mixins/mxList.js';
import mxSearch from '/src/js/mixins/mxSearch.js';
import mxAction from '/src/js/mixins/mxAction.js';
import mxWebsockets from '/src/js/mixins/mxWebsockets.js';
import mxAlert from '/src/js/mixins/mxAlert.js';
import mxModal from '/src/js/mixins/mxModal.js';

export default function (data) {
    return {
        // mixins
        ...mxList(data),
        ...mxSearch(data),
        ...mxAction(data),
        ...mxWebsockets(data),
        ...mxAlert(data),
        ...mxModal(data),

        // PROPERTIES
        items: [],
        selectedItem: {},
        userId: '',
        searchUrl: '',
        filterEvent: '',
        modalEvent: 'open-modal-media',
        itemEvent: '',
        quoteEvent: '',
        modalId: 'gridCardMediaModal',

        async init() {
            const self = this;
            data = data != null ? data : {}
            this.items = data.items;
            this.searchUrl = data.searchUrl;
            this.userId = data.userId;

            component = data.component || component

            // On updates from filter
            this.$events.on(this.filterEvent, async (filterUpdates) => {
                await this.search(filterUpdates);
            })
            this.$events.on(this.modalEvent, async (item) => {
              this.$nextTick(() => {
                this.selectedItem = item;
                this._mxModal_Open(this.modalId)
              })
            })
            //await this.initSearch();

            this.setHtml(data);
        },
        async initSearch() {
          let queryData = {}
          
          await this.search(queryData);
        },

        browseNextMedia() {
          if (this.selectedIndex == this.items.legnth - 1) return;
          this.selectedItem = this.items[this.selectedIndex+1];
        },
        browsePreviousMedia() {
          if (this.selectedIndex == 0) return;
          this.selectedItem = this.items[this.selectedIndex-1];
        },
        get selectedIndex() {
          if(this.selectedItem == null) return -1;
          return  this.items.map(x => x.id).indexOf(this.selectedItem.id)
        },

        // METHODS
        async search(filters) {
            let query = this._mxList_GetFilters(filters);
            const postQuery = this._mxSearch_CreateSearchQuery(query);
            if (postQuery == null) return;
            this.items = await this._mxSearch_Post(this.searchUrl, postQuery);
        },
        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div x-transition class="grid col-4">
              <template x-for="(item, i) in items" :key="item.id+item.updatedOn || i" >
                <div x-data="cardImage({
                  item: item,
                  userId: userId,
                  modalEvent: modalEvent,
                })"></div>
              </template>
              <template x-if="items == null || items.length == 0">
                <article>
                  <header><strong>No results!</strong></header>
                  No results could be found
                </article>
              </template>
            </div>
          
            <dialog :id="modalId">
              <article class="fullscreen">
                <header>
                  <nav>
                    <ul>
                      <p>
                        <strong x-text="selectedItem.name"></strong>
                      </p>
                    </ul>
                    <ul>
                      <i 
                      :disabled="selectedIndex > 0"
                      aria-label="Previous" 
                      @click="browsePreviousMedia" 
                      class="icon material-icons icon-click" 
                      rel="prev">chevron_left</i>

                      <i aria-label="Next" 
                      :disabled="selectedIndex < items.length-1"
                      @click="browseNextMedia" 
                      class="icon material-icons icon-click" 
                      rel="prev">chevron_right</i>

                      <button aria-label="Close" rel="prev" @click="_mxModal_Close(modalId)"></button>
                    </ul>
                  </nav>
                </header>
                <figure>
                  <img 
                    :src="selectedItem.value"
                    alt="Minimal landscape"
                  /> 
                  <figcaption>
                    Image from
                    <a href="https://unsplash.com/photos/a562ZEFKW8I" target="_blank">unsplash.com</a>
                  </figcaption> 
              </figure>
                <ul>
                  <li>Date: Saturday, April 15</li>
                  <li>Time: 10:00am - 12:00pm</li>
                </ul>
              </article>
              
            </dialog>
          
            `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}