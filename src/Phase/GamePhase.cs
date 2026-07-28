using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using RaruLib;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SoundCommand))]
public class GamePhase : MonoBehaviour
{
    /*
    private GameSceneData gsd => GameSceneData.Instance;
    private SoundCommand _soundCommand;

    private CancellationTokenSource wave_ct;

    private void Start()
    {
        if (gsd == null) Debug.Log("ないよ！");
        gsd.onChangePhase
            .Subscribe
            (
            phaseKind =>
            {
                EventForPhaseStart(phaseKind).Forget();
            }
            ).AddTo(this);

        _soundCommand = gameObject.GetComponent<SoundCommand>();

        gsd.SetPhase(GamePhaseKind.Initialization);
    }

    private async UniTask EventForPhaseStart(GamePhaseKind kind)
    {
        var token = this.GetCancellationTokenOnDestroy();

        switch (kind)
        {
            case GamePhaseKind.Initialization:      // 初期化

                gsd.SetPhase(GamePhaseKind.PreGameCountdown);
                break;

            case GamePhaseKind.PreGameCountdown:    // スタート前準備

                gsd.SetPhase(GamePhaseKind.GameStart);
                break;
        }
    }

    private void OnDestroy()
    {
        wave_ct?.Cancel();
        wave_ct?.Dispose();
        wave_ct = null;
    }
    */

}
