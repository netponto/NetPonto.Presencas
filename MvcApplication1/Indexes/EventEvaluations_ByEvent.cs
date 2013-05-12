using System;
using MvcApplication1.Services;
using Raven.Client.Indexes;
using System.Linq;

namespace MvcApplication1.Indexes
{
    public class EventEvaluations_ByEvent : AbstractIndexCreationTask<EventEvaluations, EventEvaluations_ByEvent.Result>
    {
        public class Result
        {
            public long Version { get; set; }
            public long EventId { get; set; }
            public string EventTitle { get; set; }
            public long Count { get; set; }
            public DateTime CreationDate { get; set; }
        }

        public EventEvaluations_ByEvent()
        {
            Map = evaluations => from eventEvaluation in evaluations
                                 from eval in eventEvaluation.Evaluations
                                 select new
                                     {
                                         Version = long.Parse(eventEvaluation.Id.Split('/')[2]),
                                         EventId = eventEvaluation.EventId,
                                         CreationDate = eventEvaluation.CreationDate,
                                         EventTitle = eventEvaluation.Event.Title,
                                         Count = 1
                                     };

            Reduce = result => from r in result
                               group r by new {r.EventId, r.EventTitle}
                               into g
                               let version = g.Max(m => m.Version)
                               let creationDate = g.First(m => m.Version == version).CreationDate
                               select new
                                   {
                                       g.Key.EventId,
                                       g.Key.EventTitle,
                                       CreationDate = creationDate,
                                       Version = version,
                                       Count = g.Where(e => e.Version == version).Sum(e => e.Count),
                                   };

        }
    }
}