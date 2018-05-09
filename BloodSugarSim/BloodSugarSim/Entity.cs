using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodSugarSim
{
    struct Constants
    {
        public const int SUGAR_FLOOR = 80;
    }

    public class Entity
    {
        public Entity() { }
        public Entity(int Id)
        {
            this.Id = Id;
        }

        public int Id { get; set; }
    }
}
