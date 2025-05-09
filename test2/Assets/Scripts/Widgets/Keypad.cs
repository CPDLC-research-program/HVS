using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Keypad : MonoBehaviour
{
    Image button1;
    Image button2;
    Image button3;
    Image button4;
    Image button5;
    Image button6;
    Image button7;
    Image button8;
    Image button9;
    Image button0;
    Image buttonPlusMinus;
    Image buttonEnter;

    Text digit1;
    Text digit2;
    Text digit3;
    Text digit4;
    Text digit5;
    Text comma;
    Text minus;

    Text currentDigitInd;

    int currentDigit;
    int currentField;
    int oldField;

    Global global;
    AltTape alt;
    Hdg hdg;
    BaroBox baro;
    VsTape vs;
    SpeedTape speed;

    Color lightGrey;
    Color darkGrey;

    void setCurrentDigitInd()
    {
        currentDigitInd.rectTransform.anchoredPosition = new Vector3(
            GameObject.Find("Canvas/Keypad/Digit " + currentDigit.ToString()).GetComponent<Text>().rectTransform.anchoredPosition.x,
            currentDigitInd.rectTransform.anchoredPosition.y);
    }

    void setDisplayMode()
    {
        //IAS
        if (currentField == 0)
        {
            digit1.color = lightGrey;
            digit2.color = lightGrey;
            comma.color = lightGrey;
            minus.color = lightGrey;
            currentDigit = 3;
        }
        //ALT
        else if (currentField == 1)
        {
            digit1.color = darkGrey;
            digit2.color = darkGrey;
            comma.color = lightGrey;
            minus.color = lightGrey;
            currentDigit = 1;
        }
        //VS
        else if (currentField == 2)
        {
            digit1.color = lightGrey;
            digit2.color = darkGrey;
            comma.color = lightGrey;
            minus.color = lightGrey;
            currentDigit = 2;
        }
        //Baro
        else if (currentField == 3)
        {
            digit1.color = lightGrey;
            digit2.color = darkGrey;
            comma.color = darkGrey;
            minus.color = lightGrey;
            currentDigit = 2;
        }
        //HDG
        else
        {
            digit1.color = lightGrey;
            digit2.color = lightGrey;
            comma.color = lightGrey;
            minus.color = lightGrey;
            currentDigit = 3;
        }

        digit1.text = 0.ToString();
        digit2.text = 0.ToString();
        digit3.text = 0.ToString();
        digit4.text = 0.ToString();
        digit5.text = 0.ToString();
    }

    void incrementDigit()
    {
        if (currentDigit == 5)
        {
            //IAS
            if (currentField == 0)
            {
                currentDigit = 3;
            }
            //ALT
            else if (currentField == 1)
            {
                currentDigit = 1;
            }
            //VS
            else if (currentField == 2)
            {
                currentDigit = 2;
            }
            //Baro
            else if (currentField == 3)
            {
                currentDigit = 2;
            }
            //HDG
            else
            {
                currentDigit = 3;
            }
        }
        else
        {
            currentDigit = currentDigit + 1;
        }
    }

    void enterDigit(int digit)
    {
        GameObject.Find("Canvas/Keypad/Digit " + currentDigit.ToString()).GetComponent<Text>().text = digit.ToString();
        incrementDigit();
    }

    void confirmNumber()
    {
        Field f;

        switch (currentField)
        {
            case 0: 
                f = speed;
                break;
            case 1:
                f = alt;
                break;
            case 2:
                f = vs;
                break;
            case 3:
                f = baro;
                break;
            default:
                f = hdg;
                break;
        }

        float value;

        //Si c'est pour le baro
        if (currentField == 3)
        {
            value = int.Parse(digit1.text) * 100 + int.Parse(digit2.text) * 10 + int.Parse(digit3.text) * 1 + int.Parse(digit4.text) * (float)0.1
            + int.Parse(digit5.text) * (float)0.01;
        }
        else
        {
            value = int.Parse(digit1.text) * 10000 + int.Parse(digit2.text) * 1000 + int.Parse(digit3.text) * 100 + int.Parse(digit4.text) * 10
            + int.Parse(digit5.text);
        }
        
        //Si le signe négatif est affiché
        if(minus.color == darkGrey)
        {
            value = value * -1;
        }

        //éditer la valeur target et revenir en mode normal
        f.editTarget(value);
        //f.toggleMode3();

        //réinitialiser l'affichage avec des 0
        setDisplayMode();
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
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        if (isInside(button0, mouseX, mouseY))
        {
            enterDigit(0);
        }
        else if (isInside(button1, mouseX, mouseY))
        {
            enterDigit(1);
        }
        else if (isInside(button2, mouseX, mouseY))
        {
            enterDigit(2);
        }
        else if (isInside(button3, mouseX, mouseY))
        {
            enterDigit(3);
        }
        else if (isInside(button4, mouseX, mouseY))
        {
            enterDigit(4);
        }
        else if (isInside(button5, mouseX, mouseY))
        {
            enterDigit(5);
        }
        else if (isInside(button6, mouseX, mouseY))
        {
            enterDigit(6);
        }
        else if (isInside(button7, mouseX, mouseY))
        {
            enterDigit(7);
        }
        else if (isInside(button8, mouseX, mouseY))
        {
            enterDigit(8);
        }
        else if (isInside(button9, mouseX, mouseY))
        {
            enterDigit(9);
        }
        else if (isInside(buttonPlusMinus, mouseX, mouseY))
        {
            if (minus.color == lightGrey)
            {
                minus.color = darkGrey;
            }
            else
            {
                minus.color = lightGrey;
            }
        }
        else if (isInside(buttonEnter, mouseX, mouseY))
        {
            confirmNumber();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        button0 = GameObject.Find("Canvas/Keypad/Keypad 0 Button").GetComponent<Image>();
        button1 = GameObject.Find("Canvas/Keypad/Keypad 1 Button").GetComponent<Image>();
        button2 = GameObject.Find("Canvas/Keypad/Keypad 2 Button").GetComponent<Image>();
        button3 = GameObject.Find("Canvas/Keypad/Keypad 3 Button").GetComponent<Image>();
        button4 = GameObject.Find("Canvas/Keypad/Keypad 4 Button").GetComponent<Image>();
        button5 = GameObject.Find("Canvas/Keypad/Keypad 5 Button").GetComponent<Image>();
        button6 = GameObject.Find("Canvas/Keypad/Keypad 6 Button").GetComponent<Image>();
        button7 = GameObject.Find("Canvas/Keypad/Keypad 7 Button").GetComponent<Image>();
        button8 = GameObject.Find("Canvas/Keypad/Keypad 8 Button").GetComponent<Image>();
        button9 = GameObject.Find("Canvas/Keypad/Keypad 9 Button").GetComponent<Image>();
        buttonPlusMinus = GameObject.Find("Canvas/Keypad/Keypad PlusMinus Button").GetComponent<Image>();
        buttonEnter = GameObject.Find("Canvas/Keypad/Keypad enter Button").GetComponent<Image>();

        digit1 = GameObject.Find("Canvas/Keypad/Digit 1").GetComponent<Text>();
        digit2 = GameObject.Find("Canvas/Keypad/Digit 2").GetComponent<Text>();
        digit3 = GameObject.Find("Canvas/Keypad/Digit 3").GetComponent<Text>();
        digit4 = GameObject.Find("Canvas/Keypad/Digit 4").GetComponent<Text>();
        digit5 = GameObject.Find("Canvas/Keypad/Digit 5").GetComponent<Text>();
        comma = GameObject.Find("Canvas/Keypad/Comma").GetComponent<Text>();
        minus = GameObject.Find("Canvas/Keypad/Minus").GetComponent<Text>();

        currentDigitInd = GameObject.Find("Canvas/Keypad/Current Digit").GetComponent<Text>();

        global = GameObject.Find("Global").GetComponent<Global>();
        alt = GameObject.Find("Canvas/Alt Tape").GetComponent<AltTape>();
        vs = GameObject.Find("Canvas/Vs Tape").GetComponent<VsTape>();
        speed = GameObject.Find("Canvas/Speed Tape").GetComponent<SpeedTape>();
        hdg = GameObject.Find("Canvas/Heading").GetComponent<Hdg>();
        baro = GameObject.Find("Canvas/Baro").GetComponent<BaroBox>();


        lightGrey = new Color(195.0f / 255.0f, 195.0f / 255.0f, 195.0f / 255.0f);
        darkGrey = new Color(56.0f / 255.0f, 56.0f / 255.0f, 56.0f / 255.0f);

        //Pour activer setDisplayMode lors de la 1ere frame
        oldField = 1;
    }

    // Update is called once per frame
    void Update()
    {
        currentField = global.highlightedField;
        if (currentField != oldField)
        {
            setDisplayMode();
            oldField = currentField;
        }

        setCurrentDigitInd();
    }
}
