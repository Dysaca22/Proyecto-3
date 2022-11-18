using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Principal")]
    public float life;
    public int price;
    public float speed;
    public float range;
    [Header("Attack")]
    public float damageAttack;
    public float timeAttack;
    [Header("Shot")]
    public GameObject shot;
    public float attackSpeed;
    [Header("Tower")]
    private GameObject tower;
    private Vector3 GetPosition => transform.position;
    private Quaternion lastRotation;
    private float period = 0.0f;

    private void Awake()
    {
        transform.GetComponent<AIPath>().maxSpeed = speed;
        FindTower();
        transform.GetComponent<AIPath>().canMove = false;
        transform.GetComponent<AIDestinationSetter>().target = tower.transform.GetChild(0).gameObject.transform;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.is_play)
        {
            FindTower();
            if (tower != null)
            {
                transform.GetComponent<AIDestinationSetter>().target = tower.transform.GetChild(0).gameObject.transform;
                if (Vector2.Distance(tower.transform.position, GetPosition) < range)
                {
                    transform.GetComponent<AIPath>().canMove = false;
                    Rotate();
                    if (lastRotation == transform.rotation)
                    {
                        if (period > timeAttack)
                        {
                            Attack();
                            period = 0;
                        }
                        period += Time.fixedDeltaTime;
                    }
                }
                else
                {
                    transform.GetComponent<AIPath>().canMove = true;
                }
            }
            else
            {
                transform.GetComponent<AIPath>().canMove = false;
                transform.GetComponent<AIDestinationSetter>().target = null;
            }
        }
    }

    private void FindTower()
    {
        GameObject[] allTowers = { };
        if (gameObject.tag != "Infanteria Killer")
        {
            GameObject[] allTowers1 = GameObject.FindGameObjectsWithTag("Torre Ligera");
            allTowers = allTowers.Concat(allTowers1).ToArray();
            GameObject[] allTowers2 = GameObject.FindGameObjectsWithTag("Torre Pesada");
            allTowers = allTowers.Concat(allTowers2).ToArray();
        }
        GameObject[] allTowers3 = GameObject.FindGameObjectsWithTag("Torre Fuente de Poder");
        allTowers = allTowers.Concat(allTowers3).ToArray();
        float minDistance = 9999f;
        foreach (GameObject tower in allTowers)
        {
            if (Vector2.Distance(tower.transform.position, transform.position) < minDistance)
            {
                minDistance = Vector2.Distance(tower.transform.position, transform.position);
                this.tower = tower;
            }
        }
    }


    public void Attack()
    {
        if (range > 1)
        {
            GameObject firePoint = transform.GetChild(0).gameObject;
            Quaternion rotation = transform.rotation * Quaternion.Inverse(shot.transform.rotation);
            GameObject InstanceShot = Instantiate(shot, firePoint.transform.position, rotation);
            InstanceShot.GetComponent<Rigidbody2D>().velocity = transform.up.normalized * attackSpeed;
            InstanceShot.GetComponent<Shot>().damage = damageAttack;
            Destroy(InstanceShot, 3);
        }
        else
        {
            tower.transform.GetComponent<Tower>().DecrementLife(damageAttack);
        }
    }

    private void Rotate()
    {
        lastRotation = transform.rotation;
        Vector3 diff = (tower.transform.GetChild(1).gameObject.transform.position - transform.position);
        float angle = Mathf.Atan2(diff.y, diff.x);
        transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg - 90);
        /*
        lastRotation = transform.rotation;
        GameObject firePoint = transform.GetChild(0).gameObject;
        Vector3 vectorToTarget = tower.transform.position - firePoint.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10f);
        */
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

    public bool onCollision = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        onCollision = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        onCollision = false;
    }
}