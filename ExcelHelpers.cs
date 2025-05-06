using OfficeOpenXml;

namespace practice;

public static class ExcelHelpers
{
    public static void IsAvailableForAllDays(List<Worker> workers, int daysInMonth, int from, int to)
    {
        for (int i = 1; i <= daysInMonth; i++)
        {
            string date = $"4/{i}/2025";
            Console.WriteLine($"Day: {date}");
            IsAvailable(DateOnly.Parse(date), workers, from, to);
            Console.WriteLine("--------------------------------------------------------");
            
        }
    }
    public static void IsAvailable(DateOnly date, List<Worker> workers, int from, int to)
    {
        foreach (var person in workers)
        {
            if (!person.Dyspo.ContainsKey(date))
            {
                continue;
            }
            int[] ava = person.Dyspo[date];
            
            int[] toCheck = Enumerable.Range(from, to - from).ToArray();

            if (toCheck.All(x => ava.Contains(x)))
            {
                Console.WriteLine($"{person.Name} is available for {date}. All available hourse at this day: {string.Join(", ", person.Dyspo[date])}");
            }
        }
    }
    public static int[] SaveToArray(int startRow, int column, int daysInMonth, ExcelWorksheet worksheet)
    {
        int[] from  = new int[daysInMonth];
        for (int row = startRow; row < startRow+daysInMonth; row++)
        {
            var value = worksheet.Cells[row, column].Text;
            from[row - startRow] = int.Parse(value);
        }
        return from;
    }

    public static void ParseAvailability(List<Worker> workers, ExcelWorksheet worksheet, int daysInMonth)
    {
        foreach (var worker in workers)
        {
            int[] from = SaveToArray(worker.StartRow, worker.StartColumn, daysInMonth, worksheet);
            
            int[] untill = SaveToArray(worker.StartRow, worker.StartColumn + 2, daysInMonth, worksheet);
            
            for (int i = 0; i < daysInMonth; i++)
            {
                string day = $"4/{i + 1}/2025";
                int[] res = Enumerable.Range(from[i], untill[i]).ToArray();
                worker.Dyspo[DateOnly.Parse(day)] = res;
            }
        }
    }
    
    public static void IsAvailableWithTimeManaget(DateOnly date, List<Worker> workers, int from, int to, TimeManager timeManager)
    {
        foreach (var person in workers)
        {
            timeManager.Date = date;
            if (!person.Dyspo.ContainsKey(date))
            {
                continue;
            }
            int[] ava = person.Dyspo[date];
            
            int[] toCheck = Enumerable.Range(from, to - from).ToArray();

            if (toCheck.All(x => ava.Contains(x)))
            {
                Console.WriteLine($"{person.Name} is available for {date}. All available hourse at this day: {string.Join(", ", person.Dyspo[date])}");
                foreach (var i in toCheck)
                {
                    if (!timeManager.IsPropertLenght(i))
                    {
                        timeManager.AddWorker(person, i);
                    }
                }
            }
        }
    }
    
    public static void IsAvailableForAllDaysWithTimeManager(List<Worker> workers, int daysInMonth, int from, int to, Schedule schedule)
    {
        
        for (int i = 1; i <= daysInMonth; i++)
        {
            string date = $"4/{i}/2025";
            TimeManager tm = schedule.CurrentMonth[DateOnly.Parse(date)];
            Console.WriteLine($"Day: {date}");
            IsAvailableWithTimeManaget(DateOnly.Parse(date), workers, from, to, tm);
            Console.WriteLine("--------------------------------------------------------");
        }
    }
    
    public static void IsAvailableWithTimeManagetAndTempDict(DateOnly date, List<Worker> workers, int from, int to, TimeManager timeManager)
    {
        Dictionary<int, List<Worker>> tempDict = new Dictionary<int, List<Worker>>();
        
        foreach (var person in workers)
        {
            timeManager.Date = date;
            if (!person.Dyspo.ContainsKey(date))
            {
                continue;
            }
            int[] ava = person.Dyspo[date];
            
            int[] toCheck = Enumerable.Range(from, to - from).ToArray();

            if (toCheck.All(x => ava.Contains(x)))
            {
                Console.WriteLine($"{person.Name} is available for {date}. All available hourse at this day: {string.Join(", ", person.Dyspo[date])}");
                foreach (var i in toCheck)
                {
                    if (!tempDict.ContainsKey(i))
                    {
                        tempDict.Add(i, new List<Worker>());
                    }
                    tempDict[i].Add(person);
                }
            }
        }

        foreach (var key in tempDict.Keys)
        {
            List<Worker> prior1 = tempDict[key].Where(x => x.Priority == 1).OrderBy(x => x.HoursAtMonth).ToList();
            List<Worker> prior2 = tempDict[key].Where(x => x.Priority == 2).OrderBy(x => x.HoursAtMonth).ToList();
            List<Worker> prior3 = tempDict[key].Where(x => x.Priority == 3).OrderBy(x => x.HoursAtMonth).ToList();
            
            
            if (prior1.Count != 0)
            {
                if (!timeManager.IsPropertLenght(key))
                {
                    timeManager.AddWorker(prior1[0], key);
                }
            }
            if (prior2.Count != 0)
            {
                if (!timeManager.IsPropertLenght(key))
                {
                    timeManager.AddWorker(prior2[0], key);
                }
            }
            if (prior3.Count != 0)
            {
                foreach (var i in prior3)
                {
                    if (!timeManager.IsPropertLenght(key))
                    {
                        timeManager.AddWorker(i, key);
                    }
                }
            }
        }
    }
}