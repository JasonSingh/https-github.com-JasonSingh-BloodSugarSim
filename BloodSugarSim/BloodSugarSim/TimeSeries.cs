using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodSugarSim
{
    // Sugar Rules
    // Always start/reset at Constants.SUGAR_FLOOR at midnight.
    // Food. Added over 2 hours.
    // Exercise. Decrease over 1 hour.
    // Iff nothing incremented or decremented

    public enum EventType { Undefined = 0, Exercise = 1, Food = 2}

    public class TimeSeries
    {
        public SortedDictionary<string, double> glycemicEvents = new SortedDictionary<string, double>();
        public Dictionary<DateTime, DateTime> sugarEventsList = new Dictionary<DateTime, DateTime>();

        public DateTime Start = DateTime.Now;
        public DateTime End = DateTime.Now.AddDays(1);
        public int Interval;

        private void RegisterEvent(DateTime startTime, EventType eventType)
        {
            DateTime endTime = startTime;

            if (eventType == EventType.Exercise)
                endTime = endTime.AddHours(1);

            if (eventType == EventType.Food)
                endTime = endTime.AddHours(2);

            sugarEventsList.Add(startTime, endTime);
        }

        public TimeSeries(DateTime start, DateTime end, int seconds)
        {
            // Populate initial values each minute
            Start = start;
            End = end;
            Interval = seconds;

            while (start < end)
            {
                string str = GetKeyValue(start);
                glycemicEvents.Add(str, Constants.SUGAR_FLOOR);
                start = start.AddSeconds(seconds);
            }
        }

        public static string GetKeyValue(DateTime time)
        {
            return time.ToString("s");
        }

        
        public void InsertEvent(ExerciseEntity exerciseEvent, DateTime time, int interval)
        {
            DateTime start = time;
            DateTime end = start;
            end = end.AddHours(1);

            DateTime EndOfDay = time.AddDays(1);
            EndOfDay = EndOfDay.AddHours(-EndOfDay.Hour);
            EndOfDay = EndOfDay.AddMinutes(-EndOfDay.Minute);
            EndOfDay = EndOfDay.AddSeconds(-EndOfDay.Second);

            RegisterEvent(time, EventType.Exercise);

            int i = (60 * 60) / interval;

            double increments = (double)((double) exerciseEvent.ExerciseIndex / i);
            int counter = 0;

            double Total = 0;
            while (start < end && start <= EndOfDay)
            {
                counter++;
                start = start.AddSeconds(Interval);
                string key = GetKeyValue(start);

                if (glycemicEvents.ContainsKey(key))
                {
                    Total += increments;
                    double currentValue = glycemicEvents[key];
                    // Keep adding for exercise.
                    currentValue -= Total;
                    glycemicEvents[key] = currentValue;
                }
                else
                    throw new ApplicationException("Operation attempted on uninitialized data");
            }
            // TODO. Assert Counter and TWOHOURS ARE EQUAL

            string endstr = GetKeyValue(end);
            string endofdaystr = GetKeyValue(EndOfDay);

            // Apply the Total to the rest of the day. Store in temp.
            Dictionary<string, double> tmpDict = new Dictionary<string, double>();

            foreach (KeyValuePair<string, double> kvp in glycemicEvents)
            {
                if (kvp.Key.CompareTo(endstr) > 0 && kvp.Key.CompareTo(endofdaystr) < 0)
                {
                    double currentValue = kvp.Value;
                    currentValue -= Total;
                    tmpDict[kvp.Key] = currentValue;
                }
            }

            // Get from temp and update.
            foreach (KeyValuePair<string, double> kvp in tmpDict)
                glycemicEvents[kvp.Key] = kvp.Value;

            return;
         
        }

        public void InsertEvent(FoodEntity foodEvent, DateTime time, int interval)
        {
            DateTime start = time;
            DateTime end = start;
            end = end.AddHours(2);

            DateTime EndOfDay = time.AddDays(1);
            EndOfDay = EndOfDay.AddHours(-EndOfDay.Hour);
            EndOfDay = EndOfDay.AddMinutes(-EndOfDay.Minute);
            EndOfDay = EndOfDay.AddSeconds(-EndOfDay.Second);

            RegisterEvent(time, EventType.Food);

            int i = (120 * 60) / interval;

            double increments = (double)((double) foodEvent.GlycemicIndex / i);
            int counter = 0;

            double Total = 0;
            while (start < end && start <= EndOfDay)
            {
                counter++;
                start = start.AddSeconds(Interval);
                string key = GetKeyValue(start);

                if (glycemicEvents.ContainsKey(key))
                {
                    Total += increments;
                    double currentValue = glycemicEvents[key];
                    // Keep adding for food.
                    currentValue += Total;
                    glycemicEvents[key] = currentValue;
                }
                else
                    throw new ApplicationException("Operation attempted on uninitialized data");
            }
            // TODO. Assert Counter and TWOHOURS ARE EQUAL

            string endstr = GetKeyValue(end);
            string endofdaystr = GetKeyValue(EndOfDay);

            // Apply the Total to the rest of the day. Store in temp.
            Dictionary<string, double> tmpDict = new Dictionary<string, double>();

            foreach (KeyValuePair<string, double> kvp in glycemicEvents)
            {
                if (kvp.Key.CompareTo(endstr) > 0 && kvp.Key.CompareTo(endofdaystr) < 0)
                {
                    double currentValue = kvp.Value;
                    currentValue += Total;
                    tmpDict[kvp.Key] = currentValue;
                }
            }

            // Get from temp and update.
            foreach (KeyValuePair<string, double> kvp in tmpDict)
                glycemicEvents[kvp.Key] = kvp.Value;

            return;
        }
    }
}
