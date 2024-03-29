export function emit(wssEvent, ev, data) {
    const event = `${wssEvent}:${ev}`
    window.dispatchEvent(new CustomEvent(event, { detail: data }));
};