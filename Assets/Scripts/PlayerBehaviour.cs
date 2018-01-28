using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class PlayerBehaviour : MonoBehaviour {

    #region Public Attributes
    public List<GameObject> hookPivots;
    public GameObject hookReference;

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

    public bool autonomous = true;
    public List<GameObject> capturedPlayersStack;
    public List<char> capturedInputsStack;
    #endregion

    #region Private Attributes
    Rigidbody2D _rigidbody2D;
    Vector2 _levelBoundaries;
    #endregion

    // Use this for initialization
    void Start ()
    {
        //capturedPlayersStack = new List<GameObject>();
        //capturedInputsStack = new List<char>();

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
        //Debug.Log(capturedInputsStack.Count);
        for (int i = 0; i < capturedInputsStack.Count; i++)
        {
            //Debug.Log("Checking " + capturedInputsStack[i]);
            if (Input.GetKeyDown(capturedInputsStack[i].ToString()))
            {
                //Debug.Log("INPUUUT");
                LaunchHook(i);
            }
        }
        
        if (autonomous)
        {
            MovementBehaviour();
            RotationBehaviour();
        }
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
        if(transform.position.x >= _levelBoundaries.x)
        {
            __newVelocity = __newVelocity.magnitude * new Vector2(-1, 0);
        }
        else if (transform.position.x <= -_levelBoundaries.x)
        {
            __newVelocity = __newVelocity.magnitude * new Vector2(1, 0);
        }
        else if(transform.position.y >= _levelBoundaries.y)
        {
            __newVelocity = __newVelocity.magnitude * new Vector2(0, -1);
        }
        else if (transform.position.y <= -_levelBoundaries.y)
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
        //Debug.Log("Changed Rotation Direction");
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

    public void LaunchHook(int p_index)
    {
        if (hookPivots[p_index].GetComponent<HookController>().readyToFire)
        {
            hookPivots[p_index].GetComponent<HookController>().readyToFire = false;
            //inicia instanciando um Hook e inicializando seus atributos
            GameObject __tempHook = Instantiate(hookReference, hookPivots[p_index].transform.position, transform.rotation);
            Vector2 __currentRotation = new Vector2(Mathf.Cos(hookPivots[p_index].transform.parent.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(hookPivots[p_index].transform.parent.eulerAngles.z * Mathf.Deg2Rad)).normalized * 10;
            __tempHook.GetComponent<HookBehaviour>().velocity = __currentRotation;
            __tempHook.GetComponent<HookBehaviour>().maxDistance = 10;
            __tempHook.GetComponent<HookBehaviour>().onFinished += delegate
            {
            //Adicionar um Cooldown no disparo do Hook e criar uma pool de Hooks a serem usados e reutilizados
            __tempHook.SetActive(false);
            };
            __tempHook.GetComponent<HookBehaviour>().onEnemyCollision += delegate (PlayerBehaviour p_other)
            {
                if (p_other != this)
                {
                //atingiu um player isolado
                if (p_other.capturedPlayersStack.Count == 1)
                    {
                    //Adiciona elementos nas stacks: Referencia ao objeto a ser animado, e referência ao Input q ele usa
                    capturedPlayersStack.Add(p_other.capturedPlayersStack[0]);
                        capturedInputsStack.Add(p_other.capturedInputsStack[0]);
                        hookPivots.Add(p_other.hookPivots[0]);

                    //Reseta atributos do player capturado
                    CleanCapturedPlayer(p_other.gameObject);
                    //p_other.transform.position = transform.position;

                    //adiciona um componente de Tween que será usado para as animações
                    PotaTween.Create(p_other.gameObject);

                    //Play animations
                    BlobJoinAnimation(p_other.gameObject, p_other.transform.position);
                        GrowingAnimation(1f);
                    }
                //atingiu um Blob
                else
                    {
                    //A blob atingida desprende um de seus blobs: chamamos um método nela que retorna o objeto a ser manipulado, e um outro que retorna o input
                    //Pega a referência das coisas do outro
                    GameObject __tempObject = p_other.RemoveParticleFromBlob();
                        char __tempInput = p_other.RemoveInputFromBlob();
                        GameObject __tempPivot = p_other.RemoveHookPivotFromBlob();
                        capturedPlayersStack.Add(__tempObject);
                        capturedInputsStack.Add(__tempInput);
                        hookPivots.Add(__tempPivot);
                        __tempPivot.transform.parent.SetParent(transform);
                    //__tempObject.transform.position = p_other.transform.position;
                    //Debug.Log(p_other.transform.position);
                    //__tempPivot.transform.parent.position = transform.position;

                    //chama as animações
                    BlobJoinAnimation(__tempObject, p_other.transform.position);
                        GrowingAnimation(1);
                        p_other.GrowingAnimation(0);
                    }

                //desativa pra ele não colidir com mais nada
                __tempHook.SetActive(false);
                }
            };
        }
    }
    public void UpdateHookPivots()
    {
        float __archSize = (2 * Mathf.PI) / hookPivots.Count;
        for (int i = 0; i < hookPivots.Count; i++)
        {
            //atualiza o parent do pivot 
            hookPivots[i].transform.parent.parent = gameObject.transform;
            hookPivots[i].transform.parent.position = transform.position;
            hookPivots[i].transform.localScale = Vector3.one/2;
            hookPivots[i].transform.parent.localScale = Vector3.one;

            //calcula o novo ângulo
            float __targetAngle = i * __archSize;
            int __num = i;

            //coloca tween nos pivots, ou confere se eles já têm
            if (hookPivots[i].GetComponent<PotaTween>() == null) PotaTween.Create(hookPivots[i].gameObject);
            PotaTween __pTween = hookPivots[i].GetComponent<PotaTween>();
            __pTween.Reset();
            __pTween.SetFloat(hookPivots[i].transform.eulerAngles.z, __targetAngle * Mathf.Rad2Deg);
            __pTween.UpdateCallback(delegate()
            {
                if(hookPivots.Count > __num) hookPivots[__num].transform.parent.eulerAngles = new Vector3(0, 0, __pTween.Float.Value);
            });

            //ativa o tween pra ir pra posição
            __pTween.Play();
        }
    }

    public void BlobJoinAnimation(GameObject p_target, Vector3 p_from)
    {
        p_target.GetComponent<Renderer>().enabled = true;
        //p_target.transform.SetParent(null);
        //p_target.transform.localScale = Vector3.one * 3;
        Vector2 __startingPosition = p_target.transform.position;
        //p_target.transform.SetParent(transform);
        PotaTween __pTween = p_target.GetComponent<PotaTween>();
        __pTween.Reset();
        __pTween.SetPosition(p_from, transform.position);
        __pTween.SetEaseEquation(Ease.Equation.InSine);
        __pTween.UpdateCallback(delegate
        {
            //Debug.Log(new Vector2((p_target.transform.position.x - __startingPosition.x), p_target.transform.position.y - __startingPosition.y));
            //p_target.transform.position = (Vector2)transform.position + ((__startingPosition - (Vector2)p_target.transform.position) * __pTween.Float.Value);
            __pTween.Position.To = (Vector2)transform.position;
        });        
        __pTween.Play(delegate 
        {
            p_target.GetComponent<Renderer>().enabled = false;
        });        
    }
    public void GrowingAnimation(float p_delay)
    {        
        if (GetComponent<PotaTween>() == null) PotaTween.Create(gameObject);
        PotaTween __pTween = GetComponent<PotaTween>();
        __pTween.SetScale(transform.localScale, Vector3.one*(capturedPlayersStack.Count)*3);
        __pTween.SetEaseEquation(Ease.Equation.OutElastic);
        __pTween.SetDelay(p_delay);
        __pTween.Play(delegate 
        {
            //UpdateHookPivots();
        });
        UpdateHookPivots();
    }

    public GameObject RemoveParticleFromBlob()
    {
        GameObject __tempObject = capturedPlayersStack[capturedPlayersStack.Count - 1];
        __tempObject.transform.position = transform.position;
        capturedPlayersStack.RemoveAt(capturedPlayersStack.Count - 1);
        return __tempObject;
    }
    public char RemoveInputFromBlob()
    {
        char __tempInput = capturedInputsStack[capturedInputsStack.Count - 1];
        capturedInputsStack.RemoveAt(capturedInputsStack.Count - 1);
        return __tempInput;
    }
    public GameObject RemoveHookPivotFromBlob()
    {
        GameObject __tempPivot = hookPivots[hookPivots.Count - 1];
        hookPivots.RemoveAt(hookPivots.Count - 1);
        return __tempPivot;
    }

    public void CleanCapturedPlayer(GameObject p_captured)
    {
        Destroy(p_captured.GetComponent<PlayerBehaviour>());
        Destroy(p_captured.GetComponent<Rigidbody2D>());
        Destroy(p_captured.GetComponent<Collider2D>());
        p_captured.GetComponent<Renderer>().enabled = false;
        //Transform[] __children = p_captured.transform.GetComponentsInChildren<Transform>();
        //for (int i = 0; i < __children.Length; i++)
        //{
        //    if(__children[i].gameObject != p_captured.gameObject) Destroy(__children[i].gameObject);
        //}
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
