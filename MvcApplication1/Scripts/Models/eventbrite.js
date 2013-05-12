var extend_with_eventbrite =
    function(app_key, user_key) {
        return function(self) {
            Eventbrite({ 'app_key': app_key, 'user_key': user_key }, function(eb_client) {
                //eb_client interaction goes here... 
                self.eb_client = eb_client;
            });
            return self;
        };
    };