using UnityEngine;
using System.Collections;

public class PositionData 
{
    public GameObject GameObject { get; set; }
    public Transform  Transform  { get; set; }
    public float SpeedForArrive  { get; set; }
    public Vector3    Offset     { get; set; }

    private Vector3 m_targetForPosition = Vector3.zero;
    public Vector3 TargetForPosition
    {
        get
        {
            return m_targetForPosition;
        }
        set
        {
            m_targetForPosition = value;
            m_targetForPosition += Offset;
        }
    }

    private Vector3 m_targetForArrive = Vector3.zero;
    public Vector3 TargetForArrive
    {
        get
        {
            return m_targetForArrive;
        }
        set
        {
            m_targetForArrive = value;
            m_targetForArrive += Offset;
        }
    }

    private Vector3 m_targetForLookAt = Vector3.zero;
    public Vector3 TargetForLookAt
    {
        get
        {
            return m_targetForLookAt;
        }
        set
        {
            m_targetForLookAt = value;
            m_targetForLookAt += Offset;
        }
    }
}
