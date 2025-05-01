using OfficeOpenXml;

namespace practice;

public static class ExcelHelpers
{
    public static void IsAvailableForAllDays(List<Worker> workers, int daysInMonth, int from, int to)
    {
        for (int i = 1; i <= daysInMonth; i++)
        {
            string date = $"4/{i}/2025";
            Console.WriteLine($"Day: {date}");
            IsAvailable(DateOnly.Parse(date), workers, from, to);
            Console.WriteLine("--------------------------------------------------------");;
        }
    }
    public static void IsAvailable(DateOnly date, List<Worker> workers, int from, int to)
    {
        foreach (var person in workers)
        {
            if (!person.Dyspo.ContainsKey(date))
            {
                continue;
            }
            int[] ava = person.Dyspo[date];
            
            int[] toCheck = Enumerable.Range(from, to - from).ToArray();

            if (toCheck.All(x => ava.Contains(x)))
            {
                Console.WriteLine($"{person.Name} is available for {date}. All available hourse at this day: {string.Join(", ", person.Dyspo[date])}");
            }
            // else
            // {
            //     Console.WriteLine($"{person.name} is not available for {date}");
            // }
        }
    }
    public static int[] SaveToArray(int startRow, int column, int daysInMonth, ExcelWorksheet worksheet)
    {
        int[] from  = new int[daysInMonth];
        for (int row = startRow; row < startRow+daysInMonth; row++)
        {
            var value = worksheet.Cells[row, column].Text;
            from[row - startRow] = int.Parse(value);
        }
        return from;
    }
}