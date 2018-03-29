using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour {

	const float RECOVER_DIS = 50.0f;

	public Transform End;

	void Update () {
        if (BallManager.curBall != null) {
            float aDis = BallManager.curBall.transform.position.z - transform.position.z;
            if (aDis > RECOVER_DIS) {
                RouteManager.Instant.RecoverOneRoute();
                RouteManager.Instant.GenerateOneRoute();
            }
        }
	}
}
