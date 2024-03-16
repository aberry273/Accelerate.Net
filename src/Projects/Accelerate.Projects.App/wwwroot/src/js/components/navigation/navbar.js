
//import { generator } from '../_generator.js'

function ahref(href, text) {
  return `<a href="${href}">${text}</a>`
}

function li(el) {
  return `<li>${el}</li>`
}

function list(links) {
  let link = ''
  if (links != null && links.length > 0) {
    link = `<ul>${ links.map(x => li(ahref(x.href, x.text))).join('') }</ul>`
  }
  return link
}

export default function navbar(data) {
    return {
      init() {
        const header = `<ul>${li(`<strong>${data.title}</strong>`)}</ul>`;
        let links = list(data.links)
        this.$root.innerHTML = `
          <nav>
            ${header}
            ${links}
          </nav>
        `;
      }
    };
  }