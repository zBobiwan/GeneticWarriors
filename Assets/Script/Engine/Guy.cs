using System;
using System.Linq;

namespace Assets.Script.Engine
{
    [Serializable]
    public class Guy
    {

        public int Id;
        public string FamillyName;
        public Charateristic[] Charateristics = new Charateristic[8];
        public bool Man;

        private Guy()
        {
        }

        public Guy(string name, int id, bool man)
        {
            Man = man;
            Id = id;
            FamillyName = name;
            Charateristics[0] = new Charateristic(Charateristic.Type.Strenght, 10);
            Charateristics[1] = new Charateristic(Charateristic.Type.Size, 10);
            Charateristics[2] = new Charateristic(Charateristic.Type.Agility, 10);
            Charateristics[3] = new Charateristic(Charateristic.Type.Dexterity, 10);
            Charateristics[4] = new Charateristic(Charateristic.Type.Intelligence, 10);
            Charateristics[5] = new Charateristic(Charateristic.Type.Wits, 10);
            Charateristics[6] = new Charateristic(Charateristic.Type.Mental, 10);
            Charateristics[7] = new Charateristic(Charateristic.Type.Charisme, 10);
        }

        public double Average
        {
            get
            {
                return Charateristics.Average(x => x.Value);
            }
        }

        public int GetCharateristic(Charateristic.Type type)
        {
            return Charateristics.First(x => x.CharateristicType == type).Value;
        }
    }
}