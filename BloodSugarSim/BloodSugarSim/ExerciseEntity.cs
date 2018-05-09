using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace BloodSugarSim
{

    public class ExerciseEntity : Entity
    {
        private ExerciseEntity()
        {
        }

        public ExerciseEntity(int Id, string Exercise, int ExerciseIndex)
        {
            base.Id = Id;
            this.Exercise = Exercise;
            this.ExerciseIndex = ExerciseIndex;
        }

        public string Exercise { get; set; }
        public int ExerciseIndex { get; set; }
    }

    public class LoadExercises: ILoadFile
    {
        public Dictionary<string, ExerciseEntity> ExerciseValues = new Dictionary<string, ExerciseEntity>();


        public string pathName { get; set; }

        public LoadExercises(string PathName)
        {
            this.pathName = PathName;
        }

        public void LoadFile()
        {
            bool done = false;
            int count = 0;
            using (TextFieldParser parser = new TextFieldParser(pathName))
            {
                parser.Delimiters = new string[] { "," };
                while (!done)
                {
                    string[] parts = parser.ReadFields();
                    count++;
                    if (parts == null)
                        done = true;
                    else if (parts.Length != 3)
                    {
                        // TODO. Put this line into an error collection and report
                        done = true;
                    }
                    else if (!done && count > 1)
                    {
                        ExerciseEntity ee = new ExerciseEntity(Convert.ToInt32(parts[0]), parts[1].ToLower(), Convert.ToInt32(parts[2]));
                        ExerciseValues.Add(ee.Exercise, ee);
                    }
                }
            }

            return;
        }
    }
}
