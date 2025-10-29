using System.Text.RegularExpressions;

namespace HayDayPro;

public static partial class GeneratedRegex
{
    public static readonly Regex Parentheses = ParenthesesRegex(); 
    public static readonly Regex Time = TimeRegex(); 
    public static readonly Regex TimeDelimiter = TimeDelimiterRegex(); 
    
    [GeneratedRegex(@"\s*\((?<value>[^()]*)\)\s*")]
    private static partial Regex ParenthesesRegex();
    
    [GeneratedRegex(@"^\s*(?:(\d+)\s*d)?\s*(?:(\d+)\s*h)?\s*(?:(\d+)\s*min)?\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "uk-UA")]
    private static partial Regex TimeRegex();
    
    [GeneratedRegex(@"(?:\s*★★★\s*|\s+or\s+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex TimeDelimiterRegex();
}