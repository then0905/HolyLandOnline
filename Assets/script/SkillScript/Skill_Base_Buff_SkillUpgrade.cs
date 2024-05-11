using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//==========================================
//  創建者:家豪
//  創建日期:2023/12/27
//  創建用途:被動型buff技能_強化指定技能
//==========================================
public class Skill_Base_Buff_SkillUpgrade : Skill_Base_Buff_Passive
{
    [Header("升級的指定技能"), SerializeField] protected string upgradeSkillID;
    protected override void SkillEffectStart(ICombatant attacker = null, ICombatant defenfer = null)
    {
        //尋找場景上所有SkillUI
        var findSkillUIResult = PassiveSkillManager.Instance.SkillUIList;
        if (findSkillUIResult.Count > 0 && findSkillUIResult != null)
        {
            string upgragdeSkillName = GameData.SkillsDataDic[upgradeSkillID].Name;
            var getSkillUIListOnScene = findSkillUIResult.Where(x => x.SkillName.text.Contains(upgragdeSkillName)).ToList();
            foreach (var skillUIData in getSkillUIListOnScene)
            {
                skillUIData.SkillBeUpgrade = true;
                skillUIData.SkillUpgradeID = skillName;
                skillUIData.SkillUpgradeIcon = CommonFunction.LoadObject<Sprite>(GameConfig.SkillIcon + "/" + PlayerDataOverView.Instance.PlayerData_.Job, skillName);

            }
        }

        //更新快捷鍵上的技能圖片

        //獲取快捷鍵資料
        var hotKeyDataList = SkillDisplayAction.Instance.SkillHotKey.ToList();
        //收尋快捷鍵上資料ID 找尋升級的指定技能ID
        var item = hotKeyDataList.Where(x => x.HotKeyDataID.Contains(upgradeSkillID)).FirstOrDefault();
        if (item != null)
        {
            //更換圖片與更新升級技能ID資訊
            item.Background.sprite = CommonFunction.LoadObject<Sprite>(GameConfig.SkillIcon + "/" + PlayerDataOverView.Instance.PlayerData_.Job, skillName);
            item.UpgradeSkillID = skillName;
        }
        if (gameObject != null)
            gameObject.transform.parent = PassiveSkillManager.Instance.transform;
    }

    protected override void SkillEffectEnd()
    {
        //尋找場景上所有SkillUI
        var findSkillUIResult = PassiveSkillManager.Instance.SkillUIList;
        if (findSkillUIResult.Count > 0 && findSkillUIResult != null)
        {
            string upgragdeSkillName = GameData.SkillsDataDic[upgradeSkillID].Name;
            var getSkillUIListOnScene = findSkillUIResult.Where(x => x.SkillName.text.Contains(upgragdeSkillName)).ToList();
            foreach (var skillUIData in getSkillUIListOnScene)
            {
                skillUIData.SkillBeUpgrade = false;
                skillUIData.SkillUpgradeID = "";
                skillUIData.SkillUpgradeIcon = CommonFunction.LoadObject<Sprite>(GameConfig.SkillIcon + "/" + PlayerDataOverView.Instance.PlayerData_.Job, upgradeSkillID);

            }
        }

        //更新快捷鍵上的技能圖片

        //獲取快捷鍵資料
        var hotKeyDataList = SkillDisplayAction.Instance.SkillHotKey.ToList();
        //收尋快捷鍵上資料ID 將升級的技能還原
        var item = hotKeyDataList.Where(x => x.HotKeyDataID.Contains(upgradeSkillID)).FirstOrDefault();
        if (item != null)
        {
            //更換圖片與更新升級技能ID資訊
            item.Background.sprite = CommonFunction.LoadObject<Sprite>(GameConfig.SkillIcon + "/" + PlayerDataOverView.Instance.PlayerData_.Job, upgradeSkillID);
            item.UpgradeSkillID = "";
        }
    }

    /// <summary>
    /// 重新啟動技能效果 用來穿脫裝時 重新計算數值
    /// </summary>
    public override void RestartSkillEffect()
    {
        SkillEffectEnd();
        SkillEffectStart();
    }

    private void OnDestroy()
    {
        SkillEffectEnd();
    }
}
