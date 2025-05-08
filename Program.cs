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
        
        Worker vm1 = new Worker(){Name = "Vm1", Priority = 2, WorkerId = 701258, StartColumn = 29, Dyspo = new Dictionary<DateOnly, int[]>()};
        
        Worker vm2 = new Worker(){Name = "Vm2",Priority = 2, WorkerId = 701259, StartColumn = 33, Dyspo = new Dictionary<DateOnly, int[]>()};
        
        Worker manager1 = new Worker(){Name = "Manager1",Priority = 1, WorkerId = 701260, StartColumn = 37, Dyspo = new Dictionary<DateOnly, int[]>()};
        
        Worker manager2 = new Worker(){Name = "Manager2",Priority = 1, WorkerId = 701261, StartColumn = 41, Dyspo = new Dictionary<DateOnly, int[]>()};
        
        List<Worker> allWorkers = new List<Worker>(){me, wladek, shaposhnikov, shapik, vladyslav, vlad, vm1, vm2, manager1, manager2};
        
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        int dim;
        int m;
        
        using(var package = new ExcelPackage(new FileInfo(GetPath.Path)))
        {
            if (package.Workbook.Worksheets.Count == 0)
            {
                Console.WriteLine("No worksheets found in the Excel file.");
                return;
            }
            
            var worksheet = package.Workbook.Worksheets[1];

            if (!int.TryParse(worksheet.Cells["B3"].Text, out int daysInMonth))
            {
                Console.WriteLine("Invalid number in cell B3.");
                return;
            }
            dim = daysInMonth;
            
            if (!int.TryParse(worksheet.Cells["A3"].Text, out int monthNum))
            {
                Console.WriteLine("Invalid number in cell A3.");
                return;
            }
            m = monthNum;

            ExcelHelpers.ParseAvailability(allWorkers, worksheet, dim, m);
        }
        
        //ExcelHelpers.IsAvailable(DateOnly.Parse("4/12/2025"), workers, 9, 16);

        //ExcelHelpers.IsAvailableForAllDays(workers, 30, 9, 16);
        
        //ExcelHelpers.IsAvailableForAllDays(allWorkers, 30, 9, 16);
        
        // TimeManager tm = new TimeManager(9, 22);
        //
        // ExcelHelpers.IsAvailableWithTimeManaget(DateOnly.Parse("4/12/2025"), workers, 9, 16, tm);
        // Console.WriteLine("---------------------------------------------------");
        // tm.DisplayWorkingHours();

        Schedule s = new Schedule(dim, m);

        foreach (var key in s.CurrentMonth.Keys)
        {
            if (key.DayOfWeek.ToString() == "Monday" || key.DayOfWeek.ToString() == "Thursday")
            {
                ExcelHelpers.IsAvailableWithTimeManagetAndTempDict(key, allWorkers, 6, 14, s.CurrentMonth[key]);
            }
            else
            {
                ExcelHelpers.IsAvailableWithTimeManagetAndTempDict(key, allWorkers, 9, 15, s.CurrentMonth[key]);
            }
            if (key.DayOfWeek.ToString() == "Monday" || key.DayOfWeek.ToString() == "Thursday")
            {
                ExcelHelpers.IsAvailableWithTimeManagetAndTempDict(key, allWorkers, 14, 22, s.CurrentMonth[key]);
            }
            else
            {
                ExcelHelpers.IsAvailableWithTimeManagetAndTempDict(key, allWorkers, 15, 22, s.CurrentMonth[key]);
                // ExcelHelpers.IsAvailableWithTimeManagetAndTempDict(key, allWorkers, 15, 21, s.CurrentMonth[key]); to set workers who can work until 21:00
            }
        }
        
        ExcelHelpers.IsAvailableWithTimeManaget(DateOnly.Parse("06/01/2025"), allWorkers, 15, 21, s.CurrentMonth[DateOnly.Parse("06/01/2025")]);
        
        // ExcelHelpers.IsAvailableForAllDaysWithTimeManager(allWorkers, 30, 9, 15, s);
        // ExcelHelpers.IsAvailableForAllDaysWithTimeManager(allWorkers, 30, 15, 22, s);

        using (StreamWriter writer = new StreamWriter("output.txt"))
        {
            writer.WriteLine("----------------------------------------------------Plan---------------------------------------------------------------");

            foreach (var item in s.CurrentMonth.Keys)
            {
                writer.WriteLine($"Date: {item}");
                writer.WriteLine();
                s.CurrentMonth[item].DisplayWorkingHoursToFile(writer);
                writer.WriteLine(s.CurrentMonth[item].Date);
                writer.WriteLine(item.DayOfWeek.ToString());
                writer.WriteLine("---------------------------------------");
            }
            
            writer.WriteLine("----------------------------------------------------Hour Total For Each Worker---------------------------------------------------------------");
            
            foreach (var worker in allWorkers)
            {
                writer.WriteLine($"Name: {worker.Name}, {worker.WorkerId}, Working hours at this month: {worker.HoursAtMonth.Values.Sum()}");
            }
            
            writer.WriteLine("-----------------------------------------------GetNotSetHours--------------------------------------------------------------------");
            
            foreach (var item in s.CurrentMonth.Keys)
            {
                s.CurrentMonth[item].GetNotSetHoursToFile(writer);
            }
        }
        
        foreach (var item in s.CurrentMonth.Keys)
        {
            Console.WriteLine();
            Console.WriteLine($"Date: {item}");
            s.CurrentMonth[item].DisplayWorkingHours();
            Console.WriteLine(s.CurrentMonth[item].Date);
            Console.WriteLine(item.DayOfWeek.ToString());
            Console.WriteLine("---------------------------------------");
        }

        foreach (var worker in allWorkers)
        {
            Console.WriteLine($"Name: {worker.Name}, {worker.WorkerId}, Working hours at this month: {worker.HoursAtMonth.Values.Sum()}");
        }
        
        // foreach (var item in s.CurrentMonth.Keys)
        // {
        //     s.CurrentMonth[item].GetNotSetHours();
        // }
    }
}
