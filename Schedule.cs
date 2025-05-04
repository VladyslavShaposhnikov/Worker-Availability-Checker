namespace practice;

public class Schedule
{
    public Schedule(int daysInMonth)
    {
        for (int i = 1; i <= daysInMonth; i++)
        {
            DateOnly onlyDate = DateOnly.Parse($"4/{i}/2025");
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
}