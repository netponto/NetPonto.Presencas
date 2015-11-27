using System;
using System.Configuration;
using System.Web.Mvc;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        private string _eventBriteOAuthToken;

        public HomeController()
        {
            _eventBriteOAuthToken = ConfigurationManager.AppSettings.Get("eventbrite.oauth_token");
            if (string.IsNullOrWhiteSpace(_eventBriteOAuthToken))
            {
                throw new InvalidOperationException(
                    string.Format("Event brite app oauth token missing in app.settings. eventbrite.oauth_token: '{0}'", _eventBriteOAuthToken));
            }
        }

        public ActionResult Index()
        {
         
            return View(new HomeIndexModel {Username = User.Identity.Name, EventBriteOAuthToken = _eventBriteOAuthToken});
        }
    }
}