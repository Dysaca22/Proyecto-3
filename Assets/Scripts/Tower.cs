using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static Tower Instance;
    [Header("Principal")]
    public float life;
    public float range;

    [Header("Attack")]
    public float damageAttack;

    [Header("Shot")]
    public GameObject shot;
    public float speedShot;

    private GameObject enemy;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        float minDistance = 9999f;
        GameObject[] allTowers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in allTowers)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);
            if (distance < minDistance && distance <= range)
            {
                enemy = player;
            }
        }
        Attack();
        enemy = null;
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
            ScenesManager.Instance.ChangeScene("StartMenu", ScenesManager.GameState.start);
        }
    }

    public void Attack()
    {
        if (enemy != null)
        {
            GameObject firePoint = transform.GetChild(0).gameObject;
            Quaternion rotation = transform.rotation * Quaternion.Inverse(shot.transform.rotation);
            GameObject InstanceShot = Instantiate(shot, firePoint.transform.position, rotation);

            Vector2 direction = enemy.transform.position - transform.position;
            direction.Normalize();
            float factor = Time.deltaTime * speedShot;
            InstanceShot.transform.Translate(direction.x * factor, direction.y * factor, transform.position.z, Space.World);

            //InstanceShot.GetComponent<Rigidbody2D>().velocity = transform.up.normalized * attackSpeed;
            //InstanceShot.GetComponent<Shot>().damage = damageAttack;
            Destroy(InstanceShot, 3);
        }
    }
}
