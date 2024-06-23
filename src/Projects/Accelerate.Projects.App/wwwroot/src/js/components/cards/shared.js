const defaults = {}
import { mxCardPost } from '/src/js/mixins/index.js';
/*
Previously content was paginated, now only quotes
export function content(data) {
    return `
    <template x-for="(post, i) in mxCardPost_thread" :key="i">
        <div x-show="i == currentPage">
            <div class="padded pt-0 pb-0" x-html="post.content"></div>
        </div>
    </template>
    `
}
*/
export function content(data) {
    return `
        <div class="pt-0 pb-0" x-html="_mxCardPost_ParseEncodedTextElements(selectedPost.content)"></div>
    `
}

export function media(data) {
    return `
    <template x-if="selectedPost.media != null && selectedPost.media.length > 0">
        <div x-data="cardMedia( {
                userId: selectedPost.userId,
                items: selectedPost.media,
                modalId: 'media-modal'+selectedPost.id,
                cols: 3
            })">
        </div>
    </template>
    `
}

export function quotes(data) {
    return `
    <template x-for="(quotedPost, i) in quotedPosts" :key="i">
        <div class="padded" x-show="i == currentPage">
            <div x-data="cardPostQuote({
                quotePost: quotedPost,
                item: _mxCardPost_getQuotePost(quotedPost.contentPostQuoteId)
            })"></div>
            <div x-show="quotedPost.response" x-html="quotedPost.response" class="pt"></div>
        </div>
    </template>
    `
}
export function pagination(data) {
    return `
   <template x-if="quotedPosts.length > 0">
        <div class="grid" align="center">
            <ul>
                <template x-for="page in quotedPosts.length">
                    <i @click="currentPage = page-1" :class="currentPage == page-1 ? 'primary': ''" class="icon material-iconss icon-click">•</i>
                </template>
            </ul>
        </div>
    </template>
    `
}
export function link(data) {
    return `
    <template x-if="selectedPost.link">
        <article @click="" class="link" style="padding: 0px;" x-show="!selectedPost.link.hide">
            <header class="padless"><button style="position:absolute" class="small secondary material-icons flat" @click="selectedPost.link.hide = true">clear</button></header>
            <div>
                <hr />
                <figure style="text-align:center;">
                    <a style="text-decoration:none" :href="selectedPost.link.url">
                        <img
                            style="max-height: 200px; border-radius: 8px"
                            :src="selectedPost.link.image"
                            :alt="selectedPost.link.title"
                        />
                    </a>
                </figure>
                <div class="padless" style="padding: 0px 8px;">
                    <a style="text-decoration:none" :href="selectedPost.link.url">
                        <sup x-text="selectedPost.link.url"></sup>
                    </a>
                    <div>
                        <b x-text="selectedPost.link.title"></b>
                        <p x-text="selectedPost.link.description"></p>
                    </div>
                </div>
            </div>
        </article>
    </template>
    `
}

export function header(data) {
    return `
    <header class="padded pb-0">
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
                            <small class="pl muted noselect" x-show="selectedPost.date"><em><small x-text="selectedPost.date"></small></em></small>
                        </li>
                    </aside>
                </ul>
            </template>
            <ul>
                <li>
                    <i class="material-icons muted noselect" x-show="selectedPost.status == 0">visibility_off</i>
                    <strong x-show="selectedPost.channelName">
                        <a class="py-0 primary my-0" style='text-decoration:none' :href="'/channels/'+selectedPost.channelId">
                            <sup x-text="selectedPost.channelName"></sup>
                        </a>
                    </strong>

                     <!--Show more-->
                    <template x-if="selectedPost.userId == userId">
                        <details class="dropdown flat no-chevron">
                            <summary role="outline">
                                <i aria-label="Close" class="icon material-icons icon-click" rel="prev">more_vert</i>
                            </summary>
                            <ul dir="rtl">
                                <li x-show="!showMetadata && selectedPost.tags"><a class="click" @click="showMetadata = true">Show tags</a></li>
                                <li x-show="showMetadata && selectedPost.tags"><a class="click" @click="showMetadata = false">Hide tags</a></li>

                                <li><a class="click" @click="_mxCardPost_modalAction('share', selectedPost)">Share</a></li>
                                <li><a class="click" @click="_mxCardPost_modalAction('copy', selectedPost)">Copy Link</a></li>
                                <li><a class="click" @click="_mxCardPost_modalAction('edit', selectedPost)">Edit</a></li>
                                <li><a class="click" @click="_mxCardPost_modalAction('delete', selectedPost)">Delete</a></li>
                            </ul>
                        </details>
                    </template>
                </li>
            </ul>
        </nav>
    </header>
    `
}

