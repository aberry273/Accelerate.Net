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
            this.mxDropdown_imageSrc = params.image;
            
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
        avatarLetters() {
            if (this.mxContent_title == null || this.mxContent_title.length == 0)
                return '??'
            const letters = this.mxContent_title.split(' ');
            return letters.map(x => x[0]).join('')
        },
        render() {
            const html = `
            <div class="relative">
        
            <button @click="mxDropdown_open=true" :class="mxDropdown_buttonClass">
                <img 
                    x-show="mxDropdown_imageSrc"
                    :src="mxDropdown_imageSrc" 
                    :class="mxDropdown_imageClass">
                <!-- No image-->
                <div 
                    x-show="!mxDropdown_imageSrc" class="relative inline-flex items-center justify-center w-10 h-10 overflow-hidden bg-gray-100 rounded-full dark:bg-gray-600">
                    <span 
                        class="font-medium text-gray-600 dark:text-gray-300"
                        x-text="avatarLetters"></span>
                </div>

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