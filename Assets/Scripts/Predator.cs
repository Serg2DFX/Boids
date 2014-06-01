using UnityEngine;
using System.Linq;

public class Predator : MonoBehaviour
{
	public const int ОбластьВидимости = 50;
	public const float Скорость = 15;//15
	private const float ДистанцияПоедания = 3f;//2f;

	internal string Name = string.Empty;
	private string СообщениеОПоедании = "Хищник{0}: Съел рыбку {1}!";

	private Vector3 anchor = Vector3.zero;
	private int turnSpeed = 10;
	//public int maxBoids = 10;
	// Дистанция с которой начинают действовать силы отталкивания
	private float separationDistance = 5;
	// Стремление друг к другу
	private float cohesionCoefficient = 1;
	private float findCoefficient = 4;
	private float separationCoefficient = 10;
	private float tick = 2;//2;
	public Transform model;
	//private LayerMask boidsLayer;

	[HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Transform tr;
	//
	private Collider[] boids;
	private Vector3 cohesion;
	private Vector3 separation;
	private Vector3 find;
	private int separationCount;

	private Boid b;
	private Predator p;
	private Vector3 vector;
	private int i;
	private float fishDistance, testFishDistance;

	void Awake()
	{
		tr = transform;
		velocity = UnityEngine.Random.onUnitSphere * Скорость;
	}

	private void Start()
	{
		InvokeRepeating ("CalculateVelocity", Random.value * tick, tick);
		InvokeRepeating("UpdateRotation", Random.value, 0.1f);
	}

	void CalculateVelocity()
	{
		velocity = Vector3.zero;
		cohesion = Vector3.zero;
		separation = Vector3.zero;
		find = Vector3.zero;
		separationCount = 0;

		cohesion = cohesion - tr.position;
		cohesion = Vector3.ClampMagnitude(cohesion, Скорость);
		cohesion *= cohesionCoefficient;

		boids = Physics.OverlapSphere(tr.position, ОбластьВидимости, -1)
			.Where(t => string.Compare(t.tag, "Predator") == 0)
			.ToArray();

		if (boids.Length >= 2)
		{
			for (i = 0; i < boids.Length /*&& i < maxBoids*/; i++)
			{
				p = boids[i].GetComponent<Predator>();

				//cohesion += b.tr.position;
				//alignment += b.velocity;
				vector = tr.position - p.tr.position;
				if (vector.sqrMagnitude > 0 && vector.sqrMagnitude < separationDistance * separationDistance)
				{
					separation += vector / vector.sqrMagnitude;
					separationCount++;
				}
			}

			/**/
			if (separationCount > 0)
			{
				separation = separation / separationCount;
				separation = Vector3.ClampMagnitude(separation, Скорость);
				separation *= separationCoefficient;
			}/**/
		}
		Find();

		velocity = Vector3.ClampMagnitude(cohesion + separation + find, Скорость * 1.2f);
	}

	/// <summary>
	/// Поиск жертвы
	/// </summary>
	private void Find()
	{
		boids = Physics.OverlapSphere(tr.position, ОбластьВидимости, -1);
		if (boids.Length < 1) return;

		testFishDistance = fishDistance = float.MaxValue;
		for (i = 0; i < boids.Length; i++)
		{
			if (string.Compare(boids[i].tag, "Boid") != 0)
				continue;
			b = boids[i].GetComponent<Boid>();
			testFishDistance = //(b.tr.position - tr.position).sqrMagnitude;
				Vector3.Distance(b.transform.position, tr.position);
			if (testFishDistance < fishDistance)
			{
				fishDistance = testFishDistance;
				find = b.tr.position - tr.position;// - tr.position;
				find = Vector3.ClampMagnitude(find, Скорость);
				find *= findCoefficient;
				//cohesion = Vector3.ClampMagnitude(cohesion - tr.position, Скорость);
			}
		}
	}

	/// <summary>
	/// Кушаем рыбок
	/// </summary>
	private void FishEat()
	{
		var eatBoids = Physics.OverlapSphere(tr.position, ДистанцияПоедания, -1);
		if (eatBoids.Length <= 1)
			return;
		
		for (i = 0; i < eatBoids.Length; i++)
		{
			if (string.Compare(eatBoids[i].tag, "Boid") != 0)
				continue;
			b = eatBoids[i].GetComponent<Boid>();
			Destroy(eatBoids[i].gameObject);
			Debug.Log(string.Format(this.СообщениеОПоедании, this.Name, b.Name));
			//Destroy(b.transform.gameObject);
		}
	}

	/// <summary>
	/// Поворачиваемся
	/// </summary>
	void UpdateRotation()
	{
		if (velocity != Vector3.zero && model.forward != velocity.normalized)
		{
			model.forward = Vector3.RotateTowards(model.forward, velocity, turnSpeed, 1);
		}
	}

	/// <summary>
	/// Обновляем координаты
	/// </summary>
	void Update()
	{
		FishEat();
		if (Vector3.Distance(tr.position, anchor) > Consts.РазмерСферы)
		{
			velocity += (anchor - tr.position) / Consts.РазмерСферы;
		}
		tr.position += velocity * Time.deltaTime;
		
		//Debug.DrawRay(tr.position, velocity - tr.position, Color.magenta);

		//Debug.DrawRay(transform.position, separation, Color.green);
		//Debug.DrawRay(transform.position, cohesion, Color.magenta);
		Debug.DrawRay(transform.position, find, Color.red);
	}
}
