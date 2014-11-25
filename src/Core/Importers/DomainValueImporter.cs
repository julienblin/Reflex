using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ClosedXML.Excel;

namespace CGI.Reflex.Core.Importers
{
    public class DomainValueImporter : BaseImporter
    {
        public DomainValueImporter()
            : base("DomainValue")
        {
        }

        public override void GetTemplate(Stream stream)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("DomainValues");
            worksheet.Cell(0, 0).Value = "Category";
            worksheet.Cell(0, 1).Value = "Name";
            worksheet.Range(0, 0, 1, 0).Style.Font.SetBold();
            workbook.SaveAs(stream);
        }
    }
}
