using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DirectManip : MonoBehaviour
{
    VsTape vs;
    SpeedTape speed;
    AltTape alt;
    Hdg hdg;
    BaroBox baro;

    //Hdg
    Image compass;
    Image hdgBox;
    float initialCompassRotation;
    bool hdgPossibleClick;
    Vector2 hdgClickPosition;

    //Baro
    Image baroBox;
    float baroTimer;
    float baroBuffer;
    float baroBufferDisplacement;
    float lastBaroPosition;
    float baroClickPosition;
    bool baroPossibleClick;
    Vector2 initialBaroTapePosition;

    //Vs
    Image vsBg;
    Image vsTape;
    Image vsBox;
    Image vsPointer;
    float vsTimer;
    float lastVsPosition;
    float vsClickPosition;
    float vsBuffer;
    float vsBufferDisplacement;
    bool vsPossibleClick;
    Vector2 initialVsTapePosition;

    //Alt
    Image altBg;
    Image altBox;
    Image altTape;
    Image altPointer;
    float altTimer;
    float altBuffer;
    float altBufferDisplacement;
    float lastAltPosition;
    float altClickPosition;
    bool altPossibleClick;
    Vector2 initialAltTapePosition;

    //Ias
    Image speedBg;
    Image speedBox;
    Image speedTape;
    Image speedPointer;
    float speedTimer;
    float lastSpeedPosition;
    float speedBuffer;
    float speedBufferDisplacement;
    float speedClickPosition;
    bool speedPossibleClick;
    Vector2 initialSpeedTapePosition;

    bool hdgCompassHeld;
    bool baroTapeHeld;
    bool vsTapeHeld;
    bool altTapeHeld;
    bool speedTapeHeld;

    bool firstFrame = true;

    Global global;
    DataManager dataManager;

    //Quand on clique sur le mauvais tape (à vérifier dans le drag)
    bool wrongTape = false;

    public void resetPfd()
    {
        StopAllCoroutines();

        //Si c'est alt
        if (global.highlightedField == 1)
        {
            altTape.rectTransform.anchoredPosition = initialAltTapePosition;
            resetAltPointer();
        }
        //ou ias
        else if (global.highlightedField == 0)
        {
            speedTape.rectTransform.anchoredPosition = initialSpeedTapePosition;
            resetSpeedPointer();
        }
        //ou vs
        else if (global.highlightedField == 2)
        {
            vsTape.rectTransform.anchoredPosition = initialVsTapePosition;
            resetVsPointer();
        }
        //ou baro
        else if (global.highlightedField == 3)
        {
            altTape.rectTransform.anchoredPosition = initialBaroTapePosition;
        }

        global.restoreOldTarget();
        global.toggleMode();
        global.highlightedField = -1;
    }

    void moveAltTape()
    {
        //Bouger le tape
        altTape.rectTransform.anchoredPosition = new Vector3(altTape.rectTransform.anchoredPosition.x, alt.altToPixel(alt.targetAlt));

        //Bouger le bug
        alt.bug.rectTransform.anchoredPosition = new Vector3(alt.bug.rectTransform.anchoredPosition.x, alt.bugPosition(alt.targetAlt));

        //Bouger le pointeur (centre: y = 49.7) 
        altPointer.rectTransform.anchoredPosition = new Vector3(altPointer.rectTransform.anchoredPosition.x,
            Mathf.Clamp((float)49.7 - (alt.targetAlt - alt.currentAlt) * (float)0.7, (float)-125.97, (float)247.7));
    }

    void resetAltPointer()
    {
        altPointer.rectTransform.anchoredPosition = new Vector3(altPointer.rectTransform.anchoredPosition.x, (float)49.7);
    }

    void moveSpeedTape()
    {
        //Bouger le tape
        speedTape.rectTransform.anchoredPosition = new Vector3(speedTape.rectTransform.anchoredPosition.x, speed.speedToPixel(speed.targetSpeed));

        //Bouger le bug
        speed.bug.rectTransform.anchoredPosition = new Vector3(speed.bug.rectTransform.anchoredPosition.x, speed.bugPosition(speed.targetSpeed));

        //Bouger le pointeur (centre: y = 49.7) 
        speedPointer.rectTransform.anchoredPosition = new Vector3(speedPointer.rectTransform.anchoredPosition.x,
            Mathf.Clamp((float)49.7 - (speed.targetSpeed - speed.currentSpeed) * (float)7.2, (float)-125.97, (float)247.7));
    }

    void resetSpeedPointer()
    {
        speedPointer.rectTransform.anchoredPosition = new Vector3(speedPointer.rectTransform.anchoredPosition.x, (float)49.7);
    }

    void moveVsTape()
    {
        //Bouger le tape
        vsTape.rectTransform.anchoredPosition = new Vector3(vsTape.rectTransform.anchoredPosition.x, vs.tapePosition(-vs.targetVs));

        //Bouger le bug
        vs.bug.rectTransform.anchoredPosition = new Vector3(vs.bug.rectTransform.anchoredPosition.x, vs.bugPosition(vs.targetVs));

        //Bouger le pointeur (centre: y = 49.7) 
        vsPointer.rectTransform.anchoredPosition = new Vector3(vsPointer.rectTransform.anchoredPosition.x,
            Mathf.Clamp((float)58.5 - (vs.targetVs - vs.currentVs) * (float)0.09, (float)-125.97, (float)247.7));
    }

    void resetVsPointer()
    {
        vsPointer.rectTransform.anchoredPosition = new Vector3(vsPointer.rectTransform.anchoredPosition.x, (float)58.5);
    }

    void moveBaroTape()
    {
        //Bouger le tape
        altTape.rectTransform.anchoredPosition = new Vector3(altTape.rectTransform.anchoredPosition.x, alt.altToPixel(alt.targetAlt));

        //Bouger le bug
        alt.bug.rectTransform.anchoredPosition = new Vector3(alt.bug.rectTransform.anchoredPosition.x, alt.bugPosition(alt.targetAlt));
    }

    void stopAltFlick(bool baro = false)
    {
        //Arreter le flick et mettre un multiple de 100 comme target
        StopAllCoroutines();

        //Pour le baro, on ne veut pas de cet arrondissement
        if (!baro)
        {
            int rem = Mathf.RoundToInt(alt.targetAlt) % 100;
            alt.targetAlt = (rem >= 50 ? (Mathf.RoundToInt(alt.targetAlt) - rem + 100) : Mathf.RoundToInt(alt.targetAlt) - rem);
        }
    }

    void stopSpeedFlick()
    {
        StopAllCoroutines();
        speed.targetSpeed = Mathf.RoundToInt(speed.targetSpeed);
    }

    void stopVsFlick()
    {
        StopAllCoroutines();
        int rem = Mathf.RoundToInt(alt.targetAlt) % 100;
        alt.targetAlt = (rem >= 50 ? (Mathf.RoundToInt(alt.targetAlt) - rem + 100) : Mathf.RoundToInt(alt.targetAlt) - rem);
    }

    //initialSpeed est en pieds
    //Vitesse n�gative: augmenter l'altitude
    public IEnumerator altTapeFlick(float initialSpeed)
    {
        float increment = (float)0.5;

        for (float speed = initialSpeed; (initialSpeed < 0 ? speed < 0 : speed > 0); speed += increment * (initialSpeed < 0 ? 1 : -1))
        {
            //R�duire l'incr�ment pour adoucir le changement
            increment = increment / (float)1.0001;

            //Changer le target (soustraire la vitesse parce qu'invers�e) et bouger le tape
            alt.editTarget(alt.targetAlt - speed);
            moveAltTape();

            yield return new WaitForSeconds(0.03f);
        }

        //Mettre un multiple de 20 comme target
        int rem = Mathf.RoundToInt(alt.targetAlt) % 100;
        alt.targetAlt = (rem >= 50 ? (Mathf.RoundToInt(alt.targetAlt) - rem + 100) : Mathf.RoundToInt(alt.targetAlt) - rem);
    }

    //initialSpeed est en pieds
    //Vitesse n�gative: augmenter l'altitude
    public IEnumerator speedTapeFlick(float initialSpeed)
    {
        //Changer initialSpeed pour la vitesse g�n�rale du scroll
        //Changer incr�ment et la division pour la courbe de ralentissement

        //Debug.Log(initialSpeed);
        float increment = (float)0.2;

        for (float speedSpeed = initialSpeed / 15; (initialSpeed < 0 ? speedSpeed < 0 : speedSpeed > 0); speedSpeed += increment * (initialSpeed < 0 ? 1 : -1))
        {
            //R�duire l'incr�ment pour adoucir le changement
            increment = increment / (float)1.000000001;

            //Changer le target (soustraire la vitesse parce qu'invers�e) et bouger le tape
            speed.editTarget(speed.targetSpeed - speedSpeed);
            moveSpeedTape();

            yield return new WaitForSeconds(0.03f);
        }

        speed.targetSpeed = Mathf.RoundToInt(speed.targetSpeed);
    }

    //initialSpeed est en pieds
    //Vitesse n�gative: augmenter l'altitude
    public IEnumerator vsTapeFlick(float initialSpeed)
    {
        //Changer initialSpeed pour la vitesse g�n�rale du scroll
        //Changer incr�ment et la division pour la courbe de ralentissement

        //Debug.Log(initialSpeed);
        float increment = (float)0.5;

        for (float speedSpeed = initialSpeed / 15; (initialSpeed < 0 ? speedSpeed < 0 : speedSpeed > 0); speedSpeed += increment * (initialSpeed < 0 ? 1 : -1))
        {
            //R�duire l'incr�ment pour adoucir le changement
            increment = increment / (float)1.0001;

            //Changer le target (soustraire la vitesse parce qu'invers�e) et bouger le tape
            vs.editTarget(vs.targetVs - speedSpeed);
            moveVsTape();

            yield return new WaitForSeconds(0.03f);
        }

        //Mettre un multiple de 100 comme target
        int rem = Mathf.RoundToInt(vs.targetVs) % 100;
        vs.targetVs = (rem >= 50 ? (Mathf.RoundToInt(vs.targetVs) - rem + 100) : Mathf.RoundToInt(vs.targetVs) - rem);
    }

    //initialSpeed est en pieds
    //Vitesse n�gative: augmenter l'altitude
    public IEnumerator baroTapeFlick(float initialSpeed)
    {
        float increment = (float)0.5;

        for (float speed = initialSpeed; (initialSpeed < 0 ? speed < 0 : speed > 0); speed += increment * (initialSpeed < 0 ? 1 : -1))
        {
            //R�duire l'incr�ment pour adoucir le changement
            increment = increment / (float)1.0001;

            float oldAlt = alt.targetAlt;

            //Changer le target (soustraire la vitesse parce qu'invers�e) et bouger le tape
            alt.targetAlt = Mathf.Clamp(alt.targetAlt - speed, 0, 20000);
            alt.currentAlt = alt.targetAlt;
            moveAltTape();

            baro.currentBaro = (alt.targetAlt - oldAlt) / 1200 + baro.currentBaro;

            yield return new WaitForSeconds(0.03f);
        }

        //Mettre un multiple de 100 comme target
        int rem = Mathf.RoundToInt(alt.targetAlt) % 100;
        alt.targetAlt = (rem >= 50 ? (Mathf.RoundToInt(alt.targetAlt) - rem + 100) : Mathf.RoundToInt(alt.targetAlt) - rem);
        alt.currentAlt = alt.targetAlt;
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

    private void OnMouseDown()
    {
        if (global.actionInProgress)
        {
            return;
        }

        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        if (isInside(hdgBox, mouseX, mouseY))
        {
            if (global.highlightedField != -1 && global.highlightedField != 4)
            {
                return;
            }

            //Si on est pas en édition
            if (hdg.mode == 0)
            {
                resetPfd();
                global.highlightedField = 4;

                int result = global.toggleMode();
                if (result == -1)
                {
                    global.highlightedField = -1;
                    wrongTape = true;
                    return;
                }
            }
            else
            {
                int result = hdg.confirmTarget();

                if (result == 0)
                {
                    ++dataManager.nbValError;

                    //Feedback d'�chec pfd
                    StartCoroutine(global.failure(4));
                }
                else
                {
                    global.highlightedField = -1;

                    hdg.toggleMode();

                    //Feedback d'�chec pfd
                    StartCoroutine(global.success(4));
                }
            }
        }
        else if (isInside(compass, mouseX, mouseY))
        {
            if (global.highlightedField != -1 && global.highlightedField != 4)
            {
                return;
            }

            if (hdg.mode == 0)
            {
                resetPfd();
                global.highlightedField = 4;
                
                int result = global.toggleMode();
                if (result == -1)
                {
                    global.highlightedField = -1;
                    wrongTape = true;
                    return;
                }
            }

            ++dataManager.nbInteractionVal;

            hdgCompassHeld = true;
            hdgClickPosition = new Vector2(mouseX, mouseY);
            hdgPossibleClick = true;
            initialCompassRotation = compass.rectTransform.eulerAngles.z;
        }
        else if (isInside(baroBox, mouseX, mouseY))
        {
            if (global.highlightedField != -1 && global.highlightedField != 3)
            {
                return;
            }

            if (baro.mode == 0)
            {
                resetPfd();
                global.highlightedField = 3;
                
                int result = global.toggleMode();
                if (result == -1)
                {
                    global.highlightedField = -1;
                    wrongTape = true;
                    return;
                }

                initialBaroTapePosition = altTape.rectTransform.anchoredPosition;
            }
            else
            {
                //Arr�ter le flick
                stopAltFlick(true);

                int result = baro.confirmTarget();

                if (result == 0)
                {
                    ++dataManager.nbValError;

                    //Feedback d'�chec pfd
                    StartCoroutine(global.failure(3));
                }
                else
                {
                    //Remettre le tape � son emplacement intial
                    //TODO: Faire une transition?
                    //altTape.rectTransform.anchoredPosition = initialAltTapePosition;

                    //Remettre le pointeur � son emplacement intial
                    //resetAltPointer();

                    global.highlightedField = -1;

                    baro.toggleMode();

                    //Mettre un multiple de 100 comme target
                    int rem = Mathf.RoundToInt(alt.targetAlt) % 100;
                    alt.targetAlt = (rem >= 50 ? (Mathf.RoundToInt(alt.targetAlt) - rem + 100) : Mathf.RoundToInt(alt.targetAlt) - rem);
                    alt.currentAlt = alt.targetAlt;

                    //Feedback d'�chec pfd
                    StartCoroutine(global.success(3));
                }
            }
        }
        else if (isInside(vsBox, mouseX, mouseY))
        {
            if (global.highlightedField != -1 && global.highlightedField != 2)
            {
                return;
            }

            if (vs.mode == 0)
            {
                resetPfd();
                global.highlightedField = 2;

                int result = global.toggleMode();
                if (result == -1)
                {
                    global.highlightedField = -1;
                    wrongTape = true;
                    return;
                }
                
                initialVsTapePosition = vsTape.rectTransform.anchoredPosition;

                //Mettre une valeur target par d�faut
                vs.targetVs = 0;
            }
            else
            {
                //Arr�ter le flick
                stopVsFlick();

                int result = vs.confirmTarget();

                if (result == 0)
                {
                    ++dataManager.nbValError;

                    //Feedback d'�chec pfd
                    StartCoroutine(global.failure(2));
                }
                else
                {
                    //Remettre le tape � son emplacement intial
                    //TODO: Faire une transition?
                    vsTape.rectTransform.anchoredPosition = initialVsTapePosition;

                    //Remettre le pointeur � son emplacement intial
                    resetVsPointer();

                    global.highlightedField = -1;

                    vs.toggleMode();

                    //Feedback d'�chec pfd
                    StartCoroutine(global.success(2));
                }
            }
        }
        else if (isInside(vsBg, mouseX, mouseY))
        {
            if (global.highlightedField != -1 && global.highlightedField != 2)
            {
                return;
            }

            //Arr�ter le flick
            stopVsFlick();

            if (vs.mode == 0)
            {
                resetPfd();
                global.highlightedField = 2;
                
                int result = global.toggleMode();
                if (result == -1)
                {
                    global.highlightedField = -1;
                    wrongTape = true;
                    return;
                }

                initialVsTapePosition = vsTape.rectTransform.anchoredPosition;

                //Mettre une valeur target par d�faut
                vs.targetVs = 0;
            }

            ++dataManager.nbInteractionVal;

            vsTapeHeld = true;
            vsPossibleClick = true;
            vsClickPosition = mouseY;
            vsTimer = Time.time;
            lastVsPosition = mouseY;
        }
        else if (isInside(altBox, mouseX, mouseY))
        {
            if (global.highlightedField != -1 && global.highlightedField != 1)
            {
                return;
            }

            if (alt.mode == 0)
            {
                resetPfd();
                global.highlightedField = 1;
                
                int result = global.toggleMode();
                if (result == -1)
                {
                    global.highlightedField = -1;
                    wrongTape = true;
                    return;
                }

                initialAltTapePosition = altTape.rectTransform.anchoredPosition;
            }
            else
            {
                //Arr�ter le flick
                stopAltFlick();

                int result = alt.confirmTarget();

                if (result == 0)
                {
                    ++dataManager.nbValError;

                    //Feedback d'�chec pfd
                    StartCoroutine(global.failure(1));
                }
                else
                {
                    //Remettre le tape � son emplacement intial
                    //TODO: Faire une transition?
                    altTape.rectTransform.anchoredPosition = initialAltTapePosition;

                    //Remettre le pointeur � son emplacement intial
                    resetAltPointer();

                    global.highlightedField = -1;

                    alt.toggleMode();

                    //Feedback d'�chec pfd
                    StartCoroutine(global.success(1));
                }
            }
        }
        else if (isInside(altBg, mouseX, mouseY))
        {
            if (global.highlightedField != -1 && global.highlightedField != 1 && global.highlightedField != 3)
            {
                return;
            }

            if (global.highlightedField == 3)
            {
                //Arr�ter le flick
                stopAltFlick(true);

                ++dataManager.nbInteractionVal;

                baroTapeHeld = true;
                baroPossibleClick = true;
                baroClickPosition = mouseY;
                baroTimer = Time.time;
                lastBaroPosition = mouseY;
            }
            else
            {
                //Arr�ter le flick
                stopAltFlick();

                if (alt.mode == 0)
                {
                    resetPfd();
                    global.highlightedField = 1;
                    
                    int result = global.toggleMode();
                    if (result == -1)
                    {
                        global.highlightedField = -1;
                        wrongTape = true;
                        return;
                    }

                    initialAltTapePosition = altTape.rectTransform.anchoredPosition;
                }

                ++dataManager.nbInteractionVal;

                altTapeHeld = true;
                altPossibleClick = true;
                altClickPosition = mouseY;
                altTimer = Time.time;
                lastAltPosition = mouseY;
            }
            
        }
        else if (isInside(speedBox, mouseX, mouseY))
        {
            if (global.highlightedField != -1 && global.highlightedField != 0)
            {
                return;
            }

            if (speed.mode == 0)
            {
                resetPfd();
                global.highlightedField = 0;
                
                int result = global.toggleMode();
                if (result == -1)
                {
                    global.highlightedField = -1;
                    wrongTape = true;
                    return;
                }

                initialSpeedTapePosition = speedTape.rectTransform.anchoredPosition;

                //Mettre une valeur target par d�faut
                speed.targetSpeed = speed.currentSpeed;
            }
            else
            {
                //Arr�ter le flick
                stopSpeedFlick();

                int result = speed.confirmTarget();

                if (result == 0)
                {
                    ++dataManager.nbValError;

                    //Feedback d'�chec pfd
                    StartCoroutine(global.failure(0));
                }
                else
                {
                    //Remettre le tape � son emplacement intial
                    //TODO: Faire une transition?
                    speedTape.rectTransform.anchoredPosition = initialSpeedTapePosition;

                    //Remettre le pointeur � son emplacement intial
                    resetSpeedPointer();

                    global.highlightedField = -1;

                    speed.toggleMode();

                    //Feedback d'�chec pfd
                    StartCoroutine(global.success(0));
                }
            }
        }
        else if (isInside(speedBg, mouseX, mouseY))
        {
            if (global.highlightedField != -1 && global.highlightedField != 0)
            {
                return;
            }

            //Arr�ter le flick
            stopAltFlick();

            if (speed.mode == 0)
            {
                resetPfd();
                global.highlightedField = 0;
                
                int result = global.toggleMode();
                if (result == -1)
                {
                    global.highlightedField = -1;
                    wrongTape = true;
                    return;
                }

                initialSpeedTapePosition = speedTape.rectTransform.anchoredPosition;

                //Mettre une valeur target par d�faut
                speed.targetSpeed = speed.currentSpeed;
            }

            ++dataManager.nbInteractionVal;

            speedTapeHeld = true;
            speedPossibleClick = true;
            speedClickPosition = mouseY;
            speedTimer = Time.time;
            lastSpeedPosition = mouseY;
        }
    }

    private void OnMouseDrag()
    {
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        if (wrongTape)
        {
            return;
        }

        //Vecteur vertical arbitraire
        Vector3 origin = new Vector3(0,1);

        if (hdgCompassHeld)
        {
            if (Mathf.Abs(mouseY - hdgClickPosition.y) > 5 || Mathf.Abs(mouseX - hdgClickPosition.x) > 5)
            {
                hdgPossibleClick = false;
            }
            else
            {
                return;
            }

            float compassX = compass.rectTransform.position.x;
            float compassY = compass.rectTransform.position.y;

            //Trouver l'angle p/r la position centrale de la boussole entre la position du clic et la position actuelle du doigt
            float angle = Vector3.SignedAngle(new Vector3(hdgClickPosition.x - compassX, hdgClickPosition.y - compassY),
                new Vector3(mouseX - compassX, mouseY - compassY),
                new Vector3(0, 0, 1));

            //Suite d'opérations un peu floues...
            angle = hdg.currentHdg + angle;
            
            if (angle < 0)
            {
                angle = (360 + angle);
            }

            angle = angle % 360;

            hdg.editTarget(Mathf.RoundToInt((initialCompassRotation + angle)) % 360);
        }
        else if (baroTapeHeld)
        {
            if (Mathf.Abs(mouseY - baroClickPosition) > 5)
            {
                baroPossibleClick = false;
            }
            else
            {
                return;
            }

            //if (Time.time - altTimer > 0.01f) //Souris
            if (Time.time - baroTimer > 0.01f) //Touch
            {
                //float difference = (mouseY - lastAltPosition) / (float)0.7; //Souris
                float difference = (mouseY - lastBaroPosition) / (float)4;

                float oldAlt = alt.targetAlt;

                //-difference car le sens est invers�
                alt.targetAlt = Mathf.Clamp(alt.targetAlt - difference, 0, 20000);
                alt.currentAlt = alt.targetAlt;
                moveAltTape();

                baro.currentBaro = (alt.targetAlt - oldAlt) / 1200 + baro.currentBaro;

                lastBaroPosition = mouseY;
                baroTimer = Time.time;
            }

            if (Time.time - baroBuffer > 0.15f)
            {
                baroBufferDisplacement = mouseY;
                baroBuffer = Time.time;
            }
        }
        else if (vsTapeHeld)
        {
            if (Mathf.Abs(mouseY - vsClickPosition) > 5)
            {
                vsPossibleClick = false;
            }
            else
            {
                return;
            }

            //if (Time.time - altTimer > 0.01f) //Souris
            if (Time.time - vsTimer > 0.01f) //Touch
            {
                //float difference = (mouseY - lastAltPosition) / (float)0.7; //Souris
                float difference = (mouseY - lastVsPosition) / (float)0.09;

                //-difference car le sens est invers�
                vs.editTarget(vs.targetVs - difference);
                moveVsTape();

                lastVsPosition = mouseY;
                vsTimer = Time.time;
            }

            if (Time.time - vsBuffer > 0.15f)
            {
                vsBufferDisplacement = mouseY;
                vsBuffer = Time.time;
            }
        }
        else if (altTapeHeld)
        {
            if (Mathf.Abs(mouseY - altClickPosition) > 5)
            {
                altPossibleClick = false;
            }
            else
            {
                return;
            }

            //if (Time.time - altTimer > 0.01f) //Souris
            if (Time.time - altTimer > 0.01f) //Touch
            {
                //float difference = (mouseY - lastAltPosition) / (float)0.7; //Souris
                float difference = (mouseY - lastAltPosition) / (float)1.2;

                //-difference car le sens est invers�
                alt.editTarget(alt.targetAlt - difference);
                moveAltTape();

                lastAltPosition = mouseY;
                altTimer = Time.time;
            }

            if (Time.time - altBuffer > 0.15f)
            {
                altBufferDisplacement = mouseY;
                altBuffer = Time.time;
            }
        }
        else if (speedTapeHeld)
        {
            if (Mathf.Abs(mouseY - speedClickPosition) > 5)
            {
                speedPossibleClick = false;
            }
            else
            {
                return;
            }

            //if (Time.time - speedTimer > 0.01f) //Souris
            if (Time.time - speedTimer > 0.01f) //Touch
            {
                //Calculer la diff�rence entre l'ancienne position et la nouvelle
                //float difference = (mouseY - lastSpeedPosition) / (float)0.7; //Souris
                float difference = (mouseY - lastSpeedPosition) / (float)7.2;

                //-difference car le sens est invers�
                speed.editTarget(speed.targetSpeed - difference);
                moveSpeedTape();

                lastSpeedPosition = mouseY;
                speedTimer = Time.time;
            }

            if (Time.time - speedBuffer > 0.15f)
            {
                speedBufferDisplacement = mouseY;
                speedBuffer = Time.time;
            }
        }
    }

    private void OnMouseUp()
    {
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        if (wrongTape)
        {
            wrongTape = false;
            return;
        }

        if (hdgCompassHeld)
        {
            hdgCompassHeld = false;

            if (hdgPossibleClick)
            {
                float position = compass.rectTransform.position.x;

                if (mouseX > position)
                {
                    global.incrementValue(4, 1, 0);
                }
                else if (mouseX < position)
                {
                    global.incrementValue(4, -1, 0);
                }

                return;
            }
        }
        else if (baroTapeHeld)
        {
            baroTapeHeld = false;

            //Si c'est un clic
            if (baroPossibleClick)
            {
                if (mouseY > altBg.rectTransform.position.y)
                {
                    float oldAlt = alt.targetAlt;

                    alt.targetAlt = Mathf.Clamp(alt.targetAlt + 12, 0, 20000);
                    alt.currentAlt = alt.targetAlt;
                    moveAltTape();

                    baro.currentBaro = (alt.targetAlt - oldAlt) / 1200 + baro.currentBaro;
                }
                else if (mouseY < altBg.rectTransform.position.y)
                {
                    float oldAlt = alt.targetAlt;

                    alt.targetAlt = Mathf.Clamp(alt.targetAlt - 12, 0, 20000);
                    alt.currentAlt = alt.targetAlt;
                    moveAltTape();

                    baro.currentBaro = (alt.targetAlt - oldAlt) / 1200 + baro.currentBaro;
                }

                moveAltTape();

                return;
            }

            //1 ft = 0.7 pixel
            float baroSpeed;
            float difference = mouseY - baroBufferDisplacement;

            baroSpeed = difference / (float)6;

            //Debug.Log(altSpeed);

            StartCoroutine(baroTapeFlick(baroSpeed));
        }
        else if (vsTapeHeld)
        {
            vsTapeHeld = false;

            //Si c'est un clic
            if (vsPossibleClick)
            {
                if (mouseY > vsBg.rectTransform.position.y)
                {
                    global.incrementValue(2, 1, 0);
                }
                else if (mouseY < vsBg.rectTransform.position.y)
                {
                    global.incrementValue(2, -1, 0);
                }

                moveVsTape();

                return;
            }

            //1 ft = 0.7 pixel
            float vsSpeed;
            float difference = mouseY - vsBufferDisplacement;

            vsSpeed = difference / (float)0.08;

            //Debug.Log(altSpeed);

            StartCoroutine(vsTapeFlick(vsSpeed));
        }
        else if (altTapeHeld)
        {
            altTapeHeld = false;

            //Si c'est un clic
            if (altPossibleClick)
            {
                if (mouseY > altBg.rectTransform.position.y)
                {
                    global.incrementValue(1, 1, 0);
                }
                else if (mouseY < altBg.rectTransform.position.y)
                {
                    global.incrementValue(1, -1, 0);
                }

                moveAltTape();

                return;
            }

            //1 ft = 0.7 pixel
            float altSpeed;
            float difference = mouseY - altBufferDisplacement;

            altSpeed = difference / (float)6;

            //Debug.Log(altSpeed);

            StartCoroutine(altTapeFlick(altSpeed));
        }
        else if (speedTapeHeld)
        {
            speedTapeHeld = false;

            //Si c'est un clic
            if (speedPossibleClick)
            {
                if (mouseY > speedBg.rectTransform.position.y)
                {
                    global.incrementValue(0, 1, 0);
                }
                else if (mouseY < speedBg.rectTransform.position.y)
                {
                    global.incrementValue(0, -1, 0);
                }

                moveSpeedTape();

                return;
            }

            //1 knt = 7.2 pixel
            float speedSpeed;
            float difference = mouseY - speedBufferDisplacement;

            speedSpeed = difference / (float)3;

            //Debug.Log(altSpeed);

            StartCoroutine(speedTapeFlick(speedSpeed));
        }
    }

    void Start()
    {
        vs = GameObject.Find("Canvas/Vs Tape").GetComponent<VsTape>();
        speed = GameObject.Find("Canvas/Speed Tape").GetComponent<SpeedTape>();
        alt = GameObject.Find("Canvas/Alt Tape").GetComponent<AltTape>();
        hdg = GameObject.Find("Canvas/Heading").GetComponent<Hdg>();
        baro = GameObject.Find("Canvas/Baro").GetComponent<BaroBox>();

        compass = GameObject.Find("Canvas/Heading/Moving").GetComponent<Image>();
        hdgBox = GameObject.Find("Canvas/Heading/Hdg Target Box").GetComponent<Image>();

        baroBox = GameObject.Find("Canvas/Baro/Baro Box").GetComponent<Image>();

        vsBg = GameObject.Find("Canvas/Vs Tape/VsBg").GetComponent<Image>();
        vsBox = GameObject.Find("Canvas/Vs Tape/Vs Target Box").GetComponent<Image>();
        vsPointer = GameObject.Find("Canvas/Vs Tape/Vs Pointer").GetComponent<Image>();
        vsTape = GameObject.Find("Canvas/Vs Tape/Moving").GetComponent<Image>();

        altBg = GameObject.Find("Canvas/Alt Tape/AltBg").GetComponent<Image>();
        altBox = GameObject.Find("Canvas/Alt Tape/Alt Target Box").GetComponent<Image>();
        altTape = GameObject.Find("Canvas/Alt Tape/Moving").GetComponent<Image>();
        altPointer = GameObject.Find("Canvas/Alt Tape/Alt Pointer").GetComponent<Image>();

        speedBg = GameObject.Find("Canvas/Speed Tape/SpeedBg").GetComponent<Image>();
        speedBox = GameObject.Find("Canvas/Speed Tape/Speed Target Box").GetComponent<Image>();
        speedTape = GameObject.Find("Canvas/Speed Tape/Moving").GetComponent<Image>();
        speedPointer = GameObject.Find("Canvas/Speed Tape/Speed Pointer").GetComponent<Image>();

        global = GameObject.Find("Global").GetComponent<Global>();
        dataManager = GameObject.Find("Global").GetComponent<DataManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (firstFrame)
        {
            global.highlightedField = -1;
            firstFrame = false;
        }
    }
}
