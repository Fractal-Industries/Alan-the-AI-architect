using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearingConn : MonoBehaviour
{
    public Transform Rotor;
    bool CanConnect = true;
    public float Speed;
    public bool Spin;

    void Start()
    {
        
    }

    void Update()
    {
        if (Spin)
        {
            Rotor.Rotate(new Vector3(0, Speed * Time.deltaTime, 0));
        }
    }

    public bool Connectable(Vector3 GlobalPos)
    {
        return Vector3.Distance(GlobalPos, Rotor.position) < 0.5f && CanConnect;
    }

    public void ResetRot()
    {
        Spin = false;
        Rotor.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Rotor.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Rotor.transform.localEulerAngles = Vector3.zero;
        Rotor.transform.localPosition = Vector3.zero;
    }

    public BearingConn Connect(GameObject ToPlace)
    {
        GameObject Temp = Instantiate(ToPlace);
        Temp.name = ToPlace.name;
        Temp.transform.SetParent(Rotor);
        Temp.transform.localPosition = Vector3.zero;
        Temp.transform.localEulerAngles = Vector3.zero;
        CanConnect = false;
        return this;
    }

    public void Backtrack()
    {
        Destroy(Rotor.GetChild(0).gameObject);
        CanConnect = true;
    }
}
