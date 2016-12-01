using UnityEngine;
using System.Collections;
using System.Linq;
using Foundation.Databinding;
using UnityEngine.SceneManagement;
using System;

public class ReplaySelectViewModel : ObservableBehaviour
{
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

}
