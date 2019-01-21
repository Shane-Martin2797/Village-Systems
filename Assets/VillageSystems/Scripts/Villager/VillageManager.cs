using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageManager : SingletonBehaviour<VillageManager>
{

	public Vector2 sizeOfVillage = new Vector2(100, 100);
	public Vector2 sectionSize = new Vector2(20, 20);

	public int timeMultiplier = 1;
	public System.DateTime startingDate = System.DateTime.Now;
	public int villagerGrowthAccelerator = 1;

	public float years = 0;

	public int initialVillagerCount = 100;
	public int adultAge = 16;

	public List<Villager> villagers = new List<Villager>();
	public Section[,] sections;

	void Start()
	{
		startingDate = System.DateTime.Now;

		SetSections();

		for (int i = 0; i < initialVillagerCount; i++)
		{
			GenerateVillager();
		}
	}

	public void SetSections()
	{
		if (Mathf.RoundToInt(sectionSize.x) == 0 || Mathf.RoundToInt(sectionSize.y) == 0)
		{
			sectionSize = new Vector2(20, 20);
		}

		int x = Mathf.RoundToInt(sizeOfVillage.x / sectionSize.x);
		int y = Mathf.RoundToInt(sizeOfVillage.y / sectionSize.y);

		sections = new Section[x, y];

		for (int i = 0; i < sections.GetLength(0); i++)
		{
			for (int j = 0; j < sections.GetLength(1); j++)
			{
				sections [i, j] = new Section();
				sections [i, j].Initialise(i, j, sectionSize);
			}
		}
	}

	public Villager GenerateVillager(Villager parentM = null, Villager parentF = null)
	{
		bool m = ((Random.value * villagerGrowthAccelerator) > 0.5f);

		Villager vil;

		if (m)
		{
			vil = new VillagerM();
		}
		else
		{
			vil = new VillagerF();
		}


		if (parentM == null || parentF == null)
		{
			vil.Initialise();
		}
		else
		{
			vil.Initialise(parentM, parentF);
		}

		int x = Random.Range(0, sections.GetLength(0));
		int y = Random.Range(0, sections.GetLength(1));

		Section sectionAssign = sections [x, y];


		if (sectionAssign != null)
		{
			vil.sectionAssigned = (sectionAssign);

			sectionAssign.GetVillagers().Add(vil);
		}

		villagers.Add(vil);

		return vil;
	}

	int updateTimes = 1;
	int maxVillagers = 0;


	void Update()
	{
		if (logged)
		{
			return;
		}

		if (Input.GetKeyUp(KeyCode.Alpha1))
		{
			updateTimes = 1;
		}
		else if (Input.GetKeyUp(KeyCode.Alpha2))
		{
			updateTimes = 2;
		}
		else if (Input.GetKeyUp(KeyCode.Alpha3))
		{
			updateTimes = 4;
		}
		else if (Input.GetKeyUp(KeyCode.Alpha4))
		{
			updateTimes = 8;
		}
		else if (Input.GetKeyUp(KeyCode.Alpha5))
		{
			updateTimes = 16;
		}
		else if (Input.GetKeyUp(KeyCode.Alpha6))
		{
			updateTimes = 32;
		}
		else if (Input.GetKeyUp(KeyCode.Alpha7))
		{
			updateTimes = 64;
		}
		else if (Input.GetKeyUp(KeyCode.Alpha8))
		{
			updateTimes = 100;
		}


		//Starts Lagging After ~500 Villagers
		if (villagers.Count > 500)
		{
			if (!logged)
			{
				Debug.Log("Village Reached " + villagers.Count + " Villagers After: " + years.ToString() + " years");
				Debug.Log("Max Villagers = " + maxVillagers.ToString());
				logged = true;
			}
		}
		else if (villagers.Count > 0)
		{
			updateTimes = (updateTimes * timeMultiplier);

			for (int i = 0; i < updateTimes; i++)
			{
				if (villagers.Count > 500)
				{
					break;
				}
				years += Time.deltaTime;
				UpdateVillagers(Time.deltaTime);
			}

			if (villagers.Count > maxVillagers)
			{
				maxVillagers = villagers.Count;
			}
		}
		else
		{
			if (!logged)
			{
				Debug.Log("Village Died After: " + years.ToString() + " years");
				Debug.Log("Max Villagers = " + maxVillagers.ToString());
				logged = true;
			}
		}
	}

	bool logged = false;


	private List<Villager> killList = new List<Villager>();

	public void AddToKillList(Villager vil)
	{
		if (!killList.Contains(vil))
		{
			killList.Add(vil);
		}
	}

	public void UpdateVillagers(float villageTime)
	{


		for (int i = 0; i < villagers.Count; i++)
		{
			villagers [i].Update(villageTime);
		}

		for (int i = 0; i < killList.Count; i++)
		{
			killList [i].Kill();
		}
	}
}
