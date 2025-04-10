using System.Text.RegularExpressions;
using System.Web;

namespace Eve.Application.StaticDataLoaders.Common;
internal static class CustomDecoder
{
    public static string HtmlDecode(string input)
    {
        string decoded = HttpUtility.HtmlDecode(input);

        decoded = decoded.Replace("\r\n", " ").Replace("\\n", " ");

        decoded = Regex.Replace(decoded, "<.*?>", string.Empty);

        return decoded;
    }
}
