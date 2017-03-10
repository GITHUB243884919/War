/// <summary>
/// CharObjPool初始化参数的中介，提供获取参数配置的能力
/// author : fanzhengyong
/// date  : 2017-03-09
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharObjPoolConfiger 
{
    private int       m_count = 0;
    private string [] m_paths = new string[4];
    public CharObjPoolConfiger(int count, string[] paths)
    {
        if (paths.Length != 4)
        {
            Debug.LogError("CharObjPool 的参数错误");
            return;
        }
        if (count < 4)
        {
            Debug.LogError("CharObjPool 的参数错误");
            return;
        }

        m_count = count;
        for (int i = 0; i < paths.Length; i++)
        {
            m_paths[i] = paths[i];
        }
    }

    public int Count 
    {
        get { return m_count; }
    }

    public string[] Paths
    {
        get { return m_paths; }
    }
}

public class CharObjPoolConfigerMediator
{
    private Dictionary<BattleObjManager.E_BATTLE_OBJECT_TYPE, CharObjPoolConfiger> m_cfgs
        = new Dictionary<BattleObjManager.E_BATTLE_OBJECT_TYPE, CharObjPoolConfiger>();

    private static CharObjPoolConfigerMediator s_Instance = null;
    public static CharObjPoolConfigerMediator Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new CharObjPoolConfigerMediator();
                s_Instance.Init();
            }
            return s_Instance;
        }
    }

    private void Init()
    {
        int tankCount = 32;
        string [] tankPaths =  new string[4]
        {
            "TankRuntime_Bake/TankMeshBaker",
            "TankRuntime_Bake/TankRuntime_Bake-mat",
            "TankRuntime_Bake/TankRuntime_Bake",
            "TankRuntime_Bake/Tank_Seed"
        };
        CharObjPoolConfiger tankCfg = new CharObjPoolConfiger(tankCount, tankPaths);
        m_cfgs.Add(BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK, tankCfg);

        ///////////////////M_Arm_Tank/////////////////
        int armTankCount = 32;
        string[] armTankPaths = new string[4]
        {
            "RuntimeMeshBaker/M_Arm_Tank/MaterialBaker",
            "RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank-mat",
            "RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank",
            "RuntimeMeshBaker/M_Arm_Tank/M_Arm_Tank_Seed"
        };
        AddOneConfig(BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK,
            armTankCount, armTankPaths);

        /////////////////////M_Arm_Airplane_01////////////
        int armPlaneCount = 32;
        string[] armPlanePaths = new string[4]
        {
            "RuntimeMeshBaker/M_Arm_Airplane_01/MaterialBaker",
            "RuntimeMeshBaker/M_Arm_Airplane_01/M_Arm_Airplane_01-mat",
            "RuntimeMeshBaker/M_Arm_Airplane_01/M_Arm_Airplane_01",
            "RuntimeMeshBaker/M_Arm_Airplane_01/M_Arm_Airplane_01_Seed"
        };
        AddOneConfig(BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_AIRPLANE_01,
            armPlaneCount, armPlanePaths);

        //////////////////M_Arm_Engineercar/////
        int armCarCount = 32;
        string[] armCarPaths = new string[4]
        {
            "RuntimeMeshBaker/M_Arm_Engineercar/MaterialBaker",
            "RuntimeMeshBaker/M_Arm_Engineercar/M_Arm_Engineercar-mat",
            "RuntimeMeshBaker/M_Arm_Engineercar/M_Arm_Engineercar",
            "RuntimeMeshBaker/M_Arm_Engineercar/M_Arm_Engineercar_Seed"
        };
        AddOneConfig(BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCAR,
            armCarCount, armCarPaths);

        //////////////////M_Arm_Engineercorps/////
        int armCorpCarCount = 32;
        string[] armCorpPaths = new string[4]
        {
            "RuntimeMeshBaker/M_Arm_Engineercorps/MaterialBaker",
            "RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps-mat",
            "RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps",
            "RuntimeMeshBaker/M_Arm_Engineercorps/M_Arm_Engineercorps_Seed"
        };
        AddOneConfig(BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS,
            armCorpCarCount, armCorpPaths);

        //////////////////M_Build_Jeep/////
        int armJeepCarCount = 32;
        string[] armJeepPaths = new string[4]
        {
            "RuntimeMeshBaker/M_Build_Jeep/MaterialBaker",
            "RuntimeMeshBaker/M_Build_Jeep/M_Build_Jeep-mat",
            "RuntimeMeshBaker/M_Build_Jeep/M_Build_Jeep",
            "RuntimeMeshBaker/M_Build_Jeep/M_Build_Jeep_Seed"
        };
        AddOneConfig(BattleObjManager.E_BATTLE_OBJECT_TYPE.M_BUILD_JEEP,
            armJeepCarCount, armJeepPaths);

        //////////////////M_ArmArtillery/////
        int armArtilleryCount = 32;
        string[] armArtilleryPaths = new string[4]
        {
            "RuntimeMeshBaker/M_ArmArtillery/MaterialBaker",
            "RuntimeMeshBaker/M_ArmArtillery/M_ArmArtillery-mat",
            "RuntimeMeshBaker/M_ArmArtillery/M_ArmArtillery",
            "RuntimeMeshBaker/M_ArmArtillery/M_ArmArtillery_Seed"
        };
        AddOneConfig(BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ARTILLERY,
            armArtilleryCount, armArtilleryPaths);


    }

    private void AddOneConfig(BattleObjManager.E_BATTLE_OBJECT_TYPE type,
        int count, string[] paths)
    {
        CharObjPoolConfiger cfg = new CharObjPoolConfiger(count, paths);
        m_cfgs.Add(type, cfg);
    }

    public CharObjPoolConfiger GetConfiger(BattleObjManager.E_BATTLE_OBJECT_TYPE type)
    {
        CharObjPoolConfiger cfg = null;

        m_cfgs.TryGetValue(type, out cfg);

        if (cfg == null)
        {
            Debug.LogError("没有这类对象的pool参数 " + type.ToString());
        }

        return cfg;
    }
}

