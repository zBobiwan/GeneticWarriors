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
        ChampionValue.text = champion == null ? string.Empty : champion.Value.ToString();
        ParentValue.color = gui.GameColors.IsBetterColor(parent.Value, value.Value, Color.black);
        ChampionValue.color = champion != null ? gui.GameColors.IsBetterColor(champion.Value, value.Value, Color.black) : Color.black;
    }
}