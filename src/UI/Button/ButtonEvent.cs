using System;
using R3;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    private Subject<Unit> clickSubject = new Subject<Unit>();
    public Observable<Unit> OnClickEvent => clickSubject;

    public void OnClick() { clickSubject.OnNext(Unit.Default); }
}
