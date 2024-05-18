using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RunInEditor : MonoBehaviour
{
    [SerializeField] Transform Cam;
    [SerializeField] Material FloorTile;
    [SerializeField] Material FloorTileNS;
    [SerializeField] Material FloorTileEW;

    Transform Floor;
    Transform WallN;
    Transform WallS;
    Transform WallE;
    Transform WallW;

    [SerializeField] bool LimitedSize;
    [SerializeField] Vector2Int Size;

    void Start()
    {
        Floor = transform.GetChild(0);
        WallN = transform.GetChild(1);
        WallS = transform.GetChild(2);
        WallE = transform.GetChild(3);
        WallW = transform.GetChild(4);
    }

    void Update()
    {
        Size.x = Mathf.Clamp(Size.x, 5, LimitedSize ? 1000 : 15);
        Size.y = Mathf.Clamp(Size.y, 20, LimitedSize ? 1000 : 100);
        if (!Application.isPlaying)
            Cam.position = new Vector3(-50, Cam.position.y, -100 - 5 * Size.y);
        FloorTile.mainTextureScale = Size;
        WallN.localScale = new Vector3(Size.x, 1, 1);
        WallS.localScale = new Vector3(Size.x, 1, 1);
        WallE.localScale = new Vector3(Size.y, 1, 1);
        WallW.localScale = new Vector3(Size.y, 1, 1);

        WallN.localPosition = new Vector3(0, 5, Size.y * -5);
        WallS.localPosition = new Vector3(0, 5, Size.y * 5);
        WallE.localPosition = new Vector3(Size.x * -5, 5, 0);
        WallW.localPosition = new Vector3(Size.x * 5, 5, 0);

        FloorTile.mainTextureScale = Size;
        FloorTileNS.mainTextureScale = new Vector2(Size.x, 1);
        FloorTileEW.mainTextureScale = new Vector2(Size.y, 1);
        Vector3 XZSize = new Vector3(Size.x, 1, Size.y);
        Floor.localScale = XZSize;
    }
}