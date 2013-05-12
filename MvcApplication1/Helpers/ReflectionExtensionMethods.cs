using System.Collections;
using System.Linq;
using ServiceStack.ServiceInterface.ServiceModel;

namespace MvcApplication1.Helpers
{
    public static class ReflectionExtensionMethods
    {
         public static IEnumerable UnpackIEnumerableFromPossibleIHasResponseStatus(this object obj)
         {
             if (obj == null)
                 return null;

             if (obj is IHasResponseStatus)
             {
                 var singleOtherProperty = obj.GetType().GetProperties()
                     .Where(p => p.PropertyType != typeof(ResponseStatus))
                     .ToList();
                 if (singleOtherProperty.Count == 1)
                 {
                     obj = singleOtherProperty[0].GetValue(obj, null);
                 }
             }

             return obj as IEnumerable;
         }
    }
}