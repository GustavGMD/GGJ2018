using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class PlayerBehaviour : MonoBehaviour {

    #region Public Attributes
    public GameObject hookPivot;
    public char actionButton;

    //A aceleração é definida como uma direção em que se dará o movimento
    public Vector2 movementAccelerationVector;
    public float movementAccelerationScale;
    //Define o valor máximo que a velocidade pode atingir em uma certa direção(o módulo do vetor de velocidade)
    public float maxMovementVelocityMagnitude;
    public float movementDirectionChangeMaximumWaitingTime = 1;

    public int rotationAccelerationValue = 1;
    public float rotationAccelerationScale = 10;
    public float maxRotationVelocityValue;
    public float rotationDirectionChangeMaximumWaitingTime = 1;
    #endregion

    #region Private Attributes
    Rigidbody2D _rigidbody2D;
    Vector2 _levelBoundaries;
    #endregion

    // Use this for initialization
    void Start ()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _levelBoundaries.y = Camera.main.orthographicSize;
        _levelBoundaries.x = Camera.main.orthographicSize * Camera.main.aspect;

        RandomMovement();
        RandomRotation();

        StartCoroutine(MovementDirectionChange());
        StartCoroutine(RotationDirectionChange());
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(actionButton.ToString()))
        {
            LaunchHook();
        }
        MovementBehaviour();
        RotationBehaviour();
    }

    /// <summary>
    /// É chamado a cada Update
    /// </summary>
    public void MovementBehaviour()
    {
        //Aplica a aceleração no Player
        Vector2 __newVelocity = _rigidbody2D.velocity + (movementAccelerationVector * movementAccelerationScale * Time.deltaTime);

        //Confere se o Player já atingiu a velocidade máxima
        //é mais rápido usar .SqrMagnitude() do que .Magnitude
        if (__newVelocity.SqrMagnitude() > Mathf.Pow(maxMovementVelocityMagnitude, 2))
        {
            __newVelocity = __newVelocity.normalized * maxMovementVelocityMagnitude;
        }

        //Confere se o Player está no limite do mapa
        //Caso sim, faz ele mover-se na direção oposta
        if(GetComponent<Transform>().position.x >= _levelBoundaries.x)
        {
            __newVelocity = __newVelocity.magnitude * new Vector2(-1, 0);
        }
        else if (GetComponent<Transform>().position.x <= -_levelBoundaries.x)
        {
            __newVelocity = __newVelocity.magnitude * new Vector2(1, 0);
        }
        else if(GetComponent<Transform>().position.y >= _levelBoundaries.y)
        {
            __newVelocity = __newVelocity.magnitude * new Vector2(0, -1);
        }
        else if (GetComponent<Transform>().position.y <= -_levelBoundaries.y)
        {
            __newVelocity = __newVelocity.magnitude * new Vector2(0, 1);
        }

        //Aplica a nova velocidade ao Rigidbody2D
        _rigidbody2D.velocity = __newVelocity;
    }
    public void RotationBehaviour()
    {
        //Aplica a aceleração na rotação do player
        float __newAngularVelocity = _rigidbody2D.angularVelocity + (rotationAccelerationValue * rotationAccelerationScale * Time.deltaTime);

        //confere se a velocidade de rotação passou do máximo permitido
        if(Mathf.Pow(__newAngularVelocity, 2) > Mathf.Pow(maxRotationVelocityValue, 2))
        {
            __newAngularVelocity = Mathf.Sign(__newAngularVelocity) * maxRotationVelocityValue;
        }

        //aplica a nova velocidade angular ao Rigidbody2D
        _rigidbody2D.angularVelocity = __newAngularVelocity;
    }

    IEnumerator MovementDirectionChange()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, movementDirectionChangeMaximumWaitingTime));
        RandomMovement();
        //Debug.Log("Changed MOvement Direction");
        StartCoroutine(MovementDirectionChange());
    }
    IEnumerator RotationDirectionChange()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(2, rotationDirectionChangeMaximumWaitingTime));
        RandomRotation();
        Debug.Log("Changed Rotation Direction");
        StartCoroutine(RotationDirectionChange());
    }

    public void RandomMovement()
    {
        movementAccelerationVector = RandomizeDirection();
    }
    public void RandomRotation()
    {
        rotationAccelerationValue = (int)Mathf.Sign(UnityEngine.Random.Range(-1f, 1f));
        //rotationAccelerationValue = -rotationAccelerationValue;
    }

    public void LaunchHook()
    {

    }

    #region Utility Methods
    /// <summary>
    /// Returns a normalized Vector2(with lenght == 1)
    /// </summary>
    /// <returns></returns>
    public Vector2 RandomizeDirection()
    {
        //randomiza um ângulo entre 0 e 2PI(em radianos)
        float __angle = UnityEngine.Random.Range(0f, 1f) * 2 * Mathf.PI;
        return (new Vector2(Mathf.Cos(__angle), Mathf.Sin(__angle))).normalized;
    }

    #endregion
}
