using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2023/11/19
//  創建用途:遊戲資料路徑配置
//==========================================
public static class GameConfig
{
    #region 圖示類型
    public const string SkillIcon = "Skill/Icon";       //技能圖示
    public const string WeaponIcon = "UI/Weapon";       //武器圖示
    public const string ArmorIcon = "UI/Armor";      //防具圖示
    public const string ItemIcon = "UI/Item";       //道具圖示
    #endregion

    #region 效果預製物
    public const string SkillPrefab = "SkillPrefab";
    public const string ItemEffectPrefab = "ItemEffectPrefab";
    #endregion

    public const string MonsterPrefab = "Monster/Prefab";

    public const string NpcAvatar = "Npc/Avatar";
    public const string NpcPrefab = "Npc/Prefab";

    public const string SpriteItem = "CommonSprite/Item";
    public const string SpriteWeapon = "CommonSprite/Weapon";
    public const string SpriteArmor = "CommonSprite/Armor";

    public const string PlayerPrefab = "Player/Prefab";

    public const string OtherPlayerPrefab = "OtherPlayer/Prefab";

    public const string EffectIcon = "Effect/Sprite";
    public const string EffectPrefab = "Effect/Prefab";

    public const string CharacterStatusHintPrefab = "CharacterStatusHint/Prefab";

    public const string AnimatorJob = "Animator/Job";

    public const string CommonWindow = "CommonWindow";

    public const string SystemHint = "SystemHint";

    #region 創角選角相關物件路徑

    public const string ChooseSceneCharacterPrefab = "ChooseCharacterScene/Prefab";

    #endregion

    #region 教學系統

    public const string TutorialObjPrefab = "Tutorial/Prefab";

    #endregion
}
