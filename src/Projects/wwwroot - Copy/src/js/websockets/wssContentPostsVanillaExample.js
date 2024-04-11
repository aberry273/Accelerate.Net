import { emit } from './utilities.js'
const wssEvent = 'wss:contentPosts';
const url = "wss://localhost:7220/ContentPosts";

export default function (settings, user) {
    return {
        socket: null,
        settings: {},
        user: {},
        init() {
            this.settings = settings;
            this.user = user;
            const self = this;
            let socket = new WebSocket(settings.url);
            //let socket = new WebSocket(wssPath);
            
            socket.onopen = function (e) {
                console.log("[open] Connection established");
                console.log(e);
                const endChar = String.fromCharCode(30);
                emit(wssEvent, 'onopen', e);
                
                self.sendMessage("SendMessage", self.user.id, "LOL");

                // send the protocol & version
                socket.send(`{"protocol":"json","version":1}${endChar}`);
            };
            const curlyBracesInclusiveRegex = /\{([^}]+)\}/
            socket.onmessage = function (event) {
                console.log(`[message] Data received from server: ${event.data}`);

                // parse server data
                const serverData = event.data.substring(0, event.data.length - 1);

                // after sending the protocol & version subscribe to your method(s)
                if (serverData === "{}") {
                    const endChar = String.fromCharCode(30);
                    socket.send(`{"arguments":[],"invocationId":"0","target":"Your-Method","type":1}${endChar}`);
                    return;
                }

                // handle server messages
                const data = event.data;
                if (data == null) return;
                const cleaned = data.replace('\u001e', '')
                if (cleaned == null) return;

                const message = JSON.parse(cleaned);

                if (message.type == 1) {
                    // Handle responses
                    const payload = {
                        user: message.arguments[0],
                        // {code, type, data}
                        data: message.arguments[1]
                    }
                    emit(wssEvent, 'onmessage', payload);
                };
            };

            socket.onclose = function (event) {
                if (event.wasClean) {
                    console.log(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
                } else {
                    console.log('[close] Connection died');
                }
                emit(wssEvent, 'onclose', event);
            };

            socket.onerror = function (error) {
                console.log(`[error] ${error.message}`);
                emit(wssEvent, 'onerror', error);
            };

            this.socket = socket;
        },
        sendMessage(command = "SendMessage", user, message) {
            this.socket.send({command, user, message})
        }
    }
}