using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

//==========================================
//  創建者:家豪
//  創建日期:2024/11/04
//  創建用途:角色狀態效果提示物件_負面狀態
//==========================================
public class CharacterStatusHint_DeBuff : CharacterStatusHint_Base
{
    protected DebuffEffectBase debuffEffect;

    /// <summary>
    /// 初始化狀態提示資料
    /// </summary>
    public void BuffHintInit(DebuffEffectBase debuffEffectBaseData)
    {
        //狀態提示基本資料設定
        debuffEffect = debuffEffectBaseData;
        CharacterStatusName = debuffEffectBaseData.EffectName;
        CharacterStatusID = debuffEffectBaseData.InfluenceStatus;
        CharacterStatusIntro = debuffEffectBaseData.EffectIntro;
        CharacterStatusDuration = debuffEffectBaseData.Duration;
        CharacterStatusType = CharacterStatusManager.Instance.ReturnCharacterTypeStr(debuffEffectBaseData.EffectType);
        buffIcon.sprite = CommonFunction.LoadObject<Sprite>(GameConfig.EffectIcon, CharacterStatusID);

        //訂閱移除事件
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent += RemoveCharacterStatusHint;

        //設定狀態提示物件 位置 大小 與 旋轉方向
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;

        //暫存
        CharacterStatusManager.Instance.CharacterStatusHintDic.Add(this);

        if (CharacterStatusManager.Instance.ReturnTimerCheck(debuffEffectBaseData.EffectType))
            //開始運行計時
           UpdateTimer(CharacterStatusDuration);
    }

    public override async void UpdateTimer(float timer)
    {
        TempCoolDownTime = timer;
        while (TempCoolDownTime > 0)
        {
            await Task.Delay(100); // 100ms = 0.1秒
            TempCoolDownTime -= 0.1f;
            CharacterHintTimeEvent?.Invoke(this, TempCoolDownTime);
            if (TempCoolDownTime <= 10)
                CanvasGroupAnim();
        }
        RemoveCharacterStatusHint();
    }

    public void RemoveCharacterStatusHint()
    {
        CharacterStatusManager.Instance.CharacterSatusRemoveEvent -= RemoveCharacterStatusHint;
        debuffEffect.EffectEnd();
        CharacterStatusManager.Instance.CharacterStatusHintDic.Remove(this);
        Destroy(this.gameObject);
    }
}
