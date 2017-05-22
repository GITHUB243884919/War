using UnityEngine;
using System.Collections;

public class MixBaker_Test : MonoBehaviour {

	// Use this for initialization
    QObjPool<CharObj> m_pool = null;
	void Start () 
    {
        /// path[0] meshbaker生成器资源路径
        /// path[1] meshbaker材质资源路径
        /// path[2] meshbaker贴图资源路径
        /// path[3] meshbaker合并对象资源路径
        /// 
        string [] paths = new string[4];
        paths[0] = "RuntimeMeshBaker/Mix/MaterialBaker";
        paths[1] = "RuntimeMeshBaker/Mix/Mix-mat";
        paths[2] = "RuntimeMeshBaker/Mix/Mix";
        paths[3] = "RuntimeMeshBaker/M_Arm_Artillery/M_Arm_Artillery_Seed," +
            "RuntimeMeshBaker/M_Arm_Artillery/M_Arm_Artillery_Seed," + 
            "RuntimeMeshBaker/M_Arm_Artillery/M_Arm_Artillery_Seed," + 
            "RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank_Seed," +
            "RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank_Seed";

        InitMixCharObjsPool(paths, BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK);

        CharObj obj = m_pool.BorrowObj();
        //obj.AI_Arrive(new Vector3(0, 0, 0), new Vector3(32, 0, 32), 1f);

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void InitMixCharObjsPool(string[] paths, BattleObjManager.E_BATTLE_OBJECT_TYPE type)
    {
        QObjCreatorFactory<CharObj> creatorFactory = new MixCharObjsCreatorFactory(
            paths, type);

        m_pool = new QObjPool<CharObj>();
        m_pool.Init(null, creatorFactory);

        

    }
}
