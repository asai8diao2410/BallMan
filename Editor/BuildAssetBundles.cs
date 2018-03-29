using UnityEngine;
using UnityEditor;

public static class BuildAssetBundles {

    [MenuItem ("Assets/打包/Windows")]
    static void BuildForWindows () {
        BuildAssetBundle (BuildTarget.StandaloneWindows64);
    }

    [MenuItem ("Assets/打包/Android")]
    static void BuildForAndroid () {
        BuildAssetBundle (BuildTarget.Android);
    }

    [MenuItem ("Assets/打包/IOS")]
    static void BuildForIOS () {
        BuildAssetBundle (BuildTarget.iOS);
    }

    static void BuildAssetBundle (BuildTarget buildTarget) {
        string TargetPath = EditorUtility.OpenFolderPanel ("选择打包目录", Application.streamingAssetsPath, "");

        Object[] selects = Selection.GetFiltered (typeof (Object), SelectionMode.TopLevel);
        int length = selects.Length;
        AssetBundleBuild[] buildMap = new AssetBundleBuild[length];
        Debug.Log ("Build Cout " + length);
        for (int i = 0; i < length; i++) {
            Debug.Log ("Build " + selects[i].name);
            string objPath = AssetDatabase.GetAssetPath (selects[i]);
            buildMap[i].assetBundleName = selects[i].name + ".assetbundle";
            buildMap[i].assetNames = new string[] { objPath };
        }
        BuildPipeline.BuildAssetBundles (TargetPath, buildMap, BuildAssetBundleOptions.None, buildTarget);
    }
}
