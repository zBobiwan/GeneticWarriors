using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.Script.Engine
{
    [Serializable]
    public class GameEngine
    {
        public List<House> Houses = new List<House>();
        public List<House> FuturHouses = new List<House>();
        public List<Resource> Resources = new List<Resource>();
        public float CurrentTick;
        public float TickTime = 0.1f;

        // Update is called once per frame
        void Update()
        {
           
        }

        public void Tick()
        {
            CurrentTick += Time.deltaTime;
            if (CurrentTick > TickTime)
            {
                int maxGold = 0;
                foreach (House house in Houses)
                {
                    house.Tick(this);
                    maxGold += house.GetMaxGold();
                }
                CurrentTick -= TickTime;

                GetResource(Resource.RessourceType.Gold).Max = maxGold;
                XmlSerializer ser = new XmlSerializer(typeof(GameEngine));
                TextWriter writer = new StringWriter();
                ser.Serialize(writer, this);

                PlayerPrefs.SetString("save", writer.ToString());
            }
        }

        public void Init()
        {
            CurrentTick = 0;
            Houses.Add(new House(House.HouseType.Training, "Bob", 0, 1));
            FuturHouses.Add(new House(House.HouseType.Building, "Billy", 60, 2));
            FuturHouses.Add(new House(House.HouseType.Research, "Johnny", 150, 4));
            FuturHouses.Add(new House(House.HouseType.Medecine, "Mike", 250, 7));
            FuturHouses.Add(new House(House.HouseType.Crafting, "James", 1000, 11));
            FuturHouses.Add(new House(House.HouseType.Farming, "Dylan", 3000, 16));
            FuturHouses.Add(new House(House.HouseType.Enconomy, "Kevin", 10000, 22));
            FuturHouses.Add(new House(House.HouseType.Marketing, "Nick", 50000, 29));
            Resources.Add(new Resource(Resource.RessourceType.Gold));
        }

        public void NewGeneration(int houseIndex)
        {
            House house = Houses[houseIndex];
            house.NewGeneration();

        }

        public Resource GetResource(Resource.RessourceType type)
        {
            return Resources.First(x => x.Type == type);
        }

        public bool IsHouseBuild(House.HouseType houseType)
        {
            return Houses.Exists(x => x.Type == houseType);
        }

        public string Save(object obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public void CreateNextHouse()
        {
            if (FuturHouses.Count > 0)
            {
                House house = FuturHouses.First();
                this.GetResource(Resource.RessourceType.Gold).Remove(house.Price);
                Houses.Add(house);
                FuturHouses.RemoveAt(0);
            }
        }
    }
}
