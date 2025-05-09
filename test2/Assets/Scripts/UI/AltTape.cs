using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltTape : MonoBehaviour, Field
{
    Image tape;
    public Image bug;
    Text altVal;
    Text targetVal;

    VsTape vs;
    SpeedTape speed;
    Global global;
    DataManager dataManager;

    public Material mat;

    public bool moveTape = false;

    //0: normal
    //1: �dition
    public int mode;

    float maxHeight = (float)7050;
    float minBugHeight = (float)-7000;

    public float currentAlt;
    public float targetAlt;
    float oldTargetAlt;

    float nextActionTime = 0.0f;
    float period = 0.005f;
    float changeSpeed; // = (float)15;

    float pixelToAlt(float pix)
    {
        return (float)((maxHeight - pix) / 0.7);
    }

    public float altToPixel(float alt)
    {
        return (float)(maxHeight - (alt * 0.7));
    }

    public float bugPosition(float alt)
    {
        return (float)(minBugHeight + (alt * 0.7));
    }

    public void updateTape()
    {
        tape.rectTransform.anchoredPosition = new Vector3(tape.rectTransform.anchoredPosition.x, altToPixel(currentAlt));
    }

    void changeUiMode()
    {
        Image box = GameObject.Find("Canvas/Alt Tape/Alt Target Box").GetComponent<Image>();
        Text value = GameObject.Find("Canvas/Alt Tape/Alt Target Value").GetComponent<Text>();
        if (mode == 0)
        {
            box.color = Color.white;
            box.material = null;
            value.color = new Color(63.0f / 255.0f, 220.0f / 255.0f, 241.0f / 255.0f);
        }
        else
        {
            box.color = new Color(63.0f/255.0f, 220.0f/255.0f, 241.0f/255.0f);
            box.material = mat;
            value.color = new Color(61.0f / 255.0f, 61.0f / 255.0f, 61.0f / 255.0f);
        }
    }

    public int toggleMode()
    {
        if (mode == 0)
        {
            if (global.target != 1)
            {
                ++dataManager.wrongFieldSelected;
                global.playSound(1);
                StartCoroutine(global.failure(1));
                return -1;
            }

            dataManager.enterPhase0Data();

            mode = 1;

            //Changer le mode visuel
            changeUiMode();

            oldTargetAlt = targetAlt;
        }
        else
        {
            //Faire un changement en 1.5 secondes
            changeSpeed =  Mathf.Abs(targetAlt - currentAlt) * 20 / (1.5f / Time.deltaTime);

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
        targetAlt = Mathf.Clamp(value, 0, 20000);

        if (overshoot)
        {
            dataManager.enterOvershootValue(targetAlt);
        }
    }

    //0: �chec
    //1: succ�s
    public int confirmTarget()
    {
        if (targetAlt != global.targetAlt)
        {
            global.playSound(1);
            return 0;
        }
        else
        {
            global.targetAltReached = true;
            global.playSound(0);
            return 1;
        }
    }

    public void restoreOldTarget()
    {
        targetAlt = oldTargetAlt;
    }

    public float getCurrentValue()
    {
        return currentAlt;
    }

    public int getMode()
    {
        return mode;
    }

    // Start is called before the first frame update
    void Start()
    {
        tape = GameObject.Find("Canvas/Alt Tape/Moving").GetComponent<Image>();
        bug = GameObject.Find("Canvas/Alt Tape/Moving/Alt Bug").GetComponent<Image>();
        altVal = GameObject.Find("Canvas/Alt Tape/Alt Pointer/Alt Value").GetComponent<Text>();
        targetVal = GameObject.Find("Canvas/Alt Tape/Alt Target Value").GetComponent<Text>();
        vs = GameObject.Find("Canvas/Vs Tape").GetComponent<VsTape>();
        speed = GameObject.Find("Canvas/Speed Tape").GetComponent<SpeedTape>();
        global = GameObject.Find("Global").GetComponent<Global>();
        dataManager = GameObject.Find("Global").GetComponent<DataManager>();

        currentAlt = pixelToAlt(tape.rectTransform.anchoredPosition.y);
        targetAlt = 10300;

        mode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;

            //Afficher un multiple de 20 pour la valeur current
            // https://stackoverflow.com/questions/15154457/rounding-integers-to-nearest-multiple-of-10
            int rem = Mathf.RoundToInt(currentAlt) % 20;
            altVal.text = (rem >= 10 ? (Mathf.RoundToInt(currentAlt) - rem + 20) : Mathf.RoundToInt(currentAlt) - rem).ToString();

            //Afficher un multiple de 20 pour la valeur target
            rem = Mathf.RoundToInt(targetAlt) % 20;
            targetVal.text = (rem >= 10 ? (Mathf.RoundToInt(targetAlt) - rem + 20) : Mathf.RoundToInt(targetAlt) - rem).ToString();

            //Bouger le bug
            bug.rectTransform.anchoredPosition = new Vector3(bug.rectTransform.anchoredPosition.x, bugPosition(targetAlt));

            if (mode == 0)
            {
                if (targetAlt < currentAlt)
                {
                    //changer l'altitude courante et bouger le tape
                    currentAlt = Mathf.Clamp(currentAlt - changeSpeed, targetAlt, float.PositiveInfinity);
                    tape.rectTransform.anchoredPosition = new Vector3(tape.rectTransform.anchoredPosition.x, altToPixel(currentAlt));
                }
                else if (targetAlt > currentAlt)
                {
                    //changer l'altitude courante et bouger le tape
                    currentAlt = Mathf.Clamp(currentAlt + changeSpeed, float.NegativeInfinity, targetAlt);
                    tape.rectTransform.anchoredPosition = new Vector3(tape.rectTransform.anchoredPosition.x, altToPixel(currentAlt));
                }
            }
        }
    }
}
