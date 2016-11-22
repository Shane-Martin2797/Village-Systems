using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Villager
{
	public enum Gender
	{
		Male,
		Female
	}

	public string name = "Villager";

	private List<Villager> family;

	public List<Villager> GetFamily()
	{
		return family;
	}

	private List<Villager> partners;

	public List<Villager> GetPartners()
	{
		return partners;
	}

	private List<Villager> children;

	public List<Villager> GetChildren()
	{
		return children;
	}

	private Section sectionAssigned;

	public void SetSection(Section s)
	{
		sectionAssigned = s;
	}

	public Section GetSection()
	{
		return sectionAssigned;
	}


	public Villager dad;
	public Villager mum;

	public Gender gender;

	public Stats stats;

	bool setup = false;

	public void Initialise()
	{
		family = new List<Villager>();
		partners = new List<Villager>();
		children = new List<Villager>();

		gender = (Random.value > 0.5f) ? Gender.Male : Gender.Female;

		stats = new Stats();
		stats.Initialise();

		setup = true;
	}

	public void Initialise(Villager _dad, Villager _mum)
	{
		dad = _dad;
		mum = _mum;

		gender = (Random.value > 0.5f) ? Gender.Male : Gender.Female;

		family = new List<Villager>();
		partners = new List<Villager>();
		children = new List<Villager>();

		AddFamily(dad);
		AddFamily(mum);

		for (int i = 0; i < dad.family.Count; i++)
		{
			AddFamily(dad.family [i]);
		}

		for (int i = 0; i < mum.family.Count; i++)
		{
			AddFamily(mum.family [i]);
		}

		stats = new Stats();
		stats.Initialise(dad, mum, (gender == Gender.Male));

		setup = true;
	}


	public void AddFamily(Villager vil)
	{
		if (vil == this)
		{
			return;
		}

		if (!family.Contains(vil))
		{
			family.Add(vil);
		}

		if (!vil.family.Contains(this))
		{
			vil.family.Add(this);
		}
	}

	public void AddChild(Villager vil)
	{
		if (vil == this)
		{
			return;
		}

		if (!children.Contains(vil))
		{
			children.Add(vil);
			AddFamily(vil);
		}
	}


	public void AddFamily(List<Villager> vilFam)
	{
		for (int i = 0; i < vilFam.Count; i++)
		{
			AddFamily(vilFam [i]);
		}
	}


	public bool CanAddPartner(Villager vil)
	{
		//Null Check
		if (vil == null)
		{
			return false;
		}
		//Self Check
		if (vil == this)
		{
			return false;
		}

		//Gender Check
		if (vil.gender == gender)
		{
			return false;
		}

		// Checks To Add (MAYBE):
		// AGE CHECK
		//

		//Partner Count Check
		if (partners.Count == stats.possiblePartnerCount || vil.partners.Count == vil.stats.possiblePartnerCount)
		{
			return false;
		}

		//Family Check
		if (family.Contains(vil) || vil.family.Contains(this))
		{
			return false;
		}

		//Whether they are already a partner check
		if (partners.Contains(vil) || vil.partners.Contains(this))
		{
			return false;
		}

		//Age Check (20 years difference is maximum)
		if (Mathf.Abs(vil.stats.age - stats.age) > 20)
		{
			return false;
		}

		return true;
	}

	public bool AddPartner(Villager vil)
	{
		if (CanAddPartner(vil))
		{
			partners.Add(vil);
			AddFamily(vil.family);

			vil.partners.Add(this);
			vil.AddFamily(this.family);

			return true;
		}
		else
		{
			return false;
		}
	}

	public void Update(float villageTime)
	{
		if (!setup)
		{
			return;
		}

		stats.age += (villageTime * 2);

		if (stats.age > stats.lifeTime)
		{
			if (VillageManager.Instance != null)
			{
				VillageManager.Instance.AddToKillList(this);
			}
			else
			{
				Kill();
			}
		}


		if (gender == Gender.Male)
		{
			//Change the rule for when children can be had.
			if (Mathf.RoundToInt(stats.age) % 5 == 0)
			{
				if (children.Count < stats.maxChildren)
				{
					for (int i = 0; i < partners.Count; i++)
					{
						if (Random.value > 0.5f)
						{
							Villager v = VillageManager.Instance.GenerateVillager(this, partners [i]);
							AddChild(v);
							partners [i].AddChild(v);
						}
					}
				}
			}
		}
	}

	public void Kill()
	{
		if (VillageManager.Instance != null)
		{
			VillageManager.Instance.villagers.Remove(this);
		}

		if (sectionAssigned != null)
		{
			sectionAssigned.GetVillagers().Remove(this);
		}

		if (children.Count > 0)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children [i] == null)
				{
					continue;
				}
				children [i].family.Remove(this);
			}
		}

		if (family.Count > 0)
		{
			for (int i = 0; i < family.Count; i++)
			{
				if (family [i] == null)
				{
					continue;
				}
				family [i].family.Remove(this);
			}
		}

		if (partners.Count > 0)
		{
			for (int i = 0; i < partners.Count; i++)
			{
				if (partners [i] == null)
				{
					continue;
				}
				partners [i].family.Remove(this);
			}
		}
	}
}
