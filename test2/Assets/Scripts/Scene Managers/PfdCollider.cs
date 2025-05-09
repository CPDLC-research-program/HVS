using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PfdCollider : MonoBehaviour
{
    Image speedTargetBox;
    Image speedBg;
    Image altTargetBox;
    Image altBg;
    Image vsTargetBox;
    Image vsBg;
    Image hdgTargetBox;
    Image hdgBg;
    Image baroTargetBox;

    Global global;

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

    private void OnMouseDown()
    {
        if (global.actionInProgress)
        {
            return;
        }

        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        if (isInside(altTargetBox, mouseX, mouseY) || isInside(altBg, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 1;

            int result = global.toggleMode();
            if (result != -1)
            {
                global.toggleKeypadVisibility(true);
            }
            else
            {
                global.highlightedField = -1;
            }
        }
        else if (isInside(speedTargetBox, mouseX, mouseY) || isInside(speedBg, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 0;

            int result = global.toggleMode();
            if (result != -1)
            {
                global.toggleKeypadVisibility(true);
            }
            else
            {
                global.highlightedField = -1;
            }
        }
        else if (isInside(vsTargetBox, mouseX, mouseY) || isInside(vsBg, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 2;

            int result = global.toggleMode();
            if (result != -1)
            {
                global.toggleKeypadVisibility(true);
            }
            else
            {
                global.highlightedField = -1;
            }
        }
        else if (isInside(baroTargetBox, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 3;

            int result = global.toggleMode();
            if (result != -1)
            {
                global.toggleKeypadVisibility(true);
            }
            else
            {
                global.highlightedField = -1;
            }
        }
        else if (isInside(hdgTargetBox, mouseX, mouseY) || isInside(hdgBg, mouseX, mouseY))
        {
            global.resetPfdModes();
            global.highlightedField = 4;

            int result = global.toggleMode();
            if (result != -1)
            {
                global.toggleKeypadVisibility(true);
            }
            else
            {
                global.highlightedField = -1;
            }
        }
    }

    void Start()
    {
        speedTargetBox = GameObject.Find("Canvas/Speed Tape/Speed Target Box").GetComponent<Image>();
        speedBg = GameObject.Find("Canvas/Speed Tape/SpeedBg").GetComponent<Image>();
        altTargetBox = GameObject.Find("Canvas/Alt Tape/Alt Target Box").GetComponent<Image>();
        altBg = GameObject.Find("Canvas/Alt Tape/AltBg").GetComponent<Image>();
        vsTargetBox = GameObject.Find("Canvas/Vs Tape/Vs Target Box").GetComponent<Image>();
        vsBg = GameObject.Find("Canvas/Vs Tape/VsBg").GetComponent<Image>();
        hdgTargetBox = GameObject.Find("Canvas/Heading/Hdg Target Box").GetComponent<Image>();
        hdgBg = GameObject.Find("Canvas/Heading/Moving").GetComponent<Image>();
        baroTargetBox = GameObject.Find("Canvas/Baro/Baro Box").GetComponent<Image>();

        global = GameObject.Find("Global").GetComponent<Global>();

        global.highlightedField = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < 0.01f)
        {
            global.highlightedField = -1;
            global.toggleKeypadVisibility(false);
        }
    }
}
