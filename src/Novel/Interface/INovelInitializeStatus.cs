namespace RaruLib
{
    public interface INovelInitializeStatus
    {
        float CharSpeed { get; }
        float MaxCharWait { get; }
        float AutoWait { get; }
        float AutoWaitPerChar { get; }
        bool IsAuto { get; }
        string Popopo { get; }
        float StandSlideDistance { get; }
        float StandJumpHeight { get; }
        float StandShakeDistance { get; }
    }
}