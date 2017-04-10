/// <summary>
/// NCAI  对象的控制器抽象类
/// author : fanzhengyong
/// date  : 2017-04-10
/// </summary>
using UnityEngine;
using System.Collections;

public abstract class  NCAIController : MonoBehaviour 
{
    public  MoveSteers   m_steers = null;

    public  MoveSteers.E_STEER_TYPE[] m_steerTypes = null;

    //位置数据
    public PositionData PositionData { get; set; }

    private bool m_isInit = false;

    void Init()
    {
        if (m_isInit)
        {
            return;
        }

        InitSteers();

        m_isInit = true;
    }

    public virtual void  InitSteers()
    {
    }

    public virtual void OnArrived()
    {
    }

    void Release()
    {
        m_steers     = null;
        PositionData = null;
    }

    public void AI(PositionData positionData)
    {
        Debug.Log("AI");

        PositionData            = positionData;
        PositionData.GameObject = gameObject;
        PositionData.Transform  = transform;
        m_steers.PositionData   = PositionData;
        m_steers.Active         = true;
    }
    
    //----Unity--------------------------------------------------------------
    void Awake()
    {
        Init();
    }

    void Update()
    {
        if ((m_steers != null) && (m_steers.Active))
        {
            m_steers.Update();
        }
    }

    void FixedUpdate()
    {
        if ((m_steers != null) && (m_steers.Active))
        {
            m_steers.FixedUpdate();
        }
    }

    void OnDestroy()
    {
        Release();
    }
}
