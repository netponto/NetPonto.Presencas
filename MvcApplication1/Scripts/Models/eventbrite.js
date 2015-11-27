var extend_with_eventbrite =
    function(oauth_token) {
    	function EventBrite(options){
    		var self = this;
    		var base_url = "https://www.eventbriteapi.com/v3"
    		var make_url = function(url){
    			var sep = "?";
    			if(url.indexOf("?") > -1) sep ="&";
    			return base_url + url + sep + "token="+oauth_token;
    		}
    		self.user_list_events = function(callback){
    			var url = make_url("/users/me/owned_events/?order_by=start_desc");
				$.ajax({
					url : url
				}).done(function(data){
					callback(data);
				}).fail(function(error){
					console.error("Error on user_list_events");
					console.error(error);
				});
    		};

    		self.event_list_attendees = function(options, callback){
				var id = options.id;	
				var attendees = [];

				var request_more = function(current_page){
					var url = make_url("/events/"+id+"/attendees/?page="+current_page);
					if(current_page > 3) return; // DEBUG
					$.ajax({
						url : url
					}).done(function(data){
						console.log(data);
						attendees = attendees.concat(data.attendees);
						if(data.pagination.page_count == data.pagination.page_number){
							callback({attendees : attendees});
						}else{
							request_more(current_page+1);
						}

					}).fail(function(error){
						console.error("Error on event_list_attendees");
						console.error(error);
					});
				}
				request_more(1);
    		};
    		return self;
    	}
        return function(self) {
        	self.eb_client = new EventBrite();
            return self;
        };
    };