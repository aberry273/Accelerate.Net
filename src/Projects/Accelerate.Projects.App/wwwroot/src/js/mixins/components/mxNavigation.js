export default function (data) {
    return {
        // PROPERTIES
        mxNavigation_headerLinkClass: 'mr-5 font-medium leading-6 text-gray-600 hover:text-gray-900 dark:text-white dark:hover:text-grey-200',
        mxNavigation_headerButtonClass: 'inline-flex items-center justify-center px-4 py-2 text-base font-medium leading-6 text-gray-600 whitespace-no-wrap bg-white dark:bg-grey-800 border border-gray-200 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:shadow-none',
        mxNavigation_footerLinkClass: 'text-base leading-6 text-gray-500 hover:text-gray-900',

        mxNavigation_primaryItems: [],
        mxNavigation_secondaryItems: [],
        init() {
        },
        // GETTERS
        get mxNavigation_getSectionClass() { return 'w-full px-8 text-gray-700 bg-white dark:bg-gray-800' },
        get mxNavigation_getContainerClass() { return 'container flex flex-col flex-wrap dark:bg-gray-800 items-center justify-between py-5 mx-auto md:flex-row max-w-7xl' },
        get mxNavigation_getContainerLeftClass() { return 'relative flex flex-col md:flex-row' },
        get mxNavigation_getContainerRightClass() { return 'inline-flex items-center ml-5 space-x-6 lg:justify-end' },
        
        get mxNavigation_getLinkClass() { return 'flex items-center mb-5 font-medium text-gray-900 dark:text-white lg:w-auto lg:items-center lg:justify-center md:mb-0' },
        get mxNavigation_getTitleClass() { return 'mx-auto text-xl font-black leading-none text-gray-900 dark:text-white select-none' },
        get mxNavigation_getItemsClass() { return 'flex flex-wrap items-center mb-5 text-base md:mb-0 md:pl-8 md:ml-8 md:border-l md:border-gray-200' },

        // METHODS
        _mxNavigation_Example() {
        },
    }
}