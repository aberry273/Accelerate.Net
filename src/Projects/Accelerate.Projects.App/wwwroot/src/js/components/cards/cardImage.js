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
      init() {
        this.item = data.item;
        this.data = data;
        this.modalEvent = data.modalEvent;
        const self = this;
        
        this.$nextTick(() => {
          this.load(self.data)
        })
      },
      modalAction(action, data) {
        const ev = `${action}-modal-media`;
        this.$events.emit(ev, data)
      },
      load(data) {
        const html = `
        <article class="media padless flat" style="cursor: pointer" class="padless clickable" @click="modalAction('open', item)">
          <figure>
            <img 
              :src="item.filePath"
              :alt="item.name"
            />
            <!--
            <figcaption>
              Image from
              <a href="https://unsplash.com/photos/a562ZEFKW8I" target="_blank">unsplash.com</a>
            </figcaption>
            -->
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