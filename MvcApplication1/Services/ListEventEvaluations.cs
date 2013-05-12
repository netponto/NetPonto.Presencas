using MvcApplication1.Indexes;
using ServiceStack.ServiceInterface;
using Raven.Client.Linq;
using System.Linq;
using ServiceStack.ServiceInterface.ServiceModel;

namespace MvcApplication1.Services
{
    public class ListEventEvaluations
    {
    }

    public class ListEventEvaluationResponse : IHasResponseStatus
    {
        public EventEvaluations_ByEvent.Result[] Evaluations { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ListEventEvaluationsService : ServiceBase<ListEventEvaluations>
    {
        protected override object Run(ListEventEvaluations request)
        {
            using(var session = Application.DocumentStore.OpenSession())
            {
                return session.Query<EventEvaluations_ByEvent.Result, EventEvaluations_ByEvent>()
                    .OrderByDescending(r => r.CreationDate)
                    .Take(100);
            }
        }
    }
}