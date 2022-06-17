using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardScriptableObj : ScriptableObject
{
    public class Effect
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
    public Effect[] effects;

    public int GetCardCost()
    {
        return cost;
    }

    public int GetCardEffectValue()
    {
        return effects[0].value;
    }
}
