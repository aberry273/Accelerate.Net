import { getIcon } from './utilities.js'
const defaults = {
    ms: 3000,
    limit: 5
}
let timeout = null;
export default function (data = {}) {
    return {
        show: false,
        snackbars: [],
        ms: 50000,//data.ms || defaults.ms,
        showFloatingPanel: true,
        type: data.type || defaults.type,
        icon: data.icon || defaults.icon,
        limit: data.limit || defaults.limit,
        init() {
            this.limit = data.limit;
          this.show = data.show;
          
          this.load(data);
          const self = this;
          
          
          // Listen for the websocket event.
          this.$events.on(data.event || "snackbar:add", (payload) => {
            var icon = getIcon(data)

            this.snackbars.push({
                type: payload.type,
                text: payload.text,
                url: payload.url,
                success: true,
                icon: icon
            })

            self.load(data)
            self.show = true;
            clearTimeout(timeout);
            // if a timer is set
            if (self.ms && self.ms > 0)
                timeout = setTimeout(function () {
                    self.closeAll()
                }, self.ms);
          });
        },
        closeAll() {
            this.show = false;
            this.showFloatingPanel = true;
            this.snackbars = [];
        },
        removeSnackbar(i) {
            this.snackbars.splice(i, 1);
        },
        topSnackbars() {
            return this.snackbars.slice(0).slice(this.limit*-1)
        },
        remainingNotifications() {
            if (this.snackbars.length < this.limit) return '';
            const remaining = (this.snackbars.length - this.limit)
            return `${remaining} more alerts`;
        },
        load(data) {
          // Turn into object as it returns reactive Proxy
            //const data = JSON.parse(JSON.stringify(payload))
          /*
            <button
                x-show="show && showFloatingPanel"
                @click="showFloatingPanel = false"
                class="round xsmall secondary "
                style="y-index:1111; position: fixed; bottom: 65px; right: calc(var(--pico-spacing)*0.225);">
                    <i class="material-icons">notifications</i>
                    <sup class="tag" style="margin-left: -3px; color:white" x-text="snackbars.length"></sup>
            </button>
            */
          const icon = this.icon;
            const html = `
            <!--Floating button-->
          <template x-if="show">
            <article class="is-fixed dense page-modal snackbar-container" >
                <ul>
                    <div align="right" x-show="snackbars.length > 1">
                        <span x-text="remainingNotifications"></span>
                        <i aria-label="Close" class="click material-icons flat" rel="prev" @click="closeAll">close</i>
                    </div>
                    <div >
                        <template x-for="(snackbar, i) in topSnackbars" :key="i">
                            <article class="dense snackbar" :class="snackbar.type" x-transition>
                                <nav>
                                    <i class="material-icons" x-text="snackbar.icon"></i>
                                    <p x-text="snackbar.text"></p>
                                    <a target="_blank" :href="snackbar.url" x-show="snackbar.url"><i class="flat click material-icons" >open_in_fill</i></a>
                                    <i class="flat click material-icons" aria-label="Close" @click="removeSnackbar(i)" rel="prev">close</i>
                                </nav>
                            </article>
                        </template
                    </div>
                <ul>
            </article>
          </template>`
          this.$nextTick(() => { 
            this.$root.innerHTML = html
          })
        },
        defaults() {
          this.load(defaults)
        }
    }
}