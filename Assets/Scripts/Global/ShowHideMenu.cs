using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideMenu : MonoBehaviour
{
    public GameObject showObject, hideObject;

    public void ShowMenu()
    {
        showObject.SetActive(true);
        hideObject.SetActive(false);
    }

}
