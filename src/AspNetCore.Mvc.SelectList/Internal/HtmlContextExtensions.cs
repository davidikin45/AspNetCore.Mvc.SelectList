using Microsoft.AspNetCore.Html;
using System.IO;
using System.Text.Encodings.Web;

namespace AspNetCore.Mvc.SelectList.Internal
{
    internal static class HtmlContextExtensions
    {
        public static string Render(this IHtmlContent content)
        {
            using (StringWriter writer = new StringWriter())
            {
                content.WriteTo(writer, DummyEncoder.Default);
                return writer.ToString();
            }
        }

        public class DummyEncoder : HtmlEncoder
        {
            public new static DummyEncoder Default = new DummyEncoder();

            public override int MaxOutputCharactersPerInputCharacter => HtmlEncoder.Default.MaxOutputCharactersPerInputCharacter;

            public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
            {
                return HtmlEncoder.Default.FindFirstCharacterToEncode(text, textLength);
            }

            public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
            {
                return HtmlEncoder.Default.TryEncodeUnicodeScalar(unicodeScalar, buffer, bufferLength, out numberOfCharactersWritten);
            }

            public override bool WillEncode(int unicodeScalar)
            {
                return HtmlEncoder.Default.WillEncode(unicodeScalar);
            }

            public override void Encode(TextWriter output, char[] value, int startIndex, int characterCount)
            {
                output.Write(value);
            }

            public override void Encode(TextWriter output, string value, int startIndex, int characterCount)
            {
                output.Write(value);
            }

            public override string Encode(string value)
            {
                return value;
            }
        }
    }
}
