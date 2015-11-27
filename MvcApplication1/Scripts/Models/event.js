

var Event = function (json) {
    if (!(this instanceof Event)) {
        return new Event();
    }
    var self = this;
    // yea, I know, this sucks. we should have a separate "builder" from persistend and eventbrite data
    var title = json.title || json.name.text;
    self.id = ko.observable(json.id);
    self.title = ko.observable(title);
    self.date = ko.observable(json.date);

    return self;
};

var extend_with_events = function(self) {
    self.events = ko.observableArray([]);
    self.load_events_from_storage = function () {
        self.events.removeAll();
        var serializedEvents = localStorage.getItem("events");
        if (serializedEvents) {
            var eventsJson = $.parseJSON(serializedEvents);
            _.each(eventsJson, function (e) { self.events.push(new Event(e)); });
        }
    };

    self.refresh_events_data = function () {
        if (!self.eb_client) {
            self.status("client is not initialized iet");
            return;
        }
        self.status("loading data");
        self.eb_client.user_list_events(function (response) {
            self.events.removeAll();

            var orderedEvents = response.events.sort(function (a, b) {
                var aDate = parse_date_from_eventbrite(a.start.local);
                var bDate = parse_date_from_eventbrite(b.start.local);
                if (aDate > bDate)
                    return -1;
                if (aDate < bDate)
                    return 1;
                return 0;
            });
            for (var i = 0; i < orderedEvents.length; i++) {
                var event = orderedEvents[i];
                event.date = parse_date_from_eventbrite(event.start.local);
                self.events.push(new Event(event));
            }
            self.status(" " + response.events.length + " events loaded");

            persist_array("events", self.events());

        });
    };
    
    return self;
};

var extend_with_event = function (self) {

    self.event_id = ko.observable(parseInt(localStorage.getItem("last_event_id"), 10) || 3609095903);

    self.event_id.subscribe(function (newValue) {
        localStorage.setItem("last_event_id", newValue);
    });
    
    self.event = ko.computed(function () {
        if (!self.event_id()) return null;
        var events = $.grep(self.events(), function (e) {
            return e.id() == self.event_id();
        });
        if (events.length > 0) return events[0];
        return null;
    });

    self.event_key = ko.computed(function () {
        if (self.event_id()) {
            return self.event_id().toString() + "_attendees";
        }
        return null;
    });
    

    return self;
};