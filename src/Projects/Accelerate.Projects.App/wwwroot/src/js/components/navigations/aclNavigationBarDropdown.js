import { mxContent, mxNavigation } from '/src/js/mixins/index.js';

export default function (params) {
	return {
        ...mxContent(params),
        ...mxNavigation(params),
        // PROPERTIES
        heightClass: '',
        // INIT
        init() {
            this.mxContent_title = params.title;
            this.mxNavigation_primaryItems = params.primaryItems;
            this.mxNavigation_secondaryItems = params.secondaryItems;
            this.mxNavigation_dropdown = params.dropdown;
            //this.heightClass = ' h-'+this.mxNavigation_barHeight;
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
            <section class="z-40 bg-white" :class="mxNavigation_getSectionClass" {!! $attributes ?? '' !!}>
                <div :class="mxNavigation_getContainerClass + heightClass">
                    <div :class="mxNavigation_getContainerLeftClass">
                        <a href="#_" :class="mxNavigation_getLinkClass">
                            <span :class="mxNavigation_getTitleClass" x-text="mxContent_title"></span>
                        </a>
                        <!--Desktop-->
                        <nav class="lg:block md:hidden sm:hidden xs:hidden" :class="mxNavigation_getItemsClass">
                            <template x-for="item in mxNavigation_primaryItems">
                                <a :href="item.href" x-text="item.text" :class="item.class || mxNavigation_headerLinkClass"></a>
                            </template>
                        </nav>
                        <!--Mobile-->
                        <div x-show="mxNavigation_primaryItems.length > 0" class="mt-1 ml-2 lg:hidden md:flex sm:flex xs:flex" x-data="aclDropdownMenuButton({ items: mxNavigation_primaryItems })"></div>
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