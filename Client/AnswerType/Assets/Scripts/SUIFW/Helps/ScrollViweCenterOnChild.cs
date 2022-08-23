using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 
/// 拖动ScrollRect结束时始终让一个子物体位于中心位置。
/// 
/// </summary>
public class ScrollViweCenterOnChild : MonoBehaviour, IEndDragHandler, IDragHandler,IBeginDragHandler
{
    //将子物体拉到中心位置时的速度
    public float centerDur = 0.5f;

    //注册该事件获取当拖动结束时位于中心位置的子物体
    public delegate void OnCenterHandler(GameObject centerChild);
    public event OnCenterHandler onCenter;

    private ScrollRect _scrollView;
    private Transform _container;

    private List<float> _childrenPos = new List<float>();
    private float _targetPos;
    private bool _centering = false;
    private float _centerTimer = 0;
    private Vector3 _startCenterV;
    private int curChildIndex = 0;

    private float dargTimer = 0;

    public int CurChildIndex { get => curChildIndex; }

    public void Init()
    {
        _scrollView = GetComponent<ScrollRect>();
        if (_scrollView == null)
        {
            DDebug.LogError("CenterOnChild: No ScrollRect");
            return;
        }
        _container = _scrollView.content;


        GridLayoutGroup grid;
        grid = _container.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            DDebug.LogError("CenterOnChild: No GridLayoutGroup on the ScrollRect's content");
            return;
        }

        _scrollView.movementType = ScrollRect.MovementType.Unrestricted;

        //计算第一个子物体位于中心时的位置
        float childPosX = _scrollView.GetComponent<RectTransform>().rect.width * 0.5f - grid.cellSize.x * 0.5f;
        _childrenPos.Add(childPosX);
        //缓存所有子物体位于中心时的位置
        for (int i = 0; i < _container.childCount - 1; i++)
        {
            childPosX -= grid.cellSize.x + grid.spacing.x;
            _childrenPos.Add(childPosX);
        }
       
        //Invoke("SetFirst", 0.02f);
    }

    void SetFirst()
    {
        _targetPos = FindClosestPos(_container.localPosition.x ,null);
        _container.localPosition = new Vector3(_targetPos, _container.localPosition.y, _container.localPosition.z);
    }

    void Update()
    {
        if (_centering)
        {
            if (_centerTimer <= centerDur)
            {
                _container.localPosition = new Vector3(Mathf.Lerp(_startCenterV.x, _targetPos, _centerTimer / centerDur), _startCenterV.y, _startCenterV.z);
                _centerTimer += Time.deltaTime;
            }
            else
            {
                _container.localPosition = new Vector3(_targetPos, _startCenterV.y, _startCenterV.z);
                _centering = false;
                _centerTimer = 0;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dargTimer = 0;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _targetPos = FindClosestPos(_container.localPosition.x ,eventData);
        _startCenterV = _container.localPosition;
        _centering = true;

    }

    public void OnDrag(PointerEventData eventData)
    {
        _centering = false;
        dargTimer += Time.deltaTime;
    }

    private float FindClosestPos(float currentPos, PointerEventData eventData)
    {
        int childIndex = 0;
        float closest = 0;
        float distance = Mathf.Infinity;

        for (int i = 0; i < _childrenPos.Count; i++)
        {
            float p = _childrenPos[i];
            float d = Mathf.Abs(p - currentPos);
            if (d < distance)
            {
                distance = d;
                closest = p;
                childIndex = i;
            }
        }
        
        //快速滑动
        if (eventData != null && childIndex == curChildIndex)
        {
            if (dargTimer <0.2f && Vector2.Distance( eventData.pressPosition ,eventData.position )>50f)
            {
                childIndex += eventData.pressPosition.x > eventData.position.x ? 1 : -1;
            }
        }

        childIndex = Mathf.Clamp(childIndex, 0, _container.childCount - 1);
        curChildIndex = childIndex;
        GameObject centerChild = _container.GetChild(childIndex).gameObject;
        if (onCenter != null)
            onCenter(centerChild);
        closest = _childrenPos[childIndex];
        return closest;
    }

    //设置中心的子物体
    public void SetChildInIndex(int index,bool hasAnim)
    {
        curChildIndex = index;
        if(hasAnim)
            Invoke("SetCurChildPositionWithAnim", 0.02f);
        else
            Invoke("SetCurChildPosition", 0.02f);
    }
    private void SetCurChildPosition()
    {
        _targetPos = _childrenPos[curChildIndex];
        _startCenterV = _container.localPosition;
        //_centering = true;
        _container.localPosition = new Vector3(_targetPos, _startCenterV.y, _startCenterV.z);
    }

    private void SetCurChildPositionWithAnim()
    {
        _targetPos = _childrenPos[curChildIndex];
        _startCenterV = _container.localPosition;
        _centering = true;
        //_container.localPosition = new Vector3(_targetPos, _startCenterV.y, _startCenterV.z);
    }

    public GameObject GetCurCenterChild()
    {
        return _container.GetChild(curChildIndex).gameObject;
    }
}