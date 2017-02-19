using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChararcterGui : MonoBehaviour
{
    public MainGui Gui;
    public GameEngine.Guy Guy;
    public Button Button;
    public Text Name;
    public int Id;
    public Text Average;

    public void Refresh(MainGui gui, GameEngine.Guy guy, GameEngine.Guy current)
    {
        Gui = gui;
        Guy = guy;

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

    public void SelectGuy()
    {
        Gui.Show(Guy);
    }
}
