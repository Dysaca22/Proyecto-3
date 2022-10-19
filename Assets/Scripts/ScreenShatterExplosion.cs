using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShatterExplosion : MonoBehaviour
{
    private void Awake()
    {
        Vector3 explosionPosition = new Vector3(-444.21f, -1.50f, 55.00f);
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(100f, explosionPosition, 300f);
            }
        }
    }
}
