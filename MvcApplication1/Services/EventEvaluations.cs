using System;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace MvcApplication1.Services
{
    public class EventEvaluations
    {
        public string Id { get; set; }
        public long EventId { get; set; }
        public Event Event { get; set; }
        public DateTime CreationDate { get; set; }
        public Evaluation[] Evaluations { get; set; }

        public EventEvaluations()
        {
            Evaluations = new Evaluation[]{};
            CreationDate = DateTime.UtcNow;
        }
    }

    public class Evaluator
    {
        public long id { get; set; }
        public long order_id { get; set; }
        public long event_id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class Evaluation
    {
        public Evaluator user { get; set; }

        public int first_presentation_speaker { get; set; }
        public int first_presentation_content { get; set; }
        public string first_presentation_comments { get; set; }

        public int second_presentation_speaker { get; set; }
        public int second_presentation_content { get; set; }
        public string second_presentation_comments { get; set; }

        public int event_according_to_expectations { get; set; }
        public int event_place { get; set; }
        public int event_room { get; set; }
        public int event_break { get; set; }
        public int event_registration { get; set; }
        public int event_organization { get; set; }
        public string event_comments { get; set; }
    }


    public class EventEvaluationsResponse : IHasResponseStatus
    {
        public EventEvaluations Result { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EventEvaluationsService : ServiceBase<EventEvaluations>
    {
        protected override object Run(EventEvaluations request)
        {
            if (request.Event == null) throw new ArgumentException("Event cannot be null");

            using (var session = Application.DocumentStore.OpenSession())
            {
                request.Id = "EventEvaluations/" + request.EventId + "/";
                session.Store(request);
                session.SaveChanges();
            }

            return new EventEvaluationsResponse { Result = request };
        }
    }
}