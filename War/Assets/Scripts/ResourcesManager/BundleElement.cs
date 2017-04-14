using UnityEngine;
using System.Collections;

public class BundleElement
{
    private AssetBundle m_bundle   = null;
    private int         m_refCount = 0;

    public BundleElement(AssetBundle bundle)
    {
        m_bundle = bundle;
    }

    public void AddRef()
    {
        m_refCount++;
    }

    public void MinusDef()
    {
        m_refCount--;
    }

    /// <summary>
    /// 可能会有修改，主要看AssetBundle在哪里释放
    /// </summary>
    public void Release()
    {
        m_bundle   = null;
        m_refCount = 0;
    }

    public int RefCount { get { return m_refCount; } }

    public AssetBundle Bundle { get { return m_bundle; } }
}

