using System.Text.RegularExpressions;

namespace BMT.Logic;

public static class Utility
{
       // This contract duplicate whitespaces in authors full name
    public static string NormalizeFullName(string authorName) =>
        Regex.Replace(authorName.Trim(), @"\s+", " ");
}