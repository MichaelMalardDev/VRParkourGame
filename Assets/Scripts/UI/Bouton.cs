using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Bouton : MonoBehaviour
{
    public TypeBouton typeBouton;
    public UnityEvent onClickEvent;

    private Color baseColor;
    private int _clickCount = 0;

    void Start()
    {
        baseColor = GetComponent<MeshRenderer>().material.color;
    }

    [ContextMenu("OnClick")]
    public void OnClick()
    {
        Debug.Log("ClickButton");
        onClickEvent.Invoke();

        // if (typeBouton == TypeBouton.CheckBox)
        // {
        //     _clickCount++;
        //     if (_clickCount % 2 == 0)
        //     {
        //         GetComponent<MeshRenderer>().material.color = baseColor;
        //     }
        //     else
        //     {
        //         GetComponent<MeshRenderer>().material.color = Color.red;
        //     }
        // }
    }
}

public enum TypeBouton
{
    Normal,
    CheckBox,
}
