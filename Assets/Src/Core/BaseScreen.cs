using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class BaseScreen : MonoBehaviour
{
    protected bool isShow;

    public readonly ReactiveCommand OnShowEnd = new();
    public readonly ReactiveCommand OnHideEnd = new();

    public void InitShow()
    {
        if (isShow)
            return;

        isShow = true;

        Show();
    }

    public void InitHide()
    {
        if (!isShow)
            return;

        isShow = false;

        Hide();
    }

    protected virtual void Show() 
    {
        gameObject.SetActive(true);
        OnShowEnd.Execute();
    }

    protected virtual void Hide() 
    {
        gameObject.SetActive(false);
        OnHideEnd.Execute();
    }
}
