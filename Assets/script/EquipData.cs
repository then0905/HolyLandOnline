using JsonDataModel;
using System.Collections.Generic;
using UnityEngine;
//==========================================
//  創建者:    家豪
//  創建日期:  2023/06/08
//  創建用途:  裝備部位資料
//==========================================
public class EquipData :MonoBehaviour
{ 
    //裝備部位ID
    public List<string> PartID = new List<string>();

    //裝備檢測的物件 錯誤部位紅色 正確部位綠色
    public GameObject Hint;
}
