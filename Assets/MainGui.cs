using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGui : MonoBehaviour
{
    [Serializable]
    public class Colors
    {
        public Color Man;
        public Color Woman;
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
    public GameObject Champion;
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

    public void Init(GameEngine engine)
    {
        Engine = engine;
    }

    public void Refresh()
    {
        GameEngine.House house = Engine.Houses[HouseIndex];
        for (int index = 0; index < Tabs.Length; index++)
        {
            Button button = Tabs[index];
            if (index < Engine.Houses.Count)
            {
                button.GetComponentInChildren<Text>().text = Engine.Houses[index].Name;
            }

            button.interactable = index != HouseIndex;
            button.gameObject.SetActive(index < Engine.Houses.Count);
        }

        Dad.Refresh(this, house.Dad);
        Mom.Refresh(this, house.Mom);
        ChampionGuy.Refresh(this, house.Champion);
        for (int index = 0; index < HouseGuys.Length; index++)
        {
            ChararcterGui guy = HouseGuys[index];
            guy.gameObject.SetActive(index < house.HouseSize);
            if (index < house.HouseGuys.Count)
            {
                guy.Refresh(this, house.HouseGuys[index]);
            }
            else
            {
                guy.Refresh(this, null);
            }
        }

        for (int index = 0; index < NurceryGuys.Length; index++)
        {
            ChararcterGui guy = NurceryGuys[index];
            guy.gameObject.SetActive(index < house.NurcerySize);
            if (index < house.Nurcery.Count)
            {
                guy.Refresh(this, house.Nurcery[index]);
            }
            else
            {
                guy.Refresh(this, null);
            }
        }

        Detail.SetActive(Guy != null && Guy.Id != 0);
        if (Guy != null && Guy.Id != 0)
        {
            Detail.GetComponent<Image>().color = Guy.Man ? GameColors.Man : GameColors.Woman;
            GuyName.text = Guy.FamillyName + ":" + Guy.Id;
            GuyParent.text = Guy.Man ? "Dad" : "Mom";
            for (int i = 0; i < 8; i++)
            {
                GameEngine.Charateristic parent = Guy.Man?house.Dad.Charateristics[i] : house.Mom.Charateristics[i];
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
}
