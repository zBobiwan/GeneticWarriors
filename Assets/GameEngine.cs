using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEngine : MonoBehaviour
{

    public MainGui Gui;
    public List<House> Houses = new List<House>();
    public List<Resource> Resources = new List<Resource>();
    public float CurrentTick;
    public float TickTime = 1;

	// Use this for initialization
	void Start ()
	{
	    CurrentTick = 0;
        Init();
        Gui.Init(this);
		Gui.Refresh();
	}
	
	// Update is called once per frame
	void Update ()
	{
        CurrentTick += Time.deltaTime;
	    if (CurrentTick > TickTime)
	    {
	        Tick();
	        CurrentTick -= TickTime;
	    }
	}

    private void Tick()
    {
        int maxGold = 0;
        foreach (House house in Houses)
        {
            house.Tick(this);
            maxGold += house.GetMaxGold();
        }

        GetResource(Resource.RessourceType.Gold).Max = maxGold;

    }

    public void Init()
    {
       Houses.Add(new House(House.HouseType.Training.ToString(), "Bob"));
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

        public bool Changed;
        public Guy Dad;
        public Guy Mom;
        public Guy Champion;
        public List<Guy> Nurcery = new List<Guy>();
        public List<Guy> HouseGuys = new List<Guy>();
        public string Name;
        public string FamillyName;

        public int HouseSize = 2;
        public int NurcerySize = 1;
        public int IdCounter = 1;

        public int BirthCounter = 0;
        public int BirthTime = 10;

        public int ChampionCounter = 0;
        public int ChampionActionTime = 30;

        public House(string name, string familly)
        {
            this.Name = name;
            this.FamillyName = familly;
            Dad = new Guy(FamillyName, IdCounter++, true);
            Mom = new Guy(FamillyName, IdCounter++, false);
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
            if (Nurcery.Count < NurcerySize)
            {
                ++BirthCounter;
                if (BirthCounter == BirthTime)
                {
                    NewGeneration();
                    BirthCounter = 0;
                    Changed = true;
                }
            }

            if (Champion != null && Champion.Id != 0)
            {
                ++ChampionCounter;
                if (ChampionCounter == ChampionActionTime)
                {
                    DoChampionAction(engine);
                    ChampionCounter = 0;
                    Changed = true;
                }
            }
        }

        private void DoChampionAction(GameEngine engine)
        {
            if (Name == House.HouseType.Training.ToString())
            {
                engine.GetResource(Resource.RessourceType.Gold).Add(Champion.GetCharateristic(Charateristic.Type.Strenght) / 10);
            }
        }

        public void NewGeneration()
        {
            if (Nurcery.Count < NurcerySize)
            {
                Guy guy = new Guy(FamillyName, IdCounter++, UnityEngine.Random.Range(0, 2) == 0);
                for (int i = 0; i < 8; i++)
                {
                    guy.Charateristics[i].Value = Dad.Charateristics[i].Value > Mom.Charateristics[i].Value ?
                        UnityEngine.Random.Range(Mom.Charateristics[i].Value, Dad.Charateristics[i].Value + 1) :
                        UnityEngine.Random.Range(Dad.Charateristics[i].Value, Mom.Charateristics[i].Value + 1);

                    guy.Charateristics[i].Value += UnityEngine.Random.Range(0, 2) == 0 ? 0 : UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
                }
                Nurcery.Add(guy);
            }
        }

        public IEnumerable<Guy> GetMembers(
            bool addParent = false, 
            bool addChampion=false, 
            bool addHouse=false, 
            bool addNurcery=false)
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
                .Sum(x => x.GetCharateristic(Charateristic.Type.Strenght));
        }
    }

    [Serializable]
    public class Guy
    {
        
        public int Id;
        public string FamillyName;
        public Charateristic[] Charateristics = new Charateristic[8];
        public bool Man;

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
            Charateristics[7] = new Charateristic(Charateristic.Type.Mental, 10);
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

        public Charateristic(Type type, int v)
        {
            this.CharateristicType = type;
            this.Value = v;
        }
    }

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
    }

    public void CreateNextHouse()
    {
        if (Houses.Count == 1)
        {
            Houses.Add(new House(House.HouseType.Building.ToString(), "Billy"));
        }
    }
}
