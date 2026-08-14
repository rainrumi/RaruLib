namespace RaruLib
{
    public interface INovelLoader
    {
        byte[] LoadPreamble();
        byte[] LoadScenario(string key);
    }
}