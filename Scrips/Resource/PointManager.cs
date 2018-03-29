using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class PointManager {

    const string POINT_SCENE_FOLDER = "Scene/";
    const string POINT_SCENE_NAME = "Point";
    const string ASSETBUNDLE_NAME_POSTFIX = ".assetBundle";

    //static byte PointID = 1;

    public static void PointStart () {
        ResourceManager.LoadScene (POINT_SCENE_FOLDER + POINT_SCENE_NAME + ASSETBUNDLE_NAME_POSTFIX, LoadPoint);
    }

    //加载关卡场景.
    static void LoadPoint () {
        SceneManager.LoadSceneAsync (POINT_SCENE_NAME, LoadSceneMode.Additive);
    }
}
