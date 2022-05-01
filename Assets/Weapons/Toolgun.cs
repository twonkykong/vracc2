using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolgun : MonoBehaviour
{
    public bool SecondFunc;

    [SerializeField] private ToolgunFunc _toolgunFunc;
    [SerializeField] private Text _toolgunText;

    [SerializeField] private bool _secondFuncSwitchesFast;

    public void SetFunc(ToolgunFunc tFunc)
    {
        _toolgunFunc = tFunc;
        _secondFuncSwitchesFast = tFunc.SecondFuncSwitchesFast;
        _toolgunText.text = tFunc.name;
    }

    public void Work()
    {
        if (!SecondFunc) _toolgunFunc.MainEvent?.Invoke();
        else
        {
            _toolgunFunc.MainSecondEvent?.Invoke();
            if (_secondFuncSwitchesFast) SecondFunc = false;
        }
    }

    public void AltWork()
    {
        if (!SecondFunc) _toolgunFunc.AltEvent?.Invoke();
        else
        {
            _toolgunFunc.MainSecondAltEvent?.Invoke();
            if (_secondFuncSwitchesFast) SecondFunc = false;
        }
    }
}
