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
        
                    <div
                        x-data="aclSocialCardFeedPost(getCommentWithMenuActions(item))"
                        @on:click:replies="(ev) => { toggleReplies(item) }"
                    ></div>

                </template>
            </div>
            `
            this.$nextTick(() => { this.$root.innerHTML = html });
      }
    }
}