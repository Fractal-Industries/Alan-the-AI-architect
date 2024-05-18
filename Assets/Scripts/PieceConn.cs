using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceConn : MonoBehaviour
{
    public PieceDef piece;
    GameObject[] ConnectedReference;
    int FreePlaces;
    int ClosestIndx;

    void Awake()
    {
        ConnectedReference = new GameObject[piece.LocalPoint.Length];
        FreePlaces = piece.LocalPoint.Length;
    }

    void Start()
    {

    }

    void Update()
    {
        for (int i = 0; i < piece.LocalPoint.Length; i++)
            Debug.DrawLine(transform.position + transform.rotation * piece.LocalPoint[i], transform.position + transform.rotation * piece.LocalPoint[i] + transform.rotation * piece.Normal[i]);
    }

    public bool Connectable(Vector3 GlobalPos)
    {
        if (FreePlaces > 0)
        {
            float Distance = 0.5f;
            ClosestIndx = 0;
            for (int i = 0; i < piece.LocalPoint.Length; i++)
            {
                if (ConnectedReference[i] == null)
                {
                    if (Vector3.Distance(GlobalPos, transform.position + transform.rotation * piece.LocalPoint[i]) < Distance)
                    {
                        ClosestIndx = i;
                        Distance = Vector3.Distance(GlobalPos, transform.position + transform.rotation * piece.LocalPoint[i]);
                    }
                }
            }
            return Distance < 0.5f;
        }
        return false;
    }

    public int Connect(GameObject ToPlace)
    {
        GameObject Temp = Instantiate(ToPlace);
        Temp.name = ToPlace.name;
        Temp.transform.SetParent(this.transform);
        Temp.transform.position = transform.position + transform.rotation * piece.LocalPoint[ClosestIndx];
        Temp.transform.localRotation = Quaternion.FromToRotation(Temp.transform.up, piece.Normal[ClosestIndx]);
        ConnectedReference[ClosestIndx] = Temp;
        FreePlaces--;
        return ClosestIndx;
    }

    public void Backtrack(int indx)
    {
        Destroy(ConnectedReference[indx]);
        ConnectedReference[indx] = null;
        FreePlaces++;
    }
}
