using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using DG.Tweening;

public enum SCENE
{
    TITLE,
    GAME,
    MAX
}

public enum SCENE_ANIM
{
    None,
    FadeIn,
    MAX
}

public class SceneController : MonoBehaviour
{
    [SerializeField] private string[] scenes = new string[(int)SCENE.MAX];
    [SerializeField] private SCENE_ANIM sceneChangeAnim = SCENE_ANIM.None;
    [SerializeField] private Image animationImg;
    [SerializeField] private float animationDuration;

    public static SceneController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {

    }

    public void SceneChange(int scene)
    {
        SceneManager.LoadScene(scenes[scene]);
    }

    public async UniTaskVoid SceneChange(SCENE_ANIM anim, int scene)
    {

        if (animationImg == null)
        {
            Debug.LogWarning("イメージが存在しません", gameObject);
            return;
        }

        await PanelAnimationStart(anim);

        SceneManager.LoadScene(scenes[scene]);

        await PanelAnimationEnd(anim);
    }

    public async UniTask PanelAnimationStart(SCENE_ANIM anim)
    {
        switch (anim)
        {
            case SCENE_ANIM.None:
                break;
            case SCENE_ANIM.FadeIn:
                animationImg.color = new Color(animationImg.color.r, animationImg.color.g, animationImg.color.b, 0);
                await animationImg.DOFade(1f, animationDuration).SetEase(Ease.OutCubic).AsyncWaitForCompletion();
                break;
        }
    }

    public async UniTask PanelAnimationEnd(SCENE_ANIM anim)
    {
        switch (anim)
        {
            case SCENE_ANIM.None:
                break;
            case SCENE_ANIM.FadeIn:
                animationImg.color = new Color(animationImg.color.r, animationImg.color.g, animationImg.color.b, 1);
                await animationImg.DOFade(0f, animationDuration).SetEase(Ease.OutCubic).AsyncWaitForCompletion();
                break;
        }
    }

    public void SceneReLoad()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public async UniTaskVoid SceneReLoad(SCENE_ANIM anim)
    {

        if (animationImg == null)
        {
            Debug.LogWarning("イメージが存在しません", gameObject);
            return;
        }

        await PanelAnimationStart(anim);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        await PanelAnimationEnd(anim);
    }

}
