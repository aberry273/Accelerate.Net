//Based on
//https://dev.to/keuller/build-modular-app-with-alpinejs-2ece

import alpinejs from 'https://cdn.skypack.dev/alpinejs';

// Load webSockets components
/*
import * as sockets from './websockets/index.js';
Object.keys(sockets).forEach(socket => {
    alpinejs.data(socket, sockets[socket]);
});
*/
import * as sockets from './websockets/index.js';
Object.keys(sockets).forEach(socket => {
    let data = sockets[socket]();
    alpinejs.store('wss_'+socket, data);
});

// Load data components, prefix with _
import * as comps from './data/index.js';
Object.keys(comps).forEach(component => {
    alpinejs.data('_'+component, comps[component]);
});



// Load bindings
import * as bindings from './bindings/index.js';
Object.keys(bindings).forEach(binding => {
    let data = bindings[binding]();
    alpinejs.bind(binding, () => data);
});
// Load stores
import * as stores from './stores/index.js';
Object.keys(stores).forEach(store => {
    let data = stores[store]();
    alpinejs.store(store, data);
});
// Load directives
import * as directives from './directives/index.js';
Object.keys(directives).forEach(directive => {
    let data = directives[directive];
    alpinejs.directive(directive, data);
});
// Load magics
import * as magics from './magics/index.js';
Object.keys(magics).forEach(magic => {
    let data = magics[magic];
    alpinejs.magic(magic, data);
});

// Load rendering components
import * as components from './components/index.js';
Object.keys(components).forEach(component => {
    alpinejs.data(component, components[component]);
});

/*
// Init AsyncAlpine + Alpine + Components
AsyncAlpine.init(window.alpinejs);
AsyncAlpine.alias(`src/js/components/[name].js`);
AsyncAlpine.start();
window.Alpine = alpine.js
*/
window.alpinejs = alpinejs
alpinejs.start();

