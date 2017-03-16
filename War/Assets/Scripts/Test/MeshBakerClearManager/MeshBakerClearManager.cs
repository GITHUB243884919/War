using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshBakerClearManager
{
    List<GameObject>             m_gos       = new List<GameObject>();
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

    public void Add(GameObject go)
    {
        m_gos.Add(go);
    }

    public void Add(MB2_TextureBakeResults texture)
    {
        m_textures.Add(texture);
    }

    public void Add(Material material)
    {
        m_materials.Add(material);
    }

    public GameObject [] GetObjs()
    {
        return m_gos.ToArray();
    }

    public GameObject[] GetGameObjects()
    {
        return m_gos.ToArray();
    }

    public MB2_TextureBakeResults[] GetTextures()
    {
        return m_textures.ToArray();
    }

    public Material [] GetMaterials()
    {
        return m_materials.ToArray();
    }

}
