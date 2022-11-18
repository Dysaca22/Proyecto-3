using System.Collections;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using Random = UnityEngine.Random;


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
    private GameObject[] troops;

    [Header("Generation Towers")]
    public int budget_towers;
    public Transform transformForlderTowers;
    private GameObject[] towers;

    [Header("Borders")]
    public Transform LimitTop;
    public Transform LimitBot;
    public Transform LimitLeft;
    public Transform LimitRight;
    public Transform LimitTroops;
    public Transform LimitTowers;

    private float timeWait = 0.3f;
    private float period = 0.0f;
    private List<string[]> itemsResults = new List<string[]> { };
    private int count = 0;
    private conf troopsconf;

    public class conf
    {
        public int amount;
        public int[] category;
        public float[] positionX;
        public float[] positionY;
    }

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
            GameObject.FindGameObjectsWithTag("Torre Fuente de Poder").Length == 0)
        {
            is_play = false;
            if (transformForlderTowers.transform.childCount == 0 ||
            GameObject.FindGameObjectsWithTag("Torre Fuente de Poder").Length == 0)
            {
                count++;
                background.transform.GetComponent<SpriteRenderer>().color = Color.green;
                foreach (string[] item in itemsResults)
                {
                    CSVManager.AppendToReport
                    (
                        new string[5]
                        {
                        count.ToString(),
                        item[0],
                        item[1],
                        item[2],
                        item[3]
                        }
                    );
                }
            }
            else
            {
                background.transform.GetComponent<SpriteRenderer>().color = Color.red;
            }
            
            itemsResults = new List<string[]> { };
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

            for (int i = 1; i < transformForlderTroops.transform.childCount; ++i)
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
        Request();

        for (int i = 0; i < troopsconf.amount; i++)
        {
            int category = 2;
            Debug.Log(troopsconf.category[i]);
            if (troopsconf.category[i] == 1)
            {
                category = 0;
            }
            else
            {
                if (troopsconf.category[i] == 2)
                {
                    category = 1;
                }
            }
            Instantiate(troops[category], new Vector3(troopsconf.positionX[i], troopsconf.positionY[i], 100f),
                troops[category].transform.rotation, transformForlderTroops);
        }

        /*
        Instantiate(troops[0], new Vector3(4.2427f, 1.4414f, 100f), troops[0].transform.rotation, transformForlderTroops);
        Instantiate(troops[0], new Vector3(5.3442f, 1.3864f, 100f), troops[0].transform.rotation, transformForlderTroops);
        Instantiate(troops[0], new Vector3(5.2857f, 1.4463f, 100f), troops[0].transform.rotation, transformForlderTroops);
        Instantiate(troops[0], new Vector3(4.7961f, 1.5685f, 100f), troops[0].transform.rotation, transformForlderTroops);
        Instantiate(troops[0], new Vector3(4.2422f, 1.6046f, 100f), troops[0].transform.rotation, transformForlderTroops);
        Instantiate(troops[0], new Vector3(4.4583f, 1.5053f, 100f), troops[0].transform.rotation, transformForlderTroops);
        Instantiate(troops[0], new Vector3(4.2048f, 1.3445f, 100f), troops[0].transform.rotation, transformForlderTroops);
        */
        /*
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
                Vector3 coordenates = new Vector3(Random.Range(LimitLeft.position.x + 0.4f, LimitRight.position.x - 0.4f),
                    Random.Range(LimitBot.position.y + 0.4f, LimitTroops.position.y), 100);
                while (!AstarPath.active.GetNearest(coordenates).node.Walkable)
                {
                    coordenates = new Vector3(Random.Range(LimitLeft.position.x + 0.4f, LimitRight.position.x - 0.4f),
                    Random.Range(LimitBot.position.y + 0.4f, LimitTroops.position.y), 100);
                }
                Instantiate(troops[randomTroop], coordenates, troops[randomTroop].transform.rotation, transformForlderTroops);
                string tag = "0";
                if (troops[randomTroop].tag == "Infanteria Pesada")
                {
                    tag = "1";
                }else if (troops[randomTroop].tag == "Infanteria Killer")
                {
                    tag = "2";
                }
                string[] aux = new string[] { "0", tag, coordenates.x.ToString(), coordenates.y.ToString() };
                itemsResults.Add(aux);
                AstarPath.active.Scan();
            }
        }*/
    }

    List<string> towersInfo = new List<string> { };

    private void RandomTowersGenerate()
    {
        towersInfo = new List<string> { };
        Instantiate(towers[0], new Vector3(LimitTop.position.x, LimitTop.position.y - 0.7f, 100), towers[0].transform.rotation, transformForlderTowers);
        AstarPath.active.Scan();
        towers = towers.Where(val => val.name != "Torre de Poder").ToArray();

        /*
        Instantiate(towers[1], new Vector3(1.33f, 7.63f, 100), towers[1].transform.rotation, transformForlderTowers);
        Instantiate(towers[1], new Vector3(2.82f, 7.63f, 100), towers[1].transform.rotation, transformForlderTowers);
        Instantiate(towers[1], new Vector3(4f, 7.63f, 100), towers[1].transform.rotation, transformForlderTowers);
        Instantiate(towers[1], new Vector3(5.33f, 7.63f, 100), towers[1].transform.rotation, transformForlderTowers);
        Instantiate(towers[1], new Vector3(7.33f, 7.63f, 100), towers[1].transform.rotation, transformForlderTowers);
        Instantiate(towers[1], new Vector3(4.63f, 6f, 100), towers[1].transform.rotation, transformForlderTowers);
        AstarPath.active.Scan();
        StartCoroutine(Upload());
        */

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
            int randomTorre = Random.Range(0, towers.Length);
            if (DecrementBudgetTower(towers[randomTorre].transform.GetComponent<Tower>().price))
            {
                Vector3 coordenates = new Vector3(Random.Range(LimitLeft.position.x + 0.7f, LimitRight.position.x - 0.7f),
                    Random.Range(LimitTowers.position.y, LimitTop.position.y - 0.7f), 100);
                while (!AstarPath.active.GetNearest(coordenates).node.Walkable ||
                    !AstarPath.active.GetNearest(coordenates + new Vector3(0.5f, 0.7f, 0)).node.Walkable ||
                    !AstarPath.active.GetNearest(coordenates + new Vector3(-0.5f, -0.7f, 0)).node.Walkable ||
                    !AstarPath.active.GetNearest(coordenates + new Vector3(-0.5f, 0.7f, 0)).node.Walkable ||
                    !AstarPath.active.GetNearest(coordenates + new Vector3(0.5f, -0.7f, 0)).node.Walkable)
                {
                    coordenates = new Vector3(Random.Range(LimitLeft.position.x + 0.7f, LimitRight.position.x - 0.7f),
                    Random.Range(LimitTowers.position.y, LimitTop.position.y - 0.7f), 100);
                }
                Instantiate(towers[randomTorre], coordenates, towers[randomTorre].transform.rotation, transformForlderTowers);
                string tag = "0";
                if (towers[randomTorre].tag == "Torre Ligera")
                {
                    tag = "1";
                }
                else if (towers[randomTorre].tag == "Torre Pesada")
                {
                    tag = "2";
                }
                
                string[] aux = new string[] { "1", tag, coordenates.x.ToString(), coordenates.y.ToString() };
                itemsResults.Add(aux);
                towersInfo.Add("{\"info\": [" + string.Join(",", new string[] { tag, coordenates.x.ToString(), coordenates.y.ToString() }) + "]}");
                AstarPath.active.Scan();
            }
        }
        Debug.Log("Wait...");
    }


    public void Request()
    {
        try
        {
            string url = "http://localhost:8000/api/ML/get_config";
            WWWForm form = new WWWForm();
            form.AddField("amount", 6);
            form.AddField("towers", string.Join(",", towersInfo.ToArray()));
            var request = UnityWebRequest.Post(url, form);
            IEnumerator s = onResponse(request);
            while (s.MoveNext()) ;
        }
        catch (Exception e) { Debug.Log("ERROR : " + e.Message); }
    }
    IEnumerator onResponse(UnityWebRequest req)
    {
        yield return req.SendWebRequest();

        while (!req.isDone)
            yield return true;

        if (req.isNetworkError)
            Debug.Log("Network error has occured: " + req.GetResponseHeader(""));
        else
            Debug.Log("Success " + req.downloadHandler.text);
        Debug.Log("Second Success");
        // Some code after success
        troopsconf = JsonUtility.FromJson<conf>(req.downloadHandler.text);
        
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
