using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

//==========================================
//  創建者:    家豪
//  創建日期:  2023/06/15
//  創建用途:  掉落物的腳本
//==========================================
public class BootysPresent : MonoBehaviour
{
    [Header("物品圖片")]
    public Sprite ThisEquipmentImage;
    [Header("物品數量")]
    //紀錄物品數量
    public int Qty;
    [Header("物品資料"), SerializeField]
    public EquipmentData EquipmentDatas;

    [Header("金幣量")]
    public int Coins;

    //戰鬥對象資料
    private List<ICombatant> battleTargetList = new List<ICombatant>();
    //掉落物開放權限設定
    private int bootysLockSetting;
    //紀錄當前執行的協程
    private Coroutine tempCoroutine;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="bootysLockSetting">掉落物權限設定</param>
    /// <param name="battleTargetList">戰鬥對象資料</param>
    public void Init(int bootysLockSetting, List<ICombatant> battleTargetList)
    {
        SetBootysLock(bootysLockSetting);
        this.battleTargetList = battleTargetList;
    }

    /// <summary>
    /// 掉落物的權限開放設定
    /// </summary>
    /// <param name="bootysLock"></param>
    private void SetBootysLock(int bootysLock)
    {
        bootysLockSetting = bootysLock;
        switch (bootysLock)
        {
            case 1:     //掉落物權限鎖定第一階段
                tempCoroutine = StartCoroutine(FirstPhaseBootysLockTime());
                break;
            case 2:  //掉落物權限鎖定第二階段
                tempCoroutine = StartCoroutine(SecondPhaseBootysLockTime());
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 撿起掉落物處理
    /// </summary>
    public void BePickUP(PlayerDataOverView playerDAtaOverView)
    {
        //對撿取者進行權限檢查
        if (!BootysLockCheck(playerDAtaOverView))
        {
            CommonFunction.MessageHint("並沒有對此物品有撿取的權限", HintType.Warning);
            return;
        }

        //根據掉落物的資料判斷該掉落物種類
        if (EquipmentDatas.Weapon != null)
            BagManager.Instance.PickUp(ThisEquipmentImage, EquipmentDatas.Weapon);
        else if (EquipmentDatas.Armor != null)
            BagManager.Instance.PickUp(ThisEquipmentImage, EquipmentDatas.Armor);
        else if (EquipmentDatas.Item != null)
            BagManager.Instance.PickUp(ThisEquipmentImage, EquipmentDatas.Item, Qty);
        else if (!Coins.Equals(0))
            BagManager.Instance.PickUp(Coins);

        //撿起物品 本地紀錄資料刷新點
        LoadPlayerData.SaveUserData();

        //停止並清空協程
        if (tempCoroutine != null)
        {
            StopCoroutine(tempCoroutine);
            tempCoroutine = null;
        }

        //撿起物品後清除
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 對撿取者 進行 權限檢查
    /// </summary>
    /// <param name="playerDAtaOverView"></param>
    /// <returns></returns>
    private bool BootysLockCheck(PlayerDataOverView playerDAtaOverView)
    {
        switch (bootysLockSetting)
        {
            case 1:     //掉落物權限鎖定第一階段
                if (battleTargetList.Count < 1)
                    return false;
                else
                    return battleTargetList.FirstOrDefault().Name == playerDAtaOverView.PlayerData_.PlayerName;

            case 2:  //掉落物權限鎖定第二階段
                List<string> battleTargetNameList = battleTargetList.Select(x => x.Name).ToList();
                return battleTargetNameList.Any(x => x == playerDAtaOverView.PlayerData_.PlayerName);

            default:        //無任何權限設定
                return true;
        }
    }

    /// <summary>
    /// 第一階段權限鎖定
    /// </summary>
    /// <returns></returns>
    private IEnumerator FirstPhaseBootysLockTime()
    {
        //獲取鎖定時間
        float getLockTime = GameData.GameSettingDic["FirstPhaseBootysLockTime"].GameSettingValue;
        //開始計時鎖定時間
        while (getLockTime > 0)
        {
            getLockTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        //停止並清空協程
        if (tempCoroutine != null)
        {
            StopCoroutine(tempCoroutine);
            tempCoroutine = null;
        }

        //設定權限第二階段
        SetBootysLock(2);
    }

    /// <summary>
    /// 第二階段權限鎖定
    /// </summary>
    /// <returns></returns>
    private IEnumerator SecondPhaseBootysLockTime()
    {
        //獲取鎖定時間
        float getLockTime = GameData.GameSettingDic["SecondPhaseBootysLockTime"].GameSettingValue;
        //開始計時鎖定時間
        while (getLockTime > 0)
        {
            getLockTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        //停止並清空協程
        if (tempCoroutine != null)
        {
            StopCoroutine(tempCoroutine);
            tempCoroutine = null;
        }

        //設定權限最後階段
        SetBootysLock(0);
    }
}
