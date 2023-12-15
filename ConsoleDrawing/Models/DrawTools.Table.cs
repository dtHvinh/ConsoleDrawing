namespace ConsoleDrawing.Models;

#pragma warning disable S3604
public static partial class DrawTools
{
    private static class TableHelper
    {
        public static int ColumnWidthCalculate(int maxWidth, params int[] values)
        {
            int max = 0;
            foreach (var value in values)
            {
                max = int.Max(max, value);
            }
            return int.Min(max, maxWidth);
        }
    }

    private static class TableValidate
    {
        /// <summary>If 2 value different, throw exception.</summary> 
        /// <exception cref="InvalidOperationException"></exception>
        public static void IsColumnCountMatch(int tableColumn, int rowContent)
        {
            if (rowContent != tableColumn)
            {
                throw new InvalidOperationException(
                    "Can not calculate the column width due to the mismatch between " +
                    "number of column of the table and the number of content in the row" +
                    $"{Environment.NewLine}Expected: {tableColumn}, Value: {rowContent}");
            }
        }

        public static void RequireUniqueId(HashSet<string> idSet, string commaSeparateContent)
        {
            var idString = commaSeparateContent[..commaSeparateContent.IndexOf(',')];
            if (!idSet.Add(idString)) throw new ArgumentException($"Id: {idString} is duplicated");
        }
    }

    public struct Column(string name)
    {
        public readonly string Name { get; init; } = name;
        public int Width { get; set; } = 0;
    }

    public class TableColumn
    {
        public const int MaxWidth = 17;
        public const int MinWidth = 5;
        public const int ColumnMarginLeft = 2;

        public int TotalColumnsWidth => Columns.Select(e => e.Width).Sum() + Columns.Count - 1;

        public readonly List<Column> Columns = [];
        public const char LineSeparatorChar = '-';
        public const char ColumnSeparator = '|';
        public HashSet<string> IdCloumnValues { get; } = [];
        public readonly bool HasPrimaryKey;


        public TableColumn(params string[] columnHeader)
        {
            foreach (var header in columnHeader)
            {
                if (columnHeader[0].Contains("Id", StringComparison.OrdinalIgnoreCase))
                {
                    HasPrimaryKey = true;
                }
                Columns.Add(new Column(header));
            }
        }

        public static TableColumn Create(params string[] columnHeader) => new(columnHeader);

        public void CalculateColumnsWidth(TableRow rows)
        {
            rows.RowsContent.ForEach(
                row =>
                {
                    string[] columnContent = row.Split(TableRow.ContentSeparateChar);

                    for (int i = 0; i < columnContent.Length; i++)
                    {
                        var col = Columns[i];
                        // Comparison between:
                        //  * Column current width,
                        //  * The content of column width,
                        //  * Table min-width value,
                        //  * Column header width
                        col.Width = TableHelper.ColumnWidthCalculate(maxWidth: MaxWidth,
                                                                     values:
                                                                     [
                                                                         col.Width,
                                                                         columnContent[i].Length,
                                                                         MinWidth,
                                                                         col.Name.Length
                                                                     ]);
                        // Margin left content
                        col.Width += ColumnMarginLeft;
                        Columns[i] = col;
                    }
                });
            ;
        }
    }

    public struct TableRow
    {
        public int RowCount { get; set; }
        public List<string> RowsContent { get; } // john,12,male,doctor
        public const char ContentSeparateChar = ',';

        public TableRow()
        {
            RowCount = 0;
            RowsContent = [];
        }
        public readonly void Add(string commaSeparateContent) => RowsContent.Add(commaSeparateContent);
    }

    public class Table(TableColumn columns)
    {
        public TableColumn TableColumns { get; set; } = columns;
        public TableRow TableRows { get; } = new();

        public void AddRow(string commaSeparateContent)
        {
            TableValidate.IsColumnCountMatch(TableColumns.Columns.Count, commaSeparateContent.Split(',', options: StringSplitOptions.RemoveEmptyEntries).Length);
            TableValidate.RequireUniqueId(TableColumns.IdCloumnValues, commaSeparateContent);
            TableRows.Add(commaSeparateContent);
        }

        public void Draw()
        {
            Initialize();
            DrawHeader();
            DrawContent();
        }

        private void Initialize()
        {
            TableColumns.CalculateColumnsWidth(TableRows);
        }

        private void DrawHeader()
        {
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
            DrawLineSeparator();
        }

        private void DrawLineSeparator()
        {
            string line = string.Join(TableColumn.ColumnSeparator,
                                      TableColumns.Columns.Select(col => new string('-', col.Width)));
            Console.WriteLine(line);
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
                        .Select(col => new string(' ',
                                                  TableColumns.Columns[idx++].Width - col.Length) + col) // Draw white space
                );
                Console.WriteLine(line);
                DrawLineSeparator();
            });
        }
    }
}
