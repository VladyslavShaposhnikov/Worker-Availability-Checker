namespace practice;

public class TimeManager
{
    public TimeManager(int from, int to)
    {
        InitWorkingHours(from, to);
    }
    
    public DateOnly Date { get; set; } // what day is it (4-th of May)
    public Dictionary<int, List<Worker>> WorkingHours { get; set; } = new(); // key is certain working hour, value is list of assigned workers
    public Dictionary<int, int> RequiredWorkers { get; set; } = new(); // key is certain working hour, value is required amount of workers needed for this hour

    private void InitWorkingHours(int from, int to) // initialization of working hours (from 9 to 22)
    {
        for (int i = from; i < to; i++)
        {
            WorkingHours.Add(i, new List<Worker>());
            RequiredWorkers.Add(i, 3); // value "3" is hardcoded
        }
    }

    public void AddWorker(Worker worker, int hour)
    {
        WorkingHours[hour].Add(worker);
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
            Console.WriteLine($"{hour}: {WorkingHours[hour].Count}:");
            foreach (var worker in WorkingHours[hour])
            {
                Console.WriteLine($"\t{worker.Name}");
                Console.WriteLine($"\t{worker.WorkerId}");
            }
            Console.WriteLine();
        }
    }
}