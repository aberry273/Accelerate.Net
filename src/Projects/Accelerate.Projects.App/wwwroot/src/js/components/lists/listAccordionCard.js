
import mxList from '/src/js/mixins/mxList.js';
import mxSearch from '/src/js/mixins/mxSearch.js';
import mxAction from '/src/js/mixins/mxAction.js';
import mxWebsockets from '/src/js/mixins/mxWebsockets.js';
import mxAlert from '/src/js/mixins/mxAlert.js';

export default function (data) {
    return {
        // mixins
        ...mxList(data),
        ...mxSearch(data),
        ...mxAction(data),
        ...mxWebsockets(data),
        ...mxAlert(data),

        // PROPERTIES
        items: [],
        userId: '',

        async init() {
            const self = this;
            data = data != null ? data : {}
            this.items = data.items;
            this.setHtml(data);
        },
        // METHODS
        setHtml(data) {
            // make ajax request 
            const html = `
            <div x-transition>
              <template x-for="(item, i) in items" :key="item.id+item.updatedOn || i" >
                <details class="dense blockquote" @click="item._selected = !item._selected" :class="item._selected ? 'selected' : ''">
                    <summary>
                        <i x-show="i > 0" class="material-icons">subdirectory_arrow_right</i>
                        <strong x-text="item.threadId"></strong> 
                    </summary>
                    <div class="px-0" x-data="cardPost({
                        item: item,
                        userId: null
                    })"></div>
                </details>
                <hr />
              </template>
            </div>
            `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}