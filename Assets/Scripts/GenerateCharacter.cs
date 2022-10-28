using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCharacter : MonoBehaviour
{
    public Transform transformForlder;
    public void Generate(Transform gm)
    {
        int price = gm.GetComponent<Character>().price;
        if (GameManager.Instance.DecrementBudget(price))
        {
            Instantiate(gm, gm.transform.position, gm.transform.rotation, transformForlder);
        }
    }
}
