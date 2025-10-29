using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HayDayPro;

public class Item
{
    private static readonly Dictionary<string, Item> Items = new(StringComparer.OrdinalIgnoreCase);

    public static ICollection<Item> All => Items.Values;
    
    public string Name => Items.First(item => item.Value == this).Key;
    public int IngredientsCost => Needs.Sum(stack => stack.Item.MaxPrice * stack.Count);
    public int BaseNeedsCost => BaseNeeds.Sum(stack => stack.Item.MaxPrice * stack.Count);
    public TimeSpan FullySelfMadeTime => SelfMadeTime + Needs.Aggregate(TimeSpan.Zero, (totalTime, itemStack) => totalTime + itemStack.Item.FullySelfMadeTime * itemStack.Count);
    public int FullySelfMadeProfit => MaxPrice - BaseNeedsCost;
    public int FullySelfMadeProfitPerHour => (int)Math.Round(FullySelfMadeProfit / FullySelfMadeTime.TotalHours);
    public TimeSpan SelfMadeTime => Time.Max();
    public int SelfMadeProfit => MaxPrice - IngredientsCost;
    public int SelfMadeProfitPerHour => (int)Math.Round(SelfMadeProfit / SelfMadeTime.TotalHours);
    public IEnumerable<ItemStack> BaseNeeds => Needs.Any() ? Needs.SelectMany(stack => stack.Item.Needs.Any() ? stack.Item.BaseNeeds.Select(baseStack => baseStack * stack.Count) : [new ItemStack(stack.Item, stack.Count)]).GroupBy(stack => stack.Item).Select(group => new ItemStack(group.Key, group.Sum(groupedItemStack => groupedItemStack.Count))) : [];
    
    public required int Level { get; init; }
    public required int MaxPrice { get; init; }
    public required IEnumerable<TimeSpan> Time { get; init; }
    public required IEnumerable<ItemStack> Needs { get; init; }
    public required ItemSource Source { get; init; }
    public required Range Experience { get; init; }
    public required Range PerBoatCrate { get; init; }

    public static Item Get(string name)
    {
        return TryGet(name, out var item) ? item : throw new KeyNotFoundException(name);
    }

    public static bool TryGet(string name, [MaybeNullWhen(false)] out Item item)
    {
        return Items.TryGetValue(name, out item);
    }
    
    public static Item Get(Dictionary<string, string> fields)
    {
        var name = ParseName(fields["Name"]);
        
        if (!TryGet(name, out var item))
        {
            Items[name] = item = new Item
            {
                Level = int.Parse(fields["Level"].Split(' ').First()),
                MaxPrice = int.Parse(fields["Max. price"]),
                Time = ParseTimeSpans(fields["Time"]),
                Needs = fields["Needs"]
                    .Split(['\r', '\n'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(names => ParseStacks(names, name)),
                Source = ItemSource.Get(fields["Source"]),
                Experience = ParseRange(fields["XP"]),
                PerBoatCrate = ParseRange(fields["Per boat crate"])
            };
        }

        return item;
    }

    public override string ToString()
    {
        return Name;
    }

    private static IEnumerable<ItemStack> ParseStacks(string names, string skipName)
    {
        var parts = GeneratedRegex.Parentheses.Split(names);

        for (var i = 0; i < parts.Length; i += 2)
        {
            var name = ParseName(parts[i]);
            
            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (!int.TryParse(parts[i + 1], out var count))
                continue;
            
            if (name == skipName)
                continue;
            
            if (ItemSource.TryGet(name, out _))
                continue;
            
            yield return new ItemStack(Get(name), count);
        }
    }

    private static string ParseName(string name)
    {
        return name switch
        {
            "Lure" => "Red lure",
            "Eggs" => "Egg",
            "Lobster" => "Lobster trap",
            "Duck" => "Duck trap",
            "Supplies" or "Diamond" or "N/A" or "any available" or "or Fishing net" => string.Empty,
            _ when name.EndsWith("voucher") => string.Empty,
            _ => name
        };
    }

    private static Range ParseRange(string source)
    {
        var parts = source.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is 1)
        {
            return parts[0] switch
            {
                var value when int.TryParse(value, out var singleValue) => new Range(singleValue, singleValue),
                _ or "N/A" => new Range()
            };
        }

        return new Range(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private static IEnumerable<TimeSpan> ParseTimeSpans(string source)
    {
        foreach (var rawPart in GeneratedRegex.TimeDelimiter.Split(source))
        {
            var part = rawPart.Trim();

            if (part.Equals("Instant", StringComparison.OrdinalIgnoreCase))
                yield return TimeSpan.Zero;

            var match = GeneratedRegex.Time.Match(part);

            if (!match.Success)
                continue;

            var days = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0;
            var hours = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;
            var minutes = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;

            yield return new TimeSpan(days, hours, minutes, seconds: 0);
        }
    }
}
