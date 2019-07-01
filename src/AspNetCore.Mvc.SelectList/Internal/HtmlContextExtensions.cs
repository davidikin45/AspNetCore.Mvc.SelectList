using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace AspNetCore.Mvc.SelectList.Internal
{
    internal static class HtmlContextExtensions
    {
        public static string Render(this IHtmlContent content)
        {
            using (var writer = new System.IO.StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }
    }
}
