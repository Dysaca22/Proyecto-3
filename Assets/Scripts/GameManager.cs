using System.Collections;
using UnityEngine;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool is_play;

    [Header("Canva")]
    public int budget;
    public TMP_Text budgetText;
    public GameObject background;

    [Header("Generation Troops")]
    public Transform transformForlderTroops;
    private Vector4 limits_Troops = new Vector4(2.2f/*right*/, -6.3f/*left*/, -3f/*top*/, -4.8f/*bot*/);
    private GameObject[] troops;

    [Header("Generation Towers")]
    public int budget_towers;
    private GameObject[] towers;
    private Vector4 limits_towers = new Vector4(1.8f/*right*/, -6f/*left*/, 4f/*top*/, 1f/*bot*/);
    public Transform transformForlderTowers;

    private float timeWait = 0.3f;
    private float period = 0.0f;

    private void Awake()
    {
        Instance = this;
        is_play = false;
        budget_towers = 150;
        budget = 150;
        budgetText.SetText("$" + budget);
        loadObjectsFolder("Sprites/Towers/Prefabs", ref towers);
        loadObjectsFolder("Sprites/Troops/Prefabs", ref troops);
        RandomTowersGenerate();
        RandomTroopsGenerate();
    }

    private void Update()
    {
        if (period > timeWait)
        {
            AstarPath.active.Scan();
            period = 0;
        }
        period += Time.fixedDeltaTime;
        if (transformForlderTroops.transform.childCount == 1 ||
            transformForlderTowers.transform.childCount == 0 ||
            GameObject.FindGameObjectsWithTag("Torre Fuente de Poder").Length == 0)
        {
            is_play = false;
            if (transformForlderTowers.transform.childCount == 0 ||
            GameObject.FindGameObjectsWithTag("Torre Fuente de Poder").Length == 0)
            {
                background.transform.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                background.transform.GetComponent<SpriteRenderer>().color = Color.red;
            }
            GameObject[] allShots = { };
            allShots = allShots.Concat(GameObject.FindGameObjectsWithTag("Shot tower")).ToArray();
            allShots = allShots.Concat(GameObject.FindGameObjectsWithTag("Shot enemy")).ToArray();
            foreach(GameObject shot in allShots)
            {
                Destroy(shot);
            }

            foreach (Transform children in transformForlderTowers.transform)
            {
                Destroy(children.gameObject);
            }

            for (int i = 0; i < transformForlderTroops.transform.childCount; ++i)
            {
                Destroy(transformForlderTroops.transform.GetChild(i).gameObject);
            }

            budget_towers = 150;
            budget = 150;
            budgetText.SetText("$" + budget);
            loadObjectsFolder("Sprites/Towers/Prefabs", ref towers);
            loadObjectsFolder("Sprites/Troops/Prefabs", ref troops);
            RandomTowersGenerate();
            RandomTroopsGenerate();
            StartCoroutine(waitForStart());
        }
    }

    public void SetPlay()
    {
        is_play = true;
    }

    public bool DecrementBudget(int priceObject)
    {
        if (budget - priceObject >= 0)
        {
            budget -= priceObject;
            budgetText.SetText("$" + budget);
            return true;
        }
        return false;
    }

    public bool DecrementBudgetTower(int priceObject)
    {
        if (budget_towers - priceObject >= 0)
        {
            budget_towers -= priceObject;
            return true;
        }
        return false;
    }

    private void RandomTroopsGenerate()
    {
        int minCost = 9999;
        for (int i = 0; i < troops.Length; i++)
        {
            if (troops[i].transform.GetComponent<Character>().price < minCost)
            {
                minCost = troops[i].transform.GetComponent<Character>().price;
            }
        }

        while(budget >= minCost)
        {
            int randomTroop = Random.Range(0, troops.Length);
            if (DecrementBudget(troops[randomTroop].transform.GetComponent<Character>().price))
            {
                Vector2 coordenates = new Vector2(Random.Range(limits_Troops[1], limits_Troops[0]),
                    Random.Range(limits_Troops[2], limits_Troops[3]));
                while (!AstarPath.active.GetNearest(coordenates).node.Walkable)
                {
                    coordenates = new Vector2(Random.Range(limits_Troops[1], limits_Troops[0]),
                    Random.Range(limits_Troops[3], limits_Troops[2]));
                }
                Instantiate(troops[randomTroop], coordenates, troops[randomTroop].transform.rotation, transformForlderTroops);
                AstarPath.active.Scan();
            }
        }
    }

    private void RandomTowersGenerate()
    {
        Instantiate(towers[0], new Vector2(-2f, 4f), towers[0].transform.rotation, transformForlderTowers);
        AstarPath.active.Scan();
        towers = towers.Where(val => val.name != "Torre de Poder").ToArray();

        int minCost = 9999;
        for (int i = 0; i < towers.Length; i++)
        {
            if (towers[i].transform.GetComponent<Tower>().price < minCost)
            {
                minCost = towers[i].transform.GetComponent<Tower>().price;
            }
        }

        while (budget_towers >= minCost)
        {
            int randomTroop = Random.Range(0, towers.Length);
            if (DecrementBudgetTower(towers[randomTroop].transform.GetComponent<Tower>().price))
            {
                Vector2 coordenates = new Vector2(Random.Range(limits_towers[1], limits_towers[0]),
                    Random.Range(limits_towers[3], limits_towers[2]));
                while (!AstarPath.active.GetNearest(coordenates).node.Walkable ||
                    !AstarPath.active.GetNearest(coordenates + new Vector2(0.5f, 0.7f)).node.Walkable ||
                    !AstarPath.active.GetNearest(coordenates + new Vector2(-0.5f, -0.7f)).node.Walkable ||
                    !AstarPath.active.GetNearest(coordenates + new Vector2(-0.5f, 0.7f)).node.Walkable ||
                    !AstarPath.active.GetNearest(coordenates + new Vector2(0.5f, -0.7f)).node.Walkable)
                {
                    coordenates = new Vector2(Random.Range(limits_towers[1], limits_towers[0]),
                    Random.Range(limits_towers[3], limits_towers[2]));
                }
                Instantiate(towers[randomTroop], coordenates, towers[randomTroop].transform.rotation, transformForlderTowers);
                AstarPath.active.Scan();
            }
        }
    }

    void loadObjectsFolder(string folder, ref GameObject[] dest)
    {
        object[] loaded = Resources.LoadAll(folder, typeof(GameObject));
        dest = new GameObject[loaded.Length];
        for (int i = 0; i < loaded.Length; i++)
        {
            dest[i] = (GameObject)loaded[i];
        }
    }

    IEnumerator waitForStart()
    {
        yield return new WaitForSeconds(2);
        background.transform.GetComponent<SpriteRenderer>().color = Color.white;
        SetPlay();
    }
}
