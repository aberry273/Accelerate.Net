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
      init() {
        this.load(data)
      },
      load(data) {
        // Turn into object as it returns reactive Proxy
        //const data = JSON.parse(JSON.stringify(payload))
        this.$root.innerHTML = `
        <article>
          <header class="dense">
            <nav>
              <ul>
                <li>
                  <button class="round primary img">
                    <img
                    class="circular"
                    src="${data.profile}"
                    alt="username_profile"
                  />
                  </button>
                </li>
                <aside class="dense">
                  <li><strong>${data.username}</strong></li>
                  <li><a class="secondary disabled"><small>${data.handle}</small></a></li>
                </aside>
              </ul>
              <ul>
                <li>
                  <details class="dropdown flat">
                    <summary role="outline">
                      <i aria-label="Close" class="icon material-icons icon-click" rel="prev">more_vert</i>
                    </summary>
                    <ul dir="rtl">
                      <li><a class="click" @click="console.log('Edit')")>Edit</a></li>
                      <li><a class="click" @click="console.log('Remove')")>Remove</a></li>
                    </ul>
                  </details>
                </li>
              </ul>
            </nav>
          </header> 
          ${data.content}
          <footer class="dense">
            <nav>
              <ul>
                <li>
                  <!--Agree-->
                  <i aria-label="Agree" class="icon material-icons icon-click" rel="prev">expand_less</i>
                  <sup class="noselect" rel="prev">${data.agree}</sup>
                  <!--Disagree-->
                  <i aria-label="Disagree" class="icon material-icons icon-click" rel="prev">expand_more</i>
                  <sup class="noselect" rel="prev">${data.disagree}</sup> 
                </li> 
              </ul>
              <ul>
                <li>
                  <i aria-label="Reply" class="icon material-icons icon-click" rel="prev">reply</i>
                  <!--Liked-->
                  ${
                    data.liked 
                    ? '<i x-if="data.liked" aria-label="Liked" class="primary icon material-icons icon-click" rel="prev">favorite</i>'
                    : '<i x-if="data.liked" aria-label="Noy liked" class="icon material-icons icon-click" rel="prev">favorite</i>'
                  }
                </li>
              </ul>
            </nav>
          </footer>
        </article>`
      },
      defaults() {
        this.load(defaults)
      }
    }
}