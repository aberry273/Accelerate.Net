import { mxIcon } from '/src/js/mixins/index.js';
import {
    userCircle, user, userGroup, userPlus, tag, cog, dots, adjustmentsHorizontal, adjustmentsVertical, creditCard, expand, keyboard,
    chevronUp,
    chevronDown,
    chevronLeft,
    chevronRight,
    chevronUpDown,
    chatBubble,
    chatBubbles,
    calendar,
    link,
    trash,
    edit,
    close,
    closeCircle,
    filter,
    send,
    ellipsisHorizontal,
    ellipsisVertical,
    ellipsisCircle,
    menu,
    code,
    bold,
    quote,
    italic,
    mention,
    emoji,
    image,
    video,
    presentationChart,
    documentText,
    facebook,
    google,
    microsoft,
    openSide,
    openNew,
    queueList,
    funnel,
    magnifyingGlass,
} from './heroIconsLibrary.js';


export default function (params) {
    return {
        ...mxIcon(params),
        // PROPERTIES
        open: false,
        // INIT
        init() {
            this.mxIcon_class = params.mxIcon_class;
            this.mxIcon_name = params.mxIcon_name;
            this.render();
        },
        // GETTERS
        // METHODS
        getSvg(icon, mxIcon_class) {
            if (icon == 'userCircle') return userCircle(mxIcon_class)
            if (icon == 'user') return user(mxIcon_class)
            if (icon == 'userGroup') return userGroup(mxIcon_class)
            if (icon == 'userPlus') return userPlus(mxIcon_class)
            if (icon == 'tag') return tag(mxIcon_class)
            if (icon == 'adjustmentsHorizontal') return adjustmentsHorizontal(mxIcon_class)
            if (icon == 'adjustmentsVertical') return adjustmentsVertical(mxIcon_class)
            if (icon == 'creditCard') return creditCard(mxIcon_class)
            if (icon == 'expand') return expand(mxIcon_class)
            if (icon == 'keyboard') return keyboard(mxIcon_class)
            if (icon == 'cog') return cog(mxIcon_class)
            if (icon == 'funnel') return funnel(mxIcon_class)
            if (icon == 'queueList') return queueList(mxIcon_class)
            if (icon == 'dots') return dots(mxIcon_class)
            if (icon == 'chevronUp') return chevronUp(mxIcon_class)
            if (icon == 'chevronDown') return chevronDown(mxIcon_class)
            if (icon == 'chevronLeft') return chevronLeft(mxIcon_class)
            if (icon == 'chevronRight') return chevronRight(mxIcon_class)
            if (icon == 'chevronUpDown') return chevronUpDown(mxIcon_class)
            if (icon == 'chatBubble') return chatBubble(mxIcon_class)
            if (icon == 'chatBubbles') return chatBubbles(mxIcon_class)
            if (icon == 'calendar') return calendar(mxIcon_class)
            if (icon == 'trash') return trash(mxIcon_class)
            if (icon == 'link') return link(mxIcon_class)
            if (icon == 'edit') return edit(mxIcon_class)
            if (icon == 'menu') return menu(mxIcon_class)
            if (icon == 'close') return close(mxIcon_class)
            if (icon == 'closeCircle') return closeCircle(mxIcon_class)
            if (icon == 'filter') return filter(mxIcon_class)
            if (icon == 'send') return send(mxIcon_class)
            if (icon == 'ellipsisHorizontal') return ellipsisHorizontal(mxIcon_class)
            if (icon == 'ellipsisVertical') return ellipsisVertical(mxIcon_class)
            if (icon == 'ellipsisCircle') return ellipsisCircle(mxIcon_class)
            if (icon == 'code') return code(mxIcon_class)
            if (icon == 'bold') return bold(mxIcon_class)
            if (icon == 'quote') return quote(mxIcon_class)
            if (icon == 'italic') return italic(mxIcon_class)
            if (icon == 'mention') return mention(mxIcon_class)
            if (icon == 'emoji') return emoji(mxIcon_class)
            if (icon == 'image') return image(mxIcon_class)
            if (icon == 'video') return video(mxIcon_class)
            if (icon == 'presentationChart') return presentationChart(mxIcon_class)
            if (icon == 'documentText') return documentText(mxIcon_class)
            if (icon == 'facebook') return facebook(mxIcon_class)
            if (icon == 'google') return google(mxIcon_class)
            if (icon == 'microsoft') return microsoft(mxIcon_class)
            if (icon == 'openSide') return openSide(mxIcon_class)
            if (icon == 'openNew') return openNew(mxIcon_class)
            if (icon == 'magnifyingGlass') return magnifyingGlass(mxIcon_class)
            return ''
        },
        render() {
            let html = this.getSvg(this.mxIcon_name, this.mxIcon_class);
            this.$nextTick(() => { this.$root.innerHTML = html });
        }
    }
}