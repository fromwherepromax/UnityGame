using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationHistoryTracker : MonoBehaviour
{   
    private HashSet<LocationSo> spokenLocations = new HashSet<LocationSo>(); //已访问的位置列表

    public void RecordLocation(LocationSo location) //记录访问的位置
    {
        if (!spokenLocations.Contains(location)) //如果位置不在列表中
        {
            spokenLocations.Add(location); //添加位置到列表
            Debug.Log("Recorded visit to " + location.displayName);
        }
    }
    public bool HasVisited(LocationSo location) //检查是否已经访问过位置
    {
        return spokenLocations.Contains(location); //返回位置是否在列表中
    }
}
