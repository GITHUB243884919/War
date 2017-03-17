using UnityEngine;
using System.Collections;


public class BundleNode
{
    private AssetBundle m_bundle = null;
    private int m_refCount = 0;
    public BundleNode(AssetBundle bundle)
    {
        m_bundle = bundle;
        m_refCount = 1;
    }

    public void AddRef()
    {
        m_refCount++;
    }

    public void MinusDef()
    {
        m_refCount--;
    }

    public int RefCount { get { return m_refCount; } }

    public AssetBundle Bundle { get { return m_bundle; } }
}

