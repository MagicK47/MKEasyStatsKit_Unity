using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewStatsGroup", menuName = "MKGame/StatsGroup")]
public class MKStatsGroup : ScriptableObject
{
    public string GroupName;
    [TextArea]
    public string GroupDesDescription;
    [HideInInspector]
    public List<MKStats> AllStats = new List<MKStats>();

    public MKStatsGroup ParentGroup;

    public MKStats AddOneNewStat(MKStatsInfo mKStatsInfo)
    {
        if (AllStats == null) { AllStats = new List<MKStats>(); }
        MKStats mKStats = new MKStats(10, 0, 10, mKStatsInfo);
        AllStats.Add(mKStats);
        return mKStats;
    }

    public List<MKStats> GetAllParentGroupStats()
    {
        List<MKStats> mKStats = new List<MKStats>();
        if (ParentGroup) {
   
            mKStats.AddRange(ParentGroup.GetAllGroupStats(true));
        }
        return mKStats;
    }
    public List<MKStats> GetAllGroupStats(bool HaveParentGroup)
    {
        List<MKStats> mKStats = new List<MKStats>(AllStats);
        if (HaveParentGroup && ParentGroup)
        {
            mKStats.AddRange(GetAllParentGroupStats());
        }
        return mKStats;
    }
    public void RemoveOneStat(int id)
    {
        AllStats.RemoveAt(id);
    }
    public void RemoveOneStat(MKStats stats)
    {
        AllStats.Remove(stats);
    }
}
