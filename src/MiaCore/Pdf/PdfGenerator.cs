using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace MiaCore.Pdf
{
    public static class PdfGenerator
    {
        public static byte[] HtmlToPdf(string html)
        {
            byte[] bytes;
            using var ms = new MemoryStream();

            var doc = new Document();

            var writer = PdfWriter.GetInstance(doc, ms);

            doc.Open();

            var htmlWorker = new HtmlWorker(doc);
            using (var sr = new StringReader(html))
            {
                htmlWorker.Parse(sr);
            }

            doc.Close();

            bytes = ms.ToArray();

            return bytes;
        }
    }
}