namespace practice;

public static class SetWorkersIntoSchedule
{
    public static void IsAvailableWithTimeManager(DateOnly date, List<Worker> workers, int from, int to, TimeManager timeManager) // add worker for certain day
    {
        foreach (var person in workers)
        {
            timeManager.Date = date;
            if (!person.Availability.ContainsKey(date) || person.CanWorkToday(date) == false)
            {
                continue;
            }
            int[] ava = person.Availability[date];
            
            int[] toCheck = Enumerable.Range(from, to - from).ToArray();

            if (toCheck.All(x => ava.Contains(x)))
            {
                Console.WriteLine($"{person.Name} is available for {date}. All available hours at this day: {string.Join(", ", person.Availability[date])}");
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
    
    public static void IsAvailableWithTimeManagerAndTempDict(DateOnly date, List<Worker> workers, int from, int to, TimeManager timeManager)
    {
        Dictionary<int, List<Worker>> tempDict = new Dictionary<int, List<Worker>>();
        foreach (var person in workers)
        {
            timeManager.Date = date;
            if (!person.Availability.ContainsKey(date) || person.CanWorkToday(date) == false)
            {
                continue;
            }
            int[] ava = person.Availability[date];
            
            int[] toCheck = Enumerable.Range(from, to - from).ToArray();
            
            if (toCheck.All(x => ava.Contains(x)))
            {
                Console.WriteLine($"{person.Name} is available for {date}. All available hours at this day: {string.Join(", ", person.Availability[date])}");
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