using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Change : MonoBehaviour
{
    public player_Combat combat;
    public Player_Bow bow;
    public Player_Monk monk;
    public Player_Caster caster;
    public PlayerPortraitUI portraitUI;
    public SkillCoolDownUI skillCoolDownUI;
    public Animator anim;
    public float switchCooldown = 0.5f;
    private float switchTimer;

    private enum PlayerMode { Combat, Bow, Monk, Caster }
    private PlayerMode currentMode = PlayerMode.Combat;

    void Update()
    {
        if (switchTimer > 0)
        {
            switchTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("ChangeEquipment") && switchTimer <= 0)
        {
            switchTimer = switchCooldown;
            currentMode = (PlayerMode)(((int)currentMode + 1) % 4);
            ApplyMode();
        }
    }

    private void ApplyMode()
    {
        ResetAnimatorStates();

        if (combat != null) combat.enabled = (currentMode == PlayerMode.Combat);
        if (bow != null) bow.enabled = (currentMode == PlayerMode.Bow);
        if (monk != null) monk.enabled = (currentMode == PlayerMode.Monk);
        if (caster != null) caster.enabled = (currentMode == PlayerMode.Caster);
        if (portraitUI != null) portraitUI.UpdatePortrait((int)currentMode);
        if (skillCoolDownUI != null) skillCoolDownUI.OnClassChanged((int)currentMode);
    }

    private void ResetAnimatorStates()
    {
        if (anim == null) return;
        anim.SetBool("isAttacking", false);
        anim.SetBool("isShooting", false);
        anim.SetBool("isHealing", false);
        anim.SetBool("isCasting", false);
    }
}
