using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Boid : MonoBehaviour
{
	public const int ќбласть¬идимости = 20;//20;
	public const int —корость = 15;

	internal string Name = string.Empty;

	private Vector3 anchor = Vector3.zero;
	private int turnSpeed = 10;
	//public int maxSpeed = 15;
	private int maxBoids = 10;
	// ƒистанци€ с которой начинают действовать силы отталкивани€
	private float separationDistance = 5;
	// —тремление друг к другу
	private float cohesionCoefficient = 1;
	private float alignmentCoefficient = 4;
	private float separationCoefficient = 10;
	private float predatorCoefficient = 100;
	private float tick = 2;
	public Transform model;
	public LayerMask boidsLayer;

	[HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Transform tr;

	private Collider[] boids;
	private Vector3 cohesion;
	private Vector3 separation;
	private int separationCount;
	private Vector3 alignment;
	private Vector3 runPredator;
	private int predatorCount;

	private Boid b;
	private Predator p;
	private Vector3 vector;
	private int i;

	void Awake()
	{
		tr = transform;
		velocity = UnityEngine.Random.onUnitSphere * —корость;
	}

	private void Start()
	{
		InvokeRepeating("CalculateVelocity", UnityEngine.Random.value * tick, tick);
		InvokeRepeating("UpdateRotation", UnityEngine.Random.value, 0.1f);
	}

	void CalculateVelocity()
	{
		boids = Physics.OverlapSphere(tr.position, ќбласть¬идимости, boidsLayer.value);

		/**/if (boids.Length < 2) return;

		velocity = Vector3.zero;
		cohesion = Vector3.zero;
		separation = Vector3.zero;
		separationCount = 0;
		alignment = Vector3.zero;
		runPredator = Vector3.zero;

		for (i = 0; i < boids.Length /*&& i < maxBoids*/; i++)
		{
			b = boids[i].GetComponent<Boid>();

			/**/cohesion += b.tr.position;
			/**/alignment += b.velocity;
			vector = tr.position - b.tr.position;
			if (vector.sqrMagnitude > 0 && vector.sqrMagnitude < separationDistance * separationDistance)
			{
				separation += vector / vector.sqrMagnitude;
				separationCount++;
			}
		}

		/**/cohesion = cohesion / (boids.Length > maxBoids ? maxBoids : boids.Length);
		cohesion = Vector3.ClampMagnitude(cohesion - tr.position, —корость);
		cohesion *= cohesionCoefficient;
		/**/if (separationCount > 0)
		{
			separation = separation / separationCount;
			separation = Vector3.ClampMagnitude(separation, —корость);
			separation *= separationCoefficient;
		}/**/
		/**/alignment = alignment / (boids.Length > maxBoids ? maxBoids : boids.Length);
		/**/alignment = Vector3.ClampMagnitude(alignment, —корость);
		/**/alignment *= alignmentCoefficient;

		Run();

		velocity = Vector3.ClampMagnitude(cohesion + separation /**/+ alignment/**/ + runPredator, —корость);
	}

	void Run()
	{
		boids = Physics.OverlapSphere(tr.position, ќбласть¬идимости, -1)
			.Where(t => string.Compare(t.tag, "Predator") == 0)
			.ToArray();
		if (boids.Length > 0)
		{
			for (i = 0; i < boids.Length /*&& i < maxBoids*/; i++)
			{
				p = boids[i].GetComponent<Predator>();

				//cohesion += b.tr.position;
				//alignment += b.velocity;
				vector = tr.position - p.tr.position;
				if (vector.sqrMagnitude > 0 && vector.sqrMagnitude < ќбласть¬идимости * ќбласть¬идимости)
				{
					runPredator += vector;// / vector.sqrMagnitude;
					predatorCount++;
				}
			}

			/**/
			if (predatorCount > 0)
			{
				runPredator = runPredator / predatorCount;
				runPredator = Vector3.ClampMagnitude(runPredator, —корость);
				runPredator *= predatorCoefficient;
			}/**/
		}
	}

	void UpdateRotation()
	{
		if (velocity != Vector3.zero && model.forward != velocity.normalized)
		{
			model.forward = Vector3.RotateTowards(model.forward, velocity, turnSpeed, 1);
		}
	}

	void Update()
	{
		if (Vector3.Distance(tr.position, anchor) > Consts.–азмер—феры)
		{
			velocity += (anchor - tr.position) / Consts.–азмер—феры;
		}
		tr.position += velocity * Time.deltaTime;

		//Debug.DrawRay(transform.position, separation, Color.green);
		//Debug.DrawRay(transform.position, cohesion, Color.magenta);
		//Debug.DrawRay(transform.position, alignment, Color.blue);
		Debug.DrawRay(transform.position, runPredator, Color.white);
	}
}
