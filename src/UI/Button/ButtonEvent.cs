using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace RaruLib
{
    [RequireComponent(typeof(Button))]
    public class ButtonEvent : MonoBehaviour
    {
        private Subject<Unit> clickSubject = new Subject<Unit>();
        public Observable<Unit> OnClickEvent => clickSubject;

        public void OnClick() { clickSubject.OnNext(Unit.Default); }
    }
}