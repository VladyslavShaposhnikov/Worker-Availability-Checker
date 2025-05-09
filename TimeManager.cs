namespace practice;

public class TimeManager
{
    public TimeManager(DateOnly dateOnly, int from, int to)
    {
        InitWorkingHours(from, to);
        SetDefaultRequiredWorkers(dateOnly.DayOfWeek, from, to);
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
        worker.MarkAsWorkDay(date); // not necessary to change it every hour
        if (!worker.HoursAtMonth.ContainsKey(date))
        {
            worker.HoursAtMonth.Add(date, 1);
        }
        worker.HoursAtMonth[date]++;
    }
    
    private void SetDefaultRequiredWorkers(DayOfWeek dayOfWeek, int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            RequiredWorkers.Add(i, 3);
            if (dayOfWeek == DayOfWeek.Monday || dayOfWeek == DayOfWeek.Thursday)
            {
                if (new int[] { 6, 7, 8, 9, 10, 11, 12, 13 }.Contains(i))
                {
                    RequiredWorkers[i] += 1;
                    continue;
                }
            }
            if (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday)
            {
                if (new int[] { 12, 13, 14, 15, 16, 17, 18, 19 }.Contains(i))
                {
                    RequiredWorkers[i] += 1;
                }
            }
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

    public void ChangeRequiredWorkers(int hour, int changeTo)
    {
        RequiredWorkers[hour] = changeTo;
    }
    
    public void ChangeRequiredWorkersInRange(int hourFrom, int hourTo, int changeTo)
    {
        for (int i = hourFrom; i <= hourTo; i++)
        {
            RequiredWorkers[i] = changeTo;
        }
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
        int ttl = 0;
        foreach (var hour in WorkingHours.Keys)
        {
            if (WorkingHours[hour].Count != RequiredWorkers[hour])
            {
                Console.WriteLine($"{Date}, {Date.DayOfWeek}, Hour: {hour}");
                Console.WriteLine($"Workers: {WorkingHours[hour].Count}. Required workers count {RequiredWorkers[hour]}");
                Console.WriteLine("--------------------------------------------------------------------------------------");
                ttl++;
            }
        }
        Console.WriteLine($"Total hours: {ttl}");
    }
    
    public int GetNotSetHoursToFile(StreamWriter writer)
    {
        int ttl = 0;
        foreach (var hour in WorkingHours.Keys)
        {
            if (WorkingHours[hour].Count != RequiredWorkers[hour])
            {
                writer.WriteLine($"{Date}, {Date.DayOfWeek}, Hour: {hour}");
                writer.WriteLine($"Workers: {WorkingHours[hour].Count}. Required workers count {RequiredWorkers[hour]}");
                writer.WriteLine("--------------------------------------------------------------------------------------");
                ttl++;
            }
        }
        writer.WriteLine($"Total hours: {ttl}");
        return ttl;
    }
}