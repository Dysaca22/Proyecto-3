using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public float damage;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Tower.Instance.DecrementLife(damage);
        Destroy(this.gameObject);
    }
}
