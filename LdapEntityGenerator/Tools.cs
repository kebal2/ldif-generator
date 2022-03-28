using System.Text;

namespace LdapEntityGenerator;

public static class Tools
{
    public static string AsUtf8Base64(this string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string AsUnicodeBase64(this string plainText)
    {
        var plainTextBytes = Encoding.Unicode.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string DecodeUtfBase64(this byte[] base64EncodedData)
    {
        return Encoding.UTF8.GetString(base64EncodedData);
    }

    public static string DecodeUnicodeBase64(this byte[] base64EncodedData)
    {
        return Encoding.UTF8.GetString(base64EncodedData);
    }
}