using OfficeOpenXml;
using practice;
// here we go
class Program
{
    static void Main()
    {
        Worker worker1 = new Worker()
        {
            Name = "worker1",
            WorkerId = 1,
            StartColumn = 5,
            FullPartTime = 1f,
            Availability = new Dictionary<DateOnly, int[]>()
        };
        
        Worker worker2 = new Worker()
        {
            Name = "worker2",
            WorkerId = 2, 
            StartColumn = 9, 
            FullPartTime = 0.6f, 
            Availability = new Dictionary<DateOnly, int[]>()
        };

        Worker worker3 = new Worker()
        {
            Name = "worker3",
            WorkerId = 3,
            StartColumn = 13,
            FullPartTime = 0.75f,
            Availability = new Dictionary<DateOnly, int[] > ()
        };
        
        Worker worker4 = new Worker()
        {
            Name = "worker4",
            WorkerId = 4,
            StartColumn = 17,
            FullPartTime = 0.75f,
            Availability = new Dictionary<DateOnly, int[] > ()
        };
        
        Worker shaposhnikov = new Worker()
        {
            Name = "Shaposhnikov",
            WorkerId = 333333,
            StartColumn = 21,
            FullPartTime = 0.5f,
            Availability = new Dictionary<DateOnly, int[] > ()
        };
        
        Worker worker5 = new Worker()
        {
            Name = "5",
            WorkerId = 701445,
            StartColumn = 25,
            FullPartTime = 0.6f,
            Availability = new Dictionary<DateOnly, int[] > ()
        };
        
        Worker worker6 = new Worker()
        {
            Name = "worker6",
            WorkerId = 6,
            StartColumn = 29,
            FullPartTime = 0.6f,
            Availability = new Dictionary<DateOnly, int[] > ()
        };
        
        Worker vm1 = new Worker()
        {
            Name = "vm1", 
            Priority = 2, 
            WorkerId = 11, 
            StartColumn = 33, 
            FullPartTime = 1f, 
            Availability = new Dictionary<DateOnly, int[]>()
        };
        
        Worker vm2 = new Worker()
        {
            Name = "vm2",
            Priority = 2, 
            WorkerId = 12, 
            StartColumn = 37, 
            FullPartTime = 0.5f, 
            Availability = new Dictionary<DateOnly, int[]>()
        };
        
        Worker manager1 = new Worker()
        {
            Name = "manager1",
            Priority = 1, 
            WorkerId = 111, 
            StartColumn = 41, 
            FullPartTime = 1f, 
            Availability = new Dictionary<DateOnly, int[]>()
        };
        
        Worker manager2 = new Worker()
        {
            Name = "manager2",
            Priority = 1, 
            WorkerId = 222, 
            StartColumn = 45, 
            FullPartTime = 1f, 
            Availability = new Dictionary<DateOnly, int[]>()
        };
        
        List<Worker> allWorkers = new List<Worker>()
        {
            worker1, 
            worker2, 
            worker3, 
            worker4, 
            shaposhnikov, 
            worker5, 
            worker6, 
            vm1, 
            vm2, 
            manager1,
            manager2
        };
        
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
            }
        }
        
        foreach (var key in s.CurrentMonth.Keys)
        {
            SetWorkersIntoSchedule.IsAvailableWithTimeManagerAndTempDict(key, allWorkers, 10, 20, s.CurrentMonth[key]);
        }
        
        // SetWorkersIntoSchedule.IsAvailableWithTimeManager(DateOnly.Parse("06/10/2025"), allWorkers, 15, 21, s.CurrentMonth[DateOnly.Parse("06/10/2025")]); // set available worker from 15 to 21 on 06/10/2025

        using (StreamWriter writer = new StreamWriter("output.txt"))
        {
            writer.WriteLine("------------------------------Hour Total For Each Worker-----------------------------------------");

            int workedHoursMonth = 0;
            foreach (var worker in allWorkers)
            {
                writer.WriteLine($"Name: {worker.Name}, {worker.WorkerId}, Etat: {worker.FullPartTime}; in hours: {worker.FullPartTime * ExcelHelpers.workingHours}, Working hours at this month: {worker.HoursAtMonth.Values.Sum()}");
                workedHoursMonth += worker.HoursAtMonth.Values.Sum();
            }
            writer.WriteLine($"Total set hours for this month: {workedHoursMonth}");
            
            writer.WriteLine("-----------------------------------Plan---------------------------------------------");
            
            foreach (var item in s.CurrentMonth.Keys)
            {
                writer.WriteLine($"Date: {item}, {item.DayOfWeek.ToString()}");
                writer.WriteLine();
                s.CurrentMonth[item].DisplayWorkingHoursV2ToFile(writer);
                writer.WriteLine("---------------------------------------");
            }
            
            writer.WriteLine("--------------------------------GetNotSetHours--------------------------------------");

            int ttl = 0;
            foreach (var item in s.CurrentMonth.Keys)
            {
                ttl += s.CurrentMonth[item].GetNotSetHoursToFile(writer);
            }

            writer.WriteLine(ttl + " not assigned hours in total");
        }
        
        s.GetAllPlannedHours();
    }
}
