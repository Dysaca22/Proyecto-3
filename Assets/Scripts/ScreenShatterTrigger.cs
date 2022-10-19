using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShatterTrigger : MonoBehaviour
{

    [SerializeField]
    private Transform shatterTransform;

    public void PlayButton()
    {
        shatterTransform.gameObject.SetActive(true);
    }
}
