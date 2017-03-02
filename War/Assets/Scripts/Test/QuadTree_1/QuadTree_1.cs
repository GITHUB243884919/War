using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadTree_1 : MonoBehaviour
{
    //arysize 表示矩阵长度，level表示等级，curary[]表示当前矩阵
    public void Qutree(int arysize, int level, float[] curary)
    {

        float fi = curary[0];
        int i;
        //遍历当前数组，是否同构
        for (i = 0; i <= arysize * arysize - 1; i++)
        {
            if (fi != curary[i])
            {
                Debug.Log("fi != curary[i]");
                break;
            }

        }
        if (i == arysize * arysize)
        {
            //printf("%d,%f",level,fi);
            //printf("\n");
            Debug.Log("level " + level + " fi " + fi);
            return;
        }

        else
        {
            arysize /= 2;
            float[] ary1 = new float[arysize * arysize];
            float[] ary2 = new float[arysize * arysize];
            float[] ary3 = new float[arysize * arysize];
            float[] ary4 = new float[arysize * arysize];
            for (i = 0; i < arysize; i++)
            {
                for (int j = 0; j < arysize; j++)
                {
                    //左上
                    ary1[i * arysize + j] = curary[i * (arysize * 2) + j];
                    //右上
                    ary2[i * arysize + j] = curary[i * (arysize * 2) + (arysize + j)];
                    //左下
                    ary3[i * arysize + j] = curary[(arysize + i) * (arysize * 2) + j];
                    //右下
                    ary4[i * arysize + j] = curary[(arysize + i) * (arysize * 2) + (arysize + j)];
                }
            }

            level++;
            Qutree(arysize, level, ary1);
            Qutree(arysize, level, ary2);
            Qutree(arysize, level, ary3);
            Qutree(arysize, level, ary4);
        }
    }

    void Start()
    {
        
        float [] aa;
        aa = new float[16]{
            1,1,1,1,
            1,1,1,1,
            1,1,1,1,
            1,1,1,1
        };
        Qutree(4,0,aa);

    }
}
//int main()
//{
//    //float aa[16]={1,1,2,2,1,1,3,3,4,2,1,2,3,4,3,4};
//    //Qutree(4,0,aa);
//    float aa[64]={1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1};
//    Qutree(8,0,aa);
//    return 0;

//}