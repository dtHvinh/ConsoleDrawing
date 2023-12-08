namespace ConsoleDrawing.Models;

#pragma warning disable S3604
public static partial class DrawTools
{
    internal class ChartBuilderOption
    {
        private bool isDisplayColumnValue;
        private int valueInterval;
    }

    internal readonly struct ChartColum(string? name, int value)
    {
        private readonly string? name;
        private readonly int value;

        public string Name { get => name; }
        public int Value { get => value; }

        public ChartColum(int value) : this(Chart.columnCount.ToString(), value)
        {

        }
    }
    internal class Chart
    {
        private ChartBuilderOption? option = null;
        private List<ChartColum> colums = [];
        public static int columnCount = 0;
    }
}
