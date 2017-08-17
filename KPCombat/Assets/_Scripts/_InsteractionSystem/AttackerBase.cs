﻿using System;
using UnityEngine;

[RequireComponent(typeof(InteractionCollider))]
public class AttackerBase : Interaction
{
    public float BaseDamage { get; private set; }
    public float KnockBackAmount { get; private set; }

    public AttackInteractionInfo LatestAttackInfo { get; private set; }

    BattleResult _battleResult;

    #region Events

    public Action<DamagableBase, AttackInteractionInfo> OnAttackReflected;

    void FireOnAttackReflected(DamagableBase damagable, AttackInteractionInfo attackInfo)
    {
        if (OnAttackReflected != null)
            OnAttackReflected(damagable, attackInfo);
    }

    public Action<DamagableBase, AttackInteractionInfo> OnDamageGiven;

    void FireOnDamageGiven(DamagableBase damagable, AttackInteractionInfo attackInfo)
    {
        if (OnDamageGiven != null)
            OnDamageGiven(damagable, attackInfo);
    }

    #endregion

    protected override void SetInteractionType()
    {
        InteractionType = InteractionType.Combat;
    }

    public void SetAttackParameters(float baseDamage, float knockBackAmount)
    {
        BaseDamage = baseDamage;
        KnockBackAmount = knockBackAmount;
    }

    public override void InteractWithInteractable(Reaction target)
    {
        FireOnPreInteract(target);

        if (IsDebugActive)
            Debug.Log("<color=green>" + gameObject.name + " interact with: " + target.gameObject.name + "</color>");

        _battleResult = new BattleResult(this, (DamagableBase)target);

        BattleCalculator.CalculateBattleResult(ref _battleResult);
		
        if (IsDebugActive)
			Debug.Log("result damage: " + _battleResult.DamageAmount);

        object message = _battleResult;

        LatestAttackInfo = (AttackInteractionInfo)GetInteractionInfo(message);

        target.React(this, LatestAttackInfo);

        InteractionCompleted();
    }

    protected override InteractionInfo GetInteractionInfo(object message)
    {
        AttackInteractionInfo ai = new AttackInteractionInfo((BattleResult)message);

        ai.KnockBackGridAmout = KnockBackAmount;

        return ai;
    }

    protected override void InteractionCompleted()
    {
        base.InteractionCompleted();

        if (_battleResult == null)
            return;

        if (_battleResult.IsDamageGiven)
            FireOnDamageGiven(_battleResult.Damagable, LatestAttackInfo);
    }
}
