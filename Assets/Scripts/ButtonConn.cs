using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonConn : MonoBehaviour
{
    [SerializeField] Material Pressed;
    [SerializeField] Material Normal;
    public ExitDoor Door;

    public int PrimeIndx;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "AIcar")
        {
            other.GetComponent<AI>().PressButton(PrimeIndx);
        }
        else
        {
            transform.GetChild(0).GetComponent<Renderer>().material = Pressed;
            Door.PressButton();
        }
    }

    public void ResetWorld()
    {
        transform.GetChild(0).GetComponent<Renderer>().material = Normal;
    }
}
