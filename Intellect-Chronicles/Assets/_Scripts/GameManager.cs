    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject DialogPanel;

    private void Start()
    {
        DialogPanel.SetActive(false);   
    }

    int stage = 0;
    public void NextObjective()
    {
        switch (++stage)
        {
            case 1:
                PauseGane();
                DialogPanel.SetActive(true);
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
    }

    private void PauseGane()
    {
        Object[] objects = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in objects)
        {
            go.SendMessage("OnPauseGame", SendMessageOptions.DontRequireReceiver);
        }
    }
}
