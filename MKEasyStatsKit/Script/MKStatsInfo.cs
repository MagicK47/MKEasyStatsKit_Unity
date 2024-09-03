using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewStatInfo", menuName = "MKGame/MKStatsKit/StatInfo")]
public class MKStatsInfo : ScriptableObject
{
    public string StatsID;
    public string StatsDfName;
    public string StatsDfDescription;
    public Sprite StatsDfIcon;

    private void OnEnable()
    {

        if (string.IsNullOrEmpty(StatsID))
        {
            StatsID = System.Guid.NewGuid().ToString();
        }
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(StatsID)) {
            StatsID = System.Guid.NewGuid().ToString();
        }
    }

    public string GetShowName()
    {
        return string.IsNullOrEmpty(StatsDfName) ? name: StatsDfName;
    }
    public string GetShowDescription()
    {
        return StatsDfDescription;
    }
    public Sprite GetShowIcon()
    {
        return StatsDfIcon;
    }
}
