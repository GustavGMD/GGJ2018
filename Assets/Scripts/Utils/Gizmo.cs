using UnityEngine;
using System.Collections;

public class Gizmo : MonoBehaviour {

    public float radius = .75f;
    public Color color = Color.yellow;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
