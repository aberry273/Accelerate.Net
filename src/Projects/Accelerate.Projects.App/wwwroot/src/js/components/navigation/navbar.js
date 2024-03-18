
function ahref(href, text) {
  return `<a href="${href}">${text}</a>`
}

function li(el) {
  return `<li>${el}</li>`
}

function list(links) {
  let link = ''
  if (links != null && links.length > 0) {
    link = `${ links.map(x => li(ahref(x.href, x.text))).join('') }`
  }
  return link
}

export default function navbar(data) {
    return {
      init() {
        const header = `<ul>${li(`<strong>${data.title}</strong>`)}</ul>`;
        let links = list(data.items)
        this.$root.innerHTML = `
          <nav>
            ${header}
            <ul>
              ${links}
                <li>
                <details class="dropdown no-chevron">
                  <summary role="button" class="flat round xsmall material-icons">
                    <template x-if="!$store.darkmode.isDark">
                      <i>dark_mode</i>
                    </template>
                    <template x-if="$store.darkmode.isDark">
                      <i>light_mode</i>
                    </template> 
                  </summary>
                  <ul>
                    <li><a href="#" data-theme-switcher="auto">Auto</a></li>
                    <li><a href="#" data-theme-switcher="light">Light</a></li>
                    <li><a href="#" data-theme-switcher="dark">Dark</a></li>
                  </ul>
                </details>
              </li>
            </ul>
          </nav>
        `;
      }
    };
  }