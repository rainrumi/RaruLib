using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    private bool _onAlert = false;
    public bool OnAlert => _onAlert;

    [SerializeField] TextMeshProUGUI _timerTMP;
    [SerializeField] Animator _animator;

    public void UpdateText( string text )
    {
        _timerTMP.text = text;
    }

    public void SetAlert(bool set)
    {
        _animator.SetBool("IsAlert", set);
        _onAlert = set;
    }
}
