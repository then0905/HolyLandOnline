using UnityEditor;
using UnityEngine;


//==========================================
//  創建者:家豪
//  創建日期:2024/06/23
//  創建用途: 設定編輯器快捷鍵
//==========================================
public class ToggleActiveShortcut
{
    /// <summary>
    /// Alt+F 快速開關面板指定物件
    /// </summary>
    [MenuItem("Tools/Toggle Active State _&F")]
    private static void SetObjActive()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj, "Toggle Active State");
            obj.SetActive(!obj.activeSelf);
        }
    }
}

