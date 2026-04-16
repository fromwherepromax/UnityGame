using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestEvents  
{
   public static Action<QuestSO> OnQuestofferRequested; //请求提供任务事件
   public static Action<QuestSO> OnQuestTurnInRequested; //请求交付任务事件
   public static Action<QuestSO> OnQuestAccepted; //任务被接受事件
   public static Func<QuestSO, bool> IsQuestComplete; //检查任务是否完成事件
   
}
