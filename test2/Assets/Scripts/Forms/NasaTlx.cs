using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NasaTlx : MonoBehaviour
{
    DataManagerNasaTlx dataManager;

    UnityEngine.UI.Slider sliderMentale;
    UnityEngine.UI.Slider sliderPhysique;
    UnityEngine.UI.Slider sliderTemporelle;
    UnityEngine.UI.Slider sliderPerformance;
    UnityEngine.UI.Slider sliderEffort;
    UnityEngine.UI.Slider sliderFrustration;

    // Start is called before the first frame update
    void Start()
    {
        sliderMentale = GameObject.Find("Canvas/Ex mentale/Slider mentale").GetComponent<UnityEngine.UI.Slider>();
        sliderPhysique = GameObject.Find("Canvas/Ex physique/Slider physique").GetComponent<UnityEngine.UI.Slider>();
        sliderTemporelle = GameObject.Find("Canvas/Ex temporelle/Slider temporelle").GetComponent<UnityEngine.UI.Slider>();
        sliderPerformance = GameObject.Find("Canvas/Performance/Slider performance").GetComponent<UnityEngine.UI.Slider>();
        sliderEffort = GameObject.Find("Canvas/Effort/Slider effort").GetComponent<UnityEngine.UI.Slider>();
        sliderFrustration = GameObject.Find("Canvas/Frustration/Slider frustration").GetComponent<UnityEngine.UI.Slider>();

        dataManager = GameObject.Find("Code").GetComponent<DataManagerNasaTlx>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            dataManager.mental = sliderMentale.value * 20;
            dataManager.physical = sliderPhysique.value * 20;
            dataManager.temporal = sliderTemporelle.value * 20;
            dataManager.performance = sliderPerformance.value * 20;
            dataManager.effort = sliderEffort.value * 20;
            dataManager.frustration = sliderFrustration.value * 20;
        }
    }
}
