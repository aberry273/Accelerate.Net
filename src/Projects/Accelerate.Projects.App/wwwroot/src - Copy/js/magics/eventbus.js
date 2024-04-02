// TODO
export default () => (el, data) => {
    return {
        emit(ev, payload) {
            this.$dispatch(this.ev, payload)
        },
        on(ev, cb) {
            return cb()
        },
    }
}