using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Principal")]
    public float life;
    public float damageAttack;
    public float timeAttack;
    public float speed;
    public int price;
    [Header("Attack")]
    public float range;
    public float attackSpeed = 5f;
    private float period = 0.0f;
    [Header("Shot")]
    public GameObject shot;
    [Header("Tower")]
    private GameObject tower;

    private Vector3 GetPosition => transform.position;
    private Quaternion lastRotation;

    private void Awake()
    {
        GameObject[] allTowers = GameObject.FindGameObjectsWithTag("Tower");
        float minDistance = 9999f;
        foreach (GameObject tower in allTowers)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), 
                new Vector2(tower.transform.position.x, tower.transform.position.y)) < minDistance)
            {
                this.tower = tower;
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.is_play)
        {
            if (GetPosition.y < 0.5)
            {
                Move();
            }
            else
            {
                Rotate();
                if (lastRotation == transform.rotation)
                {
                    if (Vector2.Distance(transform.position, tower.transform.position) > range)
                    {
                        Move2();
                    }
                    else
                    {
                        if (period > timeAttack)
                        {
                            Attack();
                            period = 0;
                        }
                        period += Time.fixedDeltaTime;
                    }
                }
            }
        }
    }

    public void Attack()
    {
        if (range > 0)
        {
            GameObject firePoint = transform.GetChild(0).gameObject;
            Quaternion rotation = transform.rotation * Quaternion.Inverse(shot.transform.rotation);
            GameObject InstanceShot = Instantiate(shot, firePoint.transform.position, rotation);
            InstanceShot.GetComponent<Rigidbody2D>().velocity = transform.up.normalized * attackSpeed;
            InstanceShot.GetComponent<Shot>().damage = damageAttack;
            Destroy(InstanceShot, 3);
        }
    }

    private void Move()
    {
        transform.position += new Vector3(0, 1, 0) * Time.fixedDeltaTime * speed;
    }

    private void Move2()
    {
        Vector2 direction = tower.transform.position - transform.position;
        direction.Normalize();
        float factor = Time.deltaTime * speed;
        this.transform.Translate(direction.x * factor, direction.y * factor, GetPosition.z, Space.World);
    }

    private void Rotate()
    {
        lastRotation = transform.rotation;
        float angle = Mathf.Atan2(GetPosition.x - tower.transform.position.x,
            GetPosition.y + tower.transform.position.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 2.0f);
    }
}