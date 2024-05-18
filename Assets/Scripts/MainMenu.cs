using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] float[] YSelector = new float[3];
    [SerializeField] float[] YMenu = new float[3];

    [SerializeField] RectTransform SelectorTrans;
    [SerializeField] RectTransform MenuTrans;

    Animator MenuAnim;

    int CurrentMenu = -1;

    float StartSelector, TargetSelector;
    float StartMenu, TargetMenu;

    float Elapsed;

    void Start()
    {
        MenuAnim = GetComponent<Animator>();
    }

    void Update()
    {
        Elapsed += Time.deltaTime;
        MenuTrans.anchoredPosition = new Vector2(772.4537f, Mathf.Lerp(StartMenu, TargetMenu, Elapsed * 5));
        SelectorTrans.anchoredPosition = new Vector2(0, Mathf.Lerp(StartSelector, TargetSelector, Elapsed * 5));
    }

    public void ChangeMenu(int newMenu)
    {
        if (CurrentMenu == -1)
        {
            MenuTrans.anchoredPosition = new Vector2(772.4537f, YMenu[newMenu]);
            SelectorTrans.anchoredPosition = new Vector2(0, YSelector[newMenu]);
            TargetMenu = YMenu[newMenu];
            TargetSelector = YSelector[newMenu];
            MenuAnim.SetTrigger("Open");
        }
        else if (newMenu == -1)
            MenuAnim.SetTrigger("Close");
        else
        {
            StartSelector = SelectorTrans.anchoredPosition.y;
            TargetSelector = YSelector[newMenu];
            StartMenu = MenuTrans.anchoredPosition.y;
            TargetMenu = YMenu[newMenu];
            Elapsed = 0;
        }
        CurrentMenu = newMenu;
    }

    public void ShowConstructor()
    {
        SceneManager.LoadScene("RoomCreator");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
