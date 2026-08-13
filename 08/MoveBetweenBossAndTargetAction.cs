using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.CharacterInput;
using Game.World;

namespace Game.AI
{
    /// <summary>
    /// Tank用「ボスと、狙われている味方の間に入る」。EnemyAttackMakerの一覧から自分と同色の攻撃を探し、
    /// その攻撃が狙っている味方(IIncomingAttackInfo.TargetPlayer)とボスの位置の中間点(TankTuning.
    /// InterceptPositionBiasで調整可能)を目的地にする。狙われているのが自分自身の場合はここでは扱わない
    /// (間に入る、という動き自体が成立しないため)。
    /// </summary>
    public class MoveBetweenBossAndTargetAction : MoveToTargetActionBase
    {
        public override string Name => "MoveBetweenBossAndTarget";
        public override float BaseWeight => 1.2f;
        public override bool IsUrgentCapable => true;

        public override IReadOnlyList<IConsideration> Considerations { get; } = new IConsideration[]
        {
            new RoleGateConsideration(RoleFlag.Tank, key: "Intercept:IsTank"),
            new ColorMatchConsideration("Intercept:ColorMatch"),
            new TimeToImpactConsideration(TankTuning.ReactionWindowSeconds, "Intercept:TimeToImpact"),
            // 間合いに入ったかどうかはUseBarrierAction側の狭いスケールで判定するため、
            // こちらは広いスケールで正規化し、「まだ十分に近づけていない」間はしっかり移動を優先する。
            new DistanceConsideration("Intercept:NeedToMove", AttackerTuning.SearchRadius, preferNear: false),
            new StickyTargetConsideration(key: "Intercept:Sticky"),
        };

        public override IEnumerable<object> GetTargetCandidates(UtilityContext context)
        {
            if (context.Boss == null || !context.Boss.IsAlive) yield break;

            foreach (IIncomingAttackInfo attack in context.IncomingAttacks)
            {
                if (attack.TargetPlayer == null || attack.TargetPlayer == context.Self) continue;
                yield return attack;
            }
        }

        public override void Act(UtilityContext context, object target, NpcDecisionResult result)
        {
            if (target is not IIncomingAttackInfo attack || context.Boss == null) return;

            Vector3 interceptPoint = Vector3.Lerp(
                attack.TargetPlayer.transform.position,
                context.Boss.Position,
                TankTuning.InterceptPositionBias);

            result.MoveDestination = interceptPoint;
        }
    }
}
