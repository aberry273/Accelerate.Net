const defaults = {}
import cardRenderingText from './cardRenderingText.js'
export default function (data) {
    return {
        currentPage: 0,
        thread: [],
        item: null,
        userId: null,
        updateEvent: '',
        actionEvent: 'action:post',
        modalEvent: 'action:post',
        redirectEvent: 'action:post',
        filterEvent: 'on:filter:posts',
        showMetadata: false,
        showReplies: false,
        articleClass: '',
        quotedPost: null,
        init() {
            this.articleClass = data.class;
            this.item = data.item;
            this.userId = data.userId;
            this.updateEvent = data.updateEvent,
            this.thread = this.setThreadItems(data.item);

            const self = this;
            this.load(this.data)
          
            this.$events.on(this.updateEvent, (item) => {
                if (this.item.id != item.id) return;
                this.item = item;
                this.thread = this.setThreadItems(item);
            });

        },
        setThreadItems(op) {
            // flatten parent and child hierarchy into single array
            let thread = [op].concat((op.threads == null) ? [] : op.threads)
            return thread;
        },
        action(action) {
            const payload = {
                // postback type
                action: action,
                // content post item
                item: this.selectedPost,
                userId: this.userId,
            }
            this.$events.emit(this.actionEvent, payload)
        },
        redirectAction(action) {
            const ev = `redirect-${action}`;
            this.$events.emit(ev, this.selectedPost)
        },
        modalAction(action) {
            const ev = `modal-${action}-post`;
            const payload = {
                // route to append to postbackUrl 
                postbackUrlRoute: this.selectedPost.id,
                // postback type
                postbackType: 'PUT',
                // content post item
                item: this.selectedPost,
            }
            this.$events.emit(ev, payload)
        },
        get selectedPost() {
            return this.thread[this.currentPage]
        },
        get totalAgrees() {
            return this.thread.reduce((sum, item) => sum + item.agrees, 0);
        },
        get totalDisagrees() {
            return this.thread.reduce((sum, item) => sum + item.disagrees, 0);
        },
        get totalReplies() {
            return this.thread.reduce((sum, item) => sum + item.replies, 0);
        },
        get totalLikes() {
            return this.thread.reduce((sum, item) => sum + item.likes, 0);
        },
        get quotedPosts() {
            if (this.selectedPost.quoteIds == null) return [];
            return this.selectedPost.quoteIds.filter(x => x != this.selectedPost.shortThreadId);
        },
        scrollTo(id) {
            const el = document.getElementById(id);
            el.scrollIntoView()
        },
        renderPost(post) {
            if (!post.content) return null;
            return cardRenderingText(post)
        },
        userAction(item, action) {
            return false;
        },
        cleanTargetThread(post) {
            if (!post.targetThread) return post.shortThreadId
            var ids = post.targetThread.split('|')
            if (ids.length == 1) return post.shortThreadId
            // get the actual post id
            var last = ids.shift();
            // remove original post item
            //ids.shift();
            var shortenedIds = ids.map(x => this.getTinyThreadId(x));
            return shortenedIds.join('/') + "/" + post.shortThreadId;
        },
        getTinyThreadId(threadId) {
            return threadId.slice(0, 8);
        },
        getImage(filePath) {
            if (this.imageWidth) {
                return filePath + '?w=' + this.imageWidth;
            }
            return filePath;
        },
        /*
        modalAction(action, data) {
            this.$events.emit(this.modalEvent, data)
        },
        */
        filterByThreadId(threadId) {
            const filters =
                [
                    {
                        name: 'Quotes',
                        values: [threadId]
                    }
                ]
            this.$events.emit(this.filterEvent, filters)
        },
        filterByTag(tag) {
            const filters =
                [
                    {
                        name: 'Tags',
                        values: [tag]
                    }
                ]
            this.$events.emit(this.filterEvent, filters)
        },
        load(data) {
            const html = `
            <article class="dense padless" :class="articleClass" :id="selectedPost.threadId">

                <!--End Header-->
                <template x-if="quotedPosts.length > 0">
                    <div class=" blockquote dense" style="padding-left:4px;">
                        <summary class="primary">

                            <template x-for="quote in quotedPosts">
                                <a style="text-decoration:none" @click="filterByThreadId(quote)">
                                    <sup class="primary">
                                        <strong x-text="quote"></strong>,
                                    </sup>
                                </a>
                            </template>
                        </summary>
                    </div>
                </template>

                <div :class="quotedPosts.length > 0 ? ' content' : 'content'">
                <!--Header-->
                    <header class="padded">
                        <nav>
                            <template x-if="selectedPost.profile != null">
                                <ul class="profile">
                                    <template x-if="selectedPost.profile.image != null">
                                        <li>
                                            <button class="avatar small">
                                                <img 
                                                    :src="selectedPost.profile.image+'?w=40'"
                                                    :alt="selectedPost.profile.username"
                                                />
                                            </button>
                                        </li>
                                    </template>
                                    <aside>
                                        <li class="secondary pa-0">
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
                                    <template x-if="item.userId == userId">
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
                    <template x-for="(post, i) in thread" :key="i"> 
                        <div x-show="i == currentPage" x-html="renderPost(post, i)" ></div>
                    </template>
                    <!--End Text Content-->

                    <template x-if="selectedPost.media != null && selectedPost.media.length > 0">
                        <div x-data="gridCardMedia( {
                                userId: '@Model.UserId',
                                itemEvent: $store.wssContentPosts.getMessageEvent(),
                                items: selectedPost.media,
                                modalId: 'media-modal'+selectedPost.id,
                                cols: 3
                            })">
                        </div>
                    </template>


                    <!-- Pagination-->
                    <template x-if="thread.length > 1">
                    <div class="grid" align="center">
                        <ul>
                        <template x-for="page in thread.length">
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
                                    <i aria-label="Agree" :href="selectedPost.id" @click="action('agree')" :class="userAction('agree', selectedPost) ? 'primary': ''" class="icon material-icons icon-click" rel="prev">expand_less</i>
                                    <sup class="noselect" rel="prev" x-text="selectedPost.agrees || 0"></sup>
                                    <!--Disagree-->
                                    <i aria-label="Disagree" @click="action('disagree')" :class="userAction('disagree', selectedPost) ? 'primary': ''" class="icon material-icons icon-click" rel="prev">expand_more</i>
                                    <sup class="noselect" rel="prev"x-text="selectedPost.disagrees || 0"></sup>
                                 
                                    <!--Likes-->
                                    <!--
                                    <i @click="action('like')" aria-label="Liked" :class="userAction('like', selectedPost) ? 'primary': ''" class=" icon material-icons icon-click" rel="prev">favorite</i>
                                    <sup class="noselect" rel="prev" x-text="selectedPost.likes || 0 "></sup>
                                    -->
                                    
                                    <!--Quotes-->
                                    <i aria-label="Quote" @click="action('quote')" :class="false ? 'primary': ''" class="icon material-icons icon-click" rel="prev">format_quote</i>
                                    <sup class="noselect" rel="prev" x-text="selectedPost.quotes || 0"></sup>

                                </li>
                            </ul>
                        </nav>
                        <nav x-show="showMetadata && selectedPost.tags">
                            <ul>
                                <li>
                                    <div class=" chips">
                                        <template x-for="(tag, i) in selectedPost.tags">
                                            <a @click="filterByTag(tag)" style="text-decoration:none" class="tag flat closable primary small">
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
            this.$nextTick(() => {
                this.$root.innerHTML = html
            });
        },
        defaults() {
            this.load(defaults)
        }
    }
}