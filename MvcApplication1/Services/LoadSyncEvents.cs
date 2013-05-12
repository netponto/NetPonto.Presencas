using System;
using System.Collections.Generic;
using ServiceStack.ServiceHost;
using System.Linq;

namespace MvcApplication1.Services
{
    public class LoadSyncEvents : IReturn<List<SyncCheckIn>>
    {
        public long EventId { get; set; }
    }

    public class SyncCheckIn
    {
        public string Id { get; set; }
        public string CheckerUserId { get; set; }
        public long CheckedInUserId { get; set; }
        public long EventId { get; set; }
        public bool IsPresent { get; set; }
        public DateTimeOffset SyncCheckInDate { get; set; }
    }

    public class LoadSyncEventsService : IService
    {
        public List<SyncCheckIn> Get(LoadSyncEvents request)
        {
            using (var session = Application.DocumentStore.OpenSession())
            {
                return session.Query<SyncCheckIn>()
                       .Customize(q => q.WaitForNonStaleResultsAsOfNow())
                       .Where(s => s.EventId == request.EventId)
                       .Take(1024)
                       .ToList();
            }
        }
    }
}