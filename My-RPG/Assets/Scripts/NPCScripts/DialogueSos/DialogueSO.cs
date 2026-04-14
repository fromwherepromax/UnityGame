using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Diologue/DialogueNode")] //创建一个ScriptableObject，命名为DialogueSO，并在Unity编辑器中添加一个菜单项来创建这个ScriptableObject
public class DialogueSO : ScriptableObject
{
    public DialogueLine[] dialogueLines; //对话行数组
    public DialogueOption[] dialogueOptions; //对话选项数组
}



[System.Serializable] 
public class DialogueLine
{
    public ActorSO speaker; //说话的角色

    [TextArea(3, 5)]
    public string line; //对话内容
}

[System.Serializable]
public class DialogueOption
{
    public string optionText; //选项文本
    public DialogueSO nextDialogue; //选择这个选项后进入的下一个对话
}