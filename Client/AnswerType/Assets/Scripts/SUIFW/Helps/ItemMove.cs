using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    private IEnumerator Move( Vector3 targetPos, float time, bool isHide )
    {
        Vector3 startPos = transform.position;
        float dur = 0f;
        while (dur < time)
        {
            dur += Time.deltaTime;
            if (dur > time)
            {
                dur = time;
            }
            transform.position = Vector3.Lerp(startPos, targetPos, dur/time);
            yield return null;
        }
        if(isHide)
        {
            transform.gameObject.SetActive(false);
        }
    }

    public void MoveTo(Vector3 targetPos, float time, bool isHide = false)
    {
        StopAllCoroutines();
        StartCoroutine(Move(targetPos, time, isHide));
    }

    private IEnumerator Move2(Vector3 targetPos, float time, bool isHide)
    {
        Vector3 startPos = transform.position;
        float dur = 0f;
        while (dur < time)
        {
            dur += Time.deltaTime;
            if (dur > time)
            {
                dur = time;
            }
            transform.position = Vector3.Lerp(transform.position, targetPos, 1f/10f);
            yield return null;
        }
        if (isHide)
        {
            transform.gameObject.SetActive(false);
        }
    }

    public void MoveTo2(Vector3 targetPos, float time, bool isHide = false)
    {
        StopAllCoroutines();
        StartCoroutine(Move2(targetPos, time, isHide));
    }


}
