let CEvent = window.CEvent;

if (typeof CEvent === "undefined") {
    CEvent = class CEvent {
        listeners;


        constructor(listener) {
            this.listeners = [];

            if (listener) {
                this.add(listener);
            }
        }


        add(listener) {
            this.listeners.push(listener);
        }

        dispatch(event) {
            for (let i = 0; i < this.listeners.length; i++) {
                this.listeners[i](event);
            }
        }
    }

    window.CEvent = CEvent;
}

export { CEvent };
