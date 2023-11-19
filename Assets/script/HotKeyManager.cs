using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//==========================================
//  創建者:    家豪
//  翻修日期:  2023/07/05
//  創建用途:  快捷鍵管理器
//==========================================

public class HotKeyManager : MonoBehaviour
{
    //技能視窗
    public GameObject SkillsWindow;
    //背包視窗
    public GameObject BagWindow;
    //技能欄底圖
    public Image[] SkillsHotKeyBackground;
    //技能欄CD計時圖
    public Image[] SkillsHotKeyFill;
    private void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            SkillsWindowPage();
        }
        if (Input.GetKeyDown("b"))
        {
            BagWindowPage();
        }
        for(int i = 0; i < 10; i++)
        {
            SkillsHotKeyFill[i].sprite = SkillsHotKeyBackground[i].sprite;
        }
    }
    /// <summary>
    /// 開關技能視窗
    /// </summary>
    public void SkillsWindowPage()
    {
        if (!SkillsWindow.activeSelf)
        {
            SkillsWindow.SetActive(true);
        }
        else
        {
            SkillsWindow.SetActive(false);
        }
    }
    /// <summary>
    /// 開關背包視窗
    /// </summary>
    public void BagWindowPage()
    {
        if (!BagWindow.activeSelf)
        {
            BagWindow.SetActive(true);
        }
        else
        {
            BagWindow.SetActive(false);
        }
    }
}
