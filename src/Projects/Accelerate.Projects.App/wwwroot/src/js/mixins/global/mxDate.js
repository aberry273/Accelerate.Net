const customDateFormatter = new Intl.DateTimeFormat("en-US", {
    dateStyle: "short",
    timeStyle: "short",
});
export default function (data) {
    return {
        // PROPERTIES
        init() {
        },
        // GETTERS
        // METHODS
        _mxDate_FormatDate(dtStr) {
            
            var date = new Date(dtStr);
            return customDateFormatter.format(date) 
        },
        _mxDate_FormatFriendly(dtStr) {
            var date = new Date(dtStr);
            // Make a fuzzy time
            var delta = Math.round((+new Date - date) / 1000);

            var minute = 60,
                hour = minute * 60,
                day = hour * 24,
                week = day * 7;

            var fuzzy;

            if (delta < 30) {
                fuzzy = 'Just now.';
            } else if (delta < minute) {
                fuzzy = delta + ' seconds';
            } else if (delta < 2 * minute) {
                fuzzy = 'a minute ago.'
            } else if (delta < hour) {
                fuzzy = Math.floor(delta / minute) + ' minutes ago.';
            } else if (Math.floor(delta / hour) == 1) {
                fuzzy = '1 hour ago.'
            } else if (delta < day) {
                fuzzy = Math.floor(delta / hour) + ' hours ago.';
            } else if (delta < day * 2) {
                fuzzy = 'yesterday';
            }
            return fuzzy;
        },
    }
}