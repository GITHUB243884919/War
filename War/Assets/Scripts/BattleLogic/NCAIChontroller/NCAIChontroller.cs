using UnityEngine;
using System.Collections;

public class NCAIChontroller : MonoBehaviour 
{
    private MoveSteers   m_steers = null;

    private MoveSteers.E_STEER_TYPE[] m_steerTypes = null;

    //位置数据
    public PositionData PositionData { get; set; }

    private bool m_isInit = false;

    void Init()
    {
        if (m_isInit)
        {
            return;
        }
        m_steers = new MoveSteers();
        m_steerTypes = new MoveSteers.E_STEER_TYPE[1]{
            MoveSteers.E_STEER_TYPE.STATIC_BOMB
        };

        m_steers.Init(m_steerTypes, OnArrived);

        m_isInit = true;
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

    void OnArrived()
    {
        LogMediator.Log("命中目标");
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
