using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBehaviour : MonoBehaviour {

    public event Action<PlayerBehaviour> onEnemyCollision;
    public event Action onFinished;

    public Vector2 startingPosition;
    public float maxDistance;
    public Vector2 velocity;

	// Use this for initialization
	void Start ()
    {
        startingPosition = GetComponent<Transform>().position;
        GetComponent<Rigidbody2D>().velocity = velocity;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(((Vector2)GetComponent<Transform>().position - startingPosition).sqrMagnitude >= Mathf.Pow(maxDistance, 2))
        {
            if (onFinished != null) onFinished();
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (onEnemyCollision != null) onEnemyCollision(collision.gameObject.GetComponent<PlayerBehaviour>());
        }
    }
}
