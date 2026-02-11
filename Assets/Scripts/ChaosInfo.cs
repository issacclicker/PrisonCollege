using UnityEngine;

public abstract class ChaosInfo
{
    protected float _chaos;
    public abstract string Description { get; }
    public string StatText => $"혼란 +{_chaos.ToString("F0")}";

    public ChaosInfo(float chaos)
    {
        _chaos = chaos;
    }
}



public class GunShotChaos : ChaosInfo
{
    public override string Description => "총기 발사!!";

    public GunShotChaos(float chaos) : base(chaos) { }
}



public class EscapedChaos : ChaosInfo
{
    public override string Description => "대학원생 탈출!!";

    public EscapedChaos(float chaos) : base(chaos) { }
}



public class InnocentKillChaos : ChaosInfo
{
    public override string Description => "무고한 대학원생 진압!!";

    public InnocentKillChaos(float chaos) : base(chaos) { }
}



public class NormalFoodRemovedChaos : ChaosInfo
{
    public override string Description => "맛있는 음식 약탈!!";

    public NormalFoodRemovedChaos(float chaos) : base(chaos) { }
}