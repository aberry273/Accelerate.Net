
import { mxList, mxSearch, mxWebsockets, mxAlert, mxModal, mxResponsive } from '/src/js/mixins/index.js';

export default function (data) {
    return {
        // mixins
        ...mxList(data),
        ...mxSearch(data),
        ...mxWebsockets(data),
        ...mxAlert(data),
        ...mxModal(data),
        ...mxResponsive(data),

        // PROPERTIES
        items: [],
        selectedItem: {},
        userId: '',
        searchUrl: '',
        filterEvent: '',
        modalEvent: 'open-modal-media',
        brokenImage: '/src/images/broken.jpg',
        itemEvent: '',
        quoteEvent: '',
        actionUrl: '',
        actionEvent: 'modal-media-action',
        modalId: '',
        initSearch: false,

        async init() {
            const self = this;
            data = data != null ? data : {}
            this.actionUrl = data.actionUrl;
            this.items = data.items;
            this.searchUrl = data.searchUrl;
            this.userId = data.userId;
            this.initSearch = data.initSearch;
            //this.modalEvent = data.modalEvent;
            this.modalId = data.modalId;


            this.$events.on(this.modalId, async (item) => {
                this.$nextTick(() => {
                    this.selectedItem = item;
                    this._mxModal_Open(this.modalId)
                })
            })
            this.setHtml(data);
        },
        browseNextMedia() {
            if (this.selectedIndex == this.items.length) return;
            this.selectedItem = this.items[this.selectedIndex + 1];
        },
        browsePreviousMedia() {
            if (this.selectedIndex == 0) return;
            this.selectedItem = this.items[this.selectedIndex - 1];
        },

        get hasNext() {
            return this.selectedIndex < this.items.length - 1;
        },
        get hasPrevious() {
            if (this.items == null || this.items.length == 0) return false;
            return this.selectedIndex > 0;
        },
        get selectedIndex() {
            if (this.items == null) return 0;
            if (this.selectedItem == null) return 0;
            return this.items.map(x => x.id).indexOf(this.selectedItem.id)
        },
        get gridCols() {
            if (this.mxResponsive_IsXSmall) return 'col-2'
            if (this.items == null) return 'col-2'
            if (this.items.length <= 1) return 'col-2'
            if (this.items.length <= 2) return 'col-2'
            if (this.items.length <= 3) return 'col-3'
            return 'col-4';
        },
        get imageWidth() {
            if (this.items.length <= 1) return 600
            if (this.items.length <= 2) return 500
            if (this.items.length <= 3) return 400
            return 400
        },
        onImgError(image) {
            image.onerror = null;
            //retry
            setTimeout(function () {
                image.src += '?' + +new Date;
            }, 200);
        },

        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div x-transition class="grid post-media" :class="gridCols">
                <template x-for="(item, i) in items" :key="item.id+item.updatedOn || i" >
                    <div>
                        <template x-if="item.type == 'Video'">
                            <div x-data="cardVideo({
                                item: item,
                                userId: userId,
                                modalEvent: modalId,
                                imageWidth: imageWidth
                            })"></div>
                        </template>
                        <template x-if="item.type == 'Image'">
                            <div x-data="cardImage({
                                item: item,
                                userId: userId,
                                modalEvent: modalId,
                                imageWidth: imageWidth
                            })"></div>
                        </template>
                    </div>
                </template>
              
            </div>
          
            <dialog :id="modalId" class="fullscreen">
                <article class="px-0 fullscreen" style="margin-top:0px;">
                    <header class="">
                        <nav>
                        <ul> 
                            <strong x-text="selectedItem.name"></strong> 
                        </ul>
                        <ul>
                         
                            <!--
                            <details class="dropdown flat no-chevron">
                                <summary role="outline">
                                    <i aria-label="Close" class="icon material-icons icon-click" rel="prev">more_vert</i>
                                </summary>
                                <ul>
                                    <li><a class="click" @click="editItem(selectedItem)">Edit</a></li>
                                    <li><a class="click" @click="deleteItem(selectedItem)">Delete</a></li>
                                </ul>
                            </details>
                            -->

                            <button aria-label="Close" rel="prev" @click="_mxModal_Close(modalId)"></button>
                        </ul>
                        </nav>
                    </header>
                     
                    <button aria-label="Previous" 
                        :disabled="!hasPrevious"
                        @click="browsePreviousMedia" 
                        class="round xsmall flat primary material-icons floating-previous"
                        rel="next">chevron_left</button>
                    <div class="floating-centre">
                        <figure >
                            <img
                                style="height:100%"
                                :src="selectedItem.filePath"
                                :onerror="onImgError(this)"
                                :alt="selectedItem.name"
                                />
                        </figure>
                    </div>

                    <button aria-label="Next" 
                        :disabled="!hasNext"
                        @click="browseNextMedia" 
                        class="round xsmall flat primary material-icons floating-next"
                        rel="next">chevron_right</button>
                  

                </article>
            </dialog> 
            `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}