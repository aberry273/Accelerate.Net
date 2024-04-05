
const exampleThread = {
    "id": "3016f819-7c6c-4667-9875-59f27981a4c9",
    "createdOn": "2024-04-01T21:09:29.9859812Z",
    "updatedOn": "2024-04-02T18:43:26.6405323+13:00",
    "threadId": "GfgWMGx8Z0aYdVnyeYGkyQ",
    "userId": "b2edca69-6743-4f71-969c-08dc4f9da45c",
    "parentId": null,
    "status": 0,
    "content": "wdqqwdqwd",
    "targetThread": null,
    "targetChannel": null,
    "category": null,
    "tags": null,
    "reviews": {
        "agrees": 1,
        "disagrees": 1,
        "likes": 0,
        "replies": null
    },
    "username": "tester",
    "agrees": 1,
    "disagrees": 1,
    "likes": 0,
    "replies": 4
}

export default () => ({
    // PROPERTIES
    thread: {},
    posts: [],
    replies: [],
    reviews: [],
    // INIT
    init() {
    },
    getDummyThread() {
        return exampleThread
    },
    setThread(post) {
        this.thread = post;
    },
    setPosts(posts) {
        this.posts = posts;
    },
    addPosts(posts) {
        this.posts.concat(posts);
    },
    addPost(post) {
        this.posts.push(post);
    },
    setReviews(reviews) {
        this.reviews = reviews;
    },
    addReviews(reviews) {
        this.reviews.concat(reviews);
    },
    addReview(review) {
        this.posts.push(review);
    },
    addReply(posts) {
        this.posts.concat(posts);
    },
    addPost(post) {
        this.posts.push(post);
    },
    updatePosts(wssMessage) {
        const item = wssMessage.data;
        if (this.posts == null) this.posts = [];
        if (wssMessage.update == 'Created') {
            const index = this.posts.map(x => x.id).indexOf(item.id);
            if (index == -1) this.posts.push(item);
            else this.posts[index] = item
        }
        if (wssMessage.update == 'Updated') {
            const index = this.posts.map(x => x.id).indexOf(item.id);
            this.posts[index] = item
        }
        if (wssMessage.update == 'Deleted') {
            const index = this.posts.map(x => x.id).indexOf(item.id);
            this.posts.splice(index, 1);
        }
    },
    updateReviews(wssMessage) {
        const item = wssMessage.data;
        if (this.reviews == null) this.reviews = [];
        if (wssMessage.update == 'Created') {
            const index = this.reviews.map(x => x.id).indexOf(item.id);
            if (index == -1) this.reviews.push(item);
            else this.reviews[index] = item
        }
        if (wssMessage.update == 'Updated') {
            const index = this.reviews.map(x => x.id).indexOf(item.id);
            this.reviews[index] = item
        }
        if (wssMessage.update == 'Deleted') {
            const index = this.reviews.map(x => x.id).indexOf(item.id);
            this.reviews.splice(index, 1);
        }
    },
})
