using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is not placed on anything
/// ScriptableObject of cards
/// </summary>

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardScriptableObj : ScriptableObject
{
    [System.Serializable]
    public struct Effect
    {
        public string type;
        public int value;
        public string target;
    }

    public int id;
    public new string name;
    public int cost;
    public string type;
    public int image_id;
    public Effect effects;

    public int GetCardCost()
    {
        return cost;
    }

    public int GetCardEffectValue()
    {
        return effects.value;
    }

    public string GetDescription()
    {
        return effects.type + " " + effects.target + "\nfor " + effects.value + " points";
    }

    public void FillWithData(GameManager.Card c)
    {
        id = c.id;
        name = c.name;
        cost = c.cost;
        type = c.type;
        image_id = c.image_id;
        effects.type = c.effects[0].type;
        effects.value = c.effects[0].value;
        effects.target = c.effects[0].target;
    }

    public void DebugText()
    {
        Debug.Log("id: " + id + "\nname: " + name + "\ncost: " + cost + "\ntype: " + type + "\nimage_id: " + image_id + "\neffect type: " + effects.type + "\neffect value: " + effects.value + "\neffect target: " + effects.target);
    }
}
