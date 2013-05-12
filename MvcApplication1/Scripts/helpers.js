var flatten_observable = function (item) {
    return ko.toJS(item);
};

var flatten_observables = function (items) {
    return $.map(items, function (item) {
        return flatten_observable(item);
    });
};

var persist_array = function (key, items) {
    var stringify = JSON.stringify(flatten_observables(items));
    localStorage.setItem(key, stringify);
};

var persist_attendee = function (attendee) {
    var key = attendee.storage_key();
    localStorage.setItem(key, JSON.stringify(flatten_observable(attendee)));
};

var parse_date_from_eventbrite = function(str) {
    // format is like "2010-05-15 09:30:00";
    var a = str.split(/[^0-9]/);
    var d = new Date(a[0], a[1] - 1, a[2], a[3], a[4], a[5]);
    return d;
};

var generate_random_chars = function (n) {
    if (n < 1) {
        return "";
    }
    return _.foldl(_.map(_.range(n), function (x) { return 'x'; }), function (x, y) { return x + y; })
        .replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
};