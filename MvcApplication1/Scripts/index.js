var Attendee = function (json) {
    if (!(this instanceof Attendee)) {
        return new Attendee();
    }
    var self = this;
    self.id = ko.observable(json.id);
    self.order_id = ko.observable(json.order_id);
    self.event_id = ko.observable(json.event_id);
    self.first_name = ko.observable(json.first_name || (json.profile && json.profile.first_name) || "");
    self.last_name = ko.observable(json.last_name || (json.profile && json.profile.last_name) || "");
    self.email = ko.observable(json.email || json.profile.email);
    self.present = ko.observable(json.present || false);
    self.joins_for_lunch = ko.observable(json.joins_for_lunch || false);


    
    self.set_present = function (present) {
        if (present == self.present()) {
            return;
        }
        if (!present) {
            self.joins_for_lunch(false);
        }
        self.present(present);
        $("body").trigger("attendee_change", [self, false]);
    };
    self.toggle = function (event) {
        if (self.present()) {
            if (self.joins_for_lunch()) {
                self.clear();
            } else {
                self.joins_for_lunch(true);
            }
        } else {
            self.present(true);
        }
        $("body").trigger("attendee_change", [self, true]);
        //event.preventDefault();
        //event.stopPropagation();
    };
    self.clear = function () {
        self.present(false);
        self.joins_for_lunch(false);
    };

    self.fullName = ko.computed($.proxy(function () {
        return self.first_name() + " " + self.last_name();
    }), this);

    self.gravatarUrl = ko.computed($.proxy(function () {
        return $.gravatarUrl(self.email());
    }), this);

    self.storage_key = ko.computed($.proxy(function () {
        return self.event_id() + "_attendee_" + self.id();
    }), this);

    return self;
};



