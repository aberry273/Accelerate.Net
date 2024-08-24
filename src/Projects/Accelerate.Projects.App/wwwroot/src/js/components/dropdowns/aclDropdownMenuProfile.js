import { mxContent, mxIcon, mxDropdown } from '/src/js/mixins/index.js';

export default function (params) {
	return {
        ...mxContent(params),
        ...mxIcon(params),
        ...mxDropdown(params),
        // PROPERTIES 
        // INIT
        init() {
            this.mxContent_title = params.title;
            this.mxContent_subtitle = params.subtitle;
            this.mxDropdown_groups = params.groups;
            
            this.open = params.open;
            this.render();
        },
        // GETTERS
        // METHODS
        close() {
            this.open = false;
        },
        open() {
            this.mxDropdown_open = true;
        },
        onClick(btn) {
            // Fallback to dispatch
            this.$dispatch('onselect', btn);
        },
        render() {
            const html = `
            <div class="relative">
        
            <button @click="mxDropdown_open=true" :class="mxDropdown_buttonClass">
                <img :src="mxDropdown_imageSrc" :class="mxDropdown_imageClass">
                <span :class="mxDropdown_buttonTextContainerClass">
                    <span x-text="mxContent_title"></span>
                    <span :class="mxDropdown_buttonSubtitleClass" x-text="mxContent_subtitle"></span>
                </span>
                <svg 
                    :class="mxIcon_buttonRightSvg" 
                    x-data="aclIconsSvg({ mxIcon_name: 'expand' })"></svg>   
            </button>
        
            <div x-show="mxDropdown_open" 
                @click.away="mxDropdown_open=false"
                x-transition:enter="ease-out duration-200"
                x-transition:enter-start="-translate-y-2"
                x-transition:enter-end="translate-y-0"
                class="absolute top-0 z-50 w-56 mt-12 -translate-x-1/2 left-1/2"
                x-cloak>
                <div class="p-1 mt-1 bg-white border rounded-md shadow-md border-neutral-200/70 text-neutral-700">
                    <template x-for="group in mxDropdown_groups">
                        <div>
                            <div class="px-2 py-1.5 text-sm font-semibold" x-text="group.title"></div>
                            <div class="h-px my-1 -mx-1 bg-neutral-200"></div>
                            <template x-for="item in group.items">
                                <div x-data="aclButton(item)" @onclick="onClick(item)"></div>
                            </template>
                        </div>
                    </template>
                </div>
            </div>
        </div>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      }
    }
}