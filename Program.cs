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
        vladyslav.FullPartTime = 0.8f;
        vladyslav.Availability = new Dictionary<DateOnly, int[]>();
        
        Worker me = new Worker();
        me.Name = "Me";
        me.WorkerId = 701253;
        me.StartColumn = 9;
        me.FullPartTime = 1;
        me.Availability = new Dictionary<DateOnly, int[]>();
        
        Worker vlad = new Worker();
        vlad.Name = "Vlad";
        vlad.WorkerId = 701354;
        vlad.StartColumn = 13;
        vlad.FullPartTime = 1;
        vlad.Availability = new Dictionary<DateOnly, int[]>();
        
        Worker wladek = new Worker();
        wladek.Name = "Wladek";
        wladek.WorkerId = 701355;
        wladek.StartColumn = 17;
        wladek.FullPartTime = 1;
        wladek.Availability = new Dictionary<DateOnly, int[]>();
        
        Worker shaposhnikov = new Worker();
        shaposhnikov.Name = "Shaposhnikov";
        shaposhnikov.WorkerId = 701256;
        shaposhnikov.StartColumn = 21;
        shaposhnikov.FullPartTime = 0.8f;
        shaposhnikov.Availability = new Dictionary<DateOnly, int[]>();
        
        Worker shapik = new Worker();
        shapik.Name = "Shapik";
        shapik.WorkerId = 701257;
        shapik.StartColumn = 25;
        shapik.FullPartTime = 0.5f;
        shapik.Availability = new Dictionary<DateOnly, int[]>();
        
        Worker vm1 = new Worker(){Name = "Vm1", Priority = 2, WorkerId = 701258, StartColumn = 29, FullPartTime = 0.5f, Availability = new Dictionary<DateOnly, int[]>()};
        
        Worker vm2 = new Worker(){Name = "Vm2",Priority = 2, WorkerId = 701259, StartColumn = 33, FullPartTime = 1, Availability = new Dictionary<DateOnly, int[]>()};
        
        Worker manager1 = new Worker(){Name = "Manager1",Priority = 1, WorkerId = 701260, StartColumn = 37, FullPartTime = 1, Availability = new Dictionary<DateOnly, int[]>()};
        
        Worker manager2 = new Worker(){Name = "Manager2",Priority = 1, WorkerId = 701261, StartColumn = 41, FullPartTime = 1, Availability = new Dictionary<DateOnly, int[]>()};
        
        List<Worker> allWorkers = new List<Worker>(){me, wladek, shaposhnikov, shapik, vladyslav, vlad, vm1, vm2, manager1, manager2};
        
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        int dim; // days in month
        int m; // month
        
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
            
            if (!int.TryParse(worksheet.Cells["B1"].Text, out int workingHoursNum))
            {
                Console.WriteLine("Invalid number in cell B1.");
                return;
            }
            ExcelHelpers.workingHours = workingHoursNum;

            ExcelHelpers.ParseAvailability(allWorkers, worksheet, dim, m);
        }

        Schedule s = new Schedule(dim, m);
        
        // s.CurrentMonth[DateOnly.Parse("06/01/2025")].ChangeRequiredWorkers(9, 10); // to change required workers at certain day at certain hour

        foreach (var key in s.CurrentMonth.Keys)
        {
            // important note: if I am looking for employee for example from 9 to 15 and required workers from 9 to 11 is set to 4 but from 11 to 15
            // required workers is set to 3, it will find 4 workers from 9 to 11 (if they are available)
            if (key.DayOfWeek.ToString() == "Monday" || key.DayOfWeek.ToString() == "Thursday")
            {
                SetWorkersIntoSchedule.IsAvailableWithTimeManagerAndTempDict(key, allWorkers, 6, 14, s.CurrentMonth[key]);
            }
            else
            {
                SetWorkersIntoSchedule.IsAvailableWithTimeManagerAndTempDict(key, allWorkers, 9, 15, s.CurrentMonth[key]);
            }
            if (key.DayOfWeek.ToString() == "Monday" || key.DayOfWeek.ToString() == "Thursday")
            {
                SetWorkersIntoSchedule.IsAvailableWithTimeManagerAndTempDict(key, allWorkers, 14, 22, s.CurrentMonth[key]);
            }
            else
            {
                SetWorkersIntoSchedule.IsAvailableWithTimeManagerAndTempDict(key, allWorkers, 15, 22, s.CurrentMonth[key]);
                
                // SetWorkersIntoSchedule.IsAvailableWithTimeManagerAndTempDict(key, allWorkers, 15, 21, s.CurrentMonth[key]); to set workers who can work until 21:00 but each monday and thurday
            }
        }
        
        // SetWorkersIntoSchedule.IsAvailableWithTimeManager(DateOnly.Parse("06/10/2025"), allWorkers, 15, 21, s.CurrentMonth[DateOnly.Parse("06/10/2025")]); // set available worker from 15 to 21 on 06/10/2025

        using (StreamWriter writer = new StreamWriter("output.txt"))
        {
            writer.WriteLine("-----------------------------------Plan---------------------------------------------");

            foreach (var item in s.CurrentMonth.Keys)
            {
                writer.WriteLine($"Date: {item}");
                writer.WriteLine();
                s.CurrentMonth[item].DisplayWorkingHoursToFile(writer);
                writer.WriteLine(s.CurrentMonth[item].Date);
                writer.WriteLine(item.DayOfWeek.ToString());
                writer.WriteLine("---------------------------------------");
            }
            
            writer.WriteLine("------------------------------Hour Total For Each Worker-----------------------------------------");

            int workedHoursMonth = 0;
            foreach (var worker in allWorkers)
            {
                writer.WriteLine($"Name: {worker.Name}, {worker.WorkerId}, Etat: {worker.FullPartTime}; in hours: {worker.FullPartTime * ExcelHelpers.workingHours}, Working hours at this month: {worker.HoursAtMonth.Values.Sum()}");
                workedHoursMonth += worker.HoursAtMonth.Values.Sum();
            }
            writer.WriteLine($"Total hours for this month: {workedHoursMonth}");
            
            writer.WriteLine("--------------------------------GetNotSetHours--------------------------------------");

            int ttl = 0;
            foreach (var item in s.CurrentMonth.Keys)
            {
                ttl += s.CurrentMonth[item].GetNotSetHoursToFile(writer);
            }

            writer.WriteLine(ttl + " not assigned hours in total");
        }
        
        // foreach (var item in s.CurrentMonth.Keys)
        // {
        //     Console.WriteLine();
        //     Console.WriteLine($"Date: {item}");
        //     s.CurrentMonth[item].DisplayWorkingHours();
        //     Console.WriteLine(s.CurrentMonth[item].Date);
        //     Console.WriteLine(item.DayOfWeek.ToString());
        //     Console.WriteLine("---------------------------------------");
        // }
        //
        // int ttlH = 0;
        // foreach (var worker in allWorkers)
        // {
        //     ttlH += worker.HoursAtMonth.Values.Sum();
        //     Console.WriteLine($"Name: {worker.Name}, {worker.WorkerId}, Working hours at this month: {worker.HoursAtMonth.Values.Sum()}");
        // }
        //
        // Console.WriteLine($"Total hours for this month: {ttlH}");
    }
}
