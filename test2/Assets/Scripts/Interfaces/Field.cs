using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Field
{
    public void editTarget(float value, bool overshoot = true);
    public int toggleMode();
    public int confirmTarget();
    public void restoreOldTarget();
    public float getCurrentValue();
    public int getMode();
}
