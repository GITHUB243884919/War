using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamCommondFormatiomParam
{
    public int ParamID { get; set; }

    public enum E_Team_COMMOND
    {
        IDLE,
        ARRIVE,
        ATTACK,
        SKILL
    }

    //包含的group
    List<int> m_groups = new List<int>();
}
