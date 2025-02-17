namespace Eve.Domain.Constants;
public class EveConstants
{
    public const int MetadataCacheTime = 300;

    public const string MetadataUrl = "https://login.eveonline.com/.well-known/oauth-authorization-server";
    public const string LoginUrlToken = "https://login.eveonline.com/v2/oauth/token";
    public const string LoginUrlAuthorize = "https://login.eveonline.com/v2/oauth/authorize";

    public const string BaseUrlEsi = "https://esi.evetech.net/latest/";

    public static readonly string[] AcceptedIssuers = { "logineveonline.com", "https://login.eveonline.com" };
    public const string ExpectedAudience = "EVE Online";
}
