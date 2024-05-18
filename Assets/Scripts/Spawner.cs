using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> Holograms;
    public List<Collider> Collisionwith;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (!Collisionwith.Contains(other))
            Collisionwith.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (Collisionwith.Contains(other))
            Collisionwith.Remove(other);
    }

    public void ChangeToPlace(int indx)
    {
        Destroy(transform.GetChild(0));
        GameObject Temp = Instantiate(Holograms[indx]);
        Temp.transform.SetParent(transform);
        Temp.transform.localPosition = Vector3.zero;
        Temp.transform.localEulerAngles = Vector3.zero;
    }
}