// See https://aka.ms/new-console-template for more information

using HayDayPro;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Hello, World!");

var jsonSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

// Console.WriteLine(JsonSerializer.Serialize(value, jsonSerializerOptions));

var account = new Account(20);
var profitPerHourSelector = new Selector(item => item.SelfMadeProfitPerHour, account.GetFactor, "c/h");

Console.WriteLine($"Level {account.Level}");
PrintBestBy(account, profitPerHourSelector);

return;

static void PrintBestBy(Account account, Selector selector)
{
    const int tabWidth = 8;
    
    var groups = BestBy(account.Goods, selector.GetTotalValue).ToArray();
    var maxNameLen = groups.Max(group => group.Key.Name.Length);
    var padTo = (maxNameLen + tabWidth - 1) / tabWidth * tabWidth;
    
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

static IOrderedEnumerable<IGrouping<ItemSource, Item>> BestBy(IEnumerable<Item> goods, Func<Item, int> parameter)
{
    return goods
        .Where(item => parameter(item) is < int.MaxValue and > 0)
        .GroupBy(item => item.Source)
        .OrderByDescending(group => group.Max(parameter));
}

internal record Selector(Func<Item, int> GetValue, Func<Item, int> GetFactor, string Description)
{
    public int GetTotalValue(Item item) => GetValue(item) * GetFactor(item);
}

internal record Account(int Level)
{
    public IEnumerable<Item> Goods => GoodsSource.Enumerate().Where(item => item.Level <= Level);
    
    public int CountFields => Level switch
    {
        <= 0 => 0,
        <= 49 => 3 + 3 * ((Level + 1) / 2),
        <= 99 => 78 + 2 * ((Level + 1) / 2 - 25),
        _ => 79 + Level / 2
    };

    public int CountChickens => 6 * Level switch
    {
        < 12 => 1, 
        < 23 => 2, 
        _ => 3
    };

    public int CountCows => 5 * Level switch
    {
        < 6 => 0, 
        < 15 => 1, 
        < 27 => 2, 
        _ => 3
    };

    public int CountPigs => 5 * Level switch
    {
        < 10 => 0, 
        < 18 => 1, 
        < 32 => 2, 
        _ => 3
    };

    public int CountGoats => 4 * Level switch
    {
        < 32 => 0, 
        < 37 => 1, 
        < 50 => 2, 
        _ => 3
    };
    
    public int GetFactor(Item item)
    {
        return item switch
        {
            { Source.Name: "Field" } => CountFields,
            { Name: "Egg" } => CountChickens,
            { Name: "Milk" } => CountCows,
            { Name: "Bacon" } => CountPigs,
            { Name: "Goat milk" } => CountGoats,
            _ => 1
        };
    }
}
