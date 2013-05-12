using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;

namespace MvcApplication1.Helpers
{
    /// <summary>
    /// This content type picks the object that's being returned, does a ToString and outputs one per line
    /// </summary>
    public class PlainTextContentTypeHandler
    {
        public const string ContentType = "text/plain";
        public const string ApplicationContentType = "application/txt";

        public static object NonWorkingStreamDeserializer(Type type, Stream fromStream)
        {
            throw new NotImplementedException();
        }

        public static void ReallySimpleTextStreamSerializer(IRequestContext requestContext, object obj, Stream responseStream)
        {
            var enumerable = obj.UnpackIEnumerableFromPossibleIHasResponseStatus();
            if (enumerable == null) return;

            foreach (var str in enumerable.Cast<object>().Select(o => o.ToString()))
            {
                var buffer = Encoding.UTF8.GetBytes(str+Environment.NewLine);
                responseStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}