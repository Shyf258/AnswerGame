using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideList : MonoBehaviour
{
    /// <summary>
    /// 滑动页对象组件
    /// </summary>
    private ScrollRect mScrollRect;
    
    /// <summary>
    /// 滑动页显示区
    /// </summary>
    private RectTransform mRect;
    /// <summary>
    /// 显示对象
    /// </summary>
    public RectTransform Item;
    public Direction Dir;
    public uint mCount;
    /// <summary>
    /// Item的间隔
    /// </summary>
    public float mItemInterval  ;
    /// <summary>
    /// 是否参与计算
    /// </summary>
    private bool mCalcul;
    /// <summary>
    /// 间隔
    /// </summary>
    private float mInterval;
    /// <summary>
    /// 当前最小的索引
    /// </summary>
    private uint mMinIndex;
    /// <summary>
    /// 当前最大的索引
    /// </summary>
    private uint mMaxIndex;
    /// <summary>
    /// 最大显示的数量
    /// </summary>
    private uint mMaxShowCount;

    private List<LevelItem> mList = new List<LevelItem>();

    private void Awake()
    {
        Item.gameObject.SetActive(false); // 关闭按键列表格子


        mScrollRect = this.GetComponentInParent<ScrollRect>();
        var TempRect = mScrollRect.GetComponent<RectTransform>();

        mRect = this.gameObject.GetComponent<RectTransform>();
        Vector2 size = mRect.sizeDelta;
        switch (Dir)
        {
            // case Direction.Horizontal:
            //     //设置Content的对齐方向
            //     mRect.anchorMin = new Vector2(0, 0);
            //     mRect.anchorMax = new Vector2(0, 1);
            //     //计算间隔
            //     mInterval = Item.rect.size.x + mItemInterval;
            //     //计算滑动最大区间
            //     size.x = mInterval * mCount;
            //     mCalcul = size.x > TempRect.sizeDelta.x;
            //     //计算最大显示的数量
            //     mMaxShowCount = (uint)(TempRect.rect.size.x / Item.sizeDelta.x + 2);
            //     break;
            case Direction.Vertical:
                //设置Content的对齐方向
                mRect.anchorMin = new Vector2(0, 1);
                mRect.anchorMax = new Vector2(1, 1);
                //计算间隔
                mInterval = Item.rect.size.y + mItemInterval;
                //计算滑动最大区间
                size.y = mInterval * mCount;
                mCalcul = size.y > TempRect.sizeDelta.y;
                //计算最大显示的数量
                mMaxShowCount = (uint)(TempRect.rect.size.y / Item.sizeDelta.y + 2);
                break;
            default:
                break;
        }
        mRect.sizeDelta = size;
        mRect.localPosition = Vector3.zero;
        int index = 0;
        for (int i = 0; i < mMaxShowCount; i++)
        {
            if (i > mCount)
                break;
            index++;
            GameObject TempGame = GameObject.Instantiate<GameObject>(Item.gameObject);
            TempGame.transform.SetParent(Item.parent);
            TempGame.transform.localPosition = GetPos((int)i);
            TempGame.transform.localEulerAngles = Vector3.zero;
            TempGame.transform.localScale = Vector3.one;

            TempGame.gameObject.SetActive(true);
            LevelItem tempItem = TempGame.AddComponent<LevelItem>();
            tempItem.To(0, (uint)(i));
            mMaxIndex++;
            mList.Add(tempItem);
        }
        mMinIndex = 0;
    }


    private void Update()
    {
        if (mCalcul)
        {
            float index = 0;
            switch (Dir)
            {
                // case Direction.Horizontal:
                //     index = Mathf.Abs(mRect.localPosition.x) / mInterval;
                //     break;
                case Direction.Vertical:
                    index = Mathf.Abs(mRect.localPosition.y) / mInterval;
                    break;
                default:
                    break;
            }
            if (index - mMinIndex >= 2)
            {
                if (index - mMinIndex >= mMaxShowCount)
                {
                    //滑动过快-直接设置
                    if (index <= mCount - mMaxShowCount)
                    {
                        int idx = (int)(mCount - mMaxShowCount);
                        for (int i = 0; i < mList.Count; i++)
                        {
                            var item = mList[i];

                            item.transform.localPosition = GetPos((int)(i + idx));
                            item.To(item._level, (uint)(i + idx));
                        }
                        mMaxIndex = mCount;
                        mMinIndex = mCount - mMaxShowCount;
                    }
                    else
                    {
                        for (int i = 0; i < mList.Count; i++)
                        {
                            var item = mList[i];

                            item.transform.localPosition = GetPos((int)(i + index));
                            item.To(item._level, (uint)(i + index));
                        }
                        mMinIndex = (uint)index;
                        mMaxIndex = mMinIndex + mMaxShowCount;
                    }
                }
                else if (mMaxIndex < mCount)
                {
                    int indexid = 0;
                    var item = mList[indexid];
                    mList.RemoveAt(indexid);

                    item.transform.localPosition = GetPos((int)mMaxIndex);
                    item.To(mMinIndex, mMaxIndex);
                    mMinIndex += 1;
                    mMaxIndex += 1;
                    mList.Add(item);
                }
            }
            else if (mMinIndex - index >= 0)
            {
                if (mMinIndex - index >= mMaxShowCount)
                {
                    //滑动过快-直接设置
                    for (int i = 0; i < mList.Count; i++)
                    {
                        var item = mList[i];

                        item.transform.localPosition = GetPos((int)(i + index));
                        item.To(item._level, (uint)(i + index));
                    }
                    mMinIndex = (uint)index;
                    mMaxIndex = mMinIndex + mMaxShowCount;
                }
                else if (mMinIndex > 0)
                {
                    int indexid = (int)(mList.Count - 1);
                    var item = mList[indexid];
                    mList.RemoveAt(indexid);
                    mMinIndex -= 1;
                    mMaxIndex -= 1;

                    
                    item.transform.localPosition = GetPos((int)mMinIndex);
                    item.To(mMaxIndex, mMinIndex);
                    mList.Insert((int)0, item);
                }
            }
        }
    }

    /// <summary>
    /// 坐标计算
    /// </summary>
    /// <param name="index">显示的序号 第几个 </param>
    /// <returns></returns>
    private Vector3 GetPos(int index)
    {
        Vector3 pos = Vector3.zero;
        switch (Dir)
        {
            // case Direction.Horizontal:
            //     pos = new Vector3((mInterval / 2 + (index) * mInterval), Item.localPosition.y);
            //     break;
            case Direction.Vertical:
                pos = new Vector3(Item.localPosition.x, -(mInterval / 2 + index * mInterval));
                break;
            default:
                break;
        }
        //Debug.LogError(pos);
        return pos;
    }
    
    
    /// <summary>
    /// 移动方向类型
    /// </summary>
    public enum Direction
    {
        // /// <summary>
        // /// 水平方向
        // /// </summary>
        // Horizontal,
        /// <summary>
        /// 纵向
        /// </summary>
        Vertical,
    }
}
