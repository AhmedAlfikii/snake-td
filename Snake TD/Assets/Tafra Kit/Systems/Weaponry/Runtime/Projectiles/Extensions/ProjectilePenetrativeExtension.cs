using System;
using System.Collections.Generic;
using TafraKit.Healthies;
using TafraKit.Mathematics;
using TafraKit.Weaponry;
using UnityEngine;

[Serializable]
public class ProjectilePenetrativeExtension : ProjectileExtension
{
    public int penetrationsCount = 2;

    [Tooltip("x = hitsCount, y = damage factor (1 means full damage)")]
    public FormulasContainer damageFactorFormula;
    [Tooltip("Deactive projectile if damage factor <= this threshold.")]
    public float minDamageFactor = 0.15f;

    private int remainingPenetrationsCount;
    private bool resetPenetratesCounterOnInitialize;
    private readonly HashSet<Healthy> impactedTargets = new HashSet<Healthy>();
    private float hitsCount = 0;
    private float currentDamageFactor;

    protected override void OnBind()
    {
        base.OnBind();

        projectile.OnInitialized.AddListener(OnProjectileInitialized);
        projectile.OnWouldHitTarget.AddListener(OnProjectileWouldHitTarget);
        projectile.OnWouldDie.AddListener(OnProjectileWouldDie);
        projectile.OnDespawn.AddListener(OnProjectileDespawns);

        remainingPenetrationsCount = penetrationsCount;

        hitsCount = 0;

        resetPenetratesCounterOnInitialize = true;
    }
    protected override void OnUnbind()
    {
        base.OnUnbind();

        projectile.OnInitialized.RemoveListener(OnProjectileInitialized);
        projectile.OnWouldHitTarget.RemoveListener(OnProjectileWouldHitTarget);
        projectile.OnWouldDie.RemoveListener(OnProjectileWouldDie);
        projectile.OnDespawn.RemoveListener(OnProjectileDespawns);
    }
    private void OnProjectileInitialized()
    {
        if (resetPenetratesCounterOnInitialize)
        {
            remainingPenetrationsCount = penetrationsCount;
            hitsCount = 0;
        }
    }
    private void OnProjectileWouldHitTarget()
    {
        if(impactedTargets.Contains(projectile.ImpactedTarget))
        {
            projectile.PreventHittingTarget();

            return;
        }

        Vector3 forwardDir = projectile.transform.forward;

        forwardDir.y = 0;

        projectile.transform.forward = forwardDir;

        impactedTargets.Add(projectile.ImpactedTarget);

        HitInfo newHitInfo = projectile.HitInfo;

        currentDamageFactor = damageFactorFormula.Evaluate(hitsCount);

        currentDamageFactor = Mathf.Clamp(currentDamageFactor, minDamageFactor, 1);

        newHitInfo.damage = newHitInfo.originalDamage * currentDamageFactor;

        projectile.HitInfo = newHitInfo;
    }
    private void OnProjectileWouldDie()
    {
        if(currentDamageFactor <= minDamageFactor)
        {
            resetPenetratesCounterOnInitialize = false;

            return;
        }

        hitsCount++;

        if (penetrationsCount != -1)
        {
            remainingPenetrationsCount--;

            if (remainingPenetrationsCount < 0)
            {
                resetPenetratesCounterOnInitialize = false;
                return;
            }
        }

        projectile.PreventDeath();
    }
    private void OnProjectileDespawns()
    {
        ClearImpactedTargets();

        Unbind();
    }

    public void SetData(ProjectilePenetrativeExtension other)
    { 
        damageFactorFormula = other.damageFactorFormula;
        minDamageFactor = other.minDamageFactor;
    }
    public void ClearImpactedTargets()
    {
        impactedTargets.Clear();
    }

    public override void CopyTo(ProjectileExtension other)
    {
        if(other is ProjectilePenetrativeExtension penetrative)
            penetrative.SetData(this);
    }
}
