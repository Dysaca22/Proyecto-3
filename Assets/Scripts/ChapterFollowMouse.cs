using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterFollowMouse : MonoBehaviour
{

    private void Update()
    {
        if (GameManager.Instance.is_play) return;

        if (Input.GetMouseButtonDown(0)) return;

        if (!Input.GetMouseButton(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            if (hit.collider.name.Substring(0, 9) == "Character") 
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (pos.y <= -3)
                {
                    Vector3 move = new Vector3(pos.x, pos.y, 0);
                    hit.transform.gameObject.transform.position = move;
                }
            }
        }
    }
}
