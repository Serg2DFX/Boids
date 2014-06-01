using UnityEngine;

public class HeartOfTheSwarm : MonoBehaviour
{
	public Transform boidPrefab;
	public Transform predatorPrefab;

	void Start()
	{
		for (var i = 0; i < Consts.КоличествоРыбок; i++)
		{
			var boid = Instantiate(boidPrefab, Random.insideUnitSphere * Consts.РазмерСферы, Quaternion.identity) as Transform;
			boid.GetComponent<Boid>().Name = (i + 1).ToString();
			boid.parent = transform;
		}

		/**/for (var i = 0; i < Consts.КоличествоХищников; i++)
		{
			var predator = Instantiate(predatorPrefab, Random.insideUnitSphere * Consts.РазмерСферы, Quaternion.identity) as Transform;
			predator.GetComponent<Predator>().Name = (i + 1).ToString();
			predator.parent = transform;
		}/**/
	}
}

public class Consts
{
	public const int КоличествоРыбок = 100;//300;
	public const int РазмерСферы = 75;//25;
	public const int КоличествоХищников = 3;//10;
}