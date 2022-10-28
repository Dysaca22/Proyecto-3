using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public float damage;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.tag == "Shot enemy")
        {
            if (collision.gameObject.tag == "Torre Fuente de Poder" || collision.gameObject.tag == "Torre Pesada" || collision.gameObject.tag == "Torre Ligera")
            {
                collision.gameObject.transform.GetComponent<Tower>().DecrementLife(damage);
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (collision.gameObject.tag == "Infanteria Killer" || collision.gameObject.tag == "Infanteria Pesada" || collision.gameObject.tag == "Infanteria Ligera")
            {
                collision.gameObject.transform.GetComponent<Character>().DecrementLife(damage);
                Destroy(this.gameObject);
            }
        }
    }
}
