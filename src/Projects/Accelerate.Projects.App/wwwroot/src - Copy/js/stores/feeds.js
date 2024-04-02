export default () => ({
    // PROPERTIES
    // INIT
    init() {},
    // METHODS
    items: defaults
})

const defaults = [
    {
        id: 0,
        username: 'John deere',
        profile: 'https://placehold.co/150x150',
        handle: '@jdeerer',
        updated: '5 minutes ago',
        content: '<p><strong>title</strong></p><p>this is a new test of a auto-formatted markdown</p>',
        feed: 'Recipes',
        liked: false,
        agree: 22,
        disagree: 1,
        footer: 'footer',
    },
    {
        id: 1,
        profile: 'https://placehold.co/150x150',
        username: 'don doon',
        handle: '@dundoon',
        updated: '12 minutes ago',
        content: '<p>If any text contains no line breaks then we get everything in a full block of text</p>',
        agree: 16,
        disagree: 15,
        liked: true,
        feed: 'AI updates',
        footer: 'footer',
        favourited: true,
    },
    {
        id: 3,
        profile: 'https://placehold.co/150x150',
        username: 'jokarr',
        handle: '@johndoker',
        updated: '15 minutes ago',
        content: '<p><strong>what is the latest with ai</strong></p><p>here is a bunch of text to format</p>',
        feed: 'AI updates',
        liked: true,
        agree: 16,
        disagree: 15,
        footer: 'footer',
    },
    {
        id: 4,
        profile: 'https://placehold.co/150x150',
        username: 'tommy1989',
        handle: '@tommy1989',
        updated: '3 days ago',
        content: '<img src="https://placehold.co/2000x2000" alt="username_profile" />',
        feed: 'AI updates',
        liked: false,
        agree: 16,
        disagree: 15,
        footer: 'footer',
    },
];