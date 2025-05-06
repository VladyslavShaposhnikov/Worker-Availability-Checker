namespace practice;

public class Worker
{
    public string Name { get; set; }
    public int WorkerId { get; set; }
    public int StartRow { get; set; } = 4; // default to 4 if not set
    public int StartColumn { get; set; }
    public int Priority { get; set; } = 3;
    public int HoursAtMonth { get; set; }

    public Dictionary<DateOnly, int[]> Dyspo { get; set; } = new();

    public void ShowAvailability()
    {
        Console.WriteLine($"{Name} with id {WorkerId} is available at this hours:");
        foreach (var key in Dyspo.Keys)
        {
            Console.WriteLine($"{key}: {string.Join(", ", Dyspo[key])}");
        }
    }
}