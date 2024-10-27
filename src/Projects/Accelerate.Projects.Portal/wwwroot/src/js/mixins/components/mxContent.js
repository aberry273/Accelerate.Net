export default function (data) {
    return {
        // PROPERTIES
        mxContent_img: '',
        mxContent_title: '',
        mxContent_subtitle: '',
        mxContent_text: '',
        mxContent_subtext: '',
        init() {
        },
        // GETTERS
        get mxContent_titleClass() { return 'w-full text-3xl font-bold '},
        get mxContent_subtitleClass() { return 'w-full text-2xl text-gray-500'},
        get mxContent_textClass() { return 'text-lg text-gray-500'},
        // METHODS
        _mxContent_Example() {
        },
    }
}