using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// DB manager.
/// The First Line Must be Column names
/// </summary>
public abstract class DBManager<PK, T, M> : Singleton<M> where M : class,new()
{
    protected Dictionary<PK, T> caches = new Dictionary<PK, T>();
    protected List<T> values = new List<T>();

    protected static char[] LINE_SEPERATE = new char[2] { '\r', '\n' };
    protected static char[] COLUMN_SEPERATE = new char[2] { '\t', ' ' };

    //public void InitAsync(string path)
    //{
    //    DBConfig.InitAsync(this, path);
    //}

    public string TrimSemicolon(string str)
    {
        if (str[0] == '\"')
        {
            str = str.Substring(1, str.Length - 1);
        }

        if (str[str.Length - 1] == '\"')
        {
            str = str.Substring(0, str.Length - 1);
        }
        return str;
    }
    public void Init(string path)
    {
        //TextAsset textAsset = ResourceManager.LoadDataResource(path);
        TextAsset textAsset = ResourcesManagerMediator.
            GetNoGameObjectFromResourcesManager<TextAsset>(path);
        string[] lines = textAsset.ToString().Split(LINE_SEPERATE, System.StringSplitOptions.RemoveEmptyEntries);

        Resources.UnloadAsset(textAsset);

        if (lines.Length < 1)
        {
            Debug.LogErrorFormat("read data error from {0}", path);
            return;
        }
        string firstline = TrimSemicolon(lines[0]);
        string[] columns = firstline.Split(COLUMN_SEPERATE, System.StringSplitOptions.RemoveEmptyEntries);
        T value;
        PK pk = default(PK);
        string line;
        string[] properties;

        for (int i = 1; i < lines.Length; ++i)
        {
            line = lines[i];
            line = TrimSemicolon(line);
            if (line.Length < 2)
            {
                continue;
            }
            properties = line.Split(COLUMN_SEPERATE);
            if (properties.Length < columns.Length)
            {
                Debug.LogError(lines[i]);
#if UNITY_EDITOR
                throw new Exception(string.Format("{0}(line {1}) : '{2}' ", path, i + 1, line.Length));
#endif
                continue;
            }
            try
            {
                value = ParseValue(out pk, properties, columns);
                caches[pk] = value;
                values.Add(value);
            }
            catch (Exception e)
            {
                Debug.LogError(lines[i]);
                throw e;
            }
        }

    }

    protected abstract T ParseValue(out PK pk, string[] properties, string[] columns);

    public List<T> Values
    {
        get { return this.values; }
    }

    public T Get(PK pk)
    {
        T value;
        if (caches.TryGetValue(pk, out value))
        {
            return value;
        }
        return default(T);
    }

}

/// <summary>
/// Dictionary DB manager.
/// The First Column Must be Primary key of the data
/// </summary>
public class DictDBManager<M> : DBManager<string, Dictionary<string, string>, M> where M : class,new()
{
    protected override Dictionary<string, string> ParseValue(out string pk, string[] properties, string[] columns)
    {
        Dictionary<string, string> value = new Dictionary<string, string>();
        pk = properties[0];
        for (int i = 0; i < columns.Length; ++i)
        {
            value[columns[i]] = properties[i];
        }
        return value;
    }
}

/// <summary>
/// Simple DB manager.
/// The First Column Must be Primary key of the data
/// PK是主键 
/// T 是表的数据类型定义 T的字段定义的命名必须和文本配置文件上的列名一致，大小写敏感
///   T的第一个字段就是PK
/// M 就是表的Manger类自身，即是继承SimpleDBManager的那个类
/// 
/// </summary>
public class SimpleDBManager<PK, T, M> : DBManager<PK, T, M>
    where T : new()
    where M : class,new()
{
    static System.Type[] paramTypes = new System.Type[] { typeof(string) };

    protected override T ParseValue(out PK pk, string[] properties, string[] columns)
    {
        T value = new T();
        pk = default(PK);

        object obj = value as object;
        var type = obj.GetType();
        int index = 0;
        foreach (string column in columns)
        {
            FieldInfo field = type.GetField(column);
            if (field != null)
            {
                var fieldValue = ParseField(field.FieldType, properties[index]);
                field.SetValue(obj, fieldValue);
                if (index == 0)
                {
                    pk = (PK)fieldValue;
                }
            }
            ++index;
        }

        return (T)obj;
    }

    protected virtual object ParseField(System.Type fieldType, string fieldValue)
    {
        if (fieldType == typeof(string))
        {
            return fieldValue;
        }
        else
        {
            var method = fieldType.GetMethod("Parse", paramTypes);
            var returnValue = method.Invoke(fieldType, new object[] { fieldValue });
            return returnValue;
        }
    }


}

//public partial class DBConfig
//{

//    private class AsyncTask
//    {
//        public object obj;
//        public string path;

//        public AsyncTask(object obj, string path)
//        {
//            this.obj = obj;
//            this.path = path;
//        }

//        public void execute()
//        {
//            var type = this.obj.GetType();
//            var method = type.GetMethod("Init", new Type[1] { typeof(string) });
//            method.Invoke(this.obj, new object[] { this.path });
//        }

//        public override string ToString()
//        {
//            return path;
//        }
//    }

//    private static List<AsyncTask> tasks = new List<AsyncTask>();

//    public static void InitAsync(object manager, string path)
//    {
//        tasks.Add(new AsyncTask(manager, path));
//    }

//    public static IEnumerator AsyncLoad(ProgressListener listener)
//    {

//        for (int i = 0; i < tasks.Count; ++i)
//        {
//            listener("正在加载数据:" + tasks[i], i, tasks.Count);
//            yield return null;
//            tasks[i].execute();
//        }

//        listener("数据文件加载完成 ", tasks.Count, tasks.Count);
//        tasks.Clear();
//        yield return new WaitForEndOfFrame();
       

//        //ResourceManager.DestroyResource("Data");
//    }

//}
