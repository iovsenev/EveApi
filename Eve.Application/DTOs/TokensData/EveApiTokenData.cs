namespace Eve.Application.DTOs.TokensData;
public class EveApiTokenData
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public DateTime ExpiresIn { get; set; }

    public string RefreshToken { get; set; }
    public DateTime UpdatedAt { get; set; }
}
