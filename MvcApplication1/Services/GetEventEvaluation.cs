using System;
using System.Collections.Generic;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using Raven.Client.Linq;
using System.Linq;

namespace MvcApplication1.Services
{
    public class GetEventEvaluations
    {
        public long EventId { get; set; }
         
    }

    public class GetEventEvaluationsResponse : IHasResponseStatus
    {
        public IEnumerable<Evaluation> Evaluations { get; set; }
        public long EventId { get; set; }
        public DateTime CreationDate { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetEventEvaluationsService : ServiceBase<GetEventEvaluations>
    {
        protected override object Run(GetEventEvaluations request)
        {
            EventEvaluations evaluations;
            using (var session = Application.DocumentStore.OpenSession())
            {
                evaluations = session.Query<EventEvaluations>().Where(e => e.EventId == request.EventId)
                    .OrderByDescending(r => r.CreationDate).FirstOrDefault();
                if(evaluations == null)
                {
                    throw HttpError.NotFound("No evaluations stored for event " + request.EventId);
                }
            }

            return new GetEventEvaluationsResponse
                {
                    EventId = request.EventId,
                    CreationDate = evaluations.CreationDate,
                    Evaluations = evaluations.Evaluations
                };
        }
    }
}