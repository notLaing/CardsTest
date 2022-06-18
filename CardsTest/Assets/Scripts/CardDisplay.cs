using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    //public CardScriptableObj cardScriptObj;
    Vector3 dragOffset;
    Camera cam;

    public TextMesh textName;
    public TextMesh textCost;
    public TextMesh textType;
    public TextMesh textDescription;
    public SpriteRenderer icon;

    public void CreateCardDisplay(CardScriptableObj cardScriptObj)
    {
        textName.text = cardScriptObj.name;
        textCost.text = cardScriptObj.GetCardCost().ToString();
        textType.text = cardScriptObj.type;
        textDescription.text = cardScriptObj.GetDescription();
        //icon
    }

    void Awake()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        dragOffset = transform.position - GetMousePos();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePos() + dragOffset;
    }

    private void OnMouseUp()
    {
        if(transform.position.y > -1)
        {
            //play card
        }
    }

    Vector3 GetMousePos()
    {
        //var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return cam.ScreenToWorldPoint(Input.mousePosition);// mousePos;
    }
}
