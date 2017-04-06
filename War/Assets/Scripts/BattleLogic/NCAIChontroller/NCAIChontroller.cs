using UnityEngine;
using System.Collections;

public class NCAIChontroller : MonoBehaviour 
{
    public MoveSteers   m_steers = null;
    public MoveSteers.E_STEER_TYPE[] m_steerTypes = null;
    //位置数据
    //public PositionData m_positionData = new PositionData();
    //public PositionData PositionData { get { return m_positionData; } }
    public PositionData PositionData { get; set; }
    
    void Init()
    {
        //m_steers = new MoveSteers();
        //MoveSteers.E_STEER_TYPE[] steerType;
        //steerType = new MoveSteers.E_STEER_TYPE[1]{
        //    MoveSteers.E_STEER_TYPE.STATIC_BOMB
        //};
        //m_steers.Init(steerType, PositionData, OnArrived);
    }

    void Release()
    {
        m_steers     = null;
        PositionData = null;
    }

    public void AI(PositionData positionData)
    {
        Debug.Log("AI");

        m_steers = new MoveSteers();
        m_steerTypes = new MoveSteers.E_STEER_TYPE[1]{
            MoveSteers.E_STEER_TYPE.STATIC_BOMB
        };
        

        PositionData = positionData;
        PositionData.GameObject = gameObject;
        PositionData.Transform = transform;
        m_steers.Init(m_steerTypes, PositionData, OnArrived);
        m_steers.Active = true;
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
