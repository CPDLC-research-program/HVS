using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{
    //Valeurs � atteindre pour la t�che
    public int targetSpeed = int.MinValue;
    public int targetAlt = int.MinValue;
    public int targetVs = int.MinValue;
    public float targetBaro = float.MinValue;
    public int targetHdg = int.MinValue;
    public int target = -1;

    //Valeurs atteintes
    public bool targetSpeedReached = false;
    public bool targetAltReached = false;
    public bool targetVsReached = false;
    public bool targetBaroReached = false;
    public bool targetHdgReached = false;

    //Labels de valeurs cibles
    public Text hdgValue;
    public Text iasValue;
    public Text altValue;
    public Text vsValue;
    public Text baroValue;

    //�l�ments du PFD
    VsTape vs;
    SpeedTape speed;
    AltTape alt;
    Hdg hdg;
    BaroBox baro;
    public Field[] fields = new Field[5];

    float verticalOffset = 106; //Écart vertical quand on déplace les éléments du pfd

    Image highlightBox;

    DataManager dataManager;

    //Champ en surbrillance
    //-1: Aucun
    //0: IAS
    //1: ALT
    //2: VS
    //3: BARO
    //4: HDG
    public int highlightedField;

    //Objets pour le son
    AudioClip successSound;
    AudioClip errorSound;
    AudioClip taskSound;
    AudioClip tickSound;
    AudioSource audioSource;

    //Le keypad
    Keypad2 keypad;

    //Nécessaire pour avoir la référence à font material
    public Material mat;

    //Bloqueur pour les actions
    public bool actionInProgress = false;

    public int toggleMode()
    {
        Field f;

        if (highlightedField == -1)
        {
            return -1;
        }

        switch (highlightedField)
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

        return f.toggleMode();
    }

    public void restoreOldTarget()
    {
        Field f;

        if (highlightedField == -1)
        {
            return;
        }

        int mode;

        switch (highlightedField)
        {
            case 0:
                f = speed;
                mode = speed.mode;
                break;
            case 1:
                f = alt;
                mode = alt.mode;
                break;
            case 2:
                f = vs;
                mode = vs.mode;
                break;
            case 3:
                f = baro;
                mode = baro.mode;
                break;
            default:
                f = hdg;
                mode = hdg.mode;
                break;
        }

        if (mode == 1)
        {
            f.restoreOldTarget();
        }     
    }

    public int confirmTarget()
    {
        Field f;

        switch (highlightedField)
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

        return f.confirmTarget();
    }


    public void toggleKeypadVisibility(bool visible)
    {
        if (visible)
        {
            keypad.GetComponent<RectTransform>().anchoredPosition = new Vector3((float)412.74, (float)-316.26);
        }
        else
        {
            keypad.GetComponent<RectTransform>().anchoredPosition = new Vector3((float)10000, (float)10000);
        }
    }

    //0: IAS
    //1: ALT
    //2: VS
    //3: BARO
    //4: HDG
    //5: Keypad
    //6: Slider
    public IEnumerator success(int mode)
    {
        //Bloquer les autres actions
        actionInProgress = true;

        Text value;
        Image box;
        Color green = new Color(0.0f / 255.0f, 255.0f / 255.0f, 89.0f / 255.0f);
        Color blue = new Color(63.0f / 255.0f, 220.0f / 255.0f, 241.0f / 255.0f);
        Color grey = new Color(61.0f / 255.0f, 61.0f / 255.0f, 61.0f / 255.0f);
        Color lightGrey = new Color(233.0f / 255.0f, 233.0f / 255.0f, 233.0f / 255.0f);

        switch (mode)
        {
            //IAS
            case 0:
                value = GameObject.Find("Canvas/Speed Tape/Speed Target Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Speed Tape/Speed Target Box").GetComponent<Image>();
                break;
            //ALT
            case 1:
                value = GameObject.Find("Canvas/Alt Tape/Alt Target Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Alt Tape/Alt Target Box").GetComponent<Image>();
                break;
            //VS
            case 2:
                value = GameObject.Find("Canvas/Vs Tape/Vs Target Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Vs Tape/Vs Target Box").GetComponent<Image>();
                break;
            //Baro
            case 3:
                value = GameObject.Find("Canvas/Baro/Baro Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Baro/Baro Box").GetComponent<Image>();
                break;
            //HDG
            case 4:
                value = GameObject.Find("Canvas/Heading/Hdg Target Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Heading/Hdg Target Box").GetComponent<Image>();
                break;
            //Keypad
            case 5:
                value = GameObject.Find("Canvas/Keypad/Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Keypad/Keypad Display Bg").GetComponent<Image>();
                break;
            //Slider
            default:
                value = GameObject.Find("Canvas/Slider/Slider Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Slider/Slider Display Bg").GetComponent<Image>();
                break;
        }

        Color initialColorVal = value.color;
        Color initialColorBox = box.color;

        bool hasMat = true;

        

        if (box.material != mat)
        {
            box.material = mat;

            if (mode == 5 || mode == 6)
            {
                initialColorBox = lightGrey;
            }
            else
            {
                initialColorBox = grey;
            }

            hasMat = false;
        }

        

        box.color = green;

        value.color = Color.black;

        float fadeTime = (float)0.5;
        float counter = 0;

        Color currentColorVal = value.color;
        Color currentColorBox = box.color;

        while (counter < fadeTime)
        {
            counter += Time.deltaTime;
            value.color = Color.Lerp(Color.black, initialColorVal, counter / fadeTime);
            box.color = Color.Lerp(green, initialColorBox, counter / fadeTime);
            yield return null;
        }

        //Revoir si ça bug
        //TODO: Définir le mode d'édition actuel, après le while
        if (hasMat == false)
        {
            box.color = Color.white;
            box.material = null;
        }

        //Débloquer les autres actions
        actionInProgress = false;
    }

    //0: IAS
    //1: ALT
    //2: VS
    //3: BARO
    //4: HDG
    //5: Keypad
    //6: Slider
    public IEnumerator failure(int mode)
    {
        //Bloquer les autres actions
        actionInProgress = true;

        Text value;
        Image box;
        Color blue = new Color(63.0f / 255.0f, 220.0f / 255.0f, 241.0f / 255.0f);
        Color grey = new Color(61.0f / 255.0f, 61.0f / 255.0f, 61.0f / 255.0f);
        Color lightGrey = new Color(233.0f / 255.0f, 233.0f / 255.0f, 233.0f / 255.0f);
        Color red = new Color(255.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);

        switch (mode)
        {
            //IAS
            case 0:
                value = GameObject.Find("Canvas/Speed Tape/Speed Target Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Speed Tape/Speed Target Box").GetComponent<Image>();
                break;
            //ALT
            case 1:
                value = GameObject.Find("Canvas/Alt Tape/Alt Target Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Alt Tape/Alt Target Box").GetComponent<Image>();
                break;
            //VS
            case 2:
                value = GameObject.Find("Canvas/Vs Tape/Vs Target Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Vs Tape/Vs Target Box").GetComponent<Image>();
                break;
            //Baro
            case 3:
                value = GameObject.Find("Canvas/Baro/Baro Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Baro/Baro Box").GetComponent<Image>();
                break;
            //HDG
            case 4:
                value = GameObject.Find("Canvas/Heading/Hdg Target Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Heading/Hdg Target Box").GetComponent<Image>();
                break;
            //Keypad
            case 5:
                value = GameObject.Find("Canvas/Keypad/Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Keypad/Keypad Display Bg").GetComponent<Image>();
                break;
            //Slider
            default:
                value = GameObject.Find("Canvas/Slider/Slider Value").GetComponent<Text>();
                box = GameObject.Find("Canvas/Slider/Slider Display Bg").GetComponent<Image>();
                break;
        }

        Color initialColorVal = value.color;
        Color initialColorBox = box.color;

        bool hasMat = true;

        if (box.material != mat)
        {
            box.material = mat;

            if (mode == 5 || mode == 6)
            {
                initialColorBox = lightGrey;
            }
            else
            {
                initialColorBox = grey;
            }

            hasMat = false;
        }

        if (mode == 3)
        {
            //Debug.Log(hasMat);
        }
        
        box.color = red;
        value.color = Color.white;

        Color currentColorVal = value.color;
        Color currentColorBox = box.color;

        float fadeTime = (float)0.5;
        float counter = 0;

        while (counter < fadeTime)
        {
            counter += Time.deltaTime;
            value.color = Color.Lerp(currentColorVal, initialColorVal, counter / fadeTime);
            box.color = Color.Lerp(currentColorBox, initialColorBox, counter / fadeTime);
            yield return null;
        }

        //Revoir si ça bug
        if (!hasMat)
        {
            box.color = Color.white;
            box.material = null;
        }

        //Débloquer les autres actions
        actionInProgress = false;
    }

    //0: succes
    //1: erreur
    public void playSound(int sound)
    {
        AudioClip clip;
        if (sound == 0)
        {
            clip = successSound;
        }
        else if (sound == 1)
        {
            clip = errorSound;
        }
        else
        {
            clip = taskSound;
        }

        audioSource.PlayOneShot(clip);
    }

    public void resetPfdModes()
    {
        if (speed.mode == 1)
        {
            speed.toggleMode();
        }
        if (alt.mode == 1)
        {
            alt.toggleMode();
        }
        if (vs.mode == 1)
        {
            vs.toggleMode();
        }
        if (baro.mode == 1)
        {
            baro.toggleMode();
        }
        if (hdg.mode == 1)
        {
            hdg.toggleMode();
        }
    }

    public void highlightNextField()
    {
        //Skipper le hdg si on est en mode nav
        highlightedField = highlightedField == 4 ? 0 : highlightedField + 1;
    }

    public void highlightPreviousField()
    {
        //Skipper le hdg si on est en mode nav
        highlightedField = highlightedField == 0 ? 4 : highlightedField - 1;
    }

    void drawHighlight()
    {
        Vector3 position = Vector3.zero;
        float width = 0;

        switch (highlightedField)
        {
            //Aucun
            case -1:
                position = new Vector3((float)-2000, (float)2000, 0);
                width = (float)108.01;
                break;
            //IAS
            case 0:
                position = new Vector3((float)-338.3, (float)292.25 + verticalOffset, 0);
                width = (float)108.01;
                break;
            //ALT
            case 1:
                position = new Vector3((float)329.7906, (float)292.25 + verticalOffset, 0);
                width = (float)130.5;
                break;
            //VS
            case 2:
                position = new Vector3((float)454.5, (float)292.25 + verticalOffset, 0);
                width = (float)108.01;
                break;
            //BARO
            case 3:
                position = new Vector3((float)329.7906, (float)-170.5 + verticalOffset, 0);
                width = (float)130.5;
                break;
            //HDG
            case 4:
                position = new Vector3(-281, -289 + verticalOffset, 0);
                width = (float)108.01;
                break;
        }

        highlightBox.rectTransform.anchoredPosition = position;
        highlightBox.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    //field: voir plus haut
    //direction: -1 pour n�gatif, 1 pour positif
    //size: 0 pour petit, 1 pour grand
    public void incrementValue(int field, int direction, int size)
    {
        float increment;
        switch (field)
        {
            //IAS
            case 0:
                increment = (size == 0 ? 1 : 10) * direction;
                //si la vitesse target est nulle, prendre la valeur courante
                speed.editTarget(speed.targetSpeed == float.MinValue ? speed.currentSpeed + increment : speed.targetSpeed + increment);
                break;
            //ALT
            case 1:
                increment = (size == 0 ? 100 : 1000) * direction;
                alt.editTarget(alt.targetAlt + increment);
                break;
            //VS
            case 2:
                increment = (size == 0 ? 100 : 1000) * direction;
                //si la vs target est nulle, prendre la valeur courante
                vs.editTarget(vs.targetVs == float.MinValue ? vs.currentVs + increment : vs.targetVs + increment);
                break;
            //BARO
            case 3:
                increment = (float) (size == 0 ? 0.01 : 0.1) * direction;
                baro.editTarget(baro.currentBaro + increment);
                break;
            //HDG
            case 4:
                increment = (size == 0 ? 1 : 10) * direction;
                float temp = hdg.targetHdg + increment;
                if (temp < 0)
                {
                    temp = 360 + temp;
                }
                else if (temp >= 360)
                {
                    temp = temp % 360;
                }
                hdg.editTarget(temp);
                break;
        }
    }


    public void setValue(Field field, float value)
    {
        field.editTarget(value);
    }

    //direction: -1 pour n�gatif, 1 pour positif
    //size: 0 pour petit, 1 pour grand
    public void getNextMovement(int direction, int size, bool log = false)
    {
        switch (highlightedField)
        {
            //IAS
            case 0:
                if (speed.mode == 0)
                {
                    if (direction == 1)
                    {
                        highlightNextField();
                    }
                    else
                    {
                        highlightPreviousField();
                    }
                }
                else
                {
                    incrementValue(0, direction, size);

                    if (log)
                    {
                        ++dataManager.nbInteractionVal;
                    }

                    //audioSource.PlayOneShot(tickSound);
                }
                break;
            //ALT
            case 1:
                if (alt.mode == 0)
                {
                    if (direction == 1)
                    {
                        highlightNextField();
                    }
                    else
                    {
                        highlightPreviousField();
                    }
                }
                else
                {
                    incrementValue(1, direction, size);

                    if (log)
                    {
                        ++dataManager.nbInteractionVal;
                    }
                    //audioSource.PlayOneShot(tickSound);
                }
                break;
            //VS
            case 2:
                if (vs.mode == 0)
                {
                    if (direction == 1)
                    {
                        highlightNextField();
                    }
                    else
                    {
                        highlightPreviousField();
                    }
                }
                else
                {
                    incrementValue(2, direction, size);

                    if (log)
                    {
                        ++dataManager.nbInteractionVal;
                    }

                    //audioSource.PlayOneShot(tickSound);
                }
                break;
            //BARO
            case 3:
                if (baro.mode == 0)
                {
                    if (direction == 1)
                    {
                        highlightNextField();
                    }
                    else
                    {
                        highlightPreviousField();
                    }
                }
                else
                {
                    incrementValue(3, direction, size);

                    if (log)
                    {
                        ++dataManager.nbInteractionVal;
                    }

                    //audioSource.PlayOneShot(tickSound);
                }
                break;
            //HDG
            case 4:
                if (hdg.mode == 0)
                {
                    if (direction == 1)
                    {
                        highlightNextField();
                    }
                    else
                    {
                        highlightPreviousField();
                    }
                }
                else
                {
                    incrementValue(4, direction, size);

                    if (log)
                    {
                        ++dataManager.nbInteractionVal;
                    }

                    //audioSource.PlayOneShot(tickSound);
                }
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        vs = GameObject.Find("Canvas/Vs Tape").GetComponent<VsTape>();
        speed = GameObject.Find("Canvas/Speed Tape").GetComponent<SpeedTape>();
        alt = GameObject.Find("Canvas/Alt Tape").GetComponent<AltTape>();
        hdg = GameObject.Find("Canvas/Heading").GetComponent<Hdg>();
        baro = GameObject.Find("Canvas/Baro").GetComponent<BaroBox>();
        highlightBox = GameObject.Find("Canvas/Highlight Box").GetComponent<Image>();

        dataManager = GameObject.Find("Global").GetComponent<DataManager>();

        fields[0] = speed;
        fields[1] = alt;
        fields[2] = vs;
        fields[3] = baro;
        fields[4] = hdg;

        hdgValue = GameObject.Find("Canvas/Left Panel/Hdg Value").GetComponent<Text>();
        iasValue = GameObject.Find("Canvas/Left Panel/Ias Value").GetComponent<Text>(); 
        altValue = GameObject.Find("Canvas/Left Panel/Alt Value").GetComponent<Text>(); 
        vsValue = GameObject.Find("Canvas/Left Panel/Vs Value").GetComponent<Text>(); 
        baroValue = GameObject.Find("Canvas/Left Panel/Baro Value").GetComponent<Text>();

        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        successSound = Resources.Load<AudioClip>("success");
        errorSound = Resources.Load<AudioClip>("error");
        taskSound = Resources.Load<AudioClip>("task");
        tickSound = Resources.Load<AudioClip>("tick");

        if (SceneManager.GetActiveScene().name == "TouchGroupedKeyboard" || SceneManager.GetActiveScene().name == "TouchColocKeyboard")
        {
            keypad = GameObject.Find("Canvas/Keypad").GetComponent<Keypad2>();
        }        

        highlightedField = 0;

        //test
        /*
        targetAlt = 10000;
        targetSpeed = 180;
        targetVs = -900;
        targetBaro = (float)29.82;
        targetHdg = 150;
        */
    }

    // Update is called once per frame
    void Update()
    {
        //Updater les cibles
        //checkTargets();

        //Afficher les cibles
        //writeTargets();

        //changer le highlight
        drawHighlight();
    }
}
