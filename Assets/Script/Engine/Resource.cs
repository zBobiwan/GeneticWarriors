using System;

namespace Assets.Script.Engine
{
    [Serializable]
    public class Resource
    {
        public enum RessourceType
        {
            Gold,
        }

        public RessourceType Type;
        public int Value;
        public int Max;

        private Resource()
        {
        }

        public Resource(RessourceType type)
        {
            Type = type;
        }

        public void Add(int value)
        {
            Value += value;
            if (Value > Max)
            {
                Value = Max;
            }
        }

        public void Remove(int value)
        {
            Value -= value;
            if (Value < 0)
            {
                Value = 0;
            }
        }
    }
}