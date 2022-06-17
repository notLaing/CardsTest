using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    Vector3 dragOffset;
    Camera cam;

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

    Vector3 GetMousePos()
    {
        //var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return cam.ScreenToWorldPoint(Input.mousePosition);// mousePos;
    }
}
