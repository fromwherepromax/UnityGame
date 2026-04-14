using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; //单例实例
    public CanvasGroup dialogueCanvasGroup; //对话UI的CanvasGroup组件

    [Header("UI References")]  //UI引用
    public Image portraitImage;//角色头像
    public TMP_Text actorName; //角色名字文本
    public TMP_Text dialogueText; //对话内容文本
    public bool isDialogueActive; //对话是否激活
    private DialogueSO currentDialogue; //当前对话
    private int dialogueIndex;//当前对话索引
    public Button[] optionButtons; //对话选项按钮数组

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; //设置单例实例
        }
        else
        {
            Destroy(gameObject); //如果已经存在实例，销毁当前对象
        }
        dialogueCanvasGroup.alpha = 0; //初始时隐藏对话UI
        dialogueCanvasGroup.interactable = false; //初始时禁用对话UI交互
        dialogueCanvasGroup.blocksRaycasts = false; //初始时不阻挡射线

        foreach (var button in optionButtons) //禁用所有选项按钮
        {
            button.gameObject.SetActive(false);
        }
    }
    public void StartDialogue(DialogueSO dialogueSO) //开始对话
    {   
        currentDialogue = dialogueSO; //设置当前对话
        dialogueIndex = 0; //重置对话索引
        isDialogueActive = true; //激活对话
        ShowDialogue(); //显示第一行对话
    }
    public void AdvanceDialogue() //推进对话
    {
        if (dialogueIndex < currentDialogue.dialogueLines.Length) //如果还有对话行
        {
            ShowDialogue(); //显示下一行对话
        }
        else
        {
            ShowOptions(); //显示对话选项
        }
    }
    private void ShowOptions()
    {   
        clearOptionButtons(); //清除之前的选项按钮事件
        if(currentDialogue.dialogueOptions != null && currentDialogue.dialogueOptions.Length > 0) //如果有对话选项
        {
            for (int i = 0; i < optionButtons.Length; i++) //遍历选项按钮
            {
                if (i < currentDialogue.dialogueOptions.Length) //如果当前按钮索引小于选项数量
                {   
                    var option = currentDialogue.dialogueOptions[i]; //获取当前选项
                    optionButtons[i].gameObject.SetActive(true); //显示按钮
                    optionButtons[i].GetComponentInChildren<TMP_Text>().text = currentDialogue.dialogueOptions[i].optionText; //设置按钮文本

                    optionButtons[i].onClick.AddListener(() => ChooseOption(option.nextDialogue)); //添加按钮点击事件，选择当前选项
                   
                }
                else
                {
                    optionButtons[i].gameObject.SetActive(false); //隐藏多余的按钮
                }
            }
            EventSystem.current.SetSelectedGameObject(optionButtons[0].gameObject); //设置第一个选项为当前选中
        }
        else
        {
            optionButtons[0].GetComponentInChildren<TMP_Text>().text = "End"; //设置按钮文本为继续
            optionButtons[0].onClick.AddListener(() => EndDialogue()); //添加按钮点击事件，结束对话
            optionButtons[0].gameObject.SetActive(true); //如果没有选项，显示一个默认的继续按钮
            EventSystem.current.SetSelectedGameObject(optionButtons[0].gameObject); //设置第一个选项为当前选中
        }
    }
    private void ChooseOption(DialogueSO dialogueSo)
    {
        if (dialogueSo == null)
        {
            EndDialogue(); //如果选项没有下一个对话，结束对话
        }
        else
        {   
            clearOptionButtons(); //清除选项按钮事件
            StartDialogue(dialogueSo); //开始下一个对话
        }
    }
    private void clearOptionButtons() //清除选项按钮事件
    {
        foreach (var button in optionButtons)
        {   
            button.gameObject.SetActive(false); //隐藏按钮
            button.onClick.RemoveAllListeners(); //移除所有按钮点击事件
        }
    }
    private void ShowDialogue()
    {
        DialogueLine line = currentDialogue.dialogueLines[dialogueIndex]; //获取当前对话行
        portraitImage.sprite = line.speaker.portrait; //设置角色头像
        actorName.text = line.speaker.actorName; //设置角色名字
        dialogueText.text = line.line; //设置对话内容
        dialogueCanvasGroup.alpha = 1; //显示对话UI
        dialogueCanvasGroup.interactable = true; //启用对话UI交互
        dialogueCanvasGroup.blocksRaycasts = true; //阻挡射线
        dialogueIndex++; //推进对话索引
    }
     private void EndDialogue()
    {
        isDialogueActive = false; //禁用对话
        currentDialogue = null; //清除当前对话
        dialogueIndex = 0; //重置对话索引
        portraitImage.sprite = null; //清除角色头像
        actorName.text = ""; //清除角色名字
        dialogueText.text = ""; //清除对话内容
        clearOptionButtons(); //清除之前的选项按钮事件
        dialogueCanvasGroup.alpha = 0; //隐藏对话UI
        dialogueCanvasGroup.interactable = false; //禁用对话UI交互
        dialogueCanvasGroup.blocksRaycasts = false; //不阻挡射线

    }

}
