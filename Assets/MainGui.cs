using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainGui : MonoBehaviour
{
    [Serializable]
    public class Colors
    {
        public Color Man;
        public Color SelectedMan;

        public Color Woman;
        public Color SelectedWoman;

        public Color Best;
        public Color Worst;

        public Color IsBetterColor(int value1, int value2, Color baseColor)
        {
            if (value1 > value2)
            {
                return Best;
            }
            else if (value2 > value1)
            {
                return Worst;
            }
            else
            {
                return baseColor;
            }
        }
    }

    public Colors GameColors;
    public int HouseIndex;
    public GameEngine Engine;
    public GameEngine.Guy Guy;
    public Button[] Tabs;
    public GameObject Parent;
    public Image ParentBar;
    
    public GameObject Champion;
    public Image ChampionBar;

    public GameObject House;
    public GameObject Nurcery;
    public GameObject Detail;

    public ChararcterGui Dad;
    public ChararcterGui Mom;
    public ChararcterGui ChampionGuy;
    public ChararcterGui[] HouseGuys;
    public ChararcterGui[] NurceryGuys;

    public Text GuyName;
    public Text GuyParent;
    public CharateristicGui[] Charateristics;
    public Button PromoteParent;
    public Button PromoteChampion;
    public Button PromoteHouse;
    public Button Sacrifice;

    public ResourceGui[] Resources;

    public void Init(GameEngine engine)
    {
        Engine = engine;
    }

    public void Update()
    {
        GameEngine.House house = Engine.Houses[HouseIndex];
        ParentBar.fillAmount = (float)house.BirthCounter / (float)house.BirthTime;
        ChampionBar.fillAmount = (float)house.ChampionCounter / (float)house.ChampionActionTime;
        Refresh();
        if (house.Changed)
        {
            Refresh();
            house.Changed = false;
        }
    }

    public void Refresh()
    {
        GameEngine.House house = Engine.Houses[HouseIndex];
        RefreshHouses();
        RefreshResources();
        RefreshHouses(house);
        RefreshParent(house);
        RefreshNurcery(house);
        RefreshHouses(house);
        RefreshDetail(house);
    }

    private void RefreshResources()
    {
        for (int index = 0; index < Engine.Resources.Count; index++)
        {
            GameEngine.Resource resource = Engine.Resources[index];
            Resources[index].Name.text = resource.Type.ToString();
            Resources[index].Current.text = resource.Value.ToString();
            Resources[index].Max.text = resource.Max.ToString();
        }
    }

    private void RefreshDetail(GameEngine.House house)
    {
        Detail.SetActive(Guy != null && Guy.Id != 0);
        if (Guy != null && Guy.Id != 0)
        {
            Detail.GetComponent<Image>().color = Guy.Man ? GameColors.Man : GameColors.Woman;
            GuyName.text = Guy.FamillyName + ":" + Guy.Id;
            GuyParent.text = Guy.Man ? "Dad" : "Mom";
            for (int i = 0; i < 8; i++)
            {
                GameEngine.Charateristic parent = Guy.Man ? house.Dad.Charateristics[i] : house.Mom.Charateristics[i];
                GameEngine.Charateristic champion = house.Champion == null || house.Champion.Id == 0 ? null : house.Champion.Charateristics[i];

                Charateristics[i].SetValue(this, Guy.Charateristics[i], parent, champion);
            }

            PromoteParent.interactable = Guy != house.Dad && Guy != house.Mom && Guy != house.Champion;
            PromoteChampion.interactable = Guy != house.Dad && Guy != house.Mom && Guy != house.Champion;
            PromoteParent.GetComponentInChildren<Text>().text = Guy.Man ? "Promote Dad" : "Promote Mom";
            Sacrifice.interactable = Guy != house.Dad && Guy != house.Mom && Guy != house.Champion;
            PromoteHouse.interactable = Guy != house.Dad && Guy != house.Mom && Guy != house.Champion && house.HouseGuys.Count < house.HouseSize && house.Nurcery.Contains(Guy);
        }
    }

    private void RefreshNurcery(GameEngine.House house)
    {
        for (int index = 0; index < NurceryGuys.Length; index++)
        {
            ChararcterGui guy = NurceryGuys[index];
            guy.gameObject.SetActive(index < house.NurcerySize);
            if (index < house.Nurcery.Count)
            {
                guy.Refresh(this, house.Nurcery[index], Guy);
            }
            else
            {
                guy.Refresh(this, null, Guy);
            }
        }
    }

    private void RefreshParent(GameEngine.House house)
    {
        Dad.Refresh(this, house.Dad, Guy);
        Mom.Refresh(this, house.Mom, Guy);

        ChampionGuy.Refresh(this, house.Champion, Guy);
        
    }

    private void RefreshHouses(GameEngine.House house)
    {
        for (int index = 0; index < HouseGuys.Length; index++)
        {
            ChararcterGui guy = HouseGuys[index];
            guy.gameObject.SetActive(index < house.HouseSize);
            if (index < house.HouseGuys.Count)
            {
                guy.Refresh(this, house.HouseGuys[index], Guy);
            }
            else
            {
                guy.Refresh(this, null, Guy);
            }
        }
    }

    private void RefreshHouses()
    {
        
        for (int index = 0; index < Tabs.Length; index++)
        {
            Button button = Tabs[index];
            if (index < Engine.Houses.Count)
            {
                button.GetComponentInChildren<Text>().text = Engine.Houses[index].Name;
                button.interactable = index != HouseIndex;
                button.gameObject.SetActive(true);
            }
            else if (index == Engine.Houses.Count && index != 7)
            {
                button.GetComponentInChildren<Text>().text = "Buy for "+index*60+" Gold";
                button.interactable = Engine.GetResource(GameEngine.Resource.RessourceType.Gold).Value >= 60; 
                button.gameObject.SetActive(true);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
    }


    public void Show(GameEngine.Guy guy)
    {
        Guy = guy;
        Refresh();
    }

    public void PromoteToParent()
    {
        Engine.Houses[HouseIndex].PromoteToParent(Guy);
        Refresh();
    }

    public void PromotetoChampion()
    {
        Engine.Houses[HouseIndex].PromoteToChampion(Guy);
        Refresh();
    }

    public void PromoteToHouse()
    {
        Engine.Houses[HouseIndex].PromoteToHouse(Guy);
        Refresh();
    }

    public void SacrificeGuy()
    {
        Engine.Houses[HouseIndex].Sacrifice(Guy);
        Guy = null;
        Refresh();
    }


    public void NewGeneration()
    {
        Engine.NewGeneration(HouseIndex);
        Refresh();
    }

    public void HouseClicked(int index)
    {
        if (Engine.Houses.Count == index)
        {
            Engine.CreateNextHouse();
        }

        HouseIndex = index;
    }
}
