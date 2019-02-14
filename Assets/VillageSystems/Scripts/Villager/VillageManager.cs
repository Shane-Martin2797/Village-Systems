using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillageManager : MonoBehaviour
{
	// (pre-placed houses hold all villagers)
	public bool infiniteHousing = false;

	public Vector2 sizeOfVillage = new Vector2(100, 100);
	public Vector2 sectionSize = new Vector2(20, 20);

	public Vector2 villagePos = new Vector2();

	public int villagerGrowthAccelerator = 1;

	public float years = 0;

	public int initialVillagerCount = 100;
	public int adultAge = 16;
	public int maxAgeGap = 20;

	public List<Villager> villagers = new List<Villager>();
	private List<VillagerF> fVillagers = new List<VillagerF>();
	private List<VillagerM> mVillagers = new List<VillagerM>();

	public Section[,] sections;

	private Color col;

	public bool villageRequiresUpdating
	{
		get
		{
			return villagers.Count > 0 && villagers.Count <= 500;
		}
	}

	void Start()
	{
		SetSections();

		for (int i = 0; i < initialVillagerCount; i++)
		{
			GenerateVillager();
		}

		VillageWorldManager.Instance.villages.Add(this);
		VillageWorldManager.Instance.EditVillageGrid(this);
		villagePos = new Vector2(transform.position.x, transform.position.z);


		col = new Color(Random.value, Random.value, Random.value, 1);
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = col;
		if (villagers != null)
		{
			Gizmos.DrawCube(transform.position, ((new Vector3(sizeOfVillage.x, (villagers.Count / 2), sizeOfVillage.y)) / 2));
		}
		else
		{
			Gizmos.DrawCube(transform.position, ((new Vector3(sizeOfVillage.x, 50, sizeOfVillage.y)) / 2));
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

	public Villager GenerateVillager(VillagerM parentM = null, VillagerF parentF = null)
	{
		bool m = ((Random.value) > 0.5f);

		GameObject villagerObject;

		villagerObject = new GameObject();

		villagerObject.transform.SetParent(this.transform);

		Villager vil;

		if (m)
		{
			VillagerM vilm = villagerObject.AddComponent<VillagerM>();
			vil = vilm;
			mVillagers.Add(vilm);
		}
		else
		{
			VillagerF vilf = villagerObject.AddComponent<VillagerF>();
			vil = vilf;
			fVillagers.Add(vilf);
		}


		if (parentM == null || parentF == null)
		{
			vil.Initialise();
		}
		else
		{
			vil.Initialise(parentM, parentF);
			parentF.pregnant = false;
			parentF.timeSincePregnant = 0;
		}

		vil.homeVillage = this;
		vil.currentVillage = this;

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

	public void UpdateVillage(float timeInDays)
	{
		float timeInYears = timeInDays / 365;
		years += (timeInYears);
		UpdateVillagers(timeInYears);
		if (villagers.Count > maxVillagers)
		{
			maxVillagers = villagers.Count;
		}

	}

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
			villagers [i].UpdateVillager(villageTime);
		}

		for (int i = 0; i < mVillagers.Count; i++)
		{
			BreedVillager(mVillagers [i]);
		}

		CheckPregnancy();

		for (int i = 0; i < killList.Count; i++)
		{
			killList [i].Kill();
		}
	}


	//Males Pick Female Partners *Tradition Gender Roles eekkkk* TODO
	void BreedVillager(VillagerM vil)
	{
		if (vil.stats.age >= adultAge)
		{
			if (vil.partners.Count < vil.stats.possiblePartnerCount)
			{
				VillagerF partner = PickPartner(vil);
				vil.AddPartner(partner);
			}
		}

		if (vil.partnersF == null)
		{
			return;
		}

		for (int i = 0; i < vil.partnersF.Count; i++)
		{
			if (vil.partnersF [i] == null)
			{
				continue;
			}
			if (vil.partnersF [i].CanHaveChildren())
			{
				if ((Random.value * villagerGrowthAccelerator) >= 0.5f)
				{
					vil.partnersF [i].pregnant = true;
					vil.partnersF [i].timeSincePregnant = 0;
				}
			}
		}
	}

	private void CheckPregnancy()
	{
		for (int i = 0; i < fVillagers.Count; i++)
		{
			if (fVillagers [i].pregnant)
			{
				if (fVillagers [i].timeSincePregnant >= fVillagers [i].timePregnancyLasts)
				{
					Villager v = GenerateVillager(fVillagers [i].partnersM [0], fVillagers [i]);
					fVillagers [i].AddChild(v);
					fVillagers [i].partnersM [0].AddChild(v);
				}
			}

		}
	}

	private VillagerF PickPartner(VillagerM vil)
	{
		for (int j = 0; j < fVillagers.Count; j++)
		{
			if (vil.CanAddPartner(fVillagers [j]))
			{
				return fVillagers [j];
			}
		}

		return null;
	}

}

