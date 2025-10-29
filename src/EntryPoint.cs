// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using HayDayPro;
using HayDayPro.Items;
using HayDayPro.Sources;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Hello, World!");

var jsonSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

// Console.WriteLine(JsonSerializer.Serialize(value, jsonSerializerOptions));

var account = new Account(33);
Console.WriteLine($"Level {account.Level}");
PrintBestBy(account, account.CreatePropertySelector(item => item.SelfMadeProfitPerHour, "c/h"));

return;

static void PrintBestBy(Account account, ItemPropertySelector selector)
{
    const int tabWidth = 8;
    
    var groups = account.BestBy(selector).ToArray();
    var padTo = (groups.Max(group => group.Key.Name.Length) + tabWidth - 1) / tabWidth * tabWidth;
    
    var total = 0;
    
    foreach (var group in groups)
    {
        var best = group.OrderByDescending(selector.GetTotalValue).First();
        var name = group.Key.Name;
        
        var tabs = Math.Max(1, (padTo - name.Length + tabWidth - 1) / tabWidth);
        var value = selector.GetTotalValue(best);
        
        Console.WriteLine($"{name}{new string('\t', tabs)}=> {value} {selector.Description} - {best.Name}");
        total += value;
    }

    Console.WriteLine($"Total {total} {selector.Description}");
}
