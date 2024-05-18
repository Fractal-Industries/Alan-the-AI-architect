using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomBuilder : MonoBehaviour
{
    public GameObject CarPlayer;

    public GameObject Spawn;
    public InputField RotInput;

    int BuildIndex;
    bool Playing;
    public List<GameObject> BuildingBlocks;

    readonly List<BearingConn> AllBearings = new();
    readonly List<ButtonConn> AllButtons = new();

    readonly int[] Primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41 };

    CarManager Starter;
    ExitDoor Exit;

    [SerializeField] CamControl CameraMovment;

    struct HistoryPiece
    {
        public bool onGround;
        public int type;
        // 0 = Piece
        // 1 = Bearing
        // 2 = Button
        // 3 = Car spawner
        // 4 = Exit
        public GameObject parent;
        public int indx;
    }

    int Lenght;
    readonly HistoryPiece[] Timeline = new HistoryPiece[500];

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (!Input.GetMouseButton(1))
        {
            if (!Playing)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit point))
                {
                    Transform Pointer;
                    if (point.transform.name == "Connector")
                        Pointer = point.transform.GetChild(0);
                    else
                        Pointer = point.transform.parent;
                    Spawn.SetActive(true);
                    if (point.point.y <= 40)
                    {
                        Vector3 Temp = point.point / 10;
                        Temp *= 4f;
                        Vector3 CursorPointer = new Vector3(Mathf.Round(Temp.x) / 4f, Mathf.Round(Temp.y) / 4f, Mathf.Round(Temp.z) / 4f) * 10f;
                        Spawn.transform.position = CursorPointer;
                    }
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        if (Lenght == 500)
                        {
                            Debug.Log("Only 500 pices allowed in a level");
                            return;
                        }
                            
                        if (!point.transform.CompareTag("Room") && BuildIndex < 4)
                        {
                            bool BearingInBearing = false;
                            if (BuildIndex == 3)
                            {
                                Transform Check = Pointer;
                                while (Check.name != "Room")
                                {
                                    Check = Check.parent;
                                    if (Check.name == "Bearing")
                                        BearingInBearing = true;
                                }
                            }

                            if (!BearingInBearing)
                            {
                                if (Pointer.CompareTag("Bearing"))
                                {
                                    BearingConn Conn = Pointer.GetComponent<BearingConn>();
                                    if (Conn.Connectable(Spawn.transform.position))
                                    {
                                        AllBearings.Add(Conn.Connect(BuildingBlocks[BuildIndex]));
                                        float.TryParse(RotInput.text, out float Val);
                                        Conn.Speed = Val;
                                        AddToHistory(1, AllBearings.Count - 1, false, Pointer.gameObject);
                                    }
                                    else
                                        Debug.Log("Cannot place a bearing there");
                                }
                                else
                                {
                                    PieceConn Conn = Pointer.GetComponent<PieceConn>();
                                    if (Conn.Connectable(Spawn.transform.position))
                                    {
                                        AddToHistory(0, Conn.Connect(BuildingBlocks[BuildIndex]), false, Pointer.gameObject);
                                    }
                                    else
                                        Debug.Log("Cannot place that piece there");
                                }
                            }
                            else
                                Debug.Log("Cannot place a bearing on a piece that is attached to another bearing");
                        }
                        else if (BuildIndex != 3)
                        {
                            if (!point.transform.CompareTag("Room"))
                            {
                                Debug.Log((BuildIndex==4?"Buttons":BuildIndex==5?"Exits":"Car spawners") + " can only be placed on the floor");
                            }
                            else
                            {
                                if ((BuildIndex != 5 || Starter == null) && (BuildIndex != 6 || Exit == null))
                                {
                                    GameObject Temp = Instantiate(BuildingBlocks[BuildIndex]);
                                    Temp.name = BuildingBlocks[BuildIndex].name;
                                    Temp.transform.SetPositionAndRotation(Spawn.transform.position, point.transform.rotation);
                                    Temp.transform.SetParent(point.transform);
                                    if (BuildIndex == 4)
                                        AllButtons.Add(Temp.GetComponent<ButtonConn>());
                                    else if (BuildIndex == 5)
                                        Starter = Temp.GetComponent<CarManager>();
                                    else if (BuildIndex == 6)
                                    {
                                        Exit = Temp.GetComponent<ExitDoor>();
                                        for (int i = 0; i < AllButtons.Count; i++)
                                            AllButtons[i].Door = Exit;
                                    }
                                    AddToHistory(BuildIndex - 2, 0, true, Temp);
                                }
                                else
                                    Debug.Log("Can only place one " + (BuildIndex == 5 ? "spawner" : "exit") + "in a level");
                            }
                        }
                        else
                            Debug.Log("Bearings can only be placed on parts");
                    }
                }
                else
                    Spawn.SetActive(false);
            }
        }
        else
            Spawn.SetActive(false);
    }

    public void RunAI()
    {
        if (Exit == null)
        {
            Debug.Log("Level must have an exit");
            return;
        }
        if (Starter == null)
        {
            Debug.Log("Level must have a car spawner");
            return;
        }
        for (int i = 0; i < AllButtons.Count; i++)
        {
            AllButtons[i].Door = Exit;
            AllButtons[i].PrimeIndx = Primes[i];
        }
        Exit.ButtonsCount = AllButtons.Count;
        for (int i = 0; i < AllBearings.Count; i++)
            AllBearings[i].Spin = true;
        Playing = true;
        Starter.Begin(Exit.transform);
    }

    public void StopMoving()
    {
        for(int i = 0; i < AllBearings.Count; i++)
            AllBearings[i].ResetRot();
        Playing = false;
        CarPlayer.SetActive(false);
        Starter.ResetWorld();
        CameraMovment.CurrentCam = 0;
    }

    public void PlayMoving()
    {
        if (Exit == null)
        {
            Debug.Log("Level must have an exit");
            return;
        }
        if (Starter == null)
        {
            Debug.Log("Level must have a car spawner");
            return;
        }
        for (int i = 0; i < AllButtons.Count; i++)
            AllButtons[i].Door = Exit;
        Exit.ButtonsCount = AllButtons.Count;
        for (int i = 0; i < AllBearings.Count; i++)
            AllBearings[i].Spin = true;
        Playing = true;
        CarPlayer.transform.SetPositionAndRotation(Starter.transform.position + new Vector3(0, 3, -1), Starter.transform.rotation);
        CarPlayer.SetActive(true);
        CameraMovment.CurrentCam = 1;
    }

    public void SelectBlock(int Indx)
    {
        BuildIndex = Indx;
    }

    void AddToHistory(int Type, int Indx, bool OnGround, GameObject Parent)
    {
        HistoryPiece HistoryTemp = new HistoryPiece
        {
            onGround = OnGround,
            parent = Parent,
            indx = Indx,
            type = Type
        };
        Timeline[Lenght] = HistoryTemp;
        Lenght++;
    }

    public void Undo()
    {
        if (Playing)
            Debug.Log("Cannot undo during play");
        else if (Lenght > 0)
        {
            HistoryPiece Temp = Timeline[Lenght-1];
            if (Temp.onGround)
                Destroy(Temp.parent);
            else if (Temp.type == 0)
                Temp.parent.GetComponent<PieceConn>().Backtrack(Temp.indx);
            else if (Temp.type == 1)
            {
                Temp.parent.GetComponent<BearingConn>().Backtrack();
                AllBearings.RemoveAt(Temp.indx);
            }
            Lenght--;
        }
        else
            Debug.Log("No more changes in memory");
    }
}