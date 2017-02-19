using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEngine : MonoBehaviour
{

    public MainGui Gui;
    public List<House> Houses = new List<House>();
	// Use this for initialization
	void Start ()
    {
        Init();
        Gui.Init(this);
		Gui.Refresh();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Init()
    {
       Houses.Add(new House("Training", "Bob"));
    }

    [Serializable]
    public class House
    {
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
            Charateristics[0] = new Charateristic("Strenght", 10);
            Charateristics[1] = new Charateristic("Size", 10);
            Charateristics[2] = new Charateristic("Dexterity", 10);
            Charateristics[3] = new Charateristic("Agility", 10);
            Charateristics[4] = new Charateristic("Intelligence", 10);
            Charateristics[5] = new Charateristic("Wits", 10);
            Charateristics[6] = new Charateristic("Charisme", 10);
            Charateristics[7] = new Charateristic("Mental", 10);
        }

        public double Average
        {
            get
            {
                return Charateristics.Average(x => x.Value); 
            }
        }
    }

    [Serializable]
    public class Charateristic
    {
        public int Value;
        public string Name;

        public Charateristic(string name, int v)
        {
            this.Name = name;
            this.Value = v;
        }
    }

    public void NewGeneration(int houseIndex)
    {
        House house = Houses[houseIndex];
        if (house.Nurcery.Count < house.NurcerySize)
        {
            Guy guy= new Guy(house.FamillyName, house.IdCounter++, UnityEngine.Random.Range(0, 2) == 0);
            for (int i = 0; i < 8; i++)
            {
                guy.Charateristics[i].Value = house.Dad.Charateristics[i].Value > house.Mom.Charateristics[i].Value ? 
                    UnityEngine.Random.Range(house.Mom.Charateristics[i].Value, house.Dad.Charateristics[i].Value + 1) : 
                    UnityEngine.Random.Range(house.Dad.Charateristics[i].Value, house.Mom.Charateristics[i].Value + 1);

                guy.Charateristics[i].Value += UnityEngine.Random.Range(0, 2) == 0 ? 0 : UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
            }
            house.Nurcery.Add(guy);
        }
    }
}
