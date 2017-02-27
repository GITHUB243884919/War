/// <summary>
/// 一种对象池实现。因为内部采用Queue存储对象，因此取名QObjPool。
/// author : fanzhengyong
/// date  : 2017-02-22
/// 
/// 需要实现对象生成器和生成器工厂
/// QObjPool有两个重要的接口
/// T BorrowObj()        借对象
/// ReturnObj(T obj)     还对象
/// 之所以叫借和还，意思就是不能拿着T去做T的删除操作！！！
/// 
/// 对象生成器
/// public abstract class QObjCreator<T>
/// 因为对象池不需要知道对象怎么创建出来的，所以需要外部提供。
/// 
/// 对象生成器工厂
/// public abstract class QObjCreatorFactory<T>
/// 这个不是必须的，但有一种情况下需要提供。
/// 就是生成器生成时，有伴随动作，额外操作，比如生成个文件啥的。
/// 这时候如果仅仅持有对象生成器的指针，就会导致每次生成后的都放这个文件中。
/// 在极端情况下，文件就会爆掉。
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 对象生成器
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class QObjCreator<T>
{
    public virtual T [] CreateObjects()
    {
        T [] objs = null;
        return objs;
    }

    /// <summary>
    /// 隐藏对象
    /// </summary>
    /// <param name="obj"></param>
    public virtual void HideObject(T obj)
    {

    }

    /// <summary>
    /// 释放对象
    /// </summary>
    /// <param name="obj"></param>
    public virtual void RealseObject(T obj)
    {

    }

    /// <summary>
    /// 释放自己
    /// </summary>
    public virtual void Realse()
    {

    }
}

/// <summary>
/// 对象生成器工厂类，需要派生类实现
/// </summary>
public abstract class QObjCreatorFactory<T>
{
    public virtual QObjCreator<T> CreatCreator()
    {
        QObjCreator<T> creator = null;
        return creator;
    }

    public virtual void Realse()
    {

    }
}

public class QObjPool<T>
{
    private Queue<T> m_pool = new Queue<T>();
    public int FreeCount { get; private set; }
    public int TotleCount { get; private set; }

    private QObjCreatorFactory<T> m_creatorFactory;
    private QObjCreator<T> m_creator;

    public QObjCreator<T> BAK_CREATOR { get; private set; }
    public bool Init(QObjCreator<T> creator, QObjCreatorFactory<T> creatorFactory = null)
    {
        FreeCount        = 0;
        TotleCount       = 0;
        m_creator        = creator;
        m_creatorFactory = creatorFactory;
        return Resize();
    }

    public T BorrowObj()
    {
        bool retCode = false;
        T obj = default(T);
        //Debug.Log("FreeCount" + FreeCount);
        if (FreeCount > 0)
        {
            obj = m_pool.Dequeue();
            FreeCount--;
            return obj;
        }
        
        //if (obj == null)
        {
            retCode = Resize();
            if (!retCode)
            {
                return default(T);
            }
            obj = m_pool.Dequeue();
        }
        FreeCount--;
        
        if (FreeCount <= 0)
        {
            Debug.LogError("本pool出现严重逻辑问题，请联系作者");
        }

        return obj;
    }

    public void ReturnObj(T obj)
    {
        m_pool.Enqueue(obj);
        FreeCount++;
    }

    //public void Realse()
    //{
    //    if (BAK_CREATOR == null)
    //    {
    //        Debug.LogError("QObjPool 的备份对象生成器为null");
    //        return;
    //    }

    //    for (int i = 0; i < FreeCount; i++)
    //    {
    //        T obj = m_pool.Dequeue();
    //        BAK_CREATOR.RealseObject(obj);
    //    }

    //    FreeCount        = 0;
    //    BAK_CREATOR      = null;
    //    m_creator        = null;
    //    m_creatorFactory = null;
    //}

    /// <summary>
    /// 按指定对象个数释放，当leftCout == 0 时表示整个pool的对象都释放
    /// </summary>
    /// <param name="count"></param>
    /// <param name="leftCout"></param>
    public void Realse(int count, out int leftCout)
    {
        if (BAK_CREATOR == null)
        {
            Debug.LogWarning("QObjPool 的备份对象生成器为null");
            leftCout = FreeCount;
            return;
        }

        int realseCount = Mathf.Min(count, FreeCount);
        for (int i = 0; i < realseCount; i++)
        {
            T obj = m_pool.Dequeue();
            BAK_CREATOR.RealseObject(obj);
            FreeCount--;
        }

        leftCout = FreeCount;

        if (FreeCount <= 0)
        {
            m_pool           = null;

            BAK_CREATOR.Realse();
            BAK_CREATOR      = null;

            if (m_creator != null)
            {
                m_creator.Realse();
                m_creator = null;
            }

            if (m_creatorFactory != null)
            {
                m_creatorFactory.Realse();
                m_creatorFactory = null;
            }
        }
    }

    private bool Resize()
    {
        bool result = false;

        //必须没有空闲的才执行
        if (FreeCount > 0)
        {
            return result;
        }

        T [] objs = null;
        objs = CreatObjects(m_creator, m_creatorFactory);
        if (objs == null)
        {
            return result;
        }

        int countForAdd  = objs.Length;
        if ((int.MaxValue - countForAdd <= TotleCount)
            || (int.MaxValue - countForAdd <= FreeCount)
        )
        {
            Debug.LogError("本pool已碎，无力支撑，请检查上层逻辑是否合理");
            return result;
        }

        FreeCount       += countForAdd;
        TotleCount      += countForAdd;
        for (int i = 0; i < countForAdd; i++)
        {
            m_pool.Enqueue(objs[i]);
        }
        
        result = true;
        return result;
    }

    private T [] CreatObjects(QObjCreator<T> creator, QObjCreatorFactory<T> creatorFactory)
    {
        T [] objs = null;

        if (creator != null)
        {
            m_creator   = creator;
            BAK_CREATOR = creator;
            objs        = m_creator.CreateObjects();
        }
        else if (creatorFactory != null)
        {
            QObjCreator<T> _creator = creatorFactory.CreatCreator();
            BAK_CREATOR             = _creator;
            objs                    = _creator.CreateObjects();
        }
        else
        {
            objs = null;
        }

        return objs;
    }
}
