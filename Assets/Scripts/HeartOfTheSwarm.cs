using UnityEngine;

public class HeartOfTheSwarm : MonoBehaviour
{
	public Transform boidPrefab;
	public Transform predatorPrefab;

	void Start()
	{
		for (var i = 0; i < Consts.���������������; i++)
		{
			var boid = Instantiate(boidPrefab, Random.insideUnitSphere * Consts.�����������, Quaternion.identity) as Transform;
			boid.GetComponent<Boid>().Name = (i + 1).ToString();
			boid.parent = transform;
		}

		/**/for (var i = 0; i < Consts.������������������; i++)
		{
			var predator = Instantiate(predatorPrefab, Random.insideUnitSphere * Consts.�����������, Quaternion.identity) as Transform;
			predator.GetComponent<Predator>().Name = (i + 1).ToString();
			predator.parent = transform;
		}/**/
	}
}

public class Consts
{
	public const int ��������������� = 100;//300;
	public const int ����������� = 75;//25;
	public const int ������������������ = 3;//10;
}