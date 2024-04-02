export default function (data) {
    return {
        items: [],
        async init() {
            this.postItems = data != null ? data.postItems : [];
            this.reviewItems = data != null ? data.reviewItems : [];

            await this.setupPostWebsockets();
            await this.setupReviewWebsockets();

            const postQuery = this.createPostQuery(data);
            if (postQuery != null)
                await this.fetchPosts(postQuery);

            const reviewQuery = this.createReviewQuery(data);
            if (reviewQuery != null)
                await this.fetchReviews(reviewQuery);

            // On updates from UI > contentPost/contentPostReply
            this.$events.on('action:post', async (e) => {
                await this.handleAction(e);
            })
        },
        async setupPostWebsockets() {
            // On websocket client initialized, send channel to server
            this.$events.on(this.$store.wssContentPosts.getConnectedEvent(), async (ev) => {
                await this.$store.wssContentPosts.connectUser(this.userId);

                if (this.targetThread) {
                    await this.$store.wssContentPosts.connectThread(this.userId, this.targetThread);
                }
                if (this.targetChannel) {
                    await this.$store.wssContentPosts.connectChannel(this.userId, this.targetChannel);
                }
            })
            // On updates from the websocket
            this.$events.on(this.$store.wssContentPosts.getMessageEvent(), async (e) => {
                const data = e.data;
                if (data.alert) this.sendAlert(data);
                this.updatePostItems(data);
            })
        },
        async setupReviewWebsockets() {
            // On websocket client initialized, send channel to server
            this.$events.on(this.$store.wssContentPostReviews.getConnectedEvent(), async (ev) => {
                await this.$store.wssContentPostReviews.connectUser(this.userId);

                if (this.targetThread) {
                    await this.$store.wssContentPostReviews.connectThread(this.userId, this.targetThread);
                }
                if (this.targetChannel) {
                    await this.$store.wssContentPostReviews.connectChannel(this.userId, this.targetChannel);
                }
            })
            // On updates from the websocket
            this.$events.on(this.$store.wssContentPostReviews.getMessageEvent(), async (e) => {
                const data = e.data;
                if (data.alert) this.sendAlert(data);
                this.updateReviewItems(data);
            })
        },
        updatePostItems(wssMessage) {
            const item = wssMessage.data;
            if (this.postItems == null) this.postItems = [];
            if (wssMessage.update == 'Created') {
                const index = this.postItems.map(x => x.id).indexOf(item.id);
                if (index == -1) this.postItems.push(item);
                else this.postItems[index] = item
            }
            if (wssMessage.update == 'Updated') {
                const index = this.postItems.map(x => x.id).indexOf(item.id);
                this.postItems[index] = item
            }
            if (wssMessage.update == 'Deleted') {
                const index = this.postItems.map(x => x.id).indexOf(item.id);
                this.postItems.splice(index, 1);
            }
        },
        updateReviewItems(wssMessage) {
            const item = wssMessage.data;
            if (this.reviewItems == null) this.reviewItems = [];
            if (wssMessage.update == 'Created') {
                const index = this.reviewItems.map(x => x.id).indexOf(item.id);
                if (index == -1) this.reviewItems.push(item);
                else this.reviewItems[index] = item
            }
            if (wssMessage.update == 'Updated') {
                const index = this.reviewItems.map(x => x.id).indexOf(item.id);
                this.reviewItems[index] = item
            }
            if (wssMessage.update == 'Deleted') {
                const index = this.reviewItems.map(x => x.id).indexOf(item.id);
                this.reviewItems.splice(index, 1);
            }
        },
        get posts() { return this.postItems },
        get reviews() { return this.reviewItems },
        agrees(post) {
            if (!this.reviews || this.reviews.length == 0) return false;
            const review = this.reviews.filter(x => x.userId == this.userId && x.contentPostId == post.id)[0];
            if (!review) return false;
            if (review.agree == null) return false
            return review.agree != null && review.agree == true;
        },
        disagrees(post) {
            if (!this.reviewItems || this.reviewItems.length == 0) return false;
            const review = this.reviewItems.filter(x => x.userId == this.userId && x.contentPostId == post.id)[0];
            if (!review) return false;
            return review.disagree != null && review.disagree == true;
        },
        likes(post) {
            if (!this.reviewItems || this.reviewItems.length == 0) return false;
            const review = this.reviewItems.filter(x => x.userId == this.userId && x.contentPostId == post.id)[0];
            if (!review) return false;
            return review.like != null && review.like == true;
        },
        sendAlert(data) {
            const snackbarType = (data.code == 200) ? 'success' : 'error';
            const wasSuccess = (data.code == 200) ? 'successfully' : 'failed';
            const message = `${data.update} post ${wasSuccess}`
            const event = 'snackbar-add';//`snackbar-${snackbarType}`;
            this.$events.emit(event, { code: data.code, type: snackbarType, text: message });
        },
        setPostItems(items) {
            this.postItems = items
        },
        createPostQuery(data) {
            if (!data) return;
            const filters = {};
            if (data.targetThread) {
                filters['targetThread'] = [data.targetThread];
            }
            let payload = {
                page: 0,
                itemsPerPage: 10,
                filters: filters,
            }
            return payload;
        },
        async fetchPosts(query) {
            if (!this.fetchPostsUrl || !query) return;
            const results = await this.$fetch.POST(this.fetchPostsUrl, query);
            this.postItems = results;
        },
        createReviewQuery(data) {
            if (!data) return;
            const filters = {};
            if (data.userId) {
                filters['userId'] = [data.userId];
            }
            let payload = {
                page: 0,
                itemsPerPage: 10,
                filters: filters,
            }
            return payload;
        },
        async fetchReviews(query) {
            if (!this.fetchReviewsUrl || !query) return;
            const results = await this.$fetch.POST(this.fetchReviewsUrl, query);
            this.reviewItems = results;
        },
        async handleAction(request) {
            const action = request.action;
            if (action == 'agree') {
                await this.postAgree(request);
            }
            if (action == 'disagree') {
                await this.postDisagree(request);
            }
            if (action == 'like') {
                await this.toggleLike(request);
            }
        },
        getOrCreatePayload(request) {
            if (this.reviewItems == null) return;
            const review = this.reviewItems.filter(x => x.userId == request.userId && x.contentPostId == request.item.id)[0];
            if (review != null) return review;
            return {
                userId: request.userId,
                contentPostId: request.item.id,
                agree: null,
                disagree: null,
                like: null,
            }
        },
        async postAgree(request) {
            const payload = this.getOrCreatePayload(request);
            await this.$fetch.POST(this.reviewUrl + '/agree', payload);
        },
        async postDisagree(request) {
            const payload = this.getOrCreatePayload(request);
            await this.$fetch.POST(this.reviewUrl + '/disagree', payload);
        },
        async toggleLike(request) {
            const payload = this.getOrCreatePayload(request);
            if (payload.like)
                await this.$fetch.POST(this.reviewUrl + '/unlike', payload);
            else
                await this.$fetch.POST(this.reviewUrl + '/like', payload);
        },
    }
}