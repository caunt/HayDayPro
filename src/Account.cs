using HayDayPro.Items;
using HayDayPro.Sources;

namespace HayDayPro;

public record Account(int Level)
{
    public IEnumerable<Item> AvailableGoods => GoodsSource.Enumerate().Where(item => item.Level <= Level);
    
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

    public IOrderedEnumerable<IGrouping<ItemSource, Item>> BestBy(ItemPropertySelector selector)
    {
        return AvailableGoods
            .Where(item => selector.GetTotalValue(item) is < int.MaxValue and > 0)
            .GroupBy(item => item.Source)
            .OrderByDescending(group => group.Max(selector.GetTotalValue));
    }
}
