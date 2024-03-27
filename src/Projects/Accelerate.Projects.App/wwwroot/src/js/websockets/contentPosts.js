export default function (wssPath){
  return {
    socket: null,
    event: 'websocket:contentPost:update',
    init() {
        const self = this;
        console.log(self)
        console.log('socketInit')
        let socket = new WebSocket("wss://localhost:7220/ContentPosts");
        //let socket = new WebSocket(wssPath);

        socket.onopen = function (e) {
            console.log("[open] Connection established");

            const endChar = String.fromCharCode(30);

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
            if(data == null) return;
            const cleaned = data.replace('\u001e', '')
            if(cleaned == null) return;
            
            const message = JSON.parse(cleaned);
            
            if(message.type == 1) {
                // Handle responses
                const user = message.arguments[0];
                const payload = message.arguments[1];
                window.dispatchEvent(new CustomEvent(self.event, { detail: payload }));
                window.dispatchEvent(new CustomEvent('snackbar-success', { detail: { target: 'New post created!' } }));
            };
        };

        socket.onclose = function (event) {
            if (event.wasClean) {
                console.log(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
            } else {
                console.log('[close] Connection died');
            }
        };

        socket.onerror = function (error) {
            console.log(`[error] ${error.message}`);
        };

        this.socket = socket;
    },
  }
}