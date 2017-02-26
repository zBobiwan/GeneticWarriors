using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Engine
{
    [Serializable]
    public class House
    {
        public enum HouseType
        {
            Training,
            Building,
            Research,
            Medecine,
            Crafting,
            Enconomy,
            Farming,
            Marketing
        }

        public Guy Dad;
        public Guy Mom;
        public Guy Champion;
        public List<Guy> Nurcery = new List<Guy>();
        public List<Guy> HouseGuys = new List<Guy>();
        public HouseType Type;
        public string FamillyName;

        public int HouseSize = 2;
        public int BuildHouseCounter = 0;
        public int NurcerySize = 1;
        public int BuildNurceryCounter = 0;

        public int IdCounter = 1;

        public int BirthCounter = 0;
        public int BirthTime = 10;

        public int ChampionCounter = 0;
        public int ChampionActionTime = 30;

        public int Index;

        public int Price
        {
            get { return (int)(Math.Pow(1.9f, Index*1.5)*100); }
        }

        public int PriceMultiplier
        {
            get { return (int) Math.Pow(2, Index); }
        }

        public int Power;
        public float HouseRatio = 0.1f;

        private House()
        {
        }

        public House(HouseType type, string familly, int index)
        {
            this.Type = type;
            this.FamillyName = familly;
            this.Index = index;
            Dad = new Guy(FamillyName, IdCounter++, true);
            Mom = new Guy(FamillyName, IdCounter++, false);
        }

        public int NextHousePrice
        {
            get { return (int) (HouseSize*HouseSize*100*PriceMultiplier); }
        }

        public int NextNurceryPrice
        {
            get { return (int) (HouseSize*HouseSize*100*PriceMultiplier); }
        }

        public void PromoteToParent(Guy guy)
        {
            if (guy.Man)
            {
                Dad = guy;
            }
            else
            {
                Mom = guy;
            }
            Nurcery.Remove(guy);
            HouseGuys.Remove(guy);
        }

        public void PromoteToChampion(Guy guy)
        {
            Champion = guy;
            Nurcery.Remove(guy);
            HouseGuys.Remove(guy);
        }

        public void PromoteToHouse(Guy guy)
        {
            HouseGuys.Add(guy);
            Nurcery.Remove(guy);
        }

        public void Sacrifice(Guy guy)
        {
            Nurcery.Remove(guy);
            HouseGuys.Remove(guy);
        }

        public void Tick(GameEngine engine)
        {
            Power = ComputePower();

            if (Nurcery.Count < NurcerySize)
            {
                ++BirthCounter;
                if (BirthCounter == BirthTime)
                {
                    NewGeneration();
                    BirthCounter = 0;
                }
            }

            if (Champion != null && Champion.Id != 0)
            {
                ++ChampionCounter;
                if (ChampionCounter == ChampionActionTime)
                {
                    DoChampionAction(engine);
                    ChampionCounter = 0;
                }
            }
        }

        private int ComputePower()
        {
            if (Type == HouseType.Training)
            {
                return (int)GetMembers(addChampion: true, addHouse: true)
                    .Sum(x => x.GetCharateristic(Charateristic.Type.Strenght)*(x == Champion ? 1.0f : HouseRatio));
            }
            else if (Type == HouseType.Building)
            {
                return (int)GetMembers(addChampion: true, addHouse: true)
                    .Sum(x => x.GetCharateristic(Charateristic.Type.Dexterity) * (x == Champion ? 1.0f : HouseRatio));
            }

            return 0;
        }

        private void DoChampionAction(GameEngine engine)
        {
            if (Type == House.HouseType.Training)
            {
                engine.GetResource(Resource.RessourceType.Gold)
                    .Add(Power);
            }
            if (Type == HouseType.Building)
            {
                foreach (House house in engine.Houses)
                {
                    if (house.BuildHouseCounter > 0)
                    {
                        house.BuildHouseCounter -= Power;
                        if (house.BuildHouseCounter <= 0)
                        {
                            house.BuildHouseCounter = 0;
                            house.HouseSize++;
                        }
                    }

                    if (house.BuildNurceryCounter > 0)
                    {
                        house.BuildNurceryCounter -= Power;
                        if (house.BuildNurceryCounter <= 0)
                        {
                            house.BuildNurceryCounter = 0;
                            house.NurcerySize++;
                        }
                    }
                }
            }
        }

        public void NewGeneration()
        {
            if (Nurcery.Count < NurcerySize)
            {
                Guy guy = new Guy(FamillyName, IdCounter++, UnityEngine.Random.Range(0, 2) == 0);
                for (int i = 0; i < 8; i++)
                {
                    guy.Charateristics[i].Value = Dad.Charateristics[i].Value > Mom.Charateristics[i].Value
                        ? UnityEngine.Random.Range(Mom.Charateristics[i].Value, Dad.Charateristics[i].Value + 1)
                        : UnityEngine.Random.Range(Dad.Charateristics[i].Value, Mom.Charateristics[i].Value + 1);

                    guy.Charateristics[i].Value += UnityEngine.Random.Range(0, 2) == 0
                        ? 0
                        : UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
                }
                Nurcery.Add(guy);
            }
        }

        public IEnumerable<Guy> GetMembers(
            bool addParent = false,
            bool addChampion = false,
            bool addHouse = false,
            bool addNurcery = false)
        {
            if (addParent && Mom != null && Mom.Id != 0)
            {
                yield return Mom;
            }

            if (addParent && Dad != null && Dad.Id != 0)
            {
                yield return Dad;
            }

            if (addChampion && Champion != null && Champion.Id != 0)
            {
                yield return Champion;
            }

            if (addHouse)
            {
                foreach (Guy guy in HouseGuys)
                {
                    yield return guy;
                }
            }

            if (addNurcery)
            {
                foreach (Guy guy in Nurcery)
                {
                    yield return guy;
                }
            }
        }


        public int GetMaxGold()
        {
            return GetMembers(addParent: true, addChampion: true, addHouse: true)
                .Sum(x => x.GetCharateristic(Charateristic.Type.Size)*10);
        }

        public bool IsBuilding()
        {
            return BuildHouseCounter != 0 || BuildNurceryCounter != 0;
        }
    }
}