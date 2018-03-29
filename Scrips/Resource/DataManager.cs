using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void LoadDataFinish(string[][] datas);

public static class DataManager {

    const string DATA_FOLDER = "Data/";

    public static void LoadData (string fileName, LoadTableDataFinish onFinish) {
        ResourceManager.LoadTableData (DATA_FOLDER + fileName, onFinish);
    }

    public static byte[] StringToByteArray (string value) {
        string[] strArr = value.Split (',');

        int length = strArr.Length;
        byte[] bytArr = new byte[length];
        for (int i = 0; i < length; i++) {
            bytArr[i] = byte.Parse (strArr[i]);
        }

        return bytArr;
    }
}
