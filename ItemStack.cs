namespace HayDayPro;

public record ItemStack(Item Item, int Count)
{
    public static ItemStack operator *(ItemStack source, int count)
    {
        return source with { Count = source.Count * count };
    }
}
