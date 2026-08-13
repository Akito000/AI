using System.Collections.Generic;
using System.Linq;
using Game.CharacterInput;
using Game.World;

namespace Game.AI
{
    /// <summary>
    /// 共通行動候補の「ジョブ切り替え」(Healerへ)。
    /// 考慮事項は2つだけ: ①味方に自分と同じ色の状態異常のキャラがいる ②自分が今Healerでない。
    /// 役割があるのに切り替わらないストレスを避けるため、BaseWeightはかなり高めにしてある。
    /// </summary>
    public class JobChangeToHealerAction : IUtilityAction
    {
        public string Name => "JobChangeToHealer";
        public float BaseWeight => 2.0f;
        public bool IsUrgentCapable => true;

        public IReadOnlyList<IConsideration> Considerations { get; } = new IConsideration[]
        {
            new RoleGateConsideration(RoleFlag.Healer, invert: true, key: "JobChangeToHealer:NotHealerYet"),
            new ColorMatchConsideration("JobChangeToHealer:ColorMatch"),
        };

        public IEnumerable<object> GetTargetCandidates(UtilityContext context)
        {
            return context.Ailments.Cast<object>();
        }

        public void Act(UtilityContext context, object target, NpcDecisionResult result)
        {
            result.Commands.Add(new RoleSwapRequestCommand(RoleFlag.Healer));
        }
    }
}
