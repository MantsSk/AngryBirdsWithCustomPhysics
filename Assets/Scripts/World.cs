using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

// Entry point to the simulations. Technically there could be multiple worlds
// that are be completely isolated from each other
public class World : MonoBehaviour 
{	
	public GameObject templateObject;
	public GameObject enemyTemplateObject;
	public GameObject enemyTemplateSmashedObj;
	public int entityCount = 1;
	public int enemyEntityCount = 1;
	public Rect worldBounds = new Rect(-10f, -5f, 20f, 10f);
	public Vector2 gravity = Vector2.down * 9.81f;
	public bool shouldShoot = false;
	public bool shouldSmash = false;


	[NonSerialized]
	public PlayerEntities entities;
	public EnemyEntities enemyEntities;

	protected List<ISystemInterface> systems;
	
	// Use this for initialization
	void Start () 
	{
		systems = new List<ISystemInterface>();
		
		entities = new PlayerEntities();
		enemyEntities = new EnemyEntities();

		entities.Init(entityCount);
		enemyEntities.Init(enemyEntityCount);

		// System addition order matters, they will run in the same order
		systems.Add(new GravitySystem());
		systems.Add(new ForceSystem());
		systems.Add(new InputSystem());	
		systems.Add(new MoveSystem());
		systems.Add(new CollisionSystem());
		systems.Add(new WorldBoundsSystem());
		systems.Add(new RenderingSystem());
	
		foreach (var system in systems)
		{
			system.Start(this);
		}
	}

	
	// Update is called once per frame
	void Update () 
	{
		foreach (var system in systems)
		{
			system.Update(this, Time.timeSinceLevelLoad, Time.deltaTime);
		}
	}
}
