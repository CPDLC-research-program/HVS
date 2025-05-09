using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RightPanel : MonoBehaviour
{
    Image fieldHighlightBox;

    Image altField;
    Image vsField;
    Image iasField;
    Image hdgField;
    Image baroField;
    Image blocker;

    Global global;

    void checkBlocker()
    {
        if (global.highlightedField == -1) //aficher
        {
            blocker.rectTransform.anchoredPosition = new Vector3((float)904.7, (float)-279.7, 0);
        }
        else //cacher
        {
            blocker.rectTransform.anchoredPosition = new Vector3(10000, 10000, 0);
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

    public void changeButtonHighlight(bool shown, Image i = null)
    {
        if (shown)
        {
            fieldHighlightBox.rectTransform.anchoredPosition = 
                new Vector3(i.rectTransform.anchoredPosition.x, i.rectTransform.anchoredPosition.y);
            int result = global.toggleMode();

            //Pas pu toggle le mode
            if (result == -1)
            {
                //le sortir de l'�cran
                fieldHighlightBox.rectTransform.anchoredPosition = new Vector3(10000, 10000);
                global.highlightedField = -1;
            }
        }
        else
        {
            //le sortir de l'�cran
            fieldHighlightBox.rectTransform.anchoredPosition = new Vector3(10000, 10000);
        }
    }

    void OnMouseDown()
    {
        if (global.actionInProgress)
        {
            return;
        }

        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        if (global.highlightedField != -1)
        {
            return; //ne rien faire si un mode déjà choisi
        }

        if (isInside(altField, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 1;
            changeButtonHighlight(true, altField);
        }
        else if (isInside(vsField, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 2;
            changeButtonHighlight(true, vsField);
        }
        else if (isInside(iasField, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 0;
            changeButtonHighlight(true, iasField);
        }
        else if (isInside(hdgField, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 4;
            changeButtonHighlight(true, hdgField);
        }
        else if (isInside(baroField, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 3;
            changeButtonHighlight(true, baroField);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "TouchGroupedKeyboard" || SceneManager.GetActiveScene().name == "TouchGroupedSlider")
        {
            fieldHighlightBox = GameObject.Find("Canvas/Right Panel/Field Highlight Box").GetComponent<Image>();

            altField = GameObject.Find("Canvas/Right Panel/Alt Field Button").GetComponent<Image>();
            vsField = GameObject.Find("Canvas/Right Panel/Vs Field Button").GetComponent<Image>();
            iasField = GameObject.Find("Canvas/Right Panel/Ias Field Button").GetComponent<Image>();
            hdgField = GameObject.Find("Canvas/Right Panel/Hdg Field Button").GetComponent<Image>();
            baroField = GameObject.Find("Canvas/Right Panel/Baro Field Button").GetComponent<Image>();

            blocker = GameObject.Find("Canvas/Blocker").GetComponent<Image>();

            global = GameObject.Find("Global").GetComponent<Global>();

            //Mettre le highlight sur le field IAS
            changeButtonHighlight(false, iasField);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < 0.01f)
        {
            global.highlightedField = -1;
        }

        checkBlocker();
    }
}
