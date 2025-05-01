using OfficeOpenXml;
using practice;

class Program
{
    static void Main()
    {
        Worker vladyslav = new Worker();
        vladyslav.Name = "Vladyslav";
        vladyslav.WorkerId = 701352;
        vladyslav.StartColumn = 5;
        vladyslav.Dyspo = new Dictionary<DateOnly, int[]>();
        
        Worker me = new Worker();
        me.Name = "Me";
        me.WorkerId = 701253;
        me.StartColumn = 9;
        me.Dyspo = new Dictionary<DateOnly, int[]>();
        
        Worker vlad = new Worker();
        vlad.Name = "Vlad";
        vlad.WorkerId = 701354;
        vlad.StartColumn = 13;
        vlad.Dyspo = new Dictionary<DateOnly, int[]>();
        
        Worker wladek = new Worker();
        wladek.Name = "Wladek";
        wladek.WorkerId = 701355;
        wladek.StartColumn = 17;
        wladek.Dyspo = new Dictionary<DateOnly, int[]>();
        
        Worker shaposhnikov = new Worker();
        shaposhnikov.Name = "Shaposhnikov";
        shaposhnikov.WorkerId = 701256;
        shaposhnikov.StartColumn = 21;
        shaposhnikov.Dyspo = new Dictionary<DateOnly, int[]>();
        
        Worker shapik = new Worker();
        shapik.Name = "Shapik";
        shapik.WorkerId = 701257;
        shapik.StartColumn = 25;
        shapik.Dyspo = new Dictionary<DateOnly, int[]>();
        
        List<Worker> workers = new List<Worker>() {me, wladek, shaposhnikov, shapik, vladyslav, vlad};
        
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        using(var package = new ExcelPackage(new FileInfo(GetPath.Path)))
        {
            if (package.Workbook.Worksheets.Count == 0)
            {
                Console.WriteLine("No worksheets found in the Excel file.");
                return;
            }
            
            var worksheet = package.Workbook.Worksheets[0];

            if (!int.TryParse(worksheet.Cells["B3"].Text, out int daysInMonth))
            {
                Console.WriteLine("Invalid number in cell B3.");
                return;
            }

            foreach (var worker in workers)
            {
                int[] from = ExcelHelpers.SaveToArray(worker.StartRow, worker.StartColumn, daysInMonth, worksheet);
            
                int[] untill = ExcelHelpers.SaveToArray(worker.StartRow, worker.StartColumn + 2, daysInMonth, worksheet);
                
                for (int i = 0; i < daysInMonth; i++)
                {
                    string day = $"4/{i + 1}/2025";
                    int[] res = Enumerable.Range(from[i], untill[i]).ToArray();
                    worker.Dyspo[DateOnly.Parse(day)] = res;
                }
            }
        }
        
        ExcelHelpers.IsAvailable(DateOnly.Parse("4/12/2025"), workers, 9, 16);

        ExcelHelpers.IsAvailableForAllDays(workers, 30, 9, 16);
    }
}
