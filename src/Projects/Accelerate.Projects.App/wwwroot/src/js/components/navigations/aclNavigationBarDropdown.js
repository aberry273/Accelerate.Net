import { mxContent, mxNavigation } from '/src/js/mixins/index.js';

export default function (params) {
	return {
        ...mxContent(params),
        ...mxNavigation(params),
        // PROPERTIES
        
        // INIT
        init() {
            this.mxContent_title = params.title;
            this.mxNavigation_primaryItems = params.primaryItems;
            this.mxNavigation_secondaryItems = params.secondaryItems;
            this.mxNavigation_dropdown = params.dropdown;
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
            <section :class="mxNavigation_getSectionClass" {!! $attributes ?? '' !!}>
            <div :class="mxNavigation_getContainerClass">
                <div :class="mxNavigation_getContainerLeftClass">
                    <a href="#_" :class="mxNavigation_getLinkClass">
                        <span :class="mxNavigation_getTitleClass" x-text="mxContent_title"></span>
                    </a>
                    <nav :class="mxNavigation_getItemsClass">
                        <template x-for="item in mxNavigation_primaryItems">
                            <a :href="item.href" x-text="item.label" :class="item.class || mxNavigation_headerLinkClass"></a>
                        </template>
                    </nav>
                </div>
        
                <div :class="mxNavigation_getContainerRightClass">
                <div x-data="aclDropdownMenuProfile( mxNavigation_dropdown )"></div>
                </div>
            </div>
        </section>
        `
        this.$nextTick(() => { this.$root.innerHTML = html });
      },
    }
}