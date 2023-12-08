namespace ConsoleDrawing.Models;

#pragma warning disable S3604
public static partial class DrawTools
{
    private static class TableHelper
    {
        public static int Max(params int[] values)
        {
            int max = 0;
            foreach (var value in values)
            {
                max = int.Max(max, value);
            }
            return max;
        }
    }

    private static class TableValidate
    {
        /// <summary>If 2 value different, throw exception.</summary> 
        /// <exception cref="InvalidOperationException"></exception>
        public static void IsColumnMatch(int tableColumn, int rowContent)
        {
            if (rowContent != tableColumn)
            {
                throw new InvalidOperationException(
                    "Can not calculate the column width due to the mismatch between " +
                    "number of column of the table and the number of content in the row" +
                    $"{Environment.NewLine}Expected: {tableColumn}, Value: {rowContent}");
            }
        }
    }

    public class TableColumn
    {
        public int ColumnCount { get; private set; } = 0;
        public int TotalColumnsWidth { get; private set; } = 0;
        public const int ColumnMarginLeft = 2;
        public readonly List<int> ColumnWidth = [];
        public readonly List<string> ColumnName = [];
        public const char LineSeparatorChar = '-';
        public const char ColumnSeparator = '|';


        public TableColumn(params string[] columnHeader)
        {
            foreach (var header in columnHeader)
            {
                ColumnName.Add(header);
                ColumnWidth.Add(0);
                ++ColumnCount;
            }
        }

        public void CalculateColumnWidth(TableRow rows)
        {
            rows.RowsContent.ForEach(
                row =>
                {
                    string[] columnContent = row.Split(TableRow.ContentSeparateChar);

                    for (int i = 0; i < columnContent.Length; i++)
                    {
                        ColumnWidth[i] = TableHelper.Max(ColumnWidth[i], columnContent[i].Length, TableRow.MinWidth, ColumnName[i].Length);
                        ColumnWidth[i] += ColumnMarginLeft;
                    }
                });

            var NOColumnSeparator = ColumnName.Count - 1;
            TotalColumnsWidth = ColumnWidth.Sum() + NOColumnSeparator;
        }
    }

    public struct TableRow
    {
        public int RowCount { get; set; }
        public List<string> RowsContent { get; } // john,12,male,doctor
        public const char ContentSeparateChar = ',';

        public const int MaxWidth = 17;
        public const int MinWidth = 5;

        public TableRow()
        {
            RowCount = 0;
            RowsContent = [];
        }
    }

    public class TableOption
    {
        public bool HeaderLineSeparator { get; set; } = false;
        public bool RecordLineSeparator { get; set; } = false;
    }
    public class Table(TableColumn columns, TableOption? tableOption = null)
    {
        public TableColumn Columns { get; set; } = columns;
        public TableOption TableOption { get; set; } = tableOption is null ? new() : tableOption;
        public TableRow Rows { get; } = new();

        public void AddRow(string commaSeparateContent)
        {
            TableValidate.IsColumnMatch(Columns.ColumnCount, commaSeparateContent.Split(',', options: StringSplitOptions.RemoveEmptyEntries).Length);
            Rows.RowsContent.Add(commaSeparateContent);
        }

        public void Draw()
        {
            Columns.CalculateColumnWidth(Rows);

            DrawHeader();
            DrawContent();
        }

        private void DrawHeader()
        {
            int idx = 0;
            string header = string.Join(
                TableColumn.ColumnSeparator,
                Columns.ColumnName.Select(
                    name => new string(' ', Columns.ColumnWidth[idx++] - name.Length) + name
                    )
                );
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(header);
            Console.ResetColor();
            if (TableOption.HeaderLineSeparator)
                DrawLineSeparator();
        }

        private void DrawContent()
        {
            Rows.RowsContent.ForEach(content =>
            {
                int idx = 0;
                string line = string.Join(
                    TableColumn.ColumnSeparator,
                    content
                    .Split(TableRow.ContentSeparateChar)
                    .Select(col => new string(' ', Columns.ColumnWidth[idx++] - col.Length) + col)
                );
                Console.WriteLine(line);
                if (TableOption.RecordLineSeparator)
                    DrawLineSeparator();
            });
        }

        private void DrawLineSeparator()
        {
            Console.WriteLine(new string(TableColumn.LineSeparatorChar, Columns.TotalColumnsWidth));
        }
    }
}
