const defaults = {
  title: 'title',
  text: 'text',
  footer: 'footer'
}
export default function card() {
	return {
      init() {
        // Unable to pass parameter objects on init, use a separate function like the load function instead  
      },
      load(payload) {
        const data = payload
        this.$root.innerHTML = `
          <article>
              ${this.$render('header', data.title)}
              ${this.$render(data.text)}
              ${this.$render('footer', data.footer)}
          </article>
        `;
      },
      defaults() {
        this.load(defaults)
      }
    }
}