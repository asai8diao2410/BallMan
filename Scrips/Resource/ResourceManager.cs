using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void LoadResourceFinish ();
public delegate void LoadTableDataFinish (List<List<string>> stringLists);

public class ResourceData {
    public ResourceManager.ResourceType type;
    public string Name {
        set {
            names = new List<string> { value };
        }
    }
    public List<string> names;      //多资源时使用.
    public bool saved;

    public LoadResourceFinish onLoadResourceFinish;
    public LoadTableDataFinish onLoadTableDataFinish;
}

public class ResourceManager : MonoBehaviour {

    public enum ResourceType {
        Prefab,     //预设.
        Scene,      //场景.
        TableData,  //表格数据.
    }

    static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject> ();        //已加载完成的预设字典.
    static List<ResourceData> LoadResourceDataList = new List<ResourceData> ();      //待加载资源列表.

    static bool Loading;

    void Update () {
        if (!Loading && LoadResourceDataList.Count > 0) {
            StartCoroutine (LoadResource ());
        }
    }

    IEnumerator LoadResource () {
        Loading = true;
        while (true) {
            ResourceData data = LoadResourceDataList[0];
            for (int i = 0; i < data.names.Count; i++) {
                string path = GetResourcePath (data.names[i]);
                using (WWW www = new WWW (path)) {
                    yield return www;

                    try {
                        switch (data.type) {
                            case ResourceType.Prefab:
                                GameObject obj = (GameObject)www.assetBundle.LoadAsset (www.assetBundle.GetAllAssetNames ()[0]);
                                string objName = obj.name;
                                if (!Prefabs.ContainsKey (objName)) {
                                    Prefabs.Add (objName, obj);
                                }
                                break;
                            case ResourceType.Scene:
                                AssetBundle ab = www.assetBundle;
                                break;
                            case ResourceType.TableData:
                                string text = www.text;
                                List<List<string>> datas = TableStringAnalysis (text);

                                if (data.onLoadTableDataFinish != null) {
                                    data.onLoadTableDataFinish (datas);
                                }
                                break;
                        }
                    } catch (Exception e) {
                        Debug.LogError ("Fail to load: " + path);
                        Debug.LogError (e);
                    }
                }
            }

            //回调.
            if (data.onLoadResourceFinish != null) {
                data.onLoadResourceFinish ();
            }

            LoadResourceDataList.RemoveAt (0);
            if (LoadResourceDataList.Count == 0) {
                break;
            }
        }

        Loading = false;
    }

    //表格字符串分解.
    static List<List<string>> TableStringAnalysis (string text) {
        if (text == null)
            return null;

        List<List<string>> result = new List<List<string>> ();
        List<string> line = new List<string> ();
        string field = "";
        bool isInQuotation = false; //字符串模式  
        bool isInField = true;      //是否在读取Field，用来表示空Field  
        int i = 0;
        while (i < text.Length) {
            char ch = text[i];
            if (isInQuotation) {
                if (ch == '"') {
                    if (i < text.Length - 1 && text[i + 1] == '"') {    //重复"只算一个，切不结束字符串模式
                        field += '"';
                        i++;
                    } else {
                        isInQuotation = false;
                    }
                } else {
                    field += ch;    //字符串模式中所有字符都要加入  
                }
            } else {
                switch (ch) {
                    case ',':
                        line.Add (field);
                        field = "";
                        isInField = true;
                        break;
                    case '"':
                        if (isInField)
                            isInQuotation = true;//进入字符串模式  
                        else
                            field += ch;
                        break;
                    case '\r':
                        if (field.Length > 0 || isInField) {
                            line.Add (field);
                            field = "";
                        }
                        result.Add (line);
                        line = new List<string> ();
                        isInField = true;//下一行首先应该是数据  
                        if (i < text.Length - 1 && text[i + 1] == '\n')//跳过\r\n  
                            i++;
                        break;
                    default:
                        isInField = false;
                        field += ch;
                        break;
                }
            }
            i++;
        }
        //收尾工作  
        if (field.Length > 0 || isInField && line.Count > 0)//如果是isInField标记的单元格，则要保证这行有其他数据，否则单独一个空单元格的行是没有意义的  
            line.Add (field);

        if (line.Count > 0)
            result.Add (line);

        return result;
    }

    //资源加载.
    public static void LoadPrefab (string name) {
        LoadPrefab (name, true, null);
    }

    //资源加载，并在加载完成后调用回调.
    public static void LoadPrefab (string name, bool saved, LoadResourceFinish onFinish) {
        ResourceData data = new ResourceData {
            Name = name,
            type = ResourceType.Prefab,
            saved = saved,
            onLoadResourceFinish = onFinish
        };
        LoadResourceDataList.Add (data);
    }

    //资源加载.
    public static void LoadPrefabs (List<string> names) {
        LoadPrefabs (names, true, null);
    }

    //多资源加载，并在所有资源(包括正在加载的其他资源)加载完成后调用回调.
    public static void LoadPrefabs (List<string> names, bool saved, LoadResourceFinish onFinish) {
        ResourceData data = new ResourceData {
            names = names,
            type = ResourceType.Prefab,
            saved = saved,
            onLoadResourceFinish = onFinish
        };
        LoadResourceDataList.Add (data);
    }

    //获取预设.
    public static GameObject GetPrefab (string name) {
        GameObject obj = null;
        if (Prefabs.TryGetValue (name, out obj)) {
            obj = Instantiate (obj);
        }
        return obj;
    }

    //获取预设.
    public static bool GetPrefab (string name, out GameObject prefab) {
        return Prefabs.TryGetValue (name, out prefab);
    }

    //获取预设(没有资源时加载资源，并在加载完成后调用回调). 
    public static bool GetPrefab (string name, out GameObject prefab, LoadResourceFinish onFinish, bool saved) {
        bool result = GetPrefab (name, out prefab);
        if (!result) {
            LoadPrefab (name, saved, onFinish);
        }

        return result;
    }

    //加载场景.
    public static void LoadScene (string name, LoadResourceFinish onFinish) {
        ResourceData data = new ResourceData {
            Name = name,
            type = ResourceType.Scene,
            saved = false,
            onLoadResourceFinish = onFinish
        };
        LoadResourceDataList.Add (data);
    }

    //加载表格数据.
    public static void LoadTableData (string name, LoadTableDataFinish onFinish) {
        ResourceData data = new ResourceData {
            Name = name,
            type = ResourceType.TableData,
            saved = false,
            onLoadTableDataFinish = onFinish
        };
        LoadResourceDataList.Add (data);
    }

    //获取streamingAssetsPath文件夹下的文件路径.
    static string GetResourcePath (string name) {
        return
#if !UNITY_ANDROID
            "file://" +
#endif
            Application.streamingAssetsPath + "/" + name;
    }

    //清空资源.
    public static void ClearAllResources () {
        Prefabs.Clear ();
    }
}
