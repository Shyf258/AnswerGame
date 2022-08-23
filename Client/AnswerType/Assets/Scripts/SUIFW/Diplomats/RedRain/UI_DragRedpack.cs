using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DragRedpack : MonoBehaviour
{
    private UI_IF_DragRedpack _ownUI;
    public void Init(UI_IF_DragRedpack dr)
    {
        _ownUI = dr;
    }

    public void OnMouseDown()
    {
        _ownUI.DoMouseDown();
    }

    public void OnMouseDrag()
    {
        _ownUI.DoMouseDrag();
    }

    public void OnMouseUp()
    {
        _ownUI.DoMouseUp();
    }
    public void OnMouseUpAsButton()
    {
        _ownUI.DoMouseUpAsButton();
    }
}
