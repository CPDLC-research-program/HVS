using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VsTape : MonoBehaviour, Field
{
    Image pointer;
    public Image bug;
    Image tape;
    Text vsVal;
    Text targetVal;

    AltTape alt;
    Global global;
    DataManager dataManager;

    public Material mat;

    public int mode;

    float centerHeight = (float)58.5;
    float centerBugHeight = (float)59.1;

    public float currentVs;
    public float targetVs;
    float oldTargetVs;

    int oldVMode;

    float nextActionTime = 0.0f;
    float period = 0.03f;
    float changeSpeed; // = (float)2.5;

    //Sc�ne touch direct
    float centerTapeHeight = 61;

    float pixelToVs(float pix)
    {
        return (float)((pix - centerHeight) / 0.09);
    }

    float vsToPixel(float vs)
    {
        return (float)(centerHeight + (vs * 0.09));
    }

    public float bugPosition(float vs)
    {
        return (float)(centerBugHeight + (vs * 0.09));
    }

    void hideBug()
    {
        bug.rectTransform.anchoredPosition = new Vector3(bug.rectTransform.anchoredPosition.x, -10000);
    }

    public float tapePosition(float vs)
    {
        return (float)(centerTapeHeight + (vs * 0.09));
    }

    void changeUiMode()
    {
        Image box = GameObject.Find("Canvas/Vs Tape/Vs Target Box").GetComponent<Image>();
        Text value = GameObject.Find("Canvas/Vs Tape/Vs Target Value").GetComponent<Text>();
        if (mode == 0)
        {
            box.color = Color.white;
            box.material = null;
            value.color = new Color(63.0f / 255.0f, 220.0f / 255.0f, 241.0f / 255.0f);
        }
        else
        {
            box.color = new Color(63.0f / 255.0f, 220.0f / 255.0f, 241.0f / 255.0f);
            box.material = mat;
            value.color = new Color(61.0f / 255.0f, 61.0f / 255.0f, 61.0f / 255.0f); ;
        }
    }

    public int toggleMode()
    {
        if (mode == 0)
        {
            if (global.target != 2)
            {
                ++dataManager.wrongFieldSelected;
                global.playSound(1);
                StartCoroutine(global.failure(2));
                return -1;
            }

            dataManager.enterPhase0Data();

            mode = 1;

            //Changer le mode visuel
            changeUiMode();

            oldTargetVs = targetVs;
        }
        else
        {
            //Faire un changement en 1.5 secondes
            changeSpeed = Mathf.Abs(targetVs - currentVs) * 20 / (1.5f / Time.deltaTime);

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
        targetVs = Mathf.Clamp(value, -2000, 2000);

        if (overshoot)
        {
            dataManager.enterOvershootValue(targetVs);
        }
    }

    //0: �chec
    //1: succ�s
    public int confirmTarget()
    {
        if (targetVs != global.targetVs)
        {
            global.playSound(1);
            return 0;
        }
        else
        {
            global.targetVsReached = true;
            global.playSound(0);
            return 1;
        }
    }

    public void restoreOldTarget()
    {
        targetVs = oldTargetVs;
    }

    public float getCurrentValue()
    {
        return currentVs;
    }

    public int getMode()
    {
        return mode;
    }

    // Start is called before the first frame update
    void Start()
    {
        pointer = GameObject.Find("Canvas/Vs Tape/Vs Pointer").GetComponent<Image>();
        vsVal = GameObject.Find("Canvas/Vs Tape/Vs Pointer/Vs Value").GetComponent<Text>();
        targetVal = GameObject.Find("Canvas/Vs Tape/Vs Target Value").GetComponent<Text>();
        alt = GameObject.Find("Canvas/Alt Tape").GetComponent<AltTape>();
        global = GameObject.Find("Global").GetComponent<Global>();
        dataManager = GameObject.Find("Global").GetComponent<DataManager>();

        currentVs = 0;
        targetVs = float.MinValue;

        pointer.rectTransform.anchoredPosition = new Vector3(pointer.rectTransform.anchoredPosition.x, pixelToVs(currentVs));

        mode = 0;

        if (SceneManager.GetActiveScene().name == "TouchDirect")
        {
            tape = GameObject.Find("Canvas/Vs Tape/Moving").GetComponent<Image>();
            bug = GameObject.Find("Canvas/Vs Tape/Moving/Vs Bug").GetComponent<Image>();
            centerBugHeight = (float)-1.9;

        }
        else
        {
            bug = GameObject.Find("Canvas/Vs Tape/Vs Bug").GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;

            //Afficher un multiple de 20
            // https://stackoverflow.com/questions/15154457/rounding-integers-to-nearest-multiple-of-10
            int rem = Mathf.RoundToInt(currentVs) % 20;
            vsVal.text = (rem >= 10 ? (Mathf.RoundToInt(currentVs) - rem + 20) : Mathf.RoundToInt(currentVs) - rem).ToString();

            if ((targetVs == float.MinValue) || ((targetVs == currentVs) && mode == 0))
            {
                targetVs = float.MinValue;
                targetVal.text = 0.ToString();
                hideBug();

                //Avion est stable, mont�e ou descente termin�e
                currentVs = 0;
                pointer.rectTransform.anchoredPosition = new Vector3(pointer.rectTransform.anchoredPosition.x, vsToPixel(currentVs));
            }
            else
            {
                //Afficher un multiple de 20
                // https://stackoverflow.com/questions/15154457/rounding-integers-to-nearest-multiple-of-10
                int rem2 = Mathf.RoundToInt(targetVs) % 100;
                targetVal.text = (rem2 >= 50 ? (Mathf.RoundToInt(targetVs) - rem2 + 100) : Mathf.RoundToInt(targetVs) - rem2).ToString();

                bug.rectTransform.anchoredPosition = new Vector3(bug.rectTransform.anchoredPosition.x, bugPosition(targetVs));                

                if (mode == 0)
                {
                    //Bloquer les autres actions
                    global.actionInProgress = true;

                    if (targetVs < currentVs)
                    {
                        currentVs = Mathf.Clamp(currentVs - changeSpeed, targetVs, float.PositiveInfinity);
                        pointer.rectTransform.anchoredPosition = new Vector3(pointer.rectTransform.anchoredPosition.x, vsToPixel(currentVs));
                    }
                    else if (targetVs > currentVs)
                    {
                        currentVs = Mathf.Clamp(currentVs + changeSpeed, float.NegativeInfinity, targetVs);
                        pointer.rectTransform.anchoredPosition = new Vector3(pointer.rectTransform.anchoredPosition.x, vsToPixel(currentVs));
                    }

                    if (targetVs == currentVs)
                    {
                        //D�bloquer les autres actions
                        global.actionInProgress = false;
                    }
                }
                
            }
        }
    }
}
