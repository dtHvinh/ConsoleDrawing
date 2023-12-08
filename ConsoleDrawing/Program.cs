using ConsoleDrawing.Models;

namespace ConsoleDrawing
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var cols = new DrawTools.TableColumn("Id", "First Name", "Last Name");
            var table = new DrawTools.Table(cols);

            table.AddRow("1,John,Cater");
            table.AddRow("2,Emily,William");
            table.AddRow("3,David,More");
            table.AddRow("4,John,Linton");
            table.Draw();
        }
    }
}
