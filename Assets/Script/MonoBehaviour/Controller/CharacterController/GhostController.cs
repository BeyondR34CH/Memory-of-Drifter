using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;
using GhostStates;

public class GhostController : EnemyController
{
    protected override void LoadState()
    {
        state.Relate(CharacterStateType.Idle, new NonObstacleIdle(this));
        state.Relate(CharacterStateType.Patrol, new NonObstaclePatrol(this));
        state.Relate(CharacterStateType.Chase, new NonObstacleChase(this));
        state.Relate(CharacterStateType.AwaitAttack, new AwaitAttack(this));
        state.Relate(CharacterStateType.PrepareAttack, new PrepareAttack(this));
        state.Relate(CharacterStateType.MeleeAttack, new MeleeAttack(this));
        state.Relate(CharacterStateType.SpecialAttack, new DashAttack(this));
        state.Relate(CharacterStateType.Hurt, new Hurt(this));
        state.Relate(CharacterStateType.Death, new CommonStates.Death(this));
        state.Relate(CharacterStateType.AfterDeath, new AfterDeath(this));
    }
}
