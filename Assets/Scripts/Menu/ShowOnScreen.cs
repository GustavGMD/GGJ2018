using UnityEngine;

public class ShowOnScreen : MonoBehaviour {

    string latestEvent;

	 void OnGUI()
	{

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(KeyCode.Return);
        }
    }
}

