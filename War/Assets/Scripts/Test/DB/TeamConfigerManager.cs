using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamConfigerData
{
    public int ID;
    public string GroupIDs;
    public int Idle;
    public int Arrive;
    public int Attack;
    public int Skill;
}

public class TeamConfigerManager : SimpleDBManager<int, TeamConfigerData, TeamConfigerManager>
{
    //public List<TeamConfigerData> GetAllData()
    //{
    //    return values;
    //}

    //public int SeekMountainInArea(int id)
    //{
    //    int seekIndex = 0;
    //    try
    //    {
    //        while (true)
    //        {
    //            var data = values[seekIndex];
    //            int curId = data.areaId;
    //            if (curId < id)
    //            {
    //                seekIndex = data.nextJump;
    //                if (seekIndex == -1)
    //                {
    //                    return -1;
    //                }
    //            }
    //            else if (curId == id)
    //            {
    //                return seekIndex;
    //            }
    //            else
    //            {
    //                return -1;
    //            }
    //        }
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogException(e);
    //        return -1;
    //    }
    //}

    //public List<TeamConfigerData> GetMountainData(int startIndex)
    //{
    //    var data = values[startIndex];
    //    int endIndex = data.nextJump;
    //    int count = endIndex - startIndex + 1;
    //    return values.GetRange(startIndex, count);
    //}
}
