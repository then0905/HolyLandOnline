using UnityEngine;

[CreateAssetMenu(menuName = "CreateTargetInformation")]
public class TargetInformation : ScriptableObject
{
    public string TargetName;
    public int TargetLV;
    public int TargetHP;
    public int TargetMP;
    public int TargetEXP;
    public int TargetATK;
    public float TargetATKD;
    public int TargetDenfense;
    public int TartgetCRT;
    public int TargetCRTD;
    public string TargetClass;
}
