using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using ServiceStack.ServiceHost;
using ServiceStack.Common.Extensions;

namespace MvcApplication1.Helpers
{
    public class ExcelContentTypeHandler
    {
        public const string ContentType = "application/excel";

        private static HashSet<Type> PrimitiveTypes = new HashSet<Type>()
            {
                typeof (string),
                typeof (decimal),
                typeof (int),
                typeof (float),
                typeof (decimal),
                typeof (float),
                typeof (DateTime),
                typeof (DateTimeOffset),
                typeof (sbyte),
                typeof (short),
                typeof (uint),
                typeof (ulong),
                typeof (ushort)
            };

        public static object NonWorkingStreamDeserializer(Type type, Stream fromStream)
        {
            throw new NotImplementedException();
        }

        public static void VeryHackishExcelStreamSerializer(IRequestContext requestContext, object obj, Stream responseStream)
        {
            var enumerable = obj.UnpackIEnumerableFromPossibleIHasResponseStatus().ToList<object>();

            var stream = new MemoryStream();
            var pck = new ExcelPackage(stream);
            var ws = pck.Workbook.Worksheets.Add("Content");

            if (enumerable != null && enumerable.Count != 0)
            {
                var firstObject = enumerable[0];

                var type = firstObject.GetType();
                if (!PrimitiveTypes.Contains(type))
                {
                    var properties = type.GetProperties();

                    var firstRow = new[] {properties.Select(p => p.Name).ToArray()};
                    var otherRows = enumerable
                        .Select(o => properties.Select(p => (p.GetValue(o, null) ?? "").ToString()).ToArray())
                        .ToArray();
                    var values = firstRow.Concat(otherRows).ToArray();
                    ws.FillWorksheet(values);
                }
                else
                {
                    var values = new[] {enumerable.First().GetType().Name}
                        .Concat(enumerable.Select(o => o.ToString()).ToArray())
                        .ToArray();

                    ws.FillWorksheet(values);
                }
            }
            pck.Save();

            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(responseStream);
        }
    }
}