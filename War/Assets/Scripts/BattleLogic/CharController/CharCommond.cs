using UnityEngine;
using System.Collections;

public abstract class CharCommond 
{
    public CharController m_cctr;
    public CharCommond(CharController ctr)
    {
        if (ctr == null)
        {
            Debug.LogWarning("CharController 为空");
            return;
        }

        m_cctr = ctr;
    }

    public virtual void Init()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }
}
