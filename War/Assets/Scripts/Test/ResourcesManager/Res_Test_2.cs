﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UF_Framework;

#if _LOG_MEDIATOR_
using Debug = LogMediator;
#endif

public class Res_Test_2 : MonoBehaviour 
{

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }

    public void BtnClick()
    {
        Debug.Log("BtnClick");
        //Test_Res_2();
        Test_Res_3();

    }

    void Test_Res_2()
    {
        ResourcesManager2.Instance.Init();

        Material m = ResourcesManager2.Instance.LoadAssetSync<Material>(
            "Resources/RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps-mat");
        if (m != null)
        {
            Debug.Log("m != null " + m.GetInstanceID());
        }

        Material m2 = ResourcesManager2.Instance.LoadAssetSync<Material>(
            "Resources/RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps-mat");
        if (m2 != null)
        {
            Debug.Log("m2 != null " + m2.GetInstanceID());
        }

        GameObject go_1 = ResourcesManager2.Instance.LoadAssetSync<GameObject>(
            "Resources/RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank_ORG");
        if (go_1 != null)
        {
            Debug.Log("go_1 != null " + go_1.GetInstanceID());
        }
        GameObject tank_1 = GameObject.Instantiate<GameObject>(go_1);
        Debug.Log("tank_1 " + tank_1.GetInstanceID());

        GameObject go_2 = ResourcesManager2.Instance.LoadAssetSync<GameObject>(
            "Resources/RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank_ORG");
        if (go_1 != null)
        {
            Debug.Log("go_2 != null " + go_1.GetInstanceID());
        }
        GameObject tank_2 = GameObject.Instantiate<GameObject>(go_2);
        Debug.Log("tank_2 " + tank_2.GetInstanceID());

        ResourcesManager2.Instance.Print_CacheAssetsIDMapBunlesPath();
        ResourcesManager2.Instance.ReleaseAsset(ref m);
        Debug.Log("m " + (m == null));

    }

    void Test_Res_3()
    {
        ResourcesManager3.Instance.Init();

        Material m = ResourcesManager3.Instance.LoadAsset<Material>(
            "Resources/RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps-mat");
        if (m != null)
        {
            Debug.Log("m != null " + m.GetInstanceID() + " " + m.name);
            m.name = "ddddd";
            Debug.Log("m != null " + m.GetInstanceID() + " " + m.name);
        }
        ResourcesManager3.Instance.Print_CacheBundles();
        ResourcesManager3.Instance.ReleaseAsset<Material>(ref m);
        Debug.Log("m == null " + (m == null));
        ResourcesManager3.Instance.Print_CacheBundles();

        //Material m2 = ResourcesManager3.Instance.LoadAsset<Material>(
        //    "Resources/RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps-mat");
        //if (m2 != null)
        //{
        //    Debug.Log("m2 != null " + m2.GetInstanceID());
        //}

        //GameObject go_1 = ResourcesManager3.Instance.LoadAsset<GameObject>(
        //    "Resources/RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank_ORG");
        //if (go_1 != null)
        //{
        //    Debug.Log("go_1 != null " + go_1.GetInstanceID());
        //}
        //GameObject tank_1 = GameObject.Instantiate<GameObject>(go_1);
        //Debug.Log("tank_1 " + tank_1.GetInstanceID());

        //GameObject go_2 = ResourcesManager3.Instance.LoadAsset<GameObject>(
        //    "Resources/RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank_ORG");
        //if (go_1 != null)
        //{
        //    Debug.Log("go_2 != null " + go_1.GetInstanceID());
        //}
        //GameObject tank_2 = GameObject.Instantiate<GameObject>(go_2);
        //Debug.Log("tank_2 " + tank_2.GetInstanceID());

        //ResourcesManager3.Instance.Print_CacheAssetsIDMapBunlesPath();
        //ResourcesManager3.Instance.ReleaseAsset(ref m);
        //Debug.Log("m " + (m == null));

    }

}
