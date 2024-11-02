import { mxNavigation, mxIcon } from '/src/js/mixins/index.js';

export default function (params) {
    return {
        ...mxNavigation(params),
        ...mxIcon(params),
        // PROPERTIES
        open: false,
        position: 0,
        // INIT
        init() {
            this._mxNavigation_SetParams(params || {});
            this.setValues(params || {});
            this.render();
        },
        // GETTERS
        // METHODS
        setPosition() {
            if (!this.mxNavigation_selected) return;
            const index = this.mxNavigation_items.select(x => x.text).indexOf(this.mxNavigation_selected)
            this.position = index > -1 ? index : 0;
        },
        itemClass(item, i) {
            let css = i < this.mxNavigation_items.length 
                ? 'sm:after:content-[""] after:w-full after:h-1 after:border-b after:border-gray-200 after:border-1 after:hidden sm:after:inline-block after:mx-6 xl:after:mx-10 dark:after:border-gray-700'
                : '';
            if (item.text == this.mxNavigation_selected)
                return css + 'md:w-full text-blue-600 dark:text-blue-500';
            if (i == this.mxNavigation_items.length - 1)
                return css + 'md:w-full';
        },
        setValues(params) {
        },
        render() {
            const html = `
                <ol class="flex items-center justify-between w-full text-sm font-medium text-center text-gray-500 dark:text-gray-400 sm:text-base">
                    <template x-for="(item, i) in mxNavigation_items">
                        <li class="flex items-center" :class="itemClass(item, i)">
                            <div class="flex text-center items-center after:content-['/'] sm:after:hidden after:mx-2 after:text-gray-200 dark:after:text-gray-500">
                                <!--Tick icon-->
                                <svg x-show="position >= i" class="w-3.5 h-3.5 sm:w-4 sm:h-4 me-2.5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 20 20">
                                    <path d="M10 .5a9.5 9.5 0 1 0 9.5 9.5A9.51 9.51 0 0 0 10 .5Zm3.707 8.207-4 4a1 1 0 0 1-1.414 0l-2-2a1 1 0 0 1 1.414-1.414L9 10.586l3.293-3.293a1 1 0 0 1 1.414 1.414Z"/>
                                </svg>
                                <template x-if="item.icon">
                                    <div :class="mxIcon_class">
                                        <svg class="w-5 h-5" x-data="aclIconsSvg({item.icon})"></svg>
                                    </div>
                                </template>
                                <span x-show="position <= i" class="me-2" x-text="i+1"></span>
                                <span x-text="item.text" class=""></span>
                            </div>
                        </li>
                    </template>
                </ol>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
        }
    }
}