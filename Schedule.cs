namespace practice;

public class Schedule
{
    public Schedule(int daysInMonth, int month)
    {
        for (int i = 1; i <= daysInMonth; i++)
        {
            DateOnly onlyDate = DateOnly.Parse($"{month}/{i}/2025");
            if (onlyDate.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }
            if (onlyDate.DayOfWeek == DayOfWeek.Monday || onlyDate.DayOfWeek == DayOfWeek.Thursday)
            {
                CurrentMonth.Add(onlyDate, new TimeManager(onlyDate, 6, 22));
            }
            else
            {
                CurrentMonth.Add(onlyDate, new TimeManager(onlyDate, 9, 22));
            }
        }
    }
    public Dictionary<DateOnly, TimeManager> CurrentMonth { get; set; } = new();

    public void GetAllPlannedHours()
    {
        int res = 0;
        foreach (var key in CurrentMonth.Keys)
        {
            res += CurrentMonth[key].GetAllRequiredHoursSum();
        }

        Console.WriteLine($"Total hours: {res}");
    }
}