using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GameAction : MonoBehaviour
{

    [System.Serializable]
    public class InteractableEvent : UnityEvent<GameAction> { }

    [SerializeField]
    protected InteractableEvent _action = new InteractableEvent();

    public void DoAction()
    {
        _action.Invoke(this);
    }

}
