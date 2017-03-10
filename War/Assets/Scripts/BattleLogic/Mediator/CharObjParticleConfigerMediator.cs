/// <summary>
/// CharObj携带的粒子特效初始化参数的中介，提供获取参数配置的能力
/// author : fanzhengyong
/// date  : 2017-03-09
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharParticleConfiger
{
    //资源路径
    public string         ResPath        { get; set; }

    //挂点
    public string         PointPath      { get; set; }
    
    public CharParticleConfiger(string resPath, string pointPath)
    {
        ResPath   = resPath;
        PointPath = pointPath;
    }
}

public class CharObjParticleConfigerMediator
{
    private Dictionary<BattleObjManager.E_BATTLE_OBJECT_TYPE,
        Dictionary<CharController.E_COMMOND, CharParticleConfiger>> m_cfgs = 
        new Dictionary<BattleObjManager.E_BATTLE_OBJECT_TYPE,
        Dictionary<CharController.E_COMMOND, CharParticleConfiger>>();

    private static CharObjParticleConfigerMediator s_Instance = null;
    public static CharObjParticleConfigerMediator Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new CharObjParticleConfigerMediator();
                s_Instance.Init();
            }
            return s_Instance;
        }
    }

    private void Init()
    {
        /////////////////Tank//////////////////////
        Dictionary<CharController.E_COMMOND, CharParticleConfiger> tankCfgs =
            new Dictionary<CharController.E_COMMOND, CharParticleConfiger>();
        AddOneParticleConfiger(tankCfgs, CharController.E_COMMOND.ATTACK,
            "Tank/Prefab/Tank_dapao_fire", "Bone01/Bone02/Dummy01");
        AddOneParticleConfiger(tankCfgs, CharController.E_COMMOND.ATTACKED,
            "Tank/Prefab/Tank_tanke_hit", "Bone01/Bone02/Dummy01");
        m_cfgs.Add(BattleObjManager.E_BATTLE_OBJECT_TYPE.TANK, tankCfgs);

        /////////////M_Arm_Tank/////////////////////
        Dictionary<CharController.E_COMMOND, CharParticleConfiger> armTankCfs =
            new Dictionary<CharController.E_COMMOND, CharParticleConfiger>();
        AddOneParticleConfiger(armTankCfs, CharController.E_COMMOND.ATTACK,
            "Tank/Prefab/eff_artillery_fight", "Bone01/Bone02/Fire");
        AddOneParticleConfiger(armTankCfs, CharController.E_COMMOND.ARRIVE,
            "Tank/Prefab/eff_artillery_contrail", "Bone01/Track_1");
        m_cfgs.Add(BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK, armTankCfs);


        ///////////////M_Arm_Airplane_01目前没有特效/////////////////////

        ///////////////M_Arm_Engineercar目前没有特效/////////////////////

        ///////////////M_Arm_Engineercorps/////////////////////
        Dictionary<CharController.E_COMMOND, CharParticleConfiger> armCorpCfs =
            new Dictionary<CharController.E_COMMOND, CharParticleConfiger>();
        AddOneParticleConfiger(armCorpCfs, CharController.E_COMMOND.ATTACK,
            "Tank/Prefab/eff_gun_fight", "Bip01/Bip01 Prop1/Fire");
        m_cfgs.Add(BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ENGINEERCORPS, armCorpCfs);

        ///////////////M_Build_Jeep目前没有特效/////////////////////

        ///////////////M_Arm_Artillery/////////////////////
        Dictionary<CharController.E_COMMOND, CharParticleConfiger> armArtilleryCfs =
            new Dictionary<CharController.E_COMMOND, CharParticleConfiger>();
        AddOneParticleConfiger(armArtilleryCfs, CharController.E_COMMOND.ATTACK,
            "Tank/Prefab/eff_artillery_fight", "Bone01/Bone05/Fire");
        AddOneParticleConfiger(armArtilleryCfs, CharController.E_COMMOND.ARRIVE,
            "Tank/Prefab/eff_artillery_contrail", "Bone01/Track_1");
        m_cfgs.Add(BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_ARTILLERY, armArtilleryCfs);
    }

    private void AddOneParticleConfiger(Dictionary<CharController.E_COMMOND, CharParticleConfiger> cfgs, 
        CharController.E_COMMOND cmd, string resPath, string PointPath)
    {
        CharParticleConfiger cfg = new CharParticleConfiger(
            resPath, PointPath);
        cfgs.Add(cmd, cfg);
    }

    public CharParticleConfiger GetConfiger(BattleObjManager.E_BATTLE_OBJECT_TYPE type,
        CharController.E_COMMOND commond)
    {
        CharParticleConfiger cfg = null;
        Dictionary<CharController.E_COMMOND, CharParticleConfiger> effects = null;
        m_cfgs.TryGetValue(type, out effects);
        if (effects == null)
        {
            Debug.LogError("没有这类CharObj的特效配置 " + type.ToString());
            return cfg;
        }

        effects.TryGetValue(commond, out cfg);
        if (cfg == null)
        {
            Debug.LogError("没有这类CharObj对应的Commond的特效配置 "
                + type.ToString() + " " + commond.ToString());
        }

        return cfg;
    }

}
