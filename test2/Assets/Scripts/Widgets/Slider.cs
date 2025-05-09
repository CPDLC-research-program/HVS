using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Slider : MonoBehaviour
{
    Image buttonPlus;
    Image buttonMinus;
    Image buttonEnter;
    Image handle;

    Text value;

    Text labelMin;
    Text labelMidMin;
    Text labelMid;
    Text labelMidMax;
    Text labelMax;

    float currentValue;
    int currentField;
    int lastField;

    Global global;
    DataManager dataManager;
    RightPanel panel;

    AltTape alt;
    Hdg hdg;
    BaroBox baro;
    VsTape vs;
    SpeedTape speed;

    float lastMouseX;
    bool handleClicked = false;

    float sliderLength = 393;
    float sliderStart = -194;

    float nextActionTime = 0.0f;
    float period = 0.08f;

    void writeScaleLabels() 
    {
        switch (currentField)
        {
            //IAS
            case 0:
                labelMin.text = "0";
                labelMidMin.text = "100";
                labelMid.text = "200";
                labelMidMax.text = "300";
                labelMax.text = "400";
                break;
            //ALT
            case 1:
                labelMin.text = "0";
                labelMidMin.text = "5k";
                labelMid.text = "10k";
                labelMidMax.text = "15k";
                labelMax.text = "20k";
                break;
            //VS
            case 2:
                labelMin.text = "-2k";
                labelMidMin.text = "-1k";
                labelMid.text = "0";
                labelMidMax.text = "1k";
                labelMax.text = "2k";
                break;
            //BARO
            case 3:
                labelMin.text = "28";
                labelMidMin.text = "29";
                labelMid.text = "30";
                labelMidMax.text = "31";
                labelMax.text = "32";
                break;
            //HDG
            case 4:
                labelMin.text = "1";
                labelMidMin.text = "90";
                labelMid.text = "180";
                labelMidMax.text = "270";
                labelMax.text = "360";
                break;
            //Rien de sélectionné
            case -1:
                labelMin.text = "";
                labelMidMin.text = "";
                labelMid.text = "";
                labelMidMax.text = "";
                labelMax.text = "";
                break;
        }
    }

    void resetDisplay()
    {
        value.text = 0.ToString();
    }

    void updateCurrentValue()
    {
        //-194 à 199 = 393
        switch (currentField)
        {
            //IAS
            case 0:
                currentValue = ((handle.rectTransform.anchoredPosition.x + 194) / 393) * 400;
                break;
            //ALT
            case 1:
                currentValue = ((handle.rectTransform.anchoredPosition.x + 194) / 393) * 20000;
                break;
            //VS
            case 2:
                currentValue = ((float)(handle.rectTransform.anchoredPosition.x + 194) / 393) * (float)4000 - 2000;
                break;
            //BARO
            case 3:
                currentValue = ((float)(handle.rectTransform.anchoredPosition.x + 194) / 393) * (float)4 + 28;
                break;
            //HDG
            case 4:
                currentValue = ((handle.rectTransform.anchoredPosition.x + 194) / 393) * 359 + 1;
                break;
        }
    }

    void fieldUpdate()
    {
        float x = 0;
        switch (currentField)
        {
            //IAS
            case 0:
                x = speed.currentSpeed / 400 * sliderLength + sliderStart;
                break;
            //ALT
            case 1:
                x = alt.currentAlt / 20000 * sliderLength + sliderStart;
                break;
            //VS
            case 2:
                x = (vs.currentVs + 2000) / 4000 * sliderLength + sliderStart;
                break;
            //BARO
            case 3:
                x = (baro.currentBaro - 28) / 4 * sliderLength + sliderStart;
                break;
            //HDG
            case 4:
                x = ((hdg.currentHdg == 0 ? 360 : hdg.currentHdg) - 1) / 359 * sliderLength + sliderStart;
                break;
        }

        handle.rectTransform.anchoredPosition = new Vector3(x, handle.rectTransform.anchoredPosition.y);
    }


    void updateValueDisplay()
    {
        //arrondir à la bonne unité
        //string valueString = string.Format(currentValue.ToString();

        switch (currentField)
        {
            //IAS
            case 0:
                value.text = Mathf.RoundToInt(currentValue).ToString();
                break;
            //ALT
            case 1:
                value.text = (Mathf.Floor((currentValue / 100) + (float)0.5) * 100).ToString();
                break;
            //VS
            case 2:
                value.text = (Mathf.Floor((currentValue / 100) + (float)0.5) * 100).ToString();
                break;
            //BARO
            case 3:
                value.text = currentValue.ToString("00.00");
                break;
            //HDG
            case 4:
                value.text = Mathf.RoundToInt(currentValue).ToString();
                break;
            //Rien
            case -1:
                value.text = 0.ToString();
                break;
        }

        //Debug.Log(float.Parse(value.text));
        if (global.highlightedField != -1)
        {
            dataManager.enterOvershootValue(float.Parse(value.text));
        }
    }

    //field: voir global
    //direction: -1 pour n?gatif, 1 pour positif
    //size: 0 pour petit, 1 pour grand
    public void incrementValue(int field, int direction, int size)
    {
        float increment = 0;
        switch (field)
        {
            //IAS
            case 0:
                increment = (size == 0 ? 1 : 10) * sliderLength / 400 * direction;
                break;
            //ALT
            case 1:
                increment = (size == 0 ? 100 : 1000) * sliderLength / 20000 * direction;
                break;
            //VS
            case 2:
                increment = (size == 0 ? 100 : 1000) * sliderLength / 4000 * direction;
                break;
            //BARO
            case 3:
                increment = (float)(size == 0 ? 0.01 : 0.1) * sliderLength / 4 * direction;
                break;
            //HDG
            case 4:
                increment = (size == 0 ? 1 : 10) * sliderLength / 359 * direction;
                break;
        }

        handle.rectTransform.anchoredPosition = new Vector3(Mathf.Clamp(handle.rectTransform.anchoredPosition.x + increment, -194, 199),
            handle.rectTransform.anchoredPosition.y);
    }

    void confirmNumber()
    {
        Field f;
        int code;

        switch (currentField)
        {
            case 0:
                f = speed;
                code = 0;
                break;
            case 1:
                f = alt;
                code = 1;
                break;
            case 2:
                f = vs;
                code = 2;
                break;
            case 3:
                f = baro;
                code = 3;
                break;
            default:
                f = hdg;
                code = 4;
                break;
        }

        //éditer la valeur target et revenir en mode normal
        f.editTarget(float.Parse(value.text), false);

        int result = f.confirmTarget();

        //échec
        if (result == 0)
        {
            ++dataManager.nbValError;

            //Remettre l'ancienne cible
            f.restoreOldTarget();

            //Feedback d'échec slider
            StartCoroutine(global.failure(6));

            //Feedback d'échec pfd
            StartCoroutine(global.failure(code));
        }
        //succès
        else
        {
            //Remettre le pfd en mode navigation
            f.toggleMode();

            global.highlightedField = -1;

            //Enlever le highlight du panneau de droite
            panel.changeButtonHighlight(false);

            //Feedback de succès slider
            StartCoroutine(global.success(6));

            //Feedback de succès pfd
            StartCoroutine(global.success(code));

            //Remettre le keypad à 0
            //resetDisplay();
        }
    }

    bool isInside(Image i, float x, float y)
    {
        //https://stackoverflow.com/questions/40566250/unity-recttransform-contains-point

        // Get the rectangular bounding box of your UI element
        Rect rect = i.rectTransform.rect;

        // Get the left, right, top, and bottom boundaries of the rect
        float leftSide = i.rectTransform.position.x - rect.width / 2;
        float rightSide = i.rectTransform.position.x + rect.width / 2;
        float topSide = i.rectTransform.position.y + rect.height / 2;
        float bottomSide = i.rectTransform.position.y - rect.height / 2;

        // Check to see if the point is in the calculated bounds
        if (x >= leftSide &&
            x <= rightSide &&
            y >= bottomSide &&
            y <= topSide)
        {
            return true;
        }
        return false;
    }

    void OnMouseDown()
    {
        if (global.actionInProgress)
        {
            return;
        }

        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        if (isInside(buttonPlus, mouseX, mouseY))
        {
            ++dataManager.nbInteractionVal;
            incrementValue(currentField, 1, 0);
        }
        else if (isInside(buttonMinus, mouseX, mouseY))
        {
            ++dataManager.nbInteractionVal;
            incrementValue(currentField, -1, 0);
        }
        else if (isInside(buttonEnter, mouseX, mouseY))
        {
            confirmNumber();
        }
        else if (isInside(handle, mouseX, mouseY))
        {
            ++dataManager.nbInteractionVal;
            lastMouseX = mouseX;
            handleClicked = true;
        }
    }

    void OnMouseDrag()
    {
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        if (handleClicked)
        {
            handle.rectTransform.anchoredPosition = new Vector3(Mathf.Clamp(handle.rectTransform.anchoredPosition.x + (mouseX - lastMouseX), -194, 199),
                handle.rectTransform.anchoredPosition.y);
            lastMouseX = mouseX;
        }
    }

    private void OnMouseUp()
    {
        handleClicked = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        buttonPlus = GameObject.Find("Canvas/Slider/Plus Button").GetComponent<Image>();
        buttonMinus = GameObject.Find("Canvas/Slider/Minus Button").GetComponent<Image>();
        buttonEnter = GameObject.Find("Canvas/Slider/Enter Button").GetComponent<Image>();
        handle = GameObject.Find("Canvas/Slider/Handle").GetComponent<Image>();

        value = GameObject.Find("Canvas/Slider/Slider Value").GetComponent<Text>();

        labelMin = GameObject.Find("Canvas/Slider/Label Min").GetComponent<Text>();
        labelMidMin = GameObject.Find("Canvas/Slider/Label MidMin").GetComponent<Text>();
        labelMid = GameObject.Find("Canvas/Slider/Label Mid").GetComponent<Text>();
        labelMidMax = GameObject.Find("Canvas/Slider/Label MidMax").GetComponent<Text>();
        labelMax = GameObject.Find("Canvas/Slider/Label Max").GetComponent<Text>();

        global = GameObject.Find("Global").GetComponent<Global>();
        dataManager = GameObject.Find("Global").GetComponent<DataManager>();
        panel = GameObject.Find("Canvas/Right Panel").GetComponent<RightPanel>();
        alt = GameObject.Find("Canvas/Alt Tape").GetComponent<AltTape>();
        vs = GameObject.Find("Canvas/Vs Tape").GetComponent<VsTape>();
        speed = GameObject.Find("Canvas/Speed Tape").GetComponent<SpeedTape>();
        hdg = GameObject.Find("Canvas/Heading").GetComponent<Hdg>();
        baro = GameObject.Find("Canvas/Baro").GetComponent<BaroBox>();

        resetDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        currentField = global.highlightedField;
        updateCurrentValue();

        if (lastField != currentField)
        {
            fieldUpdate();
            writeScaleLabels();
            lastField = currentField;
        }

        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            updateValueDisplay();
        }
    }
}
