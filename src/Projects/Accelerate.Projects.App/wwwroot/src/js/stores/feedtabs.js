export default () => ({
    init() {},
    current: 'AI updates',
 
    items: [
        "AI updates",
        "Receipes",
        "Historical bites",
        "2024 Election",
        "Movies & TV",
    ],

    setTab(tab) {
        this.current = tab;
    }
})