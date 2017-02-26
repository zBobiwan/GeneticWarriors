using System;

namespace Assets.Script.Engine
{
    [Serializable]
    public class Charateristic
    {
        public enum Type
        {
            Strenght,
            Size,
            Agility,
            Dexterity,
            Intelligence,
            Wits,
            Mental,
            Charisme
        }
        public int Value;
        public Type CharateristicType;

        private Charateristic()
        {
        }

        public Charateristic(Type type, int v)
        {
            this.CharateristicType = type;
            this.Value = v;
        }
    }
}