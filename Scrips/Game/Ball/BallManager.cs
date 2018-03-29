using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour {

	const string ASSETBUNDLE_FOLDER = "Ball/";
	const string ASSETBUNDLE_NAME_PREFIX = "ball_";
    const string ASSETBUNDLE_NAME_POSTFIX = ".assetBundle";

    public static GameObject curBall;

	public CameraController cameraController;
	
	const int BALL_KIND_COUNT = 1;

	GameObject[] ballPrefabs = new GameObject[BALL_KIND_COUNT];

    int BallID = 1;     //球默认ID.

    void Start () {
        GenerateBall ();
    }

    void GenerateBall () {
        string path = ASSETBUNDLE_FOLDER + ASSETBUNDLE_NAME_PREFIX + BallID + ASSETBUNDLE_NAME_POSTFIX;
        ResourceManager.LoadPrefab (path, true, GenerateInitBall);
    }

    //生成初始球.
    void GenerateInitBall () {
        GameObject ball = ResourceManager.GetPrefab (ASSETBUNDLE_NAME_PREFIX + BallID);
        ball.transform.position = new Vector3 (0.0f, -4.5f, 0.0f);
//		ball.transform.parent = RouteManager.Instant.transform;
        BallController ballController = ball.AddComponent<BallController> ();
        ballController.camara = cameraController.camera;
        cameraController.Ball = ball.transform;

        curBall = ball;
    }
}