var indexPageViewModelBuilder = function ($, ko, conf) {
    
    ko.bindingHandlers.presentAsClass = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            // First get the latest data that we're bound to
            var value = valueAccessor(), allBindings = allBindingsAccessor();

            // Next, whether or not the supplied model property is observable,
            // get its current value
            var valueUnwrapped = ko.utils.unwrapObservable(value);

            // Now manipulate the DOM element
            if (valueUnwrapped == true) {
                $(element).addClass("present"); // Make the element visible
            } else {
                $(element).removeClass("present");   // Make the element invisible
            }
        }
    };
    
    var PageViewModel = function(extensions) {
        var self = this;
        extensions = extensions || [];
        _.each(extensions, function(ext) { ext(self); });


        self._set_attendee_presence = function(attendee_id, is_present) {
            var attendee = _.find(self.attendees(), function (att) {
                return att.id() == attendee_id;
            });
            if (!attendee) {
                console.error("Notified of missing attendee with id " + attendee_id);
                return;
            }
            attendee.set_present(is_present);
        };

        // Declare a proxy to reference the hub. 
        self._checkInHub = $.connection.checkInHub;
        // Create a function that the hub can call to broadcast messages.
        self._checkInHub.client.broadcastMessage = function (id, eventid, is_present) {
            console.log("Hey, id " + id + " at " + eventid + " is present: " + is_present);

            self._set_attendee_presence(id, is_present);
        };
        

        self.event_id.subscribe(function(newValue) {
            self.load_attendees_from_storage();
        });

        self.export_event_attendees_selected_format = ko.observable("txt");

        self.export_event_attendees_link = ko.computed(function() {
            return "/api/exporteventattendees/" + self.event_id() + "?format="+self.export_event_attendees_selected_format();
        });
        
        self.export_event_attendees_formats = ko.observableArray([
            { format: 'txt', name: 'Plain text (just names, for raffle)' },
            { format: 'excel', name: 'Excel with emails (for survs)' }
        ]);
        

        self.attendees = ko.observableArray([]);
        self.load_attendees_from_storage = function() {
            self.attendees.removeAll();
            var eventId = self.event_id();
            if (!eventId) return;

            var keyRe = eventId.toString() + "_attendee_.+";
            var attendees = [];
            for (var key in localStorage) {
                if (key.match(keyRe)) {
                    attendees.push(new Attendee($.parseJSON(localStorage[key])));
                }
            }
            _.each(_.sortBy(attendees, function(attendee) { return attendee.fullName(); }),
                function(a) { self.attendees.push(a); });
        };

        self.load_attendees_from_storage();
        self.load_events_from_storage();
        
        $("body").bind("attendee_change", function(event, attendee, broadcast) {
            self.attendee_change(attendee);
        });

        var local_user_name = localStorage["local_user_name"];
        if (!local_user_name) {
            local_user_name = generate_random_chars(12);
        }

        self.local_user_name = ko.observable(local_user_name);
        self.local_user_name.subscribe(function (newVal) {
            localStorage["local_user_name"] = newVal;
        });
        
        self.unregistered_attendee = ko.observable("");

        self.add_unregistered_attendee = function() {
            var id = new Date().getTime() + 90000000000000;
            var order_id = new Date().getTime() + 80000000000000;
            var new_attendee = {
                id: id,
                order_id: order_id,
                event_id: self.event_id(),
                email: self.unregistered_attendee() + "_" + id + "@@unregistered.netponto.org",
                first_name: self.unregistered_attendee(),
                last_name: "",
                present: true,
                joins_for_lunch: false
            };
            var attendee = new Attendee(new_attendee);
            persist_attendee(attendee);
            self.attendees.push(attendee);
            self.unregistered_attendee("");
        };

        self.attendee_change = function(attendee) {
            self.status("saving attendee" + attendee.id() + " at " + (new Date()));
            persist_attendee(attendee);
        };

        self.status = ko.observable("ok");
        self.filter = ko.observable("");

        self.number_attendees = ko.computed(function() {
            return self.attendees().length;
        });

        self.number_attendees_present = ko.computed(function() {
            return _.filter(self.attendees(), function(a) { return a.present(); }).length;
        });

        self.number_attendees_for_lunch = ko.computed(function() {
            return _.filter(self.attendees(), function(a) { return a.joins_for_lunch(); }).length;
        });

        self.grouped_attendees = ko.computed(function() {

            var items = _.sortBy(_.map(_.groupBy(self.attendees(),
                function(item) { return item.fullName().toLowerCase()[0]; }),
                function(items) {
                    return function() {
                        var s = {};
                        s.group = ko.observable(items[0].fullName()[0].toUpperCase());
                        s.attendees = ko.observable(items);
                        return s;
                    }();
                }),
                function(item) { return item.group(); });
            if (items.length > 0) {
                $(".atteendee_group_shortcut").css("height", Math.floor(100 / (items.length + 2)) + "%");
            }

            return items;
        });

        self.refresh_attendees_data = function() {
            if (!self.eb_client) {
                self.status("client is not initialized iet");
                return;
            }
            if (confirm("Are you sure you want to refresh attendees data for event " + self.event_id())) {
                self.status("loading data");

                self.eb_client.event_list_attendees({ 'id': self.event_id() }, function(response) {
                    self.attendees.removeAll();
                    for (var i = 0; i < response.attendees.length; i++) {
                        var attendee = new Attendee(response.attendees[i]);
                        self.attendees.push(attendee);
                        persist_attendee(attendee);
                    }
                    self.status(" " + response.attendees.length + " attendees loaded");
                });
            }

        };


        self.nuke_all_local_data = function() {
            localStorage.clear();
        };

        self.nuke_status = function() {
            var attendees = self.attendees();
            $.each(attendees, function() {
                this.present(false);
                this.joins_for_lunch(false);
            });

            persist_array(self.event_key(), attendees);
            self.load_attendees_from_storage();
        };

        self.save_remote = function() {
            self.status("saving data to remote service");
            var data = JSON.stringify({
                EventId: self.event_id(),
                Event: flatten_observable(self.event()),
                Attendees: flatten_observables(self.attendees())
            });
            $.ajax({
                url: '/api/eventattendees',
                type: 'POST',
                data: data,
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).success(function(data, textStatus, jqXHR) {
                var result = data.result;
                self.status("saved remote event " + result.id + " with " + result.attendees.length + " attendees");
            }).error(function() {
                self.status("error: " + arguments);
            });
        };
        
        // Start the connection.
        $.connection.hub.start().done(function () {
            $("body").bind("attendee_change", function (event, attendee, broadcast) {
                if(broadcast) {
                    self._checkInHub.server.checkIn(self.local_user_name(), attendee.id(), attendee.event_id(), attendee.present());
                }
            });
        });

        self._sync_checkins_from_server = function (event_id) {
            self.status("sync'ing checkins for event id " + event_id);
            $.ajax({
                url: '/api/loadsyncevents/' + event_id,
                type: 'GET',
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).success(function(data, textStatus, jqXHR) {
                _.each(data, function(syncEvent) {
                    self._set_attendee_presence(syncEvent.checkedInUserId, syncEvent.isPresent);
                });
                self.status("sync'ed event id " + event_id);
            }).error(function () {
                self.status("error syncing events: " + arguments);
            });;
        };
        self._sync_checkins_from_server(self.event_id());
        
        self.event_id.subscribe(function (newValue) {
            self._sync_checkins_from_server(newValue);
        });
    };

    var pageViewModel = new PageViewModel([extend_with_eventbrite(conf.eventbrite.oauth_token), extend_with_events, extend_with_event]);

    ko.applyBindings(pageViewModel);
    return pageViewModel;
};
