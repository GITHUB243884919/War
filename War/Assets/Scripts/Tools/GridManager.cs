using UnityEngine;
using System.Collections;

/// <summary>
/// 用于格子管理器，提供格子生成，绘制（debug），世界坐标和格子坐标互相转换
/// </summary>
public class GridManager : MonoBehaviour
{
    private static GridManager s_Instance = null;
    public static GridManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(GridManager))
                        as GridManager;
                if (s_Instance == null)
                    Debug.Log("Could not locate a GridManager " +
                            "object. \n You have to have exactly " +
                            "one GridManager in the scene.");
            }
            return s_Instance;
        }
    }

    public int             m_numOfRows     = 1;
    public int             m_numOfColumns  = 1;
    public float           m_gridCellSize  = 1;
    public bool            m_showGrid      = true;
    public bool            m_showCenter    = false;
    private Vector3        _m_origin       = Vector3.zero;
    public Vector3 Origin
    {
        get { return _m_origin; }
    }

    void Awake()
    {
        _m_origin = transform.position;
    }
    
    /// <summary>
    /// 根据索引获得格子中心位置（世界坐标）
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetGridCellCenter(int index)
    {
        Vector3 cellPosition = GetGridCellPosition(index);
        cellPosition.x += (m_gridCellSize / 2.0f);
        cellPosition.z += (m_gridCellSize / 2.0f);
        return cellPosition;
    }

    /// <summary>
    /// 根据格子索引获得位置（世界坐标）
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosInGrid = col * m_gridCellSize;
        float zPosInGrid = row * m_gridCellSize;
        return Origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
    }

    /// <summary>
    /// 根据位置获得格子索引
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool GetGridIndex(Vector3 pos, out int index)
    {
        index = 0;
        if (!IsInBounds(pos))
        {
            return false;
        }

        pos -= Origin;
        int col = (int)(pos.x / m_gridCellSize);
        int row = (int)(pos.z / m_gridCellSize);
        index = row * m_numOfColumns + col;

        return true;
    }

    /// <summary>
    /// 根据位置获得格子坐标
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public bool GetGridPosition(Vector3 pos, out Vector2 gridPos)
    {
        gridPos = Vector2.zero;
        if (!IsInBounds(pos))
        {
            return false;
        }

        pos -= Origin;
        int col = (int)(pos.x / m_gridCellSize);
        int row = (int)(pos.z / m_gridCellSize);
        gridPos.x = row;
        gridPos.y = col;

        return true;
    }

    /// <summary>
    /// 根据位置获得格子索引和格子坐标
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="index"></param>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    public bool GetGridIndexAndPosition(Vector3 pos, out int index, out Vector2 gridPos)
    {
        index   = 0;
        gridPos = Vector2.zero;
        if (!IsInBounds(pos))
        {
            return false;
        }
 
        pos -= Origin;
        int col = (int)(pos.x / m_gridCellSize);
        int row = (int)(pos.z / m_gridCellSize);
        index = row * m_numOfColumns + col;
        gridPos.x = row;
        gridPos.y = col;
        
        return true;
    }


    /// <summary>
    /// 根据输入的位置的x，z判断是否在整个格子的范围内
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsInBounds(Vector3 pos)
    {
        float width  = m_numOfColumns * m_gridCellSize;
        float height = m_numOfRows    * m_gridCellSize;
        Debug.Log("width = " + width + " height= " + height + "Origin.x = "+ Origin.x + 
            " Origin.x + width = " + (Origin.x + width) + 
            " Origin.z + height = " + (Origin.z + height));
        //return (pos.x >= Origin.x && pos.x <= Origin.x + width &&
        //        pos.z <= Origin.z + height && pos.z >= Origin.z);

        return ((pos.x >= Origin.x && pos.x <= Origin.x + width) &&
            (pos.z >= Origin.z  && pos.z <= Origin.z + height));
    }

    public int GetRow(int index)
    {
        int row = index / m_numOfColumns;
        return row;
    }

    public int GetColumn(int index)
    {
        int col = index % m_numOfColumns;
        return col;
    }

    void OnDrawGizmos()
    {
        if (!m_showGrid)
        {
            return;
        }
        DebugDrawGrid(transform.position, m_numOfRows, m_numOfColumns,
            m_gridCellSize, Color.green);

        if (!m_showCenter)
        {
            return;
        }
        float width  = m_numOfColumns * m_gridCellSize / 2.0f;
        float height = m_numOfRows * m_gridCellSize / 2.0f;
        Vector3 center = transform.position;
        center.x += width;
        center.z += height;
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(center, 10.0f);

        Vector3 pointTest1 = center;
        pointTest1.y += 20.0f;
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(pointTest1, 10.0f);

        Vector3 pointTest2 = center;
        pointTest2.x += 64.0f;
        pointTest2.y += 32.0f;
        pointTest2.z += 96.0f;
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawSphere(pointTest2, 10.0f);
        Debug.Log(pointTest2);

    }

    public void DebugDrawGrid(Vector3 origin, int numRows, int
        numCols, float cellSize, Color color)
    {
        float width = (numCols * cellSize);
        float height = (numRows * cellSize);
        // Draw the horizontal grid lines  
        for (int i = 0; i < numRows + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(0.0f,
                0.0f, 1.0f);
            Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f,
                0.0f);
            Debug.DrawLine(startPos, endPos, color);
        }
        // Draw the vertial grid lines  
        for (int i = 0; i < numCols + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * 
                new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 endPos = startPos + height * 
                new Vector3(0.0f, 0.0f, 1.0f);
            Debug.DrawLine(startPos, endPos, color);
        }
    }

}