using System.Text;
using System.Security.Cryptography;

public class UtilityService
{
    RandomNumberGenerator random = RandomNumberGenerator.Create();
    public bool ValidateString(string? data)
    {
        if(data == null || data.IsWhiteSpace())
            return false;
        return true;
    }
    
    public DateTime AddTime(TimeSpan time)
    {
        DateTime now = DateTime.Now;
        return now.Add(time);
    }
    public string Sha256Encrypt(string password, string salt)
    {
        string raw_string = salt + password;
        using(SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw_string));
            StringBuilder result = new StringBuilder();
            foreach(byte i in bytes)
                result.Append(i.ToString("x2"));

            return result.ToString();
        }
    }

    public string Sha256Encrypt(string password)
    {
        string raw_string = password;
        using(SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw_string));
            StringBuilder result = new StringBuilder();
            foreach(byte i in bytes)
                result.Append(i.ToString("x2"));

            return result.ToString();
        }
    }


    public string GenerateSafeString(int length)
    {
        byte[] bytes = new byte[length];
        StringBuilder s = new StringBuilder();
        random.GetBytes(bytes);

        for(int i = 0; i < length; i++)
        {
            bytes[i] = (byte)(((double)bytes[i]/256 * 93)+33);
            s.Append(Convert.ToChar(bytes[i]));
        }

        return s.ToString();
    }

    public string GenerateSafeCode(int length)
    {
        byte[] bytes = new byte[length];
        StringBuilder s = new StringBuilder();
        random.GetBytes(bytes);

        for(int i = 0; i < length; i++)
        {
            bytes[i] = (byte)(((double)bytes[i]/256 * 10)+48);
            s.Append(Convert.ToChar(bytes[i]));
        }

        return s.ToString();
    }
}