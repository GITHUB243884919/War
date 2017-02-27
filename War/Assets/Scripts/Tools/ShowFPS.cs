using UnityEngine;
using System.Collections;

public class ShowFPS : MonoBehaviour {

    public float f_UpdateInterval = 0.5F;

    private float f_LastInterval;

    private int i_Frames = 0;

    private float f_Fps;

    void Start() 
    {
		//Application.targetFrameRate=60;

        f_LastInterval = Time.realtimeSinceStartup;

        i_Frames = 0;
    }

    void OnGUI() 
    {
//#if UNITY_EDITOR || INNER_TEST
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;    //���ñ������  
        fontStyle.normal.textColor = new Color(1, 0, 0);   //����������ɫ  
        fontStyle.fontSize = 40;       //�����С  
        //GUI.Label(new Rect(0, 0, 200, 200), "Hello Font", fontStyle); 
        GUI.Label(new Rect(0, 100, 400, 400), "FPS:" + f_Fps.ToString("f2"), fontStyle);
 

//#endif
    }

    void Update() 
    {
        ++i_Frames;

        if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval) 
        {
            f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);

            i_Frames = 0;

            f_LastInterval = Time.realtimeSinceStartup;
        }
    }
}
