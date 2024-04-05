export default () => ({
    init() {
        this.on = window.matchMedia('(prefers-color-scheme: dark)').matches
    },
    theme: 'light',
    get isDark() { return this.theme == 'dark' },
    toggle() {
        this.theme = !this.theme == 'dark' ? 'light' : 'dark'
    }
})