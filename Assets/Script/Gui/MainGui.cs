using Assets.Script.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
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

        public Color[] HouseColor;


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

    public bool Restart;
    public Colors GameColors;
    public int HouseIndex;
    public GameEngine Engine;
    public Guy Guy;
    public HouseTabGui[] Tabs;
    public GameObject Parent;
    public Image ParentBar;
    
    public GameObject Champion;
    public Image ChampionBar;

    public GameObject GOHouse;
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
    public Button[] TransfertButtons;

    public ResourceGui[] Resources;

    public void Init(GameEngine engine)
    {
        Engine = engine;
    }

    public void Start()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            XmlSerializer ser = new XmlSerializer(typeof(GameEngine));
            TextReader reader = new StringReader(PlayerPrefs.GetString("save"));
            Engine = (GameEngine) ser.Deserialize(reader);
            return;
        }

        Engine = new GameEngine();
        Engine.Init();

    }

    public void Update()
    {
        if (Restart)
        {
            Restart = false;
            PlayerPrefs.DeleteKey("save");
            Start();
        }
        Engine.Tick();
        House house = Engine.Houses[HouseIndex];
        ParentBar.fillAmount = (float)house.BirthCounter / (float)house.BirthTime;
        ChampionBar.fillAmount = (float)house.ChampionCounter / (float)house.ChampionActionTime;
        Refresh();
    }

    public void Refresh()
    {
        GetComponent<Image>().color = GameColors.HouseColor[HouseIndex];
        House house = Engine.Houses[HouseIndex];
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
            Resource resource = Engine.Resources[index];
            Resources[index].Name.text = resource.Type.ToString();
            Resources[index].Current.text = resource.Value.ToString();
            Resources[index].Max.text = resource.Max.ToString();
        }
    }

    private void RefreshDetail(House house)
    {
        Detail.SetActive(Guy != null && Guy.Id != 0);
        if (Guy != null && Guy.Id != 0)
        {
            Detail.GetComponent<Image>().color = Guy.Man ? GameColors.Man : GameColors.Woman;
            GuyName.text = Guy.FamillyName + ":" + Guy.Id;
            GuyParent.text = Guy.Man ? "Dad" : "Mom";
            for (int i = 0; i < 8; i++)
            {
                Charateristic parent = Guy.Man ? house.Dad.Charateristics[i] : house.Mom.Charateristics[i];
                Charateristic champion = house.Champion == null || house.Champion.Id == 0 ? null : house.Champion.Charateristics[i];

                Charateristics[i].SetValue(this, Guy.Charateristics[i], parent, champion);
            }

            PromoteParent.interactable = Guy != house.Dad && Guy != house.Mom && Guy != house.Champion;
            PromoteChampion.interactable = Guy != house.Dad && Guy != house.Mom && Guy != house.Champion;
            PromoteParent.GetComponentInChildren<Text>().text = Guy.Man ? "Promote Dad" : "Promote Mom";
            Sacrifice.interactable = Guy != house.Dad && Guy != house.Mom && Guy != house.Champion;
            PromoteHouse.interactable = Guy != house.Dad && Guy != house.Mom && Guy != house.Champion && house.HouseGuys.Count < house.HouseSize && house.Nurcery.Contains(Guy);

            for (int index = 0; index < this.Tabs.Length; index++)
            {
                if (index == HouseIndex)
                {
                    TransfertButtons[index].GetComponent<Button>().interactable = false;
                }
                else
                {
                    TransfertButtons[index].GetComponent<Button>().interactable = Guy != house.Dad && Guy != house.Mom && Guy != house.Champion;
                }

                if (index < Engine.Houses.Count)
                {
                    TransfertButtons[index].GetComponent<Button>().interactable &=
                        Engine.Houses[index].HouseGuys.Count < Engine.Houses[index].HouseSize;
                    TransfertButtons[index].GetComponentInChildren<Text>().text = "Transfert to " +
                                                                              Engine.Houses[index].Type.ToString();

                    TransfertButtons[index].gameObject.SetActive(true);
                }
                else
                {
                    TransfertButtons[index].gameObject.SetActive(false);
                }
            }
        }
    }

    private void RefreshNurcery(House house)
    {
        for (int index = 0; index < NurceryGuys.Length; index++)
        {
            ChararcterGui guy = NurceryGuys[index];
            guy.gameObject.SetActive(index < house.NurcerySize);
            if (index < house.Nurcery.Count)
            {
                guy.RefreshGuy(this, house.Nurcery[index], Guy);
            }
            else if (index == house.NurcerySize && Engine.IsHouseBuild(House.HouseType.Building))
            {
                if (house.BuildNurceryCounter != 0)
                {
                    guy.RefreshAction(this, "Building", ChararcterGui.Action.UpgradeHouse, house.BuildNurceryCounter / (float)house.NextNurceryPrice);
                    guy.Button.interactable = false;
                }
                else
                {
                    guy.RefreshAction(this, "Build " + house.NextNurceryPrice + " BP", ChararcterGui.Action.UpgradeNurcery, 0);
                    if (Engine.Houses.Exists(x => x.IsBuilding()))
                    {
                        guy.Button.interactable = false;
                    }
                }
                guy.gameObject.SetActive(true);
            }
            else
            {
                guy.RefreshGuy(this, null, Guy);
            }
        }
    }

    private void RefreshParent(House house)
    {
        Dad.RefreshGuy(this, house.Dad, Guy);
        Mom.RefreshGuy(this, house.Mom, Guy);

        ChampionGuy.RefreshGuy(this, house.Champion, Guy);
        
    }

    private void RefreshHouses(House house)
    {
        for (int index = 0; index < HouseGuys.Length; index++)
        {
            ChararcterGui guy = HouseGuys[index];
            guy.gameObject.SetActive(index < house.HouseSize);
            if (index < house.HouseGuys.Count)
            {
                guy.RefreshGuy(this, house.HouseGuys[index], Guy);
            }
            else if (index == house.HouseSize && Engine.IsHouseBuild(House.HouseType.Building))
            {
                if (house.BuildHouseCounter != 0)
                {
                    guy.RefreshAction(this, "Building", ChararcterGui.Action.UpgradeHouse, house.BuildHouseCounter / (float)house.NextHousePrice);
                    guy.Button.interactable = false;
                }
                else
                {
                    guy.RefreshAction(this, "Build " + house.NextHousePrice + " BP", ChararcterGui.Action.UpgradeHouse, 0);
                    if (Engine.Houses.Exists(x => x.IsBuilding()))
                    {
                        guy.Button.interactable = false;
                    }
                }
                guy.gameObject.SetActive(true);
            }
            else
            {
                guy.RefreshGuy(this, null, Guy);
            }
        }
    }

    private void RefreshHouses()
    {
        
        for (int index = 0; index < Tabs.Length; index++)
        {
            HouseTabGui tabGui = Tabs[index];
            tabGui.GetComponent<Image>().color = GameColors.HouseColor[index];
            if (index < Engine.Houses.Count)
            {
                House house = Engine.Houses[index];
                tabGui.Title.text = house.Type.ToString();
                tabGui.GetComponent<Button>().interactable = index != HouseIndex;
                tabGui.Power.text = "" + house.Power + " power";
                tabGui.gameObject.SetActive(true);
            }
            else if (index == Engine.Houses.Count)
            {
                tabGui.Title.text = "Buy for "+ Engine.FuturHouses[0].Price + " Gold";
                tabGui.GetComponent<Button>().interactable = Engine.GetResource(Resource.RessourceType.Gold).Value >= Engine.FuturHouses[0].Price; 
                tabGui.gameObject.SetActive(true);
                tabGui.Power.text = String.Empty;
            }
            else
            {
                tabGui.gameObject.SetActive(false);
            }
            
        }
    }


    public void Show(Guy guy)
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

    public void TransfertTo(int house)
    {
        Engine.Houses[HouseIndex].HouseGuys.Remove(Guy);
        Engine.Houses[HouseIndex].Nurcery.Remove(Guy);
        Engine.Houses[house].HouseGuys.Add(Guy);
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

        Guy = null;

        HouseIndex = index;
    }

    public void Cancel()
    {
        foreach (House house in Engine.Houses)
        {
            house.BuildHouseCounter = 0;
            house.BuildNurceryCounter = 0;
        }
    }
}
