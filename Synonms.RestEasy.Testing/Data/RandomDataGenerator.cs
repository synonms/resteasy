using System.Text;

namespace Synonms.RestEasy.Testing.Data;

public static class RandomDataGenerator
{
    private const string UpperAlphaCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string AlphaNumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    
    public static int GenerateInt(int min, int max) => 
        new Random().Next(min, max);

    public static string GenerateAlphaNumeric(int length)
    {
        char[] chars = new char[length];
        Random random = new();

        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = AlphaNumericCharacters[random.Next(AlphaNumericCharacters.Length)];
        }

        return new string(chars);
    }

    public static DateTime GenerateDate(DateTime? earliestDate = null, DateTime? latestDate = null)
    {
        earliestDate ??= new DateTime(1970, 1, 1);
        latestDate ??= DateTime.Today;

        TimeSpan timeSpan = latestDate.Value - earliestDate.Value;
        int totalDays = (int)timeSpan.TotalDays;

        int randomDays = GenerateInt(0, totalDays);

        return earliestDate.Value.AddDays(randomDays);
    }

    public static string GenerateNationalInsuranceNumber()
    {
        Random random = new ();
        StringBuilder stringBuilder = new();

        stringBuilder.Append(UpperAlphaCharacters[random.Next(UpperAlphaCharacters.Length)]);
        stringBuilder.Append(UpperAlphaCharacters[random.Next(UpperAlphaCharacters.Length)]);
        stringBuilder.Append(GenerateInt(100000, 999999));
        stringBuilder.Append(UpperAlphaCharacters[random.Next(UpperAlphaCharacters.Length)]);

        return stringBuilder.ToString();
    }

    public static string GenerateBankAccountNumber() => 
        GenerateInt(10000000, 99999999).ToString();

    public static string GenerateBankSortCode() =>
        GenerateInt(100000, 999999).ToString();
}