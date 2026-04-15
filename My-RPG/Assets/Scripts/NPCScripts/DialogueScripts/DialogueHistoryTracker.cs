using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHistoryTracker : MonoBehaviour
{
    private HashSet<ActorSO> spokenNPCs = new HashSet<ActorSO>(); //已说话的角色列表

    public void RecordDialogue(ActorSO npc) //记录对话角色
    {
        if (!spokenNPCs.Contains(npc)) //如果角色不在列表中
        {
            spokenNPCs.Add(npc); //添加角色到列表
            Debug.Log("Recorded dialogue with " + npc.actorName);
        }
    }
    public bool HasSpokenWith(ActorSO npc) //检查是否已经与角色对话过
    {
        return spokenNPCs.Contains(npc); //返回角色是否在列表中
    }
}
