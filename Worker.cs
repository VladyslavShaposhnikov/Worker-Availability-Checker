namespace practice;

public class Worker
{
    public string Name { get; set; }
    public int WorkerId { get; set; }
    public int StartRow { get; set; } = 4; // default to 4 if not set
    public int StartColumn { get; set; }
    public int Priority { get; set; } = 3;
    public Dictionary<DateOnly, int> HoursAtMonth { get; set; } = new();
    public List<DateOnly> WorkDay { get; set; } = new();
    
    public Dictionary<DateOnly, int[]> Dyspo { get; set; } = new();

    public bool CanWorkToday(DateOnly date) // return false in case worker cant work this day because already has shift today
    {
        return !WorkDay.Contains(date);
    }

    public void MarkAsWorkDay(DateOnly date)
    {
        if (WorkDay.Contains(date))
        {
            return;
        }
        WorkDay.Add(date);
    }
    public int GetHoursForPrevDay(DateOnly currentDate)
    {
        if (currentDate.Day == 1)
        {
            return 0;
        }

        int ttl = 0;
        for (int i = 1; i <= currentDate.Day - 1; i++)
        {
            if (HoursAtMonth.ContainsKey(DateOnly.Parse($"{currentDate.Month}/{i}/{currentDate.Year}")))
            {
                ttl += HoursAtMonth[DateOnly.Parse($"{currentDate.Month}/{i}/{currentDate.Year}")];
            }
        }
        return ttl;
    }
    
    public void ShowAvailability()
    {
        Console.WriteLine($"{Name} with id {WorkerId} is available at this hours:");
        foreach (var key in Dyspo.Keys)
        {
            Console.WriteLine($"{key}: {string.Join(", ", Dyspo[key])}");
        }
    }
}