namespace Eve.Domain.Constants;
public static class GlobalKeysCacheConstants
{
    //redis keys for esi
    //for Auth service
    public const string AdminTokenData = "AdminTokenData";
    public const string JwksMetadata = "JwksMetadata";

    //For data
    public const string ETag = "etag";
    public const string ETagForAllOrders = $"{ETag}:{OrdersKey}:all";
    public const string ETagForOrdersWithType = $"{ETag}:{OrdersKey}";
    public const string ETagForHistoryWithType = $"{ETag}:{OrdersHistoryKey}";

    public const string OrdersKey = "orders";

    public const string OrdersHistoryKey = "history";

    //redis keys for enities
    public const string MarketGroups = "marketGroups";
    public const string MarketGroupTypes = $"{MarketGroups}:types";

    public const string Product = "product";
    public const string Type = "type";
    public const string Stations = "stations";
    public const string Industry = "industry";

    public const string IndustryGroups = $"{Industry}:groups";
    public const string ESIAccessToken = "esi:access:token";

}
