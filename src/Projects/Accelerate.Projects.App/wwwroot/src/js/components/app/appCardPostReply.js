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
      data: null,
      threadUrl: null,
      expanded: false,
        init() {
          this.data = data.item;
        this.threadUrl = data.threadUrl;
        this.expanded = data.expanded;
        this.expandable = data.expandable;
        const self = this;
        this.$nextTick(() => {
          this.load(self.data)
        })
      },
      quickAction(action) {
        const ev = `action-${action}`;
        this.$events.emit(ev, this.data)
        //this.$events.emit(action, this.data)
      },
      redirectAction(action) {
        const ev = `redirect-${action}`;
        this.$events.emit(ev, this.data)
      },
    modalAction(action) {
        const ev = `modal-${action}-post`;
        const payload = {
            // route to append to postbackUrl 
            postbackUrlRoute: this.data.id,
            // postback type
            postbackType: 'PUT',
            // content post item
            item: this.data,
        }
        this.$events.emit(ev, payload)
    },
      getThreadUrl(id) {
        return `${threadUrl}/${id}`;
      },
      load(data) {
       const html = `
        <article class="dense" :id="data._id">
          <header>
            <nav>
              <ul>
                <template x-if="data.profile != null">
                    <li> 
                      <button class="round small primary img">
                        <img
                        class="circular"
                        src="${data.profile}"
                        alt="${data.username}"
                      />
                      </button>
                    </li>
                </template>
                <aside class="dense">
                  <li><strong class="secondary">${data.username}</strong></li>
                </aside>
              </ul>
              <ul>
                <li>
                  <details class="dropdown flat no-chevron">
                    <summary role="outline">
                      <i aria-label="Close" class="icon material-icons icon-click" rel="prev">more_vert</i>
                    </summary>
                    <ul dir="rtl">
                      <li><a class="click" @click="modalAction('share')">Share</a></li>
                      <li><a class="click" @click="modalAction('edit')">Edit</a></li>
                      <li><a class="click" @click="modalAction('delete')">Delete</a></li>
                    </ul>
                  </details>
                </li>
              </ul>
            </nav>
          </header>
          <template x-if="data.targetThread">
              <blockquote class="dense">
                <a :href="'#'+data.targetThread" x-text="data.targetThread"></a>
              </blockquote>
          </template>
          ${data.content}
          <footer>
            <nav>
              <ul>
                <li>
                  <!--Agree-->
                  <i aria-label="Agree" @click="quickAction('agree')" class="icon material-icons icon-click" rel="prev">expand_less</i>
                  <sup class="noselect" rel="prev">${data.agree || 0}</sup>
                  <!--Disagree-->
                  <i aria-label="Disagree" @click="quickAction('disagree')" class="icon material-icons icon-click" rel="prev">expand_more</i>
                  <sup class="noselect" rel="prev">${data.disagree || 0}</sup> 

                  <template x-if="expanded">
                   <i aria-label="Reply" @click="quickAction('close')" class="icon material-icons icon-click" rel="prev">close</i>
                  </template>

                  <i aria-label="Reply" @click="quickAction('reply')" class="icon material-icons icon-click" rel="prev">reply_all</i>
                  <sup class="noselect" rel="prev">${data.replies || 0}</sup> 
                
                </li> 
              </ul>
              <ul>
                <li>
                  <!--Liked-->
                  <i x-show="data.liked" @click="quickAction('like')" aria-label="Liked" class="primary icon material-icons icon-click" rel="prev">favorite</i>
                  <i x-show="!data.liked" @click="quickAction('like')" aria-label="Unliked" class="icon material-icons icon-click" rel="prev">favorite</i>
                </li>
              </ul>
            </nav>
          </footer>
        </article>
        `
        this.$nextTick(() => {
            this.$root.innerHTML = html
        });
      },
      defaults() {
        this.load(defaults)
      }
    }
}