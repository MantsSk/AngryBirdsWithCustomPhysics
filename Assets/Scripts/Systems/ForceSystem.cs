using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSystem : ISystemInterface 
{
    public void Start(World world)
    {
        var entities = world.entities;
        var enemyEntities = world.enemyEntities;
        
        // add randomized velocity to all player entities that have positions
        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition))
            {
                entities.AddComponent(new PlayerEntity(i), EntityFlags.kFlagForce);

                var forceComponent = new ForceComponent() {massInverse = Random.Range(0f, 5f), force = Vector2.zero};
                entities.forceComponents[i] = forceComponent;
            }
        }
        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagPosition))
            {
                enemyEntities.AddEnemyComponent(new EnemyEntity(i), EnemyEntityFlags.kFlagForce);

                var enemyForceComponent = new EnemyForceComponent() {massInverse = Random.Range(0f, 5f), force = Vector2.zero};
                enemyEntities.enemyForceComponents[i] = enemyForceComponent;
            }
        }
    }

    public void Update(World world, float time = 0, float deltaTime = 0)
    {
        var entities = world.entities;
        var enemyEntities = world.enemyEntities;

        for (var i = 0; i < entities.flags.Count; i++)
        {
            if (entities.flags[i].HasFlag(EntityFlags.kFlagPosition) && 
                entities.flags[i].HasFlag(EntityFlags.kFlagForce) &&
                entities.flags[i].HasFlag(EntityFlags.kFlagMove))
            {
                var moveComponent = entities.moveComponents[i];
                var forceComponent = entities.forceComponents[i];

                // F = m * a => a = F / m
                moveComponent.acceleration = forceComponent.massInverse * forceComponent.force;
                forceComponent.force = Vector2.zero;

                entities.moveComponents[i] = moveComponent;
                entities.forceComponents[i] = forceComponent;
            }
        }
        for (var i = 0; i < enemyEntities.enemyFlags.Count; i++)
        {
            if (enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagPosition) && 
                enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagForce) &&
                enemyEntities.enemyFlags[i].HasFlag(EnemyEntityFlags.kFlagMove))
            {
                var enemyMoveComponent = enemyEntities.enemyMoveComponents[i];
                var enemyForceComponent = enemyEntities.enemyForceComponents[i];

                // F = m * a => a = F / m
                enemyMoveComponent.acceleration = enemyForceComponent.massInverse * enemyForceComponent.force;
                enemyForceComponent.force = Vector2.zero;
                enemyEntities.enemyMoveComponents[i] = enemyMoveComponent;
                enemyEntities.enemyForceComponents[i] = enemyForceComponent;
            }
        }
    }

    public void OnMouseDrag (World world) 
    {

    }
}
