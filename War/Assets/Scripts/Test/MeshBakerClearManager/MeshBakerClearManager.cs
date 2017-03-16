using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MeshBakerClearManager
{
    List<GameObject>             m_gos        = new List<GameObject>();
    List<GameObject>             m_seeds     = new List<GameObject>();
    List<GameObject>             m_combines  = new List<GameObject>();
    List<GameObject>             m_bakers    = new List<GameObject>();
    List<MB2_TextureBakeResults> m_textures  = new List<MB2_TextureBakeResults>();
    List<Material>               m_materials = new List<Material>();

    private static MeshBakerClearManager s_Instance = null;
    public static MeshBakerClearManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new MeshBakerClearManager();
            }
            return s_Instance;
        }
    }

    public void AddCombine(MB3_MeshBaker meshBaker, GameObject seed, GameObject bakerGo)
    {
        DigitalOpus.MB.Core.MB3_MeshCombinerSingle meshCombiner
            = meshBaker.meshCombiner as DigitalOpus.MB.Core.MB3_MeshCombinerSingle;
        GameObject combineGo = meshCombiner.resultSceneObject;

        GameObject UIGo = GameObject.Find("Canvas/Button/Text");
        if (UIGo != null)
        {
            Text text = UIGo.GetComponent<Text>();
            if (text != null)
            {
                text.text += (combineGo != null);
            }
        }

        MeshBakerClearManager.Instance.AddCombine(combineGo);
        MeshBakerClearManager.Instance.AddSeed(seed);
        MeshBakerClearManager.Instance.AddBaker(bakerGo);
    }
    public void AddGo(GameObject go)
    {
        m_gos.Add(go);
    }
    public void AddSeed(GameObject go)
    {
        m_seeds.Add(go);
    }

    public void AddCombine(GameObject go)
    {
        m_combines.Add(go);
    }

    public void AddBaker(GameObject go)
    {
        m_bakers.Add(go);
    }

    public void AddTexture(MB2_TextureBakeResults texture)
    {
        m_textures.Add(texture);
    }

    public void AddMaterial(Material material)
    {
        m_materials.Add(material);
    }

    public MB2_TextureBakeResults[] GetTextures()
    {
        return m_textures.ToArray();
    }

    public Material [] GetMaterials()
    {
        return m_materials.ToArray();
    }

    public void Realse()
    {
        for (int i = 0; i < m_bakers.Count; i++)
        {
            MB3_TextureBaker textureBaker = m_bakers[i].GetComponent<MB3_TextureBaker>();
            MB3_MeshBaker meshBaker = m_bakers[i].GetComponentInChildren<MB3_MeshBaker>();
            textureBaker.resultMaterial = null;
            textureBaker.textureBakeResults = null;
            meshBaker.textureBakeResults = null;
        }

        for (int i = 0; i < m_bakers.Count; i++)
        {
            Debug.Log("m_bakers " + m_bakers[i].name);
            GameObject.Destroy(m_bakers[i]);  
        }
        m_bakers.Clear();

        for (int i = 0; i < m_seeds.Count; i++)
        {
            GameObject.Destroy(m_seeds[i]);  
        }
        m_seeds.Clear();

        for (int i = 0; i < m_gos.Count; i++)
        {
            Debug.Log("m_gos " + m_gos[i].name);
            GameObject.Destroy(m_gos[i]);
        }
        m_gos.Clear();

        for (int i = 0; i < m_combines.Count; i++)
        {
            GameObject.Destroy(m_combines[i]);
        }
        m_combines.Clear();

        for (int i = 0; i < m_materials.Count; i++)
        {
            Debug.Log("m_materials " + m_materials[i].name);
            //GameObject.DestroyImmediate(m_materials[i], true);
            m_materials[i] = null;

        }
        m_materials.Clear();

        for (int i = 0; i < m_textures.Count; i++)
        {
            Debug.Log("m_texturs " + m_textures[i].name);
            //GameObject.DestroyImmediate(m_textures[i], true);
            m_textures[i] = null;

        }
        m_textures.Clear();

    }

}
