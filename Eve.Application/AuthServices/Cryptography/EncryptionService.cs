using System.Text.Json;
using System.Text;
using System.Security.Cryptography;

namespace Eve.Application.AuthServices.Cryptography;
public class EncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(string key)
    {
        _key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
    }

    public string Encrypt<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var plainBytes = Encoding.UTF8.GetBytes(json);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV(); 

        var iv = aes.IV;
        using var encryptor = aes.CreateEncryptor();

        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var result = new byte[iv.Length + encryptedBytes.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    public T Decrypt<T>(string base64Cipher)
    {
        var fullCipher = Convert.FromBase64String(base64Cipher);

        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = new byte[16]; 
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        var cipher = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        using var decryptor = aes.CreateDecryptor();
        var decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        var json = Encoding.UTF8.GetString(decryptedBytes);
        return JsonSerializer.Deserialize<T>(json)!;
    }
}
