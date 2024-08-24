import { mxContent } from '/src/js/mixins/index.js';

export default function (params) {
	return {
        ...mxContent(params),
        // PROPERTIES
        open: false,
        // INIT
        init() {
            this.open = params.open;
            this.render();
        },
        // GETTERS
        // METHODS
        close() {
            this.open = false;
        },
        render() {
            const html = ` 
            <div class="relative flex flex-col items-center justify-center w-full h-full px-10 my-20 lg:px-16 lg:my-0  >
                <div class="flex flex-col items-start space-y-8 tracking-tight lg:max-w-3xl">
                    <div class="relative">
                        <p class="mb-2 font-medium text-gray-700 uppercase">Work smarter</p>
                        <h2 class="text-5xl font-bold text-gray-900 xl:text-6xl">Features to help you work smarter</h2>
                    </div>
                    <p class="text-2xl text-gray-700">We've created a simple formula to follow in order to gain more out of your business and your application.</p>
                    <a href="#_" class="inline-block px-8 py-5 text-xl font-medium text-center text-white transition duration-200 bg-blue-600 rounded-lg hover:bg-blue-700 ease" data-primary="blue-600" data-rounded="rounded-lg">Get Started Today</a>
                </div>
            </div>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      }
    }
}