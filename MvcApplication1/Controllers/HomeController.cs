using System;
using System.Configuration;
using System.Web.Mvc;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        private string _eventBriteAppKey;
        private string _eventBriteUserKey;

        public HomeController()
        {
            _eventBriteAppKey = ConfigurationManager.AppSettings.Get("eventbrite.app_key");
            _eventBriteUserKey = ConfigurationManager.AppSettings.Get("eventbrite.user_key");
            if (string.IsNullOrWhiteSpace(_eventBriteAppKey) || string.IsNullOrWhiteSpace(_eventBriteUserKey))
            {
                throw new InvalidOperationException(
                    string.Format("Event brite app key or user key missing in app.settings. eventbrite.app_key: '{0}', eventbrite.user_key: '{1}'", _eventBriteAppKey, _eventBriteUserKey));
            }
        }

        public ActionResult Index()
        {
         
            return View(new HomeIndexModel {Username = User.Identity.Name, EventBriteAppKey = _eventBriteAppKey, EventBriteUserKey = _eventBriteUserKey});
        }

        public ActionResult EventEvaluations()
        {
            return View(new HomeEventEvaluationsViewModel {EventBriteAppKey = _eventBriteAppKey, EventBriteUserKey = _eventBriteUserKey });
        }
    }
}