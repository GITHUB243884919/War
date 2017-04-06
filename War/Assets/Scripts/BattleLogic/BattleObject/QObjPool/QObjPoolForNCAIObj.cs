/// <summary>
/// QObjPool的QObjPoolForNCAIObj版本实现
/// author : fanzhengyong
/// date  : 2017-04-06
/// 
/// NCAIObj 不是角色但带AI的对象，比如用于轰炸的炸弹之类
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class QObjPoolForNCAIObj : QObjCreatorForGameObject
{
    public QObjPoolForNCAIObj(string path, int count)
        :base(path, count){}

    public override void Realse()
    {
        m_seed = null;
    }

    public override void HideObject(GameObject obj)
    {
        ParticleSystem [] particleSystems = obj.GetComponentsInChildren<ParticleSystem>();
        int length = particleSystems.Length;
        for (int i = 0; i < length; i++)
        {
            particleSystems[i].Stop();
        }

        obj.transform.position = INIT_POS;
    }

    public override void RealseObject(GameObject obj)
    {

    }

}


