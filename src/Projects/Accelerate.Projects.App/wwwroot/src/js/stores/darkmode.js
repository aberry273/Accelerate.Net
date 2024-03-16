export default () => ({
    init() {
        this.on = window.matchMedia('(prefers-color-scheme: dark)').matches
    },
    dark: false,
    theme: 'light',
 
    toggle() {
        this.dark = ! this.dark
        this.theme = !this.dark ? 'light' : 'dark'
        console.log(this.dark)
    }
})