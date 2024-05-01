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
        imageWidth: null,
        init() {
            this.item = data.item;
            this.data = data;
            this.imageWidth = data.imageWidth;
            this.modalEvent = data.modalEvent;
            const self = this;

            this.$nextTick(() => {
                this.load(self.data)
            })
        },
        getImage(filePath) {
            if (this.imageWidth) {
                return filePath + '?w=' + this.imageWidth;
            }
            return filePath;
        },
        modalAction(action, data) {
            this.$events.emit(this.modalEvent, data)
        },
        load(data) {
            const html = `
              <article class="media padless flat" style="cursor: pointer" class="padless clickable" @click="modalAction('open', item)">
              <video width="320" height="240" controls>
                    <source :src="item.filePath"
                    onerror="this.src='/src/images/broken.jpg'" type="video/mp4">
                    Your browser does not support the video tag.
                </video>
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