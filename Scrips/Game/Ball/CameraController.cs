using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform camera;
	public Transform Ball;

	Vector3 RelativePosition = new Vector3 (0.0f, 1.25f, -4.0f);

	void FixedUpdate () {
		if (Ball == null) {
			return;
		}

		//坐标.
		transform.position = Ball.position;

        //朝向.
        transform.eulerAngles = new Vector3 (0.0f, 0.0f, -BallController.RotateAngle);
    }
}
