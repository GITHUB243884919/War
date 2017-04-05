/// <summary>
/// MoveSteer工厂函数
/// author: fanzhengyong
/// date: 2017-04-05 
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public class MoveSteerFactory
{
    public static MoveSteer Create(MoveSteers steers, MoveSteers.E_STEER_TYPE type)
    {
        MoveSteer steer = null;
        switch (type)
        {
            case MoveSteers.E_STEER_TYPE.ARRIVE:
                steer = new MoveSteerForArrive(steers);
                break;
            default:
                break;
        }

        return steer;
    }
}
