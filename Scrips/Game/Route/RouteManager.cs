using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoutePool {
	int RouteID;

	List<GameObject> Routes = new List<GameObject> ();

	public RoutePool (int ID) {
        RouteID = ID;
	}

	public void RecoverRoute (GameObject route) {
		Routes.Add (route);
		route.SetActive (false);
	}

	public GameObject GetRoute () {
		GameObject route;
		if (Routes.Count > 0) {
			route = Routes [0];
			route.SetActive (true);
			Routes.RemoveAt (0);
		} else {
			route = ResourceManager.GetPrefab (RouteManager.ASSETBUNDLE_NAME_PREFIX + RouteID);
			route.name = RouteID.ToString ();
		}
		return route;
	}
}

public class RouteManager : MonoBehaviour {

	public static RouteManager Instant;

	const int ROUTE_EXIST_COUNT = 50;
	const int ROUTE_KIND_COUNT = 3;

	const float ROTATE_SPEED = 30.0f;

	List<GameObject> routes = new List<GameObject> ();
	Dictionary<int, RoutePool> RoutePools = new Dictionary<int, RoutePool> ();

	const string ASSETBUNDLE_FOLDER = "Route/";
	public const string ASSETBUNDLE_NAME_PREFIX = "route_";
    public const string ASSETBUNDLE_NAME_POSTFIX = ".assetBundle";

    bool rotateToLeft;
	bool rotateToRight;

	Transform end = null;
	Rigidbody routeRigidbody;

	void Start () {
		routeRigidbody = GetComponent<Rigidbody> ();
		Instant = GetComponent<RouteManager> ();
        List<string> names = new List<string>();
        for (int i = 1; i <= ROUTE_KIND_COUNT; i++) {
            names.Add(ASSETBUNDLE_FOLDER + ASSETBUNDLE_NAME_PREFIX + i + ASSETBUNDLE_NAME_POSTFIX);
        }
        ResourceManager.LoadPrefabs(names, true, GenerateInitRoutes);

        PointDataManager.LoadData();
    }

    void Rotate () {
        if (rotateToLeft) {
            if (!rotateToRight) {
                transform.Rotate (Vector3.forward, Time.deltaTime * ROTATE_SPEED);
            }
        } else if (rotateToRight) {
            transform.Rotate (-Vector3.forward, Time.deltaTime * ROTATE_SPEED);
        }
    }

	//生产初始化路径.
	void GenerateInitRoutes () {
		for (int i = 0; i < ROUTE_EXIST_COUNT; i++) {
			GenerateOneRoute ();
		}
	}

	//获取路径.
	GameObject GetRoute (int rIndex) {
		RoutePool routePool;
		if (!RoutePools.TryGetValue (rIndex, out routePool)) {
			routePool = new RoutePool (rIndex);
			RoutePools.Add (rIndex, routePool);
		}
		return routePool.GetRoute ();
	}

	//回收一个路径.
	public void RecoverOneRoute () {
		RoutePools [int.Parse (routes [0].name)].RecoverRoute (routes [0]);
		routes.RemoveAt (0);
	}

    //生产一个路径.
    public void GenerateOneRoute () {
        int index = Random.Range (0, ROUTE_KIND_COUNT);
        GameObject route = GetRoute (index + 1);
        Transform routeTra = route.transform;
        if (end != null) {
            routeTra.position = end.position;
            routeTra.rotation = end.rotation;
        } else {
            routeTra.position = new Vector3 (0.0f, 0.0f, -1.0f);
        }
        routeTra.parent = transform;
        routes.Add (route);
        end = route.GetComponent<Route> ().End;
    }
}
