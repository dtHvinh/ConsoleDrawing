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

    public struct Column(string name)
    {
        public readonly string Name { get; init; } = name;
        public int Width { get; set; } = 0;
    }

    public class TableColumn
    {
        public int ColumnCount { get; private set; } = 0;
        public int TotalColumnsWidth { get; private set; } = 0;
        public const int ColumnMarginLeft = 2;
        public readonly List<Column> Columns = [];
        public const char LineSeparatorChar = '-';
        public const char ColumnSeparator = '|';


        public TableColumn(params string[] columnHeader)
        {
            foreach (var header in columnHeader)
            {
                Columns.Add(new Column(header));
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
                        var col = Columns[i];
                        col.Width = TableHelper.Max(col.Width,
                                                    columnContent[i].Length,
                                                    TableRow.MinWidth,
                                                    col.Name.Length);
                        col.Width += ColumnMarginLeft;
                        Columns[i] = col;
                    }
                });

            var NOColumnSeparator = Columns.Count - 1;
            TotalColumnsWidth = Columns.Select(e => e.Width).Sum() + NOColumnSeparator;
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
        public TableColumn TableColumns { get; set; } = columns;
        public TableOption TableOption { get; set; } = tableOption is null ? new() : tableOption;
        public TableRow TableRows { get; } = new();

        public void AddRow(string commaSeparateContent)
        {
            TableValidate.IsColumnMatch(TableColumns.ColumnCount, commaSeparateContent.Split(',', options: StringSplitOptions.RemoveEmptyEntries).Length);
            TableRows.RowsContent.Add(commaSeparateContent);
        }

        public void Draw()
        {
            TableColumns.CalculateColumnWidth(TableRows);

            DrawHeader();
            DrawContent();
        }

        private void DrawHeader()
        {
            int idx = 0;
            string header = string.Join(
                TableColumn.ColumnSeparator,
                TableColumns.Columns.Select(
                    col => new string(' ', col.Width - col.Name.Length) + col.Name
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
            TableRows.RowsContent.ForEach(content =>
            {
                int idx = 0;
                string line = string.Join(
                    TableColumn.ColumnSeparator,
                    content
                        .Split(TableRow.ContentSeparateChar)
                        .Select(col => new string(' ', TableColumns.Columns[idx++].Width - col.Length) + col)
                );
                Console.WriteLine(line);
                if (TableOption.RecordLineSeparator)
                    DrawLineSeparator();
            });
        }

        private void DrawLineSeparator()
        {
            Console.WriteLine(new string(TableColumn.LineSeparatorChar, TableColumns.TotalColumnsWidth));
        }
    }
}
