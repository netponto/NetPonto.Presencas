using OfficeOpenXml;
using System.Linq;

namespace MvcApplication1.Helpers
{
    public static class ExcelExtensionMethods
    {
         public static void FillWorksheet(this ExcelWorksheet self, string[] data)
         {
             self.FillWorksheet(data.Select(d => new[] {d}).ToArray());
         }

        public static void FillWorksheet(this ExcelWorksheet self, string[][] data)
         {
             int iRow= 1;
             foreach (var row in data)
             {
                 int iColumn = 1;
                 foreach (var column in row)
                 {
                     self.Cells[iRow, iColumn].Value = column;
                     iColumn += 1;
                 }
                 iRow += 1;
             }
         }
    }
}