using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	//前进翻滚(给球顶部施加力).
	const float STAGE_TIME = 10.0f;		//前进速度上升时间.
	const float UP_SPEED = 10.0f;		//前进速度上升时增加的速度.

	float LastUpTime;		//上一次上升时间.

	public float RotateSpeed = 50.0f;		//前进速度.
	new Rigidbody rigidbody;

	//横向翻滚(旋转重力方向).
	const float GRAVITY_MAX_ROTATE_SPEED = 100.0f;
	const float GRAVITY_ROTATE_ACC_SPEED = 1000.0f;

	float GracvityRotateSpeed;		//重力旋转速度(顺时针负).
	static float GravityX = 0.0f;
	static float GravityY = -9.8f;

	public static Vector3 GravityDir {
		get {
			return new Vector3 (GravityX, GravityY);
		}
	}

	public static float GravityAngle {
		get {
			return Mathf.Atan (GravityX / GravityY) * Mathf.Rad2Deg;
		}
	}

	public Transform camara;

	bool deaded;
	int score;

	bool rotateToLeft;		//顺时针旋转.
	bool rotateToRight;     //逆时针旋转.

    public static float RotateAngle;

	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
		LastUpTime = Time.time;
	}

    void Update () {
        if (Time.time - LastUpTime >= STAGE_TIME) {
            LastUpTime += STAGE_TIME;
            RotateSpeed += UP_SPEED;
        }

        if (!deaded) {
            float dis = transform.position.z;
            score = (int)dis;

            float offDis = Mathf.Sqrt (transform.position.x * transform.position.x + transform.position.y * transform.position.y);
            if (offDis >= 5.0f) {
                deaded = true;
                Debug.Log ("Dead");
            }
        }
    }

    void FixedUpdate () {
#if UNITY_EDITOR_WIN
        rotateToLeft = Input.GetKey (KeyCode.LeftArrow);
        rotateToRight = Input.GetKey (KeyCode.RightArrow);
#endif

        RotateGravityVec ();

        rigidbody.AddForceAtPosition (camara.forward * RotateSpeed, transform.position - GravityDir.normalized * 0.5f, ForceMode.Force);
    }

    //旋转重力朝向.
    void RotateGravityVec () {
        if (rotateToLeft) {
            if (!rotateToRight) {
                GracvityRotateSpeed += GRAVITY_ROTATE_ACC_SPEED * Time.deltaTime;
                if (GracvityRotateSpeed > GRAVITY_MAX_ROTATE_SPEED) {
                    GracvityRotateSpeed = GRAVITY_MAX_ROTATE_SPEED;
                }
            }
        } else if (rotateToRight) {
            GracvityRotateSpeed -= GRAVITY_ROTATE_ACC_SPEED * Time.deltaTime;
            if (GracvityRotateSpeed < -GRAVITY_MAX_ROTATE_SPEED) {
                GracvityRotateSpeed = -GRAVITY_MAX_ROTATE_SPEED;
            }
        } else {
            if (GracvityRotateSpeed > 0.0f) {
                GracvityRotateSpeed -= GRAVITY_ROTATE_ACC_SPEED * Time.deltaTime;
                if (GracvityRotateSpeed < 0.0f) {
                    GracvityRotateSpeed = 0.0f;
                }
            } else if (GracvityRotateSpeed < 0.0f) {
                GracvityRotateSpeed += GRAVITY_ROTATE_ACC_SPEED * Time.deltaTime;
                if (GracvityRotateSpeed > 0.0f) {
                    GracvityRotateSpeed = 0.0f;
                }
            }
        }

        float rotateAngle = GracvityRotateSpeed * Time.deltaTime;
        RotateAngle += rotateAngle;
        RotateVec (rotateAngle);

        Physics.gravity = GravityDir;
    }

    //旋转向量.
    void RotateVec (float rAngle) {
        float sin = Mathf.Sin (Mathf.PI * rAngle / 180.0f);
        float cos = Mathf.Cos (Mathf.PI * rAngle / 180.0f);
        float newX = GravityX * cos + GravityY * sin;
        float newY = GravityX * -sin + GravityY * cos;
        GravityX = newX;
        GravityY = newY;
    }

	Rect scoreRect = new Rect (20.0f, 20.0f, 100.0f, 50.0f);
    void OnGUI () {
        GUI.Label (scoreRect, "得分：" + score.ToString ());
    }
}
