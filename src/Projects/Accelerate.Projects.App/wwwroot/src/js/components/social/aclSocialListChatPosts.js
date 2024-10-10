import { mxContent, mxIcon, mxButton, mxList, mxFetch, mxEvent, mxSocial } from '/src/js/mixins/index.js';

export default function (params) {
    return {
        ...mxContent(params),
        ...mxIcon(params),
        ...mxButton(params),
        ...mxList(params),
        ...mxFetch(params),
        ...mxEvent(params),
        ...mxSocial(params),
        // PROPERTIES
        tooltipArrow: true,
        tooltipPosition: '',
        // redirect for navigating the user to the comment page
        // inline for opening up replies on click of replies 
        loading: false,
        key: 'aclSocialListChatPosts',
        // INIT
        async init() {
            this._mxSocial_SetParams(params);
            this.setParams(params);
            this.render();
            if (this.mxSocial_websockets) await this._mxSocial_WssPushItem();
            if (this.mxSocial_searchOnInit) await this._mxSocial_Search();
        },
        // GETTERS 
        get modeThread() {
            return this.mxSocial_mode == 'thread'
        },
        get modeInline() {
            return this.mxSocial_mode == 'inline'
        },
        get toggleEnabled() {
            return this.mxSocial_postItems.filter(x => x.toggle).length > 0
        },
        // METHODS
        setParams(params) {
            this.key = params.key;
        },
        replies() {
            return JSON.parse(JSON.stringify(replyItems))
        },
        getRandomInt(max) {
            return Math.floor(Math.random() * max);
        },
        render() {
            /*
            x-show="!toggleEnabled || (toggleEnabled && item.toggle)"
            */
            const html = `
            <div class="flex flex-col w-full full-h">
                <div x-show="loading" class="ml-2 ma-4" x-data="aclCommonSpinner({ text: 'Loading comments..' })"></div> 
            
                <template x-for="(item, i) in mxSocial_postItems" :key="key+':'+item.id">
                    <div>
                        <div 
                            x-data="aclSocialCardChatPost(_mxSocial_GetCommentWithMenuActions(item))"
                            @on:click:replies="(ev) => { _mxSocial_TogglePostReplies(item) }"
                        ></div>

                        <hr x-show="(toggleEnabled && item.toggle)" />

                        <!-- Replies toggle -->
                        <div class="flex flex-col max-w rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800"
                            :class="(toggleEnabled && item.toggle) ? 'bg-slate-100' : ''">
                            
                            <!--Empty column-->    
                            <div x-show="mxSocial_showline"
                                class="w-14 sm:w-10 flex-shrink-0 flex items-center justify-center">
                            </div>
                          
                            <!-- Reply expander --> 
                            <div class="w-full flex-col max-w full-w">
                                <!-- Reply card -->
                                <template x-if="_mxSocial_HasReplies(item)">
                                    <div
                                        x-show="!item.toggle"
                                        @click="_mxSocial_TogglePostReplies(item)" 
                                        x-data="aclSocialCardReplies({
                                            ...item.replies,
                                            text: 'Show replies',
                                            active: true
                                        })">
                                    </div>
                                </template>
                                <!-- Close reply list -->
                                <div x-show="item.toggle">
                                    <div @click="_mxSocial_TogglePostReplies(item)" x-data="aclSocialCardReplies({ text: 'Hide replies' })"></div>
                                </div>

                                <template x-if="item.toggle">
                                    <div class="flex flex-col max-w dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800">
                                        <!-- Reply list -->
                                        <div
                                            class="ml-2"
                                            x-data="aclSocialListChatPosts({
                                                url: mxSocial_url,
                                                params: {
                                                    filters: [
                                                        {
                                                            name: 'related.parent',
                                                            value: item.id,
                                                        }
                                                    ],
                                                    itemsPerPage: 1,
                                                    page: 0,
                                                    query: null,
                                                    userId: mxSocial_userId
                                                },
                                                userId: mxSocial_userId,
                                                mode: 'thread',
                                                showline: true,
                                                showReplies: mxSocial_showReplies,
                                                searchOnInit: true,
                                                postItems: [],
                                                menuItems: commentMenu,
                                                actionItems: commentActions,
                                            })"
                                        ></div>
                                        <hr />
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