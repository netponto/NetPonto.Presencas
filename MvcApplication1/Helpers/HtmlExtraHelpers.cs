using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MvcApplication1.Helpers
{
    public static class HtmlExtraHelpers
    {
        public static HtmlString Rating1To9(this HtmlHelper self, string name, string valueDataBind = null,
                                            IDictionary<string, object> htmlAttributes = null)
        {
            return self.Rating1ToN(9, name, valueDataBind, htmlAttributes);
        }

        public static HtmlString Rating1To4(this HtmlHelper self, string name, string valueDataBind = null,
                                            IDictionary<string, object> htmlAttributes = null)
        {
            return self.Rating1ToN(4, name, valueDataBind, htmlAttributes);
        }

        public static HtmlString Rating1ToN(this HtmlHelper self, int n, string name, string valueDataBind = null,
                                            IDictionary<string, object> htmlAttributes = null)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            if (valueDataBind == null)
            {
                valueDataBind = name;
            }
            htmlAttributes["data-bind"] = string.Format("value: {0}", valueDataBind);
            htmlAttributes["class"] = "js_input_field";
            var selectList = Enumerable.Range(1, n).Select(i => new SelectListItem {Text = i.ToString(), Value = i.ToString()}).ToList();
            selectList.Insert(0, new SelectListItem {Text = "--", Value = "-1"});
            return self.DropDownList(name, selectList, htmlAttributes);
        }
    }
}