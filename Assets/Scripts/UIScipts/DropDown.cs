using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDown : MonoBehaviour
{
    //public GameObject Gmenue;
    public GameObject configButton;//显示菜单按钮
    public GameObject menu;//菜单
    Button button;
    bool showMenu = false;

    void Start()
    {
        menu.SetActive(showMenu);
        button = configButton.GetComponent<Button>();
        button.onClick.AddListener(delegate ()
        {
            showMenu = !showMenu;
            menu.SetActive(showMenu);
        });
    }
}