using OfficeOpenXml;

namespace practice;

public static class ExcelHelpers
{
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

    public static void ParseAvailability(List<Worker> workers, ExcelWorksheet worksheet, int daysInMonth, int month)
    {
        foreach (var worker in workers)
        {
            int[] from = SaveToArray(worker.StartRow, worker.StartColumn, daysInMonth, worksheet);
            
            int[] untill = SaveToArray(worker.StartRow, worker.StartColumn + 2, daysInMonth, worksheet);
            
            for (int i = 0; i < daysInMonth; i++)
            {
                string day = $"{month}/{i + 1}/2025";
                int[] res = Enumerable.Range(from[i], untill[i]).ToArray();
                worker.Dyspo[DateOnly.Parse(day)] = res;
            }
        }
    }
    
    public static void IsAvailableWithTimeManaget(DateOnly date, List<Worker> workers, int from, int to, TimeManager timeManager) // add worker for certain day
    {
        foreach (var person in workers)
        {
            timeManager.Date = date;
            if (!person.Dyspo.ContainsKey(date) || person.CanWorkToday(date) == false)
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
                        timeManager.AddWorker(person, i, date);
                    }
                }
            }
        }
    }
    
    public static void IsAvailableWithTimeManagetAndTempDict(DateOnly date, List<Worker> workers, int from, int to, TimeManager timeManager)
    {
        Dictionary<int, List<Worker>> tempDict = new Dictionary<int, List<Worker>>();
        foreach (var person in workers)
        {
            timeManager.Date = date;
            if (!person.Dyspo.ContainsKey(date) || person.CanWorkToday(date) == false)
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

        AddWorkersFromTemporaryDictionary(tempDict, date, timeManager);
    }

    private static void AddWorkersFromTemporaryDictionary(Dictionary<int, List<Worker>> tempDict, DateOnly date, TimeManager timeManager)
    {
        foreach (var key in tempDict.Keys)
        {
            List<Worker> prior1 = tempDict[key].Where(x => x.Priority == 1).OrderBy(x => x.GetHoursForPrevDay(date)).ToList();
            List<Worker> prior2 = tempDict[key].Where(x => x.Priority == 2).OrderBy(x => x.GetHoursForPrevDay(date)).ToList();
            List<Worker> prior3 = tempDict[key].Where(x => x.Priority == 3).OrderBy(x => x.GetHoursForPrevDay(date)).ToList(); 
            
            if (prior1.Count != 0)
            {
                if (!timeManager.IsPropertLenght(key))
                {
                    timeManager.AddWorker(prior1[0], key, date);
                }
            }
            if (prior2.Count != 0)
            {
                if (!timeManager.IsPropertLenght(key))
                {
                    timeManager.AddWorker(prior2[0], key, date);
                }
            }
            if (prior3.Count != 0)
            {
                foreach (var i in prior3)
                {
                    if (!timeManager.IsPropertLenght(key))
                    {
                        timeManager.AddWorker(i, key, date);
                    }
                }
            }
        }
    }
}