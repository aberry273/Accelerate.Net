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
            this.$nextTick(() => {
                this.load(self.data)
            })
            this.$events.on(this.updateEvent, (item) => {
                if (item.id != this.item.id) return;
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
        scrollTo(id) {
            const el = document.getElementById(id);
            el.scrollIntoView()
        },
        renderPost(post) {
            if (post.type == 'image') return cardRenderingText(post)
            if (post.type == 'video') return cardRenderingText(post)
            if (post.type == 'mixed') return cardRenderingText(post)
            if (post.type == 'text') return cardRenderingText(post)
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
        filterByThreadId(threadId) {
            const filter = {
                name: 'Quotes',
                values: [threadId]
            };
            this.$events.emit(this.filterEvent, filter)
        },
        load(data) {
            const html = `
            <article class="dense padless" :class="articleClass" :id="selectedPost.threadId">
              <!--Header-->  
              <header class="padded">
                <nav>
                    <ul>
                        <template x-if="selectedPost.profile != null">
                            <li>
                                <button class="round small primary img">
                                <img
                                class="circular"
                                :src="selectedPost.profile"
                                :alt="selectedPost.username"
                                />
                                </button>
                            </li>
                        </template>
                        <aside class="dense">
                            <li class="secondary">
                                <strong x-text="selectedPost.username"></strong> 
                            </li> 
                        </aside>
                    </ul>
                    <ul> 
                        <li>
                            <!--Show more-->
                                <sup><strong x-text="selectedPost.shortThreadId"></strong> </sup>
                            
                            <details class="dropdown flat no-chevron">
                                <summary role="outline">
                                    <i aria-label="Close" class="icon material-icons icon-click" rel="prev">more_vert</i>
                                </summary>
                                <ul dir="rtl">
                                    <li><a class="click"  @click="showMetadata = !showMetadata">Tags</a></li>
                                    <li><a class="click" @click="modalAction('share')">Share</a></li>
                                    <li><a class="click" @click="modalAction('edit')">Edit</a></li>
                                    <li><a class="click" @click="modalAction('delete')">Delete</a></li>
                                </ul>
                            </details>
                        </li>
                    </ul>
                </nav>
              </header>
              <!--End Header-->
              <template x-if="selectedPost.quoteIds && selectedPost.quoteIds.length > 0">
                 <div class="dense blockquote primary">
                    <summary class="primary">
                        <template x-for="quote in selectedPost.quoteIds">
                            <a style="text-decoration:none" @click="filterByThreadId(quote)">
                                <sup class="primary">
                                     <strong x-text="quote"></strong>,
                                </sup>
                            </a>
                        </template>
                    </summary>
                </div>
            </template>
            <!--Content-->
            <template x-for="(post, i) in thread" :key="i"> 
                <div class="content" x-show="i == currentPage" x-html="renderPost(post, i)" ></div>
            </template>
            <!--End content-->

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
                            <!--Agree-->
                            <i aria-label="Agree" :href="selectedPost.id" @click="action('agree')" :class="userAction('agree', selectedPost) ? 'primary': ''" class="icon material-icons icon-click" rel="prev">expand_less</i>
                            <sup class="noselect" rel="prev" x-text="selectedPost.agrees || 0"></sup>
                            <!--Disagree-->
                            <i aria-label="Disagree" @click="action('disagree')" :class="userAction('disagree', selectedPost) ? 'primary': ''" class="icon material-icons icon-click" rel="prev">expand_more</i>
                            <sup class="noselect" rel="prev"x-text="selectedPost.disagrees || 0"></sup>
                            <!--Likes-->
                            <i @click="action('like')" aria-label="Liked" :class="userAction('like', selectedPost) ? 'primary': ''" class=" icon material-icons icon-click" rel="prev">favorite</i>
                            <sup class="noselect" rel="prev" x-text="selectedPost.likes || 0 "></sup> 
                        </li>
                      </ul> 
                      <ul>
                        <li>
                            <!--Quotes-->
                            <i aria-label="Quote" @click="action('quote')" :class="false ? 'primary': ''" class="icon material-icons icon-click" rel="prev">format_quote</i>
                            <sup class="noselect" rel="prev" x-text="selectedPost.quotes || 0"></sup>

                            <!--Replies-->
                            <a style="text-decoration:none;" :href="'/Content/thread/'+selectedPost.id">
                            <i aria-label="Reply" :class="false ? 'primary': ''" class="icon material-icons icon-click"  rel="prev">comment</i>
                            <sup class="noselect" rel="prev" x-text="selectedPost.replies || 0"></sup>
                            </a>
                        </li>
                      </ul>
                  </nav>
                  <nav x-show="showMetadata && selectedPost.tags">
                    <ul>
                      <li>
                        <div class="container">
                        <template x-for="(tag, i) in selectedPost.tags">
                          <button class="tag flat secondary small" x-text="tag"></button>
                        </template>
                      </div>
                      </li>
                    </ul>
                  </nav>
              </footer>
              <!--End Ratings-->
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