import { mxContent, mxNavigation } from '/src/js/mixins/index.js';

export default function (params) {
    return {
        ...mxContent(params),
        ...mxNavigation(params),
        // PROPERTIES

        // INIT
        init() {
            this._mxNavigation_SetParams(params);
            this.render();
        },
        // GETTERS
        // METHODS
        close() {
            this.open = false;
        },
        // RENDER
        render() {
            const html = `
            <section class="z-40 px-0 my-2">
                <div class="flex flex-row" :class="mxNavigation_getContainerClass">
                    <div :class="mxNavigation_getContainerLeftClass">
                        <span :class="mxNavigation_getTitleClass" x-text="mxContent_title"></span>
                        
                        <div 
                            class="flex flex-wrap items-center mx-auto my-auto text-base pl-2 ml-2 md:border-l md:border-gray-200" >
                            <template x-for="item in mxNavigation_items">
                                <button x-show="mxNavigation_open || !mxNavigation_canMinimize" x-data="aclButton({
                                    ...item,
                                    textClass: 'ml-0',
                                    class: _mxNavigation_selectedButtonClass(item),
                                })"></button>
                            </template>
                        </div>
                    </div>
                </div>
            </section>
        `
            this.$nextTick(() => { this.$root.innerHTML = html });
        },
    }
}