using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace HayDayPro.Items;

public class ItemSource
{
    private static readonly Dictionary<string, ItemSource> ItemSources = [];
    private static readonly ItemSource Tree = Get("Trees");
    private static readonly ItemSource Bush = Get("Bushes");

    public static ICollection<ItemSource> All => ItemSources.Values;
    
    public string Name => ItemSources.First(item => item.Value == this).Key;

    public static bool TryGet(string name, [MaybeNullWhen(false)] out ItemSource itemSource)
    {
        itemSource = null;
        name = GeneratedRegex.Parentheses.Replace(name, string.Empty).Trim();
        
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        if (parts.Any(part => part.Contains("tree")))
            itemSource = Tree;
        
        if (parts.Any(part => part.Contains("bush")))
            itemSource = Bush;

        if (itemSource is null)
            ItemSources.TryGetValue(name, out itemSource);
        
        return itemSource is not null;
    }

    public static ItemSource Get(string name)
    {
        name = GeneratedRegex.Parentheses.Replace(name, string.Empty).Trim();
        
        if (!TryGet(name, out var itemSource))
            ItemSources[name] = itemSource = new ItemSource();
        
        return itemSource;
    }
}
