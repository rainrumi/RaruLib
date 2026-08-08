using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RaruLib
{
    public class SceneLoad : MonoBehaviour
    {
        public static SceneLoad Instance { get; private set; }

        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _image;

        // ロードが開始した時のイベント
        private Subject<Unit> LoadStartSubject = new Subject<Unit>();
        public Observable<Unit> OnLoadStart => LoadStartSubject;

        // ロードが終了した時のイベント
        private Subject<Unit> LoadFinishSubject = new Subject<Unit>();
        public Observable<Unit> OnLoadFinish => LoadFinishSubject;

        private void Awake()
        {
            if (Instance != null) { Debug.Log("SceneLoadが重複しています"); Destroy(gameObject); return; }
            Instance = this;

            _canvas.enabled = false;
        }

        public async UniTask SceneLoad_Fade(string name, float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f)
        {
            var color = _image.color;
            color.a = 0;

            LoadStartSubject.OnNext(Unit.Default);

            _canvas.enabled = true;
            await UniTask.Yield();

            _image.DOComplete();
            await _image.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);

            SceneManager.LoadScene(name);

            _image.DOComplete();
            await _image.DOFade(0f, fadeOutDuration).SetEase(Ease.OutQuad);

            _canvas.enabled = false;
            await UniTask.Yield();

            LoadFinishSubject.OnNext(Unit.Default);
        }

        public async UniTask SceneLoad_Fade(string name, Color fadeColor, float fadeInTime = 0.5f, float fadeOutTime = 0.5f)
        {
            _image.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);

            LoadStartSubject.OnNext(Unit.Default);

            _canvas.enabled = true;
            await UniTask.Yield();

            _image.DOComplete();
            await _image.DOFade(1f, fadeInTime).SetEase(Ease.OutQuad);

            SceneManager.LoadScene(name);

            _image.DOComplete();
            await _image.DOFade(0f, fadeOutTime).SetEase(Ease.OutQuad);

            _canvas.enabled = false;
            await UniTask.Yield();

            LoadFinishSubject.OnNext(Unit.Default);
        }
    }
}