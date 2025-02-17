namespace Eve.Domain.ExternalTypes;
public class TokenResponse
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }

    public override string ToString()
    {
        return $"acess token : {AccessToken}\n" +
            $"token type : {TokenType}\n" +
            $"expires in : {ExpiresIn}\n" +
            $"refresh token : {RefreshToken}";
    }
}
