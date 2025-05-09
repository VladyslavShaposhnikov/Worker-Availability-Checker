using OfficeOpenXml;

namespace practice;

public static class ExcelHelpers
{
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

    public static void ParseAvailability(List<Worker> workers, ExcelWorksheet worksheet, int daysInMonth, int month)
    {
        foreach (var worker in workers)
        {
            int[] from = SaveToArray(worker.StartRow, worker.StartColumn, daysInMonth, worksheet);
            
            int[] until = SaveToArray(worker.StartRow, worker.StartColumn + 2, daysInMonth, worksheet);
            
            for (int i = 0; i < daysInMonth; i++)
            {
                string day = $"{month}/{i + 1}/2025";
                int[] res = Enumerable.Range(from[i], until[i]).ToArray();
                worker.Availability[DateOnly.Parse(day)] = res;
            }
        }
    }
}