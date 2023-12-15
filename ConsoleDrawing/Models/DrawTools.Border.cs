namespace ConsoleDrawing.Models;

public static partial class DrawTools
{
    internal static class Border
    {
        public static void DrawNullContent()
        {
            Console.WriteLine(
                """
            ------
            -null-
            ------
            """);
        }

        /// <summary>
        /// Draw a border that cover all content.
        /// </summary>
        /// <param name="content">The content to be covered.</param>
        /// <param name="borderChar">The char that fill the border.</param>
        /// <param name="emptySpaceChar">The char that fill the empty space</param>
        public static void Draw(string? content, char borderChar, char emptySpaceChar = ' ')
        {
            if (content == null)
            {
                DrawNullContent();
                return;
            }

            var bc = borderChar;
            var esc = emptySpaceChar;

            string[] strs = content.Split(Environment.NewLine);

            for (int i = 0; i < strs.Length; i++)
            {
                strs[i] = strs[i]!.TrimEnd();
            }

            int h = strs.Length + 2;
            int w = strs.Select(str => str.Length).Max() + 2;

            for (int i = 0; i < h; i++)
            {
                if (i == 0 || i == h - 1)
                    Console.WriteLine($"{new string(bc, w)}");
                else
                {
                    Console.WriteLine($"{bc}{strs[i - 1].Replace(' ', esc)}{new string(esc, w - strs[i - 1].Length - 2)}{bc}");
                }
            }
        }


    }
}
