using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BaroBox : MonoBehaviour, Field
{
    Text baroVal;

    AltTape alt;
    Global global;
    DataManager dataManager;
    public Material mat;

    public int mode;

    public float currentBaro;
    float lastBaro;

    float nextActionTime = 0.0f;
    float period = 0.03f;

    bool direct = false; //si en mode manip directes

    void changeUiMode()
    {
        Image box = GameObject.Find("Canvas/Baro/Baro Box").GetComponent<Image>();
        if (mode == 0)
        {
            box.color = Color.white;
            box.material = null;
            baroVal.color = new Color(63.0f / 255.0f, 220.0f / 255.0f, 241.0f / 255.0f);
        }
        else
        {
            box.color = new Color(63.0f / 255.0f, 220.0f / 255.0f, 241.0f / 255.0f);
            box.material = mat;
            baroVal.color = new Color(61.0f / 255.0f, 61.0f / 255.0f, 61.0f / 255.0f);
        }
    }

    public int toggleMode()
    {
        if (mode == 0)
        {
            if (global.target != 3)
            {
                ++dataManager.wrongFieldSelected;
                global.playSound(1);
                StartCoroutine(global.failure(3));
                return -1;
            }

            dataManager.enterPhase0Data();

            mode = 1;

            //Changer le mode visuel
            changeUiMode();

            lastBaro = currentBaro;
        }
        else
        {
            mode = 0;
            changeUiMode();
        }

        return 0;
    }

    public void editTarget(float value, bool overshoot = true)
    {
        //Premier frame en mode �dition
        if (mode == 0)
        {
            toggleMode();
        }

        //Limiter les valeurs
        //(va �tre utile pour les incr�ments)
        currentBaro = Mathf.Clamp(value, 28, 32);

        if (overshoot)
        {
            dataManager.enterOvershootValue(currentBaro);
        }
    }

    //0: échec
    //1: succès
    public int confirmTarget()
    {
        if (currentBaro.ToString("n2") != global.targetBaro.ToString("n2"))
        {
            global.playSound(1);
            return 0;
        }
        else
        {
            global.targetBaroReached = true;
            global.playSound(0);
            return 1;
        }
    }

    public void restoreOldTarget()
    {
        currentBaro = lastBaro;
    }

    public float getCurrentValue()
    {
        return currentBaro;
    }

    public int getMode()
    {
        return mode;
    }

    // Start is called before the first frame update
    void Start()
    {
        baroVal = GameObject.Find("Canvas/Baro/Baro Value").GetComponent<Text>();
        alt = GameObject.Find("Canvas/Alt Tape").GetComponent<AltTape>();
        global = GameObject.Find("Global").GetComponent<Global>();
        dataManager = GameObject.Find("Global").GetComponent<DataManager>();
        currentBaro = (float)29.92;
        lastBaro = (float)29.92;

        if (SceneManager.GetActiveScene().name == "TouchDirect")
        {
            direct = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            baroVal.text = currentBaro.ToString("n2");

            //Ne pas arrondir en direct, sinon un drag lent ne changera pas la valeur
            if (!direct)
            {
                currentBaro = Mathf.Round(currentBaro * 100) / 100;
            }

            if (mode == 0)
            {
                if (lastBaro != currentBaro)
                {
                    //Mettre un multiple de 100 comme nouvelle alt
                    float newAlt = alt.currentAlt + (currentBaro - lastBaro) * 1200;
                    int rem = Mathf.RoundToInt(newAlt) % 100;
                    newAlt = (rem >= 50 ? (Mathf.RoundToInt(newAlt) - rem + 100) : Mathf.RoundToInt(newAlt) - rem);
                    newAlt = Mathf.Clamp(newAlt, 0, 20000);

                    alt.currentAlt = newAlt;
                    alt.targetAlt = alt.currentAlt;
                    alt.updateTape();
                    lastBaro = currentBaro;
                }
            }
        }
    }
}