export function footer(data) {
    return `
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
                                </small>
                            </small>
                        </a>
                    </strong>
                </li>
            </ul>
            <ul>
                <li>
                    <!--
                    <i x-show="!showMetadata && selectedPost.tags" aria-label="Show more" @click="showMetadata = true" :class="false ? 'primary': ''" class="icon material-icons icon-click" rel="prev">unfold_more</i>
                    <i x-show="showMetadata && selectedPost.tags" aria-label="Show more" @click="showMetadata = false" :class="false ? 'primary': ''" class="icon material-icons icon-click" rel="prev">unfold_less</i>
                    -->
                    <!--Agree-->
                    <button :disabled="!userId" @click="_mxCardPost_action('agree', selectedPost)" class="chip small " style="" :class="_mxCardPost_userSelectedAction('agree', selectedPost) ? 'flat primary': 'flat'" >
                        <i aria-label="Agree" class="icon material-icons">expand_less</i>
                        <sup class="noselect" x-text="selectedPost.agrees || 0"></sup>
                    </button>

                    <!--Disagree-->
                    <button :disabled="!userId" @click="_mxCardPost_action('disagree', selectedPost)" class="chip small " style="" :class="_mxCardPost_userSelectedAction('disagree', selectedPost) ? 'flat primary': 'flat'" >
                        <i aria-label="Disagree" class="icon material-icons">expand_more</i>
                        <sup class="noselect" x-text="selectedPost.disagrees || 0"></sup>
                    </button>

                    <!--Likes-->
                    <!--
                    <i @click="_mxCardPost_action('like', selectedPost)" aria-label="Liked" :class="_mxCardPost_userSelectedAction('like', selectedPost) ? 'primary': ''" class=" icon material-icons icon-click" rel="prev">favorite</i>
                    <sup class="noselect" rel="prev" x-text="selectedPost.likes || 0 "></sup>
                    -->

                    <!--Quotes-->
                    <button :disabled="!userId" @click="_mxCardPost_quote(selectedPost)" class="chip small flat" style="" >
                        <i aria-label="Quote" class="icon material-icons">format_quote</i>
                        <sup class="noselect" x-text="selectedPost.quotes || 0"></sup>
                    </button>

                    <!--Reply-->
                    <button :disabled="!userId" @click="_mxCardPost_reply(selectedPost)" class="chip small flat" style="" >
                        <i aria-label="Reply" class="icon material-icons">chat</i>
                        <sup class="noselect" x-text="selectedPost.replies || 0"></sup>
                    </button>
                </li>
            </ul>
        </nav>
        <hr x-show="showMetadata" />
        <nav x-show="showMetadata">
                <ul style="width: 100%" >
                    <li class="pl">
                        <label><sup>Category</sup></label>
                        <a href="#" style="text-decoration:none" class="tag flat closable primary small">
                            <strong><sup x-text="selectedPost.category"</sup></strong>
                        </a>
                    </li>
             
                    <li class="pl">
                        <div class=" chips">
                            <label><sup>Tags</sup></label>
                            <template x-for="(tag, i) in selectedPost.tags">
                                <a @click="_mxCardPost_filterByTag(tag)" style="text-decoration:none" class="tag closable secondary small">
                                    <strong><sup x-text="tag"</sup></strong>
                                </a>
                            </template>
                        </div>
                    </li>
                </ul>
        </nav>
    </footer>
    `
}

/// Quote specific
export function contentQuote(data) {
    return `
    <template x-for="(post, i) in mxCardPost_thread" :key="i">
        <div x-show="i == currentPage" class="content truncated">
            <div class="padded pt-0 pb-0" x-html="post.content"></div>
        </div>
    </template>
    `
}
export function headerQuote(data) {
    return `
    <header class="padded pb-0">
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
                        <a class="py-0 primary my-0" style='text-decoration:none' :href="'/channels/'+selectedPost.channelId">
                            <sup x-text="selectedPost.channelName"></sup>
                        </a>
                    </strong>
                </li>
            </ul>
        </nav>
    </header>
    `
}
export function footerQuote(data) {
    return ` 
        <footer class="">
            <nav>
                <ul>
                    <li>
                        <!--Replies-->
                        <strong class="py-0 my-0">
                            <a class="py-0 secondary my-0" style='text-decoration:none' :href="selectedPost.href">
                                <small>
                                    <small>
                                        <span x-text="item.shortThreadId"></span>
                                    </small>
                                </small>
                            </a>
                        </strong>
                    </li>
                </ul>
                <ul>
                    <li>
                        <!--Agree-->
                        <button :disabled="!userId" @click="_mxCardPost_action('agree', selectedPost)" class="chip small " style="" :class="_mxCardPost_userSelectedAction('agree', selectedPost) ? 'flat primary': 'flat'" >
                            <i aria-label="Agree" class="icon material-icons">expand_less</i>
                            <sup class="noselect" x-text="selectedPost.agrees || 0"></sup>
                        </button>

                        <!--Disagree-->
                        <button :disabled="!userId" @click="_mxCardPost_action('disagree', selectedPost)" class="chip small " style="" :class="_mxCardPost_userSelectedAction('disagree', selectedPost) ? 'flat primary': 'flat'" >
                            <i aria-label="Disagree" class="icon material-icons">expand_more</i>
                            <sup class="noselect" x-text="selectedPost.disagrees || 0"></sup>
                        </button>
                    </li>
                </ul>
            </nav>
        </footer>`
}