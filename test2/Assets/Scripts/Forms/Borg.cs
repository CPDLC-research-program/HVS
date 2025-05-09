using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Borg : MonoBehaviour
{
    DataManagerBorg dataManager;

    UnityEngine.UI.Slider sliderEpaule;
    UnityEngine.UI.Slider sliderCou;
    UnityEngine.UI.Slider sliderHautDos;
    UnityEngine.UI.Slider sliderMain;
    UnityEngine.UI.Slider sliderBasDos;
    UnityEngine.UI.Slider sliderBras;

    float convertValue(float value)
    {
        switch (value)
        {
            case 0:
                return 0;
            case 1:
                return (float)0.3;
            case 2:
                return (float)0.5;
            case 3:
                return (float)0.7;
            case 4:
                return (float)1;
            case 5:
                return (float)1.5;
            case 6:
                return (float)2;
            case 7:
                return (float)2.5;
            case 8:
                return (float)3;
            case 9:
                return (float)4;
            case 10:
                return (float)5;
            case 11:
                return (float)6;
            case 12:
                return (float)7;
            case 13:
                return (float)8;
            case 14:
                return (float)9;
            case 15:
                return (float)10;
        }

        return -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        sliderEpaule = GameObject.Find("Canvas/Epaule/Slider epaule").GetComponent<UnityEngine.UI.Slider>();
        sliderCou = GameObject.Find("Canvas/Cou/Slider cou").GetComponent<UnityEngine.UI.Slider>();
        sliderHautDos = GameObject.Find("Canvas/Haut dos/Slider haut dos").GetComponent<UnityEngine.UI.Slider>();
        sliderMain = GameObject.Find("Canvas/Main/Slider main").GetComponent<UnityEngine.UI.Slider>();
        sliderBasDos = GameObject.Find("Canvas/Bas dos/Slider bas dos").GetComponent<UnityEngine.UI.Slider>();
        sliderBras = GameObject.Find("Canvas/Bras/Slider bras").GetComponent<UnityEngine.UI.Slider>();

        dataManager = GameObject.Find("Code").GetComponent<DataManagerBorg>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            dataManager.shoulder = convertValue(sliderEpaule.value);
            dataManager.neck = convertValue(sliderCou.value);
            dataManager.upperBack = convertValue(sliderHautDos.value);
            dataManager.lowerBack = convertValue(sliderBasDos.value);
            dataManager.arm = convertValue(sliderBras.value);
            dataManager.hand = convertValue(sliderMain.value);
        }
    }
}
