using System;
using Raven.Client;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;

namespace MvcApplication1.Services
{
    public class EventAttendees
    {
        public string Id { get; set; }
        public long EventId { get; set; }
        public Event Event { get; set; }
        public DateTime CreationDate { get; set; }
        public Attendee[] Attendees { get; set; }

        public EventAttendees()
        {
            Attendees = new Attendee[]{};
            CreationDate = DateTime.UtcNow;
        }
    }

    public class Attendee
    {
        public long id { get; set; }
        public long order_id { get; set; }
        public long event_id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool present { get; set; }
        public bool joins_for_lunch { get; set; }
    }

    public class EventAttendeesResponse : IHasResponseStatus
    {
        public EventAttendees Result { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EventAttendeesService : ServiceBase<EventAttendees>
    {
        protected override object Run(EventAttendees request)
        {
            using (var session = Application.DocumentStore.OpenSession())
            {
                request.Id = "EventAttendees/" + request.EventId + "/";
                session.Store(request);
                session.SaveChanges();
            }

            return new EventAttendeesResponse { Result = request };
        }
    }
    
}