using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointData {
    public ushort ID;
    public byte[] routeIDs;
}

public static class PointDataManager {

    public static Dictionary<ushort, PointData> PointDatas = new Dictionary<ushort, PointData> ();

    const string FILE_NAME = "point.csv";

    public static void LoadData() {
        DataManager.LoadData (FILE_NAME, SaveData);
    }

    static void SaveData (List<List<string>> datas) {
        int index;
        PointData data;
        for (int i = 1; i < datas.Count; i++) {
            index = 0;
            data = new PointData ();

            data.ID = ushort.Parse (datas[i][index]);
            data.routeIDs = DataManager.StringToByteArray (datas[i][++index]);

            PointDatas.Add (data.ID, data);
        }
    }
}
