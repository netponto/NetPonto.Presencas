using System;
using Microsoft.AspNet.SignalR;
using MvcApplication1.Services;

namespace MvcApplication1.Hubs
{
    public class CheckInHub : Hub
    {
       
        public void CheckIn(string checkerUserId, long id, long eventid, bool isPresent)
        {
            using (var session = Application.DocumentStore.OpenSession())
            {
                session.Store(new SyncCheckIn
                    {
                        Id=string.Format("SyncCheckIn/{0}/{1}", eventid, id),
                        CheckerUserId = checkerUserId,
                        CheckedInUserId = id,
                        EventId = eventid,
                        IsPresent = isPresent,
                        SyncCheckInDate = DateTimeOffset.UtcNow
                    });
                session.SaveChanges();
                Clients.Others.broadcastMessage(id, eventid, isPresent);
            }
        }
    }
}