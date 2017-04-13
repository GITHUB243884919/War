using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if _LOG_MEDIATOR_
using Debug = LogMediator
#endif
public class Test_AI_Group_2 : MonoBehaviour
{
    
    GroupCharObjsController m_groupCtr = null;
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(BtnClick);
    }

    public void BtnClick()
    {
        CreateGroupCharObjs();
    }

    void CreateGroupCharObjs()
    {
        GroupCharObjsElement[] elements = new GroupCharObjsElement[2];
        elements[0] = new GroupCharObjsElement();
        elements[0].ServerEntityID = 0;
        elements[0].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        elements[1] = new GroupCharObjsElement();
        elements[1].ServerEntityID = 1;
        elements[1].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        //elements[2] = new GroupCharObjsElement();
        //elements[2].ServerEntityID = 2;
        //elements[2].Type = BattleObjManager.E_BATTLE_OBJECT_TYPE.M_ARM_TANK;

        m_groupCtr = new GroupCharObjsController();
        GroupCharObjsController.E_FORMATION_TYPE orgFormation =
            GroupCharObjsController.E_FORMATION_TYPE.TARGET_HORIZONTAL_LINE;
            //GroupCharObjsController.E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
        m_groupCtr.Init(elements, orgFormation, new Vector3(32f, 0f, 32f));

        GroupCharObjsController.E_FORMATION_TYPE switchFormation =
            //GroupCharObjsController.E_FORMATION_TYPE.TARGET_HORIZONTAL_LINE;
            GroupCharObjsController.E_FORMATION_TYPE.TARGET_VERTICAL_LINE;
        StartCoroutine(SwitchFormation(switchFormation, new Vector3(32f, 0f, 32f)));

    }

    public IEnumerator SwitchFormation(
        GroupCharObjsController.E_FORMATION_TYPE formation,
        Vector3 target)
    {

        LogMediator.Log("3秒后开始阵型变换");
        yield return new WaitForSeconds(3f);
        LogMediator.Log("阵型变换中。。。。");
        m_groupCtr.SwitchFormation(formation, target);
    }
}
