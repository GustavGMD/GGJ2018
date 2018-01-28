using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{


    private Vector3 MovingDirection = Vector3.up;
    public Vector3 targetAngle = new Vector3(0f, 0f, 2.5f);
    private Vector3 currentAngle;

    void Start()
    {
        currentAngle = transform.eulerAngles;
    }


    void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, -Mathf.PingPong(Time.time * 0.5f, 2.5f));


        gameObject.transform.Translate(MovingDirection * Time.deltaTime * 2f);

        if (gameObject.transform.position.y > 3)
        {
            MovingDirection = Vector3.down;
        }
        else if (gameObject.transform.position.y < -3)
        {
            MovingDirection = Vector3.up;
        }

        if (gameObject.transform.position.x > 3)
        {
            MovingDirection = Vector3.left;
        }
        else if (gameObject.transform.position.x < -3)
        {
            MovingDirection = Vector3.right;
        }

        /*currentAngle = new Vector3(
			Mathf.LerpAngle(currentAngle.x, targetAngle.x, Time.deltaTime),
			Mathf.LerpAngle(currentAngle.y, targetAngle.y, Time.deltaTime),
			Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime * 0.01f)
			);
		transform.eulerAngles = currentAngle;*/
    }
}


