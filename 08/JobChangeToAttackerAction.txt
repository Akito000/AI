using System.Collections.Generic;
using System.Linq;
using Game.CharacterInput;
using Game.World;

namespace Game.AI
{
    /// <summary>
    /// 共通行動候補の「ジョブ切り替え」(Attackerへ)。
    /// 考慮事項は2つだけ: ①自分と同じ色の敵スポナー(EnemySpawner)がある ②自分が今Attackerでない。
    /// Tank/Healerへの切り替えほど緊急性は高くないため、BaseWeightは2つより低めにしてある
    /// (それでも他の雑務系Actionよりは優先されるよう、Idle/Wander等よりは十分高い)。
    /// </summary>
    public class JobChangeToAttackerAction : IUtilityAction
    {
        public string Name => "JobChangeToAttacker";
        public float BaseWeight => 1.0f;
        public bool IsUrgentCapable => true;

        public IReadOnlyList<IConsideration> Considerations { get; } = new IConsideration[]
        {
            new RoleGateConsideration(RoleFlag.Attacker, invert: true, key: "JobChangeToAttacker:NotAttackerYet"),
            new ColorMatchConsideration("JobChangeToAttacker:ColorMatch"),
        };

        public IEnumerable<object> GetTargetCandidates(UtilityContext context)
        {
            return context.Spawners.Cast<object>();
        }

        public void Act(UtilityContext context, object target, NpcDecisionResult result)
        {
            result.Commands.Add(new RoleSwapRequestCommand(RoleFlag.Attacker));
        }
    }
}
