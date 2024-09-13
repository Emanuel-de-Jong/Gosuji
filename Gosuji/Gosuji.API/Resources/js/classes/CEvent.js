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
            for (const listener of this.listeners) {
                listener(event);
            }
        }

        async dispatchAsync(event) {
            for (const listener of this.listeners) {
                await listener(event);
            }
        }
    }

    window.CEvent = CEvent;
}

export { CEvent };
