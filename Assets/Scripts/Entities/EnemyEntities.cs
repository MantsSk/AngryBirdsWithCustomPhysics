
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct EnemyEntity
{
	public int id;

	public EnemyEntity(int id)
	{
		this.id = id;
	}
}

[Flags]
public enum EnemyEntityFlags
{
	kFlagPosition = 1<<0,
	kFlagWorldBounds = 1<<1,
	kFlagGravity = 1<<2,
	kFlagForce = 1<<3,
	kFlagCollision = 1<<4,
	kFlagMove = 1<<5
}

public struct EnemyMoveComponent
{
	public Vector2 velocity;
	public Vector2 acceleration;
}

public struct EnemyForceComponent
{
	public float massInverse;
	public Vector2 force;
}

public struct EnemyCollisionComponent
{
	public float radius;
	public float coeffOfRestitution;
	public bool isDamaged;
	public bool shouldSmash;
}

public class EnemyEntities
{
    public List<Vector2> enemyPositions = new List<Vector2>();
	public List<EnemyEntityFlags> enemyFlags = new List<EnemyEntityFlags>();
	public List<EnemyMoveComponent> enemyMoveComponents = new List<EnemyMoveComponent>();
	public List<EnemyForceComponent> enemyForceComponents = new List<EnemyForceComponent>();
	public List<EnemyCollisionComponent> enemyCollisionComponents = new List<EnemyCollisionComponent>();

    public void AddEnemyComponent(EnemyEntity entity, EnemyEntityFlags flag)
	{
		enemyFlags[entity.id] |= flag;
	}

    public EnemyEntity AddEnemyEntity(Vector2 position)
	{
		// We assume that all entities have at least position component
		enemyPositions.Add(position);
		enemyFlags.Add(EnemyEntityFlags.kFlagPosition);
		
		// reserve space for all other components
		enemyMoveComponents.Add(new EnemyMoveComponent());
		enemyForceComponents.Add(new EnemyForceComponent());
		enemyCollisionComponents.Add(new EnemyCollisionComponent());
		
		return new EnemyEntity(enemyPositions.Count - 1);
	}

    public EnemyEntity AddEnemyEntity()
	{
		return AddEnemyEntity(Vector2.zero);
	}

	public void Init(int count = 10)
	{
		for (var i = 0; i < count; i++)
		{
		//	AddEnemyEntity(new Vector2(2.0f,-2.75f));
			AddEnemyEntity(new Vector2(Random.Range(2.85f,7.05f),Random.Range(-8.05f,-0.75f)));
		}
	}

}

