using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodSugarSim
{
    class Program
    {

        public static int Previous = 0;

        public static int GetNormalizationValue(string time, Dictionary<DateTime, DateTime> sugarListEvents)
        {
            int result = 0;
            
            DateTime currentTimeStamp = Convert.ToDateTime(time);

            // Find the nearest sugar event which is less the the currentTimeStamp.
            DateTime nearest = DateTime.MinValue;

            foreach (KeyValuePair<DateTime, DateTime> kvp in sugarListEvents)
            {
                // Are we inside a sugar event?
                if (currentTimeStamp >= kvp.Key && currentTimeStamp <= kvp.Value)
                    return 0;
                // Capture the nearest past event.
                else if (kvp.Value < currentTimeStamp && currentTimeStamp > nearest)
                    nearest = kvp.Value;
            }

            // Get difference in minutes
            if (nearest != DateTime.MinValue)
            {
                TimeSpan timeSpan = currentTimeStamp.Subtract(nearest);
                result = (int) timeSpan.TotalMinutes;
            }

            return result;
        }

        static void PrintRange(DateTime start, DateTime end, TimeSeries ts)
        {
            string startstr = TimeSeries.GetKeyValue(start);
            string endstr = TimeSeries.GetKeyValue(end);
            
            foreach(KeyValuePair<string, double> kvp in ts.glycemicEvents)
            {
                if (kvp.Key.CompareTo(startstr) >= 0 && kvp.Key.CompareTo(endstr) <= 0)
                {
                    // Let's do some math to see if we've had idle time.
                    int normalization = GetNormalizationValue(kvp.Key, ts.sugarEventsList);
                    // normalization = 0;

                    int i = (int) (kvp.Value + 0.5) - normalization;
                        
                    if (i < Constants.SUGAR_FLOOR)
                        i = Constants.SUGAR_FLOOR;

                    Previous = i;

                    Console.WriteLine("At {0} the glycemic level was {1}", kvp.Key, i);
                }
            }
        }

        private static ExerciseEntity GetExercise(LoadExercises exercises)
        {
            return exercises.ExerciseValues["Sprinting".ToLower()];
        }

        private static FoodEntity GetFood(LoadFood loadFood)
        {
            return loadFood.FoodValues["Banana cake, made with sugar".ToLower()];
        }

        static void Main(string[] args)
        {
            LoadFood loadFood = new LoadFood(@"~\..\..\..\AppData\FoodDB.csv");
            CsvLoader foodLoader = new CsvLoader(loadFood);
            foodLoader.LoadCsvFile();

            LoadExercises exercises = new LoadExercises(@"~\..\..\..\AppData\ExerciseDB.csv");
            CsvLoader exerciseLoader = new CsvLoader(exercises);
            exerciseLoader.LoadCsvFile();

            DateTime start = new DateTime(2018, 05, 08);
            DateTime end = new DateTime(2018, 05, 09);

            // Go with this hardcoded interval in seconds.
            int interval = 60 * 30;
            TimeSeries ts = new TimeSeries(start, end, interval);
            FoodEntity foodEntity = GetFood(loadFood);
            // Hack
            foodEntity.GlycemicIndex = 200;
            ts.InsertEvent(foodEntity, start.AddHours(1), interval);

            ExerciseEntity exerciseEntity = GetExercise(exercises);
            // Hack
            exerciseEntity.ExerciseIndex = 200;
            ts.InsertEvent(exerciseEntity, start.AddHours(7), interval);

            Console.WriteLine("Start Output");
            PrintRange(start, end, ts);

            return;
        }
    }
}
