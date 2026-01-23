using RaruLib;
using UnityEngine;

public class SoundCommand : MonoBehaviour
{
    /**************************************************
     * 
     * ***********************************************/

    public void CallPlayBGM(string name)
    {
        if (this == null) return;
        Sound.instance.Play("BGM", name);
    }

    public void CallStopBGM(string name)
    {
        if (this == null) return;
        Sound.instance.Stop("BGM", name);
    }

    /**************************************************
     * 
     * ***********************************************/

    public void CallPlaySE(string name)
    {
        if (this == null) return;
        Sound.instance.Play("SE", name);
    }

    public void CallStopSE(string name)
    {
        if (this == null) return;
        Sound.instance.Stop("SE", name);
    }

    /**************************************************
     * 
     * ***********************************************/
}