using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Helpers
{
    public static class ScriptsHelpers
    {
        public static IHtmlString RenderSignalR(this HtmlHelper self)
        {
            return new HtmlString(System.Web.Optimization.Scripts.Render("~/js/signalr") +
                                  "<br/><script src='/signalr/hubs'></script>");
        }
    }
}