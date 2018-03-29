using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {

    const string POINT_NAME = "Point";

    void Start () {
        PointManager.PointStart ();
    }
}
