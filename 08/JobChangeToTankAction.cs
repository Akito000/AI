using System.Collections.Generic;
using System.Linq;
using Game.CharacterInput;
using Game.World;

namespace Game.AI
{
    /// <summary>
    /// 共通行動候補の「ジョブ切り替え」(Tankへ)。
    /// 考慮事項は2つだけ: ①自分と同じ色の敵攻撃(EnemyAttackMaker)が来ている ②自分が今Tankでない。
    /// 役割があるのに切り替わらないストレスを避けるため、BaseWeightはかなり高めにしてある。
    /// </summary>
    public class JobChangeToTankAction : IUtilityAction
    {
        public string Name => "JobChangeToTank";
        public float BaseWeight => 2.2f;
        public bool IsUrgentCapable => true;

        public IReadOnlyList<IConsideration> Considerations { get; } = new IConsideration[]
        {
            new RoleGateConsideration(RoleFlag.Tank, invert: true, key: "JobChangeToTank:NotTankYet"),
            new ColorMatchConsideration("JobChangeToTank:ColorMatch"),
        };

        public IEnumerable<object> GetTargetCandidates(UtilityContext context)
        {
            return context.IncomingAttacks.Cast<object>();
        }

        public void Act(UtilityContext context, object target, NpcDecisionResult result)
        {
            result.Commands.Add(new RoleSwapRequestCommand(RoleFlag.Tank));
        }
    }
}
