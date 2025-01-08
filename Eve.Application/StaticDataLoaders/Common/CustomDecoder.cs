using System.Text.RegularExpressions;
using System.Web;

namespace Eve.Application.StaticDataLoaders.Common;
internal static class CustomDecoder
{
    public static string HtmlDecode(string input)
    {
        string decoded = /*input*/
        //    .Replace(@"\u003C", "<")
        //    .Replace(@"\u003E", ">");
        // Декодируем HTML символы, такие как &lt;, &gt;, &amp; и т.д.
        decoded = HttpUtility.HtmlDecode(input);

        // Заменяем символы переноса строк на стандартные переносы
        decoded = decoded.Replace("\r\n", " ").Replace("\\n", " ");

        decoded = Regex.Replace(decoded, "<.*?>", string.Empty);

        return decoded;
    }
}
