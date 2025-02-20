namespace RabbsRotations.Healer;

[Rotation("Rabbs AST PVP", CombatType.PvP, GameVersion = "6.58")]
public sealed class AST_PVP : AstrologianRotation
{
    [Range(4, 20, ConfigUnitType.Seconds)]
    [RotationConfig(CombatType.PvE, Name = "Use Earthly Star during countdown timer.")]
    public float UseEarthlyStarTime { get; set; } = 15;

    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime < MaleficPvE.Info.CastTime + CountDownAhead
            && MaleficPvE.CanUse(out var act)) return act;
        if (remainTime < 3 && UseBurstMedicine(out act)) return act;
        if (remainTime is < 4 and > 3 && AspectedBeneficPvE.CanUse(out act)) return act;
        if (remainTime < UseEarthlyStarTime
            && EarthlyStarPvE.CanUse(out act)) return act;
        if (remainTime < 30 && DrawPvE.CanUse(out act)) return act;

        return base.CountDownAction(remainTime);
    }

    [RotationDesc(ActionID.CelestialIntersectionPvE, ActionID.ExaltationPvE)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (CelestialIntersectionPvE.CanUse(out act, usedUp:true)) return true;
        if (ExaltationPvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.MacrocosmosPvE)]
    protected override bool DefenseAreaGCD(out IAction? act)
    {
        act = null;
        if (MacrocosmosPvE.Cooldown.IsCoolingDown && !MacrocosmosPvE.Cooldown.WillHaveOneCharge(150)
            || CollectiveUnconsciousPvE.Cooldown.IsCoolingDown && !CollectiveUnconsciousPvE.Cooldown.WillHaveOneCharge(40)) return false;

        if (MacrocosmosPvE.CanUse(out act)) return true;
        return base.DefenseAreaGCD(out act);
    }

    [RotationDesc(ActionID.CollectiveUnconsciousPvE)]
    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (MacrocosmosPvE.Cooldown.IsCoolingDown && !MacrocosmosPvE.Cooldown.WillHaveOneCharge(150)
            || CollectiveUnconsciousPvE.Cooldown.IsCoolingDown && !CollectiveUnconsciousPvE.Cooldown.WillHaveOneCharge(40)) return false;

        if (CollectiveUnconsciousPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        ////Add AspectedBeneficwhen not in combat.
        //if (NotInCombatDelay && AspectedBeneficDefensePvE.CanUse(out act)) return true;

        if (GravityPvE.CanUse(out act)) return true;

        if (CombustPvE.CanUse(out act)) return true;
        if (MaleficPvE.CanUse(out act)) return true;
        if (CombustPvE.CanUse(out act, skipStatusProvideCheck: true)) return true;

        return base.GeneralGCD(out act);
    }

    [RotationDesc(ActionID.AspectedHeliosPvE, ActionID.HeliosPvE)]
    protected override bool HealAreaGCD(out IAction? act)
    {
        if (AspectedHeliosPvE.CanUse(out act)) return true;
        if (HeliosPvE.CanUse(out act)) return true;
        return base.HealAreaGCD(out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (base.EmergencyAbility(nextGCD, out act)) return true;

        if (!InCombat) return false;

        if (nextGCD.IsTheSameTo(true, AspectedHeliosPvE, HeliosPvE))
        {
            if (HoroscopePvE.CanUse(out act)) return true;
            if (NeutralSectPvE.CanUse(out act)) return true;
        }

        if (nextGCD.IsTheSameTo(true, BeneficPvE, BeneficIiPvE, AspectedBeneficPvE))
        {
            if (SynastryPvE.CanUse(out act)) return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {
        if (DrawPvE.CanUse(out act)) return true;
        if (RedrawPvE.CanUse(out act)) return true;
        return base.GeneralAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.AspectedBeneficPvE, ActionID.BeneficIiPvE, ActionID.BeneficPvE)]
    protected override bool HealSingleGCD(out IAction? act)
    {
        if (AspectedBeneficPvE.CanUse(out act)
            && (IsMoving
            || AspectedBeneficPvE.Target.Target?.GetHealthRatio() > 0.4)) return true;

        if (BeneficIiPvE.CanUse(out act)) return true;
        if (BeneficPvE.CanUse(out act)) return true;

        return base.HealSingleGCD(out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (IsBurst && !IsMoving
            && DivinationPvE.CanUse(out act)) return true;

        if (MinorArcanaPvE.CanUse(out act, usedUp:true)) return true;

        if (DrawPvE.CanUse(out act, usedUp: IsBurst)) return true;

        if (InCombat)
        {
            if (IsMoving && LightspeedPvE.CanUse(out act)) return true;

            if (!IsMoving)
            {
                if (!Player.HasStatus(true, StatusID.EarthlyDominance, StatusID.GiantDominance))
                {
                    if (EarthlyStarPvE.CanUse(out act)) return true;
                }
                if (AstrodynePvE.CanUse(out act)) return true;
            }

            if (DrawnCrownCard == CardType.LORD || MinorArcanaPvE.Cooldown.WillHaveOneChargeGCD(1, 0))
            {
                if (MinorArcanaPvE.CanUse(out act)) return true;
            }
        }

        if (RedrawPvE.CanUse(out act)) return true;
        if (InCombat && PlayCard(out act)) return true;

        return base.AttackAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.EssentialDignityPvE, ActionID.CelestialIntersectionPvE, ActionID.CelestialOppositionPvE,
        ActionID.EarthlyStarPvE, ActionID.HoroscopePvE)]
    protected override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (EssentialDignityPvE.CanUse(out act)) return true;
        if (CelestialIntersectionPvE.CanUse(out act, usedUp:true)) return true;

        if (DrawnCrownCard == CardType.LADY 
            && MinorArcanaPvE.CanUse(out act)) return true;

        if (CelestialOppositionPvE.CanUse(out act)) return true;

        if (Player.HasStatus(true, StatusID.GiantDominance))
        {
            act = EarthlyStarPvE;
            return true;
        }

        if (!Player.HasStatus(true, StatusID.HoroscopeHelios, StatusID.Horoscope) && HoroscopePvE.CanUse(out act)) return true;

        if ((Player.HasStatus(true, StatusID.HoroscopeHelios)
            || PartyMembersMinHP < 0.3)
            && HoroscopePvE.CanUse(out act)) return true;

        return base.HealSingleAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.CelestialOppositionPvE, ActionID.EarthlyStarPvE, ActionID.HoroscopePvE)]
    protected override bool HealAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (CelestialOppositionPvE.CanUse(out act)) return true;

        if (Player.HasStatus(true, StatusID.GiantDominance))
        {
            act = EarthlyStarPvE;
            return true;
        }

        if (Player.HasStatus(true, StatusID.HoroscopeHelios) && HoroscopePvE.CanUse(out act)) return true;

        if (DrawnCrownCard == CardType.LADY && MinorArcanaPvE.CanUse(out act)) return true;

        return base.HealAreaAbility(nextGCD, out act);
    }
}
