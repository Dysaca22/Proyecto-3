using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Principal")]
    public float life;
    public float range;
    public int price;

    [Header("Attack")]
    public float damageAttack;

    [Header("Shot")]
    public GameObject shot;
    public float speedShot;

    private GameObject enemy;
    private float period = 0.0f;

    private void Update()
    {
        GameObject[] allTowers = { };
        if (gameObject.tag != "Torre Fuente de Poder")
        {
            GameObject[] allTowers1 = GameObject.FindGameObjectsWithTag("Infanteria Ligera");
            allTowers = allTowers.Concat(allTowers1).ToArray();
            GameObject[] allTowers2 = GameObject.FindGameObjectsWithTag("Infanteria Pesada");
            allTowers = allTowers.Concat(allTowers2).ToArray();
            GameObject[] allTowers3 = GameObject.FindGameObjectsWithTag("Infanteria Killer");
            allTowers = allTowers.Concat(allTowers3).ToArray();

            float minDistance = 9999f;
            foreach (GameObject player in allTowers)
            {
                float distance = Vector2.Distance(player.transform.position, transform.position);
                if (distance < minDistance && distance <= range)
                {
                    minDistance = distance;
                    enemy = player;
                }
            }
            if (period > speedShot)
            {
                Attack();
                period = 0;
            }
            period += Time.fixedDeltaTime;
            enemy = null;
        }
    }

    public void DecrementLife(float damage)
    {
        if (life - damage >= 0)
        {
            life -= damage;
        }
        else
        {
            Destroy(this.gameObject);
            //ScenesManager.Instance.ChangeScene("StartMenu", ScenesManager.GameState.start, 0.5f);
        }
    }

    public void Attack()
    {
        if (enemy != null)
        {
            GameObject firePoint = transform.GetChild(0).gameObject;
            GameObject InstanceShot = Instantiate(shot, firePoint.transform.position, shot.transform.rotation);
            Vector3 vectorToTarget = enemy.transform.position - InstanceShot.transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
            InstanceShot.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            InstanceShot.GetComponent<Rigidbody2D>().velocity = InstanceShot.transform.up.normalized * speedShot;
            InstanceShot.GetComponent<Shot>().damage = damageAttack;
            Destroy(InstanceShot, 3);
        }
    }
}
