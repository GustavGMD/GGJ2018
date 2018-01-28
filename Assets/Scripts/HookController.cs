using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour {

    public float cooldown = 1.5f;
    public bool readyToFire = true;
    public float count = 0;

	// Use this for initialization
	void Start ()
    {
		
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
}
