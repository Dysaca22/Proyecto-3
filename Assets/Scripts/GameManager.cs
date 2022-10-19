using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool is_play;

    [Header("Canva")]
    public int budget;
    public TMP_Text budgetText;


    private void Awake()
    {
        Instance = this;
        is_play = false;
        budget = 150;
        budgetText.SetText("$" + budget);
    }

    public void SetPlay()
    {
        is_play = true;
    }

    public void DecrementBudget(int priceObject)
    {
        budget -= priceObject;
        budgetText.SetText("$" + budget);
    }
}
