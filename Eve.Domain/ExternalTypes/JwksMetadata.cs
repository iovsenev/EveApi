namespace Eve.Domain.ExternalTypes;
public class JwksMetadata
{
    public List<JwksKey> Keys {  get; set; }
}

public class JwksKey
{
    public string Kty { get; set; }
    public string Alg { get; set; }
    public  string Kid { get; set; }
    public string E {  get; set; }
    public string N { get; set; }
    public string Use { get; set; }
}

public class MetadataResponse
{
    public string JwksUri { get; set; }
}