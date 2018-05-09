using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace BloodSugarSim
{
    public class FoodEntity : Entity
    {
        private FoodEntity()
        {
        }

        public FoodEntity(int Id, string Name, int GlycemicIndex)
        {
            base.Id = Id;
            this.Name = Name;
            this.GlycemicIndex = GlycemicIndex;
        }

        public string Name { get; set; }
        public int GlycemicIndex { get; set; }
    }

    public class LoadFood : ILoadFile
    {
        public Dictionary<string, FoodEntity> FoodValues = new Dictionary<string, FoodEntity>();

        public string pathName { get; set; }
        public LoadFood(string pathName)
        {
            this.pathName = pathName;
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
                        FoodEntity ee = new FoodEntity(Convert.ToInt32(parts[0]), parts[1].ToLower(), Convert.ToInt32(parts[2]));
                        FoodValues.Add(ee.Name, ee);
                    }
                }
            }

            return;
        }
    }

}
