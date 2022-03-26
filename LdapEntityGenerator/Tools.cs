namespace LdapEntityGenerator;

public static class Tools
{
    public static string AsBase64(this string plainText) {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
    
    public static string DecodeBase64(this byte[] base64EncodedData) {
        return System.Text.Encoding.UTF8.GetString(base64EncodedData);
    }
}