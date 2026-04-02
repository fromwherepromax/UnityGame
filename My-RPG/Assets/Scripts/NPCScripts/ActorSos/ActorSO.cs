using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorSO", menuName = "Diologue/NPC")]
public class ActorSO : ScriptableObject
{
    public string actorName; //角色名称
    public Sprite portrait; //角色头像
    public string[] dialogues; //角色对话内容
}