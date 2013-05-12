using System.Collections.Generic;
using MvcApplication1.Helpers;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using Raven.Client.Linq;
using System.Linq;

namespace MvcApplication1.Services
{
    public class ExportEventAttendees
    {
        public long EventId { get; set; }
    }

    public class ExportEventAttendeesResponse : IHasResponseStatus
    {
        public class Result
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Notes { get; set; }

            public override string ToString()
            {
                return FirstName + " " + LastName;
            }
        }
        public IEnumerable<Result> Names { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ExportEventAttendeesService : ServiceBase<ExportEventAttendees>, IRequiresRequestContext
    {
        protected override object Run(ExportEventAttendees request)
        {
            using (var session = Application.DocumentStore.OpenSession())
            {
                var attendeesForEvent = session.Query<EventAttendees>().Where(e => e.EventId == request.EventId)
                    .OrderByDescending(e => e.CreationDate)
                    .FirstOrDefault();

                if (attendeesForEvent == null)
                    return HttpError.NotFound("No attendees registered for event id " + request.EventId);

                var elements = attendeesForEvent.Attendees
                    .Where(a => a.present)
                    .Select(a => new ExportEventAttendeesResponse.Result {Email = a.email, FirstName = a.first_name, LastName = a.last_name})
                    .ToArray();

                return new ExportEventAttendeesResponse {Names = elements};
            }
        }
    }
}