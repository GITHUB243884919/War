using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test_Element
{
    public GameObject m_go = null;
    public Test_Element(GameObject go)
    {
        m_go = go;
    }

    public void Release()
    {
        Debug.Log("Test_Element Release");
        m_go = null;
    }
}

public class Dictionary_Test_1 : MonoBehaviour
{
	void Start () 
    {
        Test_1();
        Test_2();
	    
	}

    void Test_1()
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();
        dic.Add("a", 2);
        dic.Add("b", 5);
        dic.Add("c", 8);
        var enumerator = dic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Debug.Log("while " + enumerator.Current.Key + " " + enumerator.Current.Value);
            
        }

        foreach (KeyValuePair<string, int> pair in dic)
        {
            Debug.Log(pair.Key + " " + pair.Value);
        }

        foreach (int value in dic.Values)
        {
            Debug.Log("value " + value);
        }

        foreach (string key in dic.Keys)
        {
            Debug.Log("key " + key);
        }

        foreach (int value in dic.Values)
        {
            Debug.Log("before " + value);
        }
        Debug.Log("---------------------");
        //foreach (string key in dic.Keys)
        //{
        //    dic[key] *= 2;
        //}
        //foreach (KeyValuePair<string, int> pair in dic)
        //{
        //    //dic[key] *= 2;
        //    dic[pair.Key] *= 2;
        //}
        Debug.Log("---------------------");
        foreach (int value in dic.Values)
        {
            Debug.Log("after " + value);
        }
    }

    void Test_2()
    {
        Dictionary<string, Test_Element> dic = new Dictionary<string, Test_Element>();
        dic.Add("a", new Test_Element(gameObject));
        dic.Add("b", new Test_Element(gameObject));
        dic.Add("c", new Test_Element(gameObject));
        foreach (KeyValuePair<string, Test_Element> pair in dic)
        {           
            pair.Value.Release();
        }
        dic.Clear();
        dic = null;
    }

}
