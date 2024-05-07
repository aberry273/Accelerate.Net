//import alpinejs from 'https://cdn.skypack.dev/alpinejs';
import alpinejs from './alpine.esm.js'

// Load webSockets components
//TODO: Switch to service based loading for each config item instead
import * as sockets from './websockets/index.js';
Object.keys(sockets).forEach(socket => {
    let settings = wssSettings[socket]
    let data = sockets[socket](settings);
    alpinejs.store(socket, data);
});
/*
import { wssService } from './websockets/index.js';
Object.keys(wssSettings).forEach(wss => {
    let setting = wssSettings[wss]
    let service = wssService(setting);
    alpinejs.store(service, setting);
});
*/
// Load stores
import * as stores from './stores/index.js';
Object.keys(stores).forEach(store => {
    let data = stores[store]();
    alpinejs.store(store, data);
});

// Load bindings
import * as bindings from './bindings/index.js';
Object.keys(bindings).forEach(binding => {
    let data = bindings[binding]();
    alpinejs.bind(binding, () => data);
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

// Load data components, prefix with _
import * as comps from './data/index.js';
Object.keys(comps).forEach(component => {
    alpinejs.data('_'+component, comps[component]);
});

// Load mixins, prefix with
import * as mixins from './mixins/index.js';
Object.keys(mixins).forEach(mixin => {
    alpinejs.data(mixin, mixins[mixin]);
});

// Load rendering components
import * as components from './components/index.js';
Object.keys(components).forEach(component => {
    alpinejs.data(component, components[component]);
});

window.alpinejs = alpinejs
alpinejs.start();

