using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour {

    public event Action<PlayerBehaviour> onAttackHit;

    public float cooldown = 1.5f;
    public bool readyToFire = true;
    public float count = 0;

    public GameObject hookTip;
    public GameObject tailSprite;
    public GameObject bodySprite;

    public Vector3 preAttackScale = new Vector3(0.1f, 1, 1);
    public Vector3 attackTargetScale = new Vector3(3, 1, 1);

    private PotaTween _potaTween;

	// Use this for initialization
	void Start ()
    {
        hookTip.SetActive(false);
        transform.localScale = preAttackScale;
        _potaTween = PotaTween.Create(gameObject);
        hookTip.GetComponent<HookBehaviour>().onEnemyCollision += OnEnemyCollision;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!readyToFire)
        {
            count += Time.deltaTime;
            if(count >= cooldown)
            {
                readyToFire = true;
                count = 0;
            }
        }
	}

    public void StartAttack()
    {
        if (readyToFire)
        {
            readyToFire = false;

            hookTip.SetActive(true);

            //set the animation parameters
            _potaTween.Clear();
            _potaTween.SetScale(preAttackScale, attackTargetScale);
            _potaTween.SetEaseEquation(Ease.Equation.OutSine);
            _potaTween.SetDuration(0.4f);
            _potaTween.Play(delegate
            {
                StopAttack();
            });
            //tailSprite.GetComponent<Animator>().SetTrigger("Attack");
        }
    }

    public void StopAttack()
    {
        hookTip.SetActive(false);
        _potaTween.SetScale(transform.localScale, preAttackScale);
        _potaTween.SetEaseEquation(Ease.Equation.OutExpo);
        _potaTween.SetDuration(0.4f);
        _potaTween.Play(delegate
        {
        });
    }

    public void OnEnemyCollision(PlayerBehaviour p_other)
    {
        //Debug.Log("Called this.......");
        if (onAttackHit != null) onAttackHit(p_other);
        StopAttack();
    }

    public void ResetOnAttackHit()
    {
        onAttackHit = null;
    }

    public void DisableHookCollider()
    {
        //hookTip.SetActive(true);
        hookTip.GetComponent<Collider2D>().enabled = false;
        //hookTip.SetActive(false);
    }

    public void SetBodySprite(bool value)
    {
        bodySprite.GetComponent<Renderer>().enabled = value;
    }
}
