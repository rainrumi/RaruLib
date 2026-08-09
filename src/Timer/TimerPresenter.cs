using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using System;

namespace RaruLib
{
    public class TimerPresenter : MonoBehaviour
    {

        [SerializeField] private TimerModel _model;
        [SerializeField] private TimerView _view;
        [SerializeField, Header("’Z‚¢‚Æ”»’è‚³‚ê‚éŽžŠÔ")] private float alertTime = 3.0f;

        private void Start()
        {
            _model.OnUpdateTime
                .Where(time => time > 0)
                .Subscribe(time => _view.UpdateText($"{time:00.00}")).AddTo(this);

            _model.OnUpdateTime
                .Where(time => time <= alertTime)
                .Subscribe(time => _view.SetAlert(true)).AddTo(this);

            _model.OnTimeUp
                .Subscribe(_ =>
                {
                    _view.UpdateText("<size=11>TIMEUP!!</size>");
                    _view.SetAlert(false);
                }).AddTo(this);
        }
    }
}