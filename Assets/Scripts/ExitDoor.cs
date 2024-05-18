using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public int ButtonsCount;
    [SerializeField] Material Open;
    Material Def;
    int PressedRN;

    void Start()
    {
        Def = GetComponent<Renderer>().material;
    }

    void Update()
    {
        
    }

    public void PressButton()
    {
        PressedRN++;
        if (PressedRN == ButtonsCount)
            GetComponent<Renderer>().material = Open;
    }

    public void ResetWorld()
    {
        GetComponent<Renderer>().material = Def;
        PressedRN = 0;
    }
}
