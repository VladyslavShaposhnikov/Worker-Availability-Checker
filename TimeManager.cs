namespace practice;

public class TimeManager
{
    public TimeManager(DateOnly dateOnly, int from, int to)
    {
        InitWorkingHours(from, to);
        SetDefaultRequiredWorkers(dateOnly.DayOfWeek);
    }
    
    public DateOnly Date { get; set; } // what day is it (4-th of May)
    public Dictionary<int, List<Worker>> WorkingHours { get; set; } = new(); // key is certain working hour, value is list of assigned workers
    public Dictionary<int, int> RequiredWorkers { get; set; } = new(); // key is certain working hour, value is required amount of workers needed for this hour

    private void InitWorkingHours(int from, int to) // initialization of working hours (from 9 to 22)
    {
        for (int i = from; i < to; i++)
        {
            WorkingHours.Add(i, new List<Worker>());
        }
    }
    
    public void AddWorker(Worker worker, int hour, DateOnly date)
    {
        WorkingHours[hour].Add(worker);
        worker.MarkAsWorkDay(date); // not nessesory to change it every hour
        if (!worker.HoursAtMonth.ContainsKey(date))
        {
            worker.HoursAtMonth.Add(date, 1);
        }
        worker.HoursAtMonth[date]++;
    }

    public void SetDefaultRequiredWorkers(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Monday:
                MondayThursdayLoop();
                break;
            case DayOfWeek.Tuesday:
                TuesdayWednesdayLoop();
                break;
            case DayOfWeek.Wednesday:
                TuesdayWednesdayLoop();
                break;
            case DayOfWeek.Thursday:
                MondayThursdayLoop();
                break;
            case DayOfWeek.Friday:
                WeekendsLoop();
                break;
            case DayOfWeek.Saturday:
                WeekendsLoop();
                break;
            case DayOfWeek.Sunday:
                WeekendsLoop();
                break;
        }
    }

    private void MondayThursdayLoop()
    {
        for (int i = 6; i <= 12; i++)
        {
            RequiredWorkers.Add(i, 4);
        }
        for (int i = 13; i <= 22; i++)
        {
            RequiredWorkers.Add(i, 3);
        }
    }
    
    private void TuesdayWednesdayLoop()
    {
        for (int i = 9; i <= 22; i++)
        {
            RequiredWorkers.Add(i, 3);
        }
    }

    private void WeekendsLoop()
    {
        for (int i = 9; i <= 22; i++)
        {
            RequiredWorkers.Add(i, 4);
        }
    }
    public bool IsPropertLenght(int hour)
    {
        if (WorkingHours.ContainsKey(hour) && RequiredWorkers.ContainsKey(hour))
        {
            if (WorkingHours[hour].Count == RequiredWorkers[hour])
            {
                Console.WriteLine("All workers are set up for this hour");
                return true;
            }
            else
            {
                Console.WriteLine($"Workers count {WorkingHours[hour].Count} but required workers count {RequiredWorkers[hour]}");
                return false;
            }
        }
        
        Console.WriteLine("There is no such key(s)");
        return false;
    }

    public void ChengeRequiredWorkers()
    {
        throw new NotImplementedException();
    }

    public void DisplayWorkingHours()
    {
        foreach (var hour in WorkingHours.Keys)
        {
            Console.WriteLine($"Hour: {hour}: Workers: {WorkingHours[hour].Count}:");
            foreach (var worker in WorkingHours[hour])
            {
                Console.WriteLine("-        -       -");
                Console.WriteLine($"\t{worker.Name}");
                Console.WriteLine($"\t{worker.WorkerId}");
            }
            Console.WriteLine("-        -       -");
            Console.WriteLine();
        }
    }
    
    public void DisplayWorkingHoursToFile(StreamWriter writer)
    {
        foreach (var hour in WorkingHours.Keys)
        {
            writer.WriteLine($"Hour: {hour}: Workers: {WorkingHours[hour].Count}:");
            foreach (var worker in WorkingHours[hour])
            {
                writer.WriteLine("-        -       -");
                writer.WriteLine($"\t{worker.Name}");
                writer.WriteLine($"\t{worker.WorkerId}");
            }
            writer.WriteLine("-        -       -");
            writer.WriteLine();
        }
    }
    
    public void GetNotSetHours()
    {
        foreach (var hour in WorkingHours.Keys)
        {
            if (WorkingHours[hour].Count != RequiredWorkers[hour])
            {
                Console.WriteLine($"{Date}, {Date.DayOfWeek}, Hour: {hour}");
                Console.WriteLine($"Workers: {WorkingHours[hour].Count}. Required workers count {RequiredWorkers[hour]}");
                Console.WriteLine("--------------------------------------------------------------------------------------");
            }

        }
    }
    
    public void GetNotSetHoursToFile(StreamWriter writer)
    {
        foreach (var hour in WorkingHours.Keys)
        {
            if (WorkingHours[hour].Count != RequiredWorkers[hour])
            {
                writer.WriteLine($"{Date}, {Date.DayOfWeek}, Hour: {hour}");
                writer.WriteLine($"Workers: {WorkingHours[hour].Count}. Required workers count {RequiredWorkers[hour]}");
                writer.WriteLine("--------------------------------------------------------------------------------------");
            }

        }
    }
}