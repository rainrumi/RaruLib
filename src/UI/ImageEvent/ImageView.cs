using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RaruLib
{
    [RequireComponent(typeof(Image))]
    public class ImageView : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Subject<Unit> OnEnterSubject = new();
        public Observable<Unit> OnEnter => OnEnterSubject.AsObservable();

        private Subject<Unit> OnExitSubject = new();
        public Observable<Unit> OnExit => OnExitSubject.AsObservable();

        private Subject<Unit> OnDownSubject = new();
        public Observable<Unit> OnDown => OnDownSubject.AsObservable();

        private Subject<Unit> OnUpSubject = new();
        public Observable<Unit> OnUp => OnUpSubject.AsObservable();

        private Subject<Unit> OnHoldSubject = new();
        public Observable<Unit> OnHold => OnHoldSubject.AsObservable();

        private Image image;
        private bool pressing = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnterSubject.OnNext(Unit.Default);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExitSubject.OnNext(Unit.Default);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDownSubject.OnNext(Unit.Default);
            pressing = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnUpSubject.OnNext(Unit.Default);
            pressing = false;
        }

        private void OnPointerHold()
        {
            OnHoldSubject.OnNext(Unit.Default);
        }

        public void SetVisible(bool set)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, set ? 1 : 0);
            image.raycastTarget = set;
        }

        private void Awake()
        {
            image = gameObject.GetComponent<Image>();
        }

        private void Update()
        {
            if (pressing) OnPointerHold();
        }
    }
}