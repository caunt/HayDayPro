namespace HayDayPro.Extensions;

public static class RangeExtensions
{
    public static float Average(this Range range)
    {
        var start = range.Start.Value;
        var end = range.End.Value;
        
        return (Math.Max(start, end) - Math.Min(start, end)) / 2F;
    }
}