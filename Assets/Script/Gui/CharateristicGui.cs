using Assets.Script.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharateristicGui : MonoBehaviour
{
    public Text Name;
    public Text Value;
    public Text ParentValue;
    public Text ChampionValue;


    public void SetValue(MainGui gui, Charateristic value, Charateristic parent, Charateristic champion)
    {
        Name.text = value.CharateristicType.ToString();
        Value.text = value.Value.ToString();
        ParentValue.text = parent.Value.ToString();
        int championValue = champion == null ? 0 : champion.Value;
        ChampionValue.text = champion == null ? string.Empty : championValue.ToString();
        if (value.Value > parent.Value || (value.Value > championValue && championValue != 0))
        {
            Value.color = gui.GameColors.Best;
            ParentValue.color = value.Value > parent.Value ? gui.GameColors.Worst : Color.black;
            ChampionValue.color = value.Value > championValue ? gui.GameColors.Worst : Color.black;
        }
        else if (value.Value < parent.Value || (value.Value < championValue && championValue != 0))
        {
            Value.color = gui.GameColors.Worst;
            ParentValue.color = value.Value < parent.Value ? gui.GameColors.Best : Color.black;
            ChampionValue.color = value.Value < championValue ? gui.GameColors.Best : Color.black;
        }
        else
        {
            Value.color = Color.black;
            ParentValue.color = Color.black;
            ChampionValue.color = Color.black;
        }
    }
}