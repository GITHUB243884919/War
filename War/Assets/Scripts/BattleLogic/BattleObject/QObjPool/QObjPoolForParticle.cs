/// <summary>
/// QObjPool的粒子特效版本
/// author : fanzhengyong
/// date  : 2017-02-27
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;

public class QObjPoolForParticle : QObjCreatorForGameObject
{
    public QObjPoolForParticle(string path, int count)
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
    }

    public override void RealseObject(GameObject obj)
    {

    }

}

