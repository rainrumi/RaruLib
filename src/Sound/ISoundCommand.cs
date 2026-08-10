public interface ISoundCommand
{
    void CallPlayBGM(string name);
    void CallStopBGM(string name);
    void CallPlaySE(string name);
    void CallStopSE(string name);
    void CallPlayBGM(string name, float duration);
    void CallStopBGM(string name, float duration);
    void CallPlaySE(string name, float duration);
    void CallStopSE(string name, float duration);

}