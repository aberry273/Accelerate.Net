const defaults = {}
import { mxCardPost } from '/src/js/mixins/index.js';
export default function (data) {
    return {
        // mixins
        ...mxCardPost(data),
        // properties
        html: '',
        init() {
            this.item = data.item;
            this._mxCardPost_init();
            const self = this;
            this.load(this.data)
        },
        get selectedPost() {
            return this.mxCardPost_thread[this.currentPage] || {}
        },
        get totalAgrees() {
            return this.mxCardPost_thread.reduce((sum, item) => sum + item.agrees, 0);
        },
        get totalDisagrees() {
            return this.mxCardPost_thread.reduce((sum, item) => sum + item.disagrees, 0);
        },
        get totalReplies() {
            return this.mxCardPost_thread.reduce((sum, item) => sum + item.replies, 0);
        },
        get totalLikes() {
            return this.mxCardPost_thread.reduce((sum, item) => sum + item.likes, 0);
        },
        get quotedPosts() {
            if (this.mxCardPost_selectedPost == null || this.mxCardPost_selectedPost.quoteIds == null) return [];
            return this.mxCardPost_selectedPost.quoteIds.filter(x => x != this.mxCardPost_selectedPost.shortThreadId);
        },
        load(data) {
            const html = `
            <article  class="dense padless" :class="articleClass" :id="selectedPost.threadId">

                <!--End Header-->
                <template x-if="mxCardPost_quotedPosts.length > 0">
                    <div class=" blockquote dense" style="padding-left:4px;">
                        <summary class="primary">

                            <template x-for="quote in mxCardPost_quotedPosts">
                                <a style="text-decoration:none" @click="_mxCardPost_filterByThreadId(quote)">
                                    <sup class="primary">
                                        <strong x-text="quote"></strong>,
                                    </sup>
                                </a>
                            </template>
                        </summary>
                    </div>
                </template>

                <div :class="mxCardPost_quotedPosts.length > 0 ? ' content' : 'content'">
                <!--Header-->
                    <header class="padded">
                        <nav>
                            <template x-if="selectedPost.profile != null">
                                <ul class="profile">
                                    <template x-if="selectedPost.profile.image != null && selectedPost.profile.image.length>0">
                                        <li>
                                            <button class="avatar small">
                                                <img 
                                                    :src="selectedPost.profile.image+'?w=40'"
                                                />
                                            </button>
                                        </li>
                                    </template>
                                    <aside>
                                        <li class="secondary pa-0" style="padding-top:0px;">
                                            <strong class="pb-0">
                                                <span x-text="selectedPost.profile.username"></span>
                                            </strong>
                                        </li>
                                     </aside>
                                </ul>
                            </template>
                            <ul> 
                                <li>
                                     <strong x-show="selectedPost.channelName">
                                        <a class="py-0 primary my-0" style='text-decoration:none' :href="'/channels/'+selectedPost.targetChannel">
                                            <sup x-text="selectedPost.channelName"></sup>
                                        </a>
                                    </strong>

                                   <i x-show="!showMetadata && selectedPost.tags" aria-label="Show more" @click="showMetadata = true" :class="false ? 'primary': ''" class="icon material-icons icon-click" rel="prev">expand_more</i>
                                   <i x-show="showMetadata && selectedPost.tags" aria-label="Show more" @click="showMetadata = false" :class="false ? 'primary': ''" class="icon material-icons icon-click" rel="prev">expand_less</i>
                                    <!--Show more-->
                                    <template x-if="selectedPost.userId == userId">
                                        <details class="dropdown flat no-chevron">
                                            <summary role="outline">
                                                <i aria-label="Close" class="icon material-icons icon-click" rel="prev">more_vert</i>
                                            </summary>
                                            <ul dir="rtl">
                                                <li><a class="click" @click="modalAction('share')">Share</a></li>
                                                <li><a class="click" @click="modalAction('edit')">Edit</a></li>
                                                <li><a class="click" @click="modalAction('delete')">Delete</a></li>
                                            </ul>
                                        </details>
                                    </template>
                                </li>
                            </ul>
                        </nav>
                    </header>
                    <!--Text Content-->
                    <template x-for="(post, i) in mxCardPost_thread" :key="i">
                        <div x-show="i == currentPage">
                            <div class="padded pt-0 pb-0" x-html="post.content"></div>
                        </div>
                    </template>
                    <!--End Text Content-->

                    <template x-if="selectedPost.media != null && selectedPost.media.length > 0">
                        <div x-data="gridCardMedia( {
                                userId: selectedPost.userId,
                                itemEvent: $store.wssContentPosts.getMessageEvent(),
                                items: selectedPost.media,
                                modalId: 'media-modal'+selectedPost.id,
                                cols: 3
                            })">
                        </div>
                    </template>


                    <!-- Pagination-->
                    <template x-if="mxCardPost_thread.length > 1">
                    <div class="grid" align="center">
                        <ul>
                        <template x-for="page in mxCardPost_thread.length">
                            <i @click="currentPage = page-1" :class="currentPage == page-1 ? 'primary': ''" class="icon material-iconss icon-click">•</i>
                        </template>
                        </ul>
                    </div>
                    </template>
                    <!--End Pagination-->

                    <!--Ratings-->
                    <footer class="padded">
                        <nav>
                            <ul>
                                <li>
                                    <!--Replies-->
                                    <strong class="py-0 my-0">
                                        <a class="py-0 secondary my-0" style='text-decoration:none' :href="selectedPost.href">
                                            <small>
                                                <small>
                                                    <span x-text="selectedPost.shortThreadId"></span>
                                                    <template x-if="selectedPost.replies > 0">
                                                        <span x-text="'('+selectedPost.replies+')'"></span>
                                                    </template>
                                                </small>
                                            </small>
                                        </a>
                                    </strong>
                                </li>
                            </ul> 
                            <ul>
                                <li>
                                    <!--Agree-->
                                    <i x-show="!userId" style="padding: 4px" aria-label="Agree" class="icon material-icons" rel="Agree">expand_less</i>
                                    <i x-show="userId" aria-label="Agree" :href="selectedPost.id" @click="_mxCardPost_action('agree', selectedPost)" :class="_mxCardPost_userSelectedAction('agree', selectedPost) ? 'primary': ''" class="icon material-icons icon-click" rel="prev">expand_less</i>
                                    <sup class="noselect" rel="prev" x-text="selectedPost.agrees || 0"></sup>
                                   
                                    <!--Disagree-->
                                    <i x-show="!userId" style="padding: 4px" aria-label="Agree" class="icon material-icons" rel="Disagree">expand_more</i>
                                    <i x-show="userId" aria-label="Disagree" @click="_mxCardPost_action('disagree', selectedPost)" :class="_mxCardPost_userSelectedAction('disagree', selectedPost) ? 'primary': ''" class="icon material-icons icon-click" rel="prev">expand_more</i>
                                    <sup class="noselect" rel="prev"x-text="selectedPost.disagrees || 0"></sup>
                                 
                                    <!--Likes-->
                                    <!--
                                    <i @click="_mxCardPost_action('like', selectedPost)" aria-label="Liked" :class="_mxCardPost_userSelectedAction('like', selectedPost) ? 'primary': ''" class=" icon material-icons icon-click" rel="prev">favorite</i>
                                    <sup class="noselect" rel="prev" x-text="selectedPost.likes || 0 "></sup>
                                    -->
                                    
                                    <!--Quotes-->
                                    <i x-show="!userId" style="padding: 4px" aria-label="Agree" class="icon material-icons" rel="Quote">format_quote</i>
                                    <i x-show="userId" aria-label="Quote" @click="_mxCardPost_quote(selectedPost)" :class="false ? 'primary': ''" class="icon material-icons icon-click" rel="prev">format_quote</i>
                                    <sup class="noselect" rel="prev" x-text="selectedPost.quotes || 0"></sup>

                                </li>
                            </ul>
                        </nav>
                        <nav x-show="showMetadata && selectedPost.tags">
                            <ul>
                                <li>
                                    <div class=" chips">
                                        <template x-for="(tag, i) in selectedPost.tags">
                                            <a @click="_mxCardPost_filterByTag(tag)" style="text-decoration:none" class="tag flat closable primary small">
                                                <strong><sup x-text="tag"</sup></strong>
                                            </a>
                                        </template>
                                    </div>
                                </li>
                            </ul>
                        </nav>
                    </footer>
                    <!--End Ratings-->
                </div>
            </article>
        `
            this.html = html;
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
    }
}