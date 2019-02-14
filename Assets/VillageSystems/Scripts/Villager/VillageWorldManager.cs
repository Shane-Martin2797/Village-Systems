using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageWorldManager : SingletonBehaviour<VillageWorldManager>
{

	// (build more housing as required, find resources, etc)
	public bool allVillagesExpand = true;

	//Randomly Spawn Villages Around Map
	public bool randomlySpawnVillages = true;

	//Whether or not villages will interact and war/ally, etc.
	public bool villagesInteract = true;

	//For Randomly Spawning Villages (Max/Min Expand Locations)
	public Vector3 startVillageGrid = new Vector3(0, 1, 0);
	public Vector3 endVillageGrid = new Vector3(500, 1, 500);


	public List<VillageManager> villages = new List<VillageManager>();


	public int timeMultiplier = 1;
	public System.DateTime startingDate = System.DateTime.Now;
	public System.DateTime currentDate = System.DateTime.Now;
	public float years;

	public float updateValueDays = 30;

	public int numberOfVillagesToSpawnRandomly = 4;

	int updateTimes = 1;


	private int spawnedVillages = 0;

	private bool initialised = false;

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		//-500,-500, -500, 500.
		//-500, -500, 500, -500.
		//500, 500, -500, 500.
		//500, 500, 500, -500

		Gizmos.DrawLine(startVillageGrid, new Vector3(startVillageGrid.x, startVillageGrid.y, endVillageGrid.z));
		Gizmos.DrawLine(startVillageGrid, new Vector3(endVillageGrid.x, startVillageGrid.y, startVillageGrid.z));
		Gizmos.DrawLine(endVillageGrid, new Vector3(startVillageGrid.x, startVillageGrid.y, endVillageGrid.z));
		Gizmos.DrawLine(endVillageGrid, new Vector3(endVillageGrid.x, startVillageGrid.y, startVillageGrid.z));
	}

	void Start()
	{
		startingDate = System.DateTime.Now;
		currentDate = System.DateTime.Now;

		StartCoroutine(Init());

	}

	IEnumerator Init()
	{
		for (int i = 0; i < numberOfVillagesToSpawnRandomly; i++)
		{
			CreateVillage();
			yield return new WaitForSeconds(1f);
		}
		initialised = true;
	}

	void Update()
	{
		if (!initialised)
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

		int timesToUpdate = updateTimes * timeMultiplier;


		for (int j = 0; j < timesToUpdate; j++)
		{
			for (int i = 0; i < villages.Count; i++)
			{
				if (villages [i].villageRequiresUpdating)
				{
					villages [i].UpdateVillage(updateValueDays);
				}
			}
			currentDate.AddDays(updateValueDays);
			years += (updateValueDays / 365);
		}
	}

	private void CreateVillage()
	{
		GameObject vil = new GameObject();


		VillageManager spawnedVillage = vil.AddComponent<VillageManager>();

		vil.transform.position = GetVillagePositionAwayFromOthers(40, spawnedVillage.sizeOfVillage, true, false);


		EditVillageGrid(spawnedVillage);

		vil.name = "Village " + (spawnedVillages + 1);

		spawnedVillages++;
	}

	//Size of Village
	// Infinite Housing
	// Size Of Sections
	// Villager Growth Accelerator
	// Initial Villager Count
	// Adult Age
	// Max Age Gap
	private void CreateVillage(Vector3 pos, Vector2 villageSize, Vector2 sectionSize, bool randomPos = false,
	                           int initVilCount = 100, int vilAdultAge = 16, int vilMaxAgeGap = 20, int villageGrowthAccel = 1)
	{
		GameObject vil = new GameObject();


		VillageManager spawnedVillage = vil.AddComponent<VillageManager>();

		if (randomPos)
		{
			vil.transform.position = GetRandomVillagePosition(spawnedVillage.sizeOfVillage);
		}
		else
		{
			vil.transform.position = pos;
		}


		spawnedVillage.sizeOfVillage = villageSize;
		spawnedVillage.sectionSize = sectionSize;
		spawnedVillage.initialVillagerCount = initVilCount;
		spawnedVillage.adultAge = vilAdultAge;
		spawnedVillage.maxAgeGap = vilMaxAgeGap;
		spawnedVillage.villagerGrowthAccelerator = villageGrowthAccel;

		EditVillageGrid(spawnedVillage);

		vil.name = "Village " + (spawnedVillages + 1);

		spawnedVillages++;
	}


	public Vector3 GetRandomVillagePosition(Vector2 size)
	{
		Vector3 rand;

		Vector3 box = new Vector3(size.x, 0, size.y);

		if (true)
		{
			rand = new Vector3(Random.Range(startVillageGrid.x + (box.x / 2), endVillageGrid.x - (box.x / 2)), 
				Random.Range(startVillageGrid.y, endVillageGrid.y), 
				Random.Range(startVillageGrid.z + (box.z / 2), endVillageGrid.z - (box.z / 2)));
		}
		else
		{
			Vector3 diff = (endVillageGrid - (box / 2)) - (startVillageGrid + (box / 2));
			rand = Random.insideUnitSphere;

			rand.x = ((Mathf.Abs(rand.x) * (diff.x)) + (startVillageGrid.x + (box.x / 2)));
			rand.y = ((Mathf.Abs(rand.y) * (diff.y)) + startVillageGrid.y);
			rand.z = ((Mathf.Abs(rand.z) * (diff.z)) + (startVillageGrid.z + (box.z / 2)));
		}
		return rand;
	}

	public Vector3 GetVillagePositionAwayFromOthers(float minDistance, Vector2 size, bool includeVillageTerritory = false, bool addSize = true)
	{
		if (villages.Count == 0)
		{
			return GetRandomVillagePosition(size);
		}
		float dist = minDistance;

		if (addSize)
		{
			dist += (Mathf.Max(size.x, size.y) / 2);
		}

		List<Vector3> possiblePositions = new List<Vector3>();

		List<Vector4> vilPos = new List<Vector4>();

		for (int i = 0; i < villages.Count; i++)
		{
			if (includeVillageTerritory)
			{
				vilPos.Add(new Vector4(villages [i].transform.position.x - ((villages [i].sizeOfVillage.x / 2) + dist), 
					villages [i].transform.position.z - ((villages [i].sizeOfVillage.y / 2) + dist), 
					villages [i].transform.position.x + ((villages [i].sizeOfVillage.x / 2) + dist), 
					villages [i].transform.position.z + ((villages [i].sizeOfVillage.y / 2) + dist)));
			}
			else
			{

				vilPos.Add(new Vector4(villages [i].transform.position.x - (dist), 
					villages [i].transform.position.z - (dist), 
					villages [i].transform.position.x + (dist), 
					villages [i].transform.position.z + (dist)));
			}
		}

		bool addPos = true;


		for (int i = Mathf.CeilToInt(startVillageGrid.x + ((size.x + minDistance) / 2)); i < Mathf.FloorToInt(endVillageGrid.x - ((size.x + minDistance) / 2)); i++)
		{
			for (int j = Mathf.CeilToInt(startVillageGrid.z + ((size.y + minDistance) / 2)); j < Mathf.FloorToInt(endVillageGrid.z - ((size.x + minDistance) / 2)); j++)
			{
				for (int l = 0; l < vilPos.Count; l++)
				{
					if (((i > vilPos [l].x && i < vilPos [l].z && j > vilPos [l].y && j < vilPos [l].w)))
					{
						addPos = false;
						break;
					}
				}

				if (addPos)
				{
					possiblePositions.Add(new Vector3(i, startVillageGrid.y, j));
				}

				addPos = true;
			}
		}


		if (possiblePositions.Count > 0)
		{

			return possiblePositions [Random.Range(0, possiblePositions.Count)];
		}
		else
		{
			Vector3 diff = endVillageGrid - startVillageGrid;
			Debug.LogWarning("No Positions Found");
			return startVillageGrid + (diff / 2);
		}
	}

	public void EditVillageGrid(VillageManager village)
	{

	}



}
