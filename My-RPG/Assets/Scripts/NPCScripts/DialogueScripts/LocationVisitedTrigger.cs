using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationVisitedTrigger : MonoBehaviour
{
    [SerializeField] private LocationSo LocationVisited; //要记录的位置SO
    [SerializeField] private bool destroyOnTouch = true; //是否在触发后销毁对象

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) //如果碰撞对象是玩家
        {
            LocationHistoryTracker.Instance.RecordLocation(LocationVisited); //记录访问的位置
            Debug.Log("Player visited " + LocationVisited.displayName);
            if (destroyOnTouch)
            {
                Destroy(gameObject); //销毁触发器对象，防止重复记录
            }
        }
    }
}
