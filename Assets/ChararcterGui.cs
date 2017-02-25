using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChararcterGui : MonoBehaviour
{
    public enum Action
    {
        ShowDetail,
        UpgradeNurcery,
        UpgradeHouse,
    }

    public Action CurrentAction;
    public MainGui Gui;
    public GameEngine.Guy Guy;
    public Button Button;
    public Text Name;
    public int Id;
    public Text Average;
    public Image Bar;

    public void RefreshGuy(MainGui gui, GameEngine.Guy guy, GameEngine.Guy current)
    {
        Bar.gameObject.SetActive(false);
        Gui = gui;
        Guy = guy;
        CurrentAction = Action.ShowDetail;
        if (guy != null && guy.Id != 0)
        {
            Id = guy.Id;
            Name.text = guy.FamillyName + ":" + Id;
            if (guy != current)
            {
                this.GetComponent<Image>().color = guy.Man ? gui.GameColors.Man : gui.GameColors.Woman;
            }
            else
            {
                this.GetComponent<Image>().color = guy.Man ? gui.GameColors.SelectedMan : gui.GameColors.SelectedWoman;
            }
            
            Average.text = guy.Average.ToString("F1");
        }
        else
        {
            Name.text = "Empty";
            Average.text = string.Empty;
            this.GetComponent<Image>().color = Color.white;
        }

        Button.interactable = guy != null && guy.Id != 0;
    }

    public void RefreshAction(MainGui mainGui, string text, Action action, float completion)
    {
        Bar.gameObject.SetActive(completion > 0);
        Bar.fillAmount = 1.0f - completion;
        Gui = mainGui;
        CurrentAction = action;
        Name.text = text;
        Average.text = string.Empty;
        this.GetComponent<Image>().color = Color.white;
        Button.interactable = true;
    }

    public void SelectGuy()
    {
        if (CurrentAction == Action.ShowDetail)
        {
            Gui.Show(Guy);
        }
        else if (CurrentAction == Action.UpgradeHouse)
        {
            Gui.Engine.Houses[Gui.HouseIndex].BuildHouseCounter += Gui.Engine.Houses[Gui.HouseIndex].NextHousePrice;
        }
        else if (CurrentAction == Action.UpgradeNurcery)
        {
            Gui.Engine.Houses[Gui.HouseIndex].BuildNurceryCounter += Gui.Engine.Houses[Gui.HouseIndex].NextNurceryPrice;
        }
    }


}
