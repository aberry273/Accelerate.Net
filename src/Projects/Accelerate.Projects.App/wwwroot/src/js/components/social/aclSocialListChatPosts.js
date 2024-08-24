import { mxContent, mxIcon, mxButton, mxList, mxFetch, mxEvent } from '/src/js/mixins/index.js';

export default function (params) {
	return {
        ...mxContent(params),
        ...mxIcon(params),
        ...mxButton(params),
        ...mxList(params),
        ...mxFetch(params),
        ...mxEvent(params),
        // PROPERTIES
        tooltipArrow: true,
        tooltipPosition: '',
        // redirect for navigating the user to the comment page
        // inline for opening up replies on click of replies
        mode: 'redirect',
        showline: false,
        searchOnInit: false,
        loading: false,
        commentItems: [],
        actionItems: [],
        menuItems: [],
        // INIT
        async init() {
            this.setParams(params);
            this.render();
            this.setEvents();
            if(this.searchOnInit) await this.search();
        },
        // GETTERS 
        get modeThread() {
            return this.mode == 'thread'
        },
        get modeInline() {
            return this.mode == 'inline'
        },
        // METHODS
        setParams(params) {
            this.mode = params.mode || 'redirect';
            this.showline = params.showline;
            this.searchOnInit = params.searchOnInit || false;
            this.commentItems = params.commentItems;
            this.actionItems = params.actionItems;
            this.menuItems = params.menuItems;

            this.mxIcon_name = params.icon;
            this.mxButton_name = params.name;
            this.mxButton_label = params.label;
            this.mxButton_tooltip = params.tooltip; 
            this.mxButton_iconClass = params.class || this.mxButton_iconClass;

            this.mxFetch_url = params.url; 
            this.mxFetch_params = params.params; 
        },
        async setEvents() {
            this._mxEvent_On(this.$store.svcComments.svcComments_eventCreate, (result) => {
                result.id += new Date().toISOString();
                this.commentItems.push(result);
            })
        },
        async search() {
            const self = this;
            self.loading = true;
            var delayInMilliseconds = 400; //1 second 
            setTimeout(async function() {
                if (self.mxFetch_url)
                {
                    let results = await self._mxFetch_Post(this.mxFetch_url, this.mxFetch_params);
                    
                    results = replyItems.map(x => {
                        const item = { ...x }
                        item.id = Math.floor(Math.random() * 999999);
                        return item;
                    })
                    self.commentItems = results;
                    self.loading = false;
                }
            }, delayInMilliseconds);
        },
        getCommentWithMenuActions(item) {
            // assign the menu actions to only what is available in the items menu array
            const menu = this.filterAvailableActions(this.menuItems, item.menu);
            item.menu = this.assignActionsItemAsValue(menu, item);

            const actions = this.filterAvailableActions(this.actionItems, item.actions);
            item.actions = this.assignActionsItemAsValue(actions, item);
            
            item.ui = {
                ...item.ui,
                mode: this.mode,
                showline: this.showline,
            }

            return item;
        },
        filterAvailableActions(actions, available) {
            return actions.filter(x => available.indexOf(x.name) > -1); 
        },
        assignActionsItemAsValue(actions, item) {
            return actions.map(x => { x.value = item; return x; }); 
        },
        setHover() {
            if(!this.mxButton_tooltip) return;
            this.mxButton_showTooltip = true
        },
        setLeave() {
            this.mxButton_showTooltip = false;
        },
        onClick() {
            this.$dispatch('onclick', this.mxButton_name);
        },
        toggleReplies(comment) {
            comment.toggle = !comment.toggle;
        },
        replies() {
            return JSON.parse(JSON.stringify(replyItems))
        },
        getRandomInt(max) {
            return Math.floor(Math.random() * max);
        },
        hasReplies(item) {
            if (item == null || !item.ui) return false;
            return item.ui.showReplies;
        },
        render() {
            const html = `
            <div class="flex flex-col w-full max-w ">
                <div x-show="loading" class="ml-2 ma-4" x-data="aclCommonSpinner({ text: 'Loading comments..' })"></div>

                <template x-for="(item, i) in commentItems" :key="item.id">
                    <div>  
                        <div 
                            x-data="aclSocialCardChatPost(getCommentWithMenuActions(item))"
                            @on:click:replies="(ev) => { toggleReplies(item) }"
                        ></div>

                        <!-- Replies toggle -->
                        <div class="flex flex-col max-w rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800">
                            
                            <!--Empty column-->    
                            <div x-show="showline"
                                class="w-14 sm:w-10 flex-shrink-0 flex items-center justify-center">
                            </div>
                          
                            <!-- Reply expander --> 
                            <div class="w-full flex-col max-w full-w">

                                <!-- Reply card -->
                                <template x-if="hasReplies(item)">
                                    <div
                                        x-show="!item.toggle"
                                        @click="toggleReplies(item)" 
                                        x-data="aclSocialCardReplies({
                                            ...item.replies,
                                            text: 'Show replies',
                                            active: true
                                        })">
                                    </div>
                                </template>
                                <!-- Close reply list -->
                                <div x-show="item.toggle">
                                    <div @click="toggleReplies(item)" x-data="aclSocialCardReplies({ text: 'Hide replies' })"></div>
                                </div>

                                <template x-if="item.toggle">
                                    <div class="flex flex-col max-w dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800">
                                        <!-- Reply list -->
                                        <div 
                                            x-data="aclSocialListChatPosts({
                                                url: mxFetch_url,
                                                params: mxFetch_params,
                                                mode: 'thread',
                                                showline: true,
                                                searchOnInit: true,
                                                commentItems: [],
                                                menuItems: commentMenu,
                                                actionItems: commentActions,
                                            })"
                                        ></div> 
                                    </div>
                                </template>
                                    
                                <hr class="pt-1 pb-1" x-show="item.toggle" />
                            </div> 
                            
                        </div>

                    </div>
                </template>
            </div>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      }
    }
}