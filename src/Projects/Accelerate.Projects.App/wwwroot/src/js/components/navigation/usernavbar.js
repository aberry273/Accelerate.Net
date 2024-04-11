
function ahref(href, text) {
    return `<a href="${href}">${text}</a>`
}

function li(el) {
    return `<li>${el}</li>`
}

function list(links) {
    let link = ''
    if (links != null && links.length > 0) {
        link = `${links.map(x => li(ahref(x.href, x.text))).join('')}`
    }
    return link
}

export default function navbar(data) {
    return {
        data: null,
        init() {
            this.data = data;
            const header = `<ul>${li(`<strong>${data.title}</strong>`)}</ul>`;
            let links = list(data.items)
            let dropdownLinks = list(data.dropdown.items || [])
            var html = `
          <nav>
            ${header}
            <ul>
              ${links}
              <template x-if="data.dropdown">
                <details class="dropdown avatar">
                  <summary role="link">
                    <template x-if="data.dropdown.img">
                      <img :src="data.dropdown.img" /> 
                    </template>
                    <template x-if="!data.dropdown.img">
                     <i class="icon material-icons icon-click">person</i>
                    </template>
                  </summary>
                <ul>
                  ${dropdownLinks}
                </ul>
              </details>
              </template>
            </ul>
          </nav>
        `;
            this.$nextTick(() => {
                this.$root.innerHTML = html;
            })
        }
    };
}