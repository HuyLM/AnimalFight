using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CharacterInfoData))]
[CanEditMultipleObjects]
public class CharacterInfoDataEditor : Editor
{
    private CharacterInfoData characterInfoData;
    
    public override void OnInspectorGUI()
    {
        characterInfoData = (CharacterInfoData)target;
        var texture = AssetPreview.GetAssetPreview(characterInfoData.Icon);
        GUILayout.Label(texture);

        base.OnInspectorGUI();

        if(GUILayout.Button("Reset AssetName"))
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(characterInfoData), characterInfoData.id);
        }
    }

}
#endif

[CreateAssetMenu(menuName = "CharacterData/CharacterInfoData", fileName = "CharacterInfoData")]
public class CharacterInfoData : CharacterData
{
    public List<Rank> ranks = new List<Rank>();

    public UpgradeData GetUpgrade(RankType rankType, int level)
    {
        return GetRank(rankType).upgrades[level];
    }

    public Rank GetRank(RankType rankType)
    {
        return ranks.Find(r => r.rankType == rankType);
    }

    public UpgradeData NextUpgrade(RankType rankType, int level)
    {
        Rank currentRank = GetRank(rankType);

        if (!currentRank.MaxLevel(level))
        {
            return GetUpgrade(rankType, level + 1);
        }

        if (currentRank.MaxLevel(level))
        {
            return GetUpgrade(rankType++, 0);
        }

        return null;
    }

    public virtual SecuredDouble HPMax(RankType rankType, int level)
    {
        return ranks.Find(r => r.rankType == rankType).upgrades[level].hpMax;
    }

    public virtual SecuredDouble Damage(RankType rankType, int level)
    {
        return ranks.Find(r => r.rankType == rankType).upgrades[level].damage;
    }

    public virtual SecuredDouble Mana(RankType rankType, int level)
    {
        return ranks.Find(r => r.rankType == rankType).upgrades[level].mana;
    }

    public override SecuredDouble Power()
    {
        return 0;
    }
}
