using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using Raven.Client.Linq;
using System.Linq;

namespace MvcApplication1.Services
{
    public class ExportEventEvaluations
    {
        public long EventId { get; set; }
    }

    public class ExportEventEvaluationsService : ServiceBase<ExportEventEvaluations>
    {
        protected override object Run(ExportEventEvaluations request)
        {
            using(var session = Application.DocumentStore.OpenSession())
            {
                var evals =  session.Query<EventEvaluations>()
                    .Where(e => e.EventId == request.EventId)
                    .OrderByDescending(e => e.CreationDate)
                    .FirstOrDefault();

                if (evals == null) throw HttpError.NotFound(string.Format("No evals for id {0}", request.EventId));

                var elements = evals.Evaluations.Select(e => new
                    {
                        e.user.first_name,
                        e.user.last_name,
                        e.user.id,
                        e.user.email,
                        first_presentation_speaker = NullIfMinusOne(e.first_presentation_speaker),
                        first_presentation_content = NullIfMinusOne(e.first_presentation_content),
                        e.first_presentation_comments,
                        second_presentation_speaker = NullIfMinusOne(e.second_presentation_speaker),
                        second_presentation_content = NullIfMinusOne(e.second_presentation_content),
                        e.second_presentation_comments,
                        event_according_to_expectations = NullIfMinusOne(e.event_according_to_expectations),
                        event_place = NullIfMinusOne(e.event_place),
                        event_room = NullIfMinusOne(e.event_room),
                        event_break = NullIfMinusOne(e.event_break),
                        event_registration = NullIfMinusOne(e.event_registration),
                        event_organization = NullIfMinusOne(e.event_organization),
                        e.event_comments,
                    }).ToList();

                return elements;
            }
        }

        private int? NullIfMinusOne(int val)
        {
            if (val == -1) return null;
            return val;
        }
    }
}