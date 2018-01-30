using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBubbleManager : MonoBehaviour {

    public GameObject[] feedbackObjects;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateObjects(bool state)
    {
        for (int i = 0; i < feedbackObjects.Length; i++)
        {
            feedbackObjects[i].SetActive(state);
        }
    }
}
