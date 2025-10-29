namespace HayDayPro.Items;

public record ItemPropertySelector(Func<Item, int> GetValue, Func<Item, int> GetFactor, string Description)
{
    public int GetTotalValue(Item item) => GetValue(item) * GetFactor(item);
}