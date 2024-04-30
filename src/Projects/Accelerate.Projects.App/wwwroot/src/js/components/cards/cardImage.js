const defaults = {
    title: 'example post',
    username: 'user name',
    handle: '@username',
    updated: '10 minutes ago',
    agree: 15,
    disagree: 12,
    text: '<p><strong>title</strong></p><p>this is a new test of a auto-formatted markdown</p>',
    footer: 'footer'
}
export default function (data) {
    return {
        item: null,
        data: null,
        imageHeight: null,
        init() {
            this.item = data.item;
            this.data = data;
            this.imageHeight = data.imageHeight;
            this.modalEvent = data.modalEvent;
            const self = this;

            this.$nextTick(() => {
                this.load(self.data)
            })
        },
        getImage(filePath) {
            if (this.imageHeight) {
                return filePath + '?h=' + this.imageHeight;
            }
            return filePath;
        },
        modalAction(action, data) {
            this.$events.emit(this.modalEvent, data)
        },
        load(data) {
            const html = `
              <article class="media padless flat" style="cursor: pointer" class="padless clickable" @click="modalAction('open', item)">
                <figure>
                  <img 
                    :src="getImage(item.filePath)"
                    onerror="this.src='/src/images/broken.jpg'"
                    :alt="item.name"
                  />
                </figure>
              </article>`
            this.$nextTick(() => {
                this.$root.innerHTML = html;
            })
        },
        defaults() {
            this.load(defaults)
        }
    }
}