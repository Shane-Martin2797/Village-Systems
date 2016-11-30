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

	public List<string> names = new List<string> { "Villager" };

	public string name = "Villager";

	public List<Villager> family { get; set; }

	public List<Villager> partners { get; set; }

	public List<Villager> children { get; set; }

	public Section sectionAssigned { get; set; }


	public Villager dad { get; set; }

	public Villager mum { get; set; }

	public Gender gender;

	public Stats stats;

	public bool setup = false;

	public virtual void Initialise()
	{
		name = names [Random.Range(0, names.Count)];

		family = new List<Villager>();
		partners = new List<Villager>();
		children = new List<Villager>();

		stats = new Stats();
		stats.Initialise();

		setup = true;
	}

	public virtual void Initialise(Villager _dad, Villager _mum)
	{
		name = names [Random.Range(0, names.Count)];

		dad = _dad;
		mum = _mum;

		family = new List<Villager>();
		partners = new List<Villager>();
		children = new List<Villager>();


		AddFamily(dad);
		AddFamily(mum);

		dad.AddChild(this);
		mum.AddChild(this);

		AddFamily(dad.children);
		AddFamily(mum.children);

		stats = new Stats();
		stats.Initialise(dad, mum, true);

		setup = true;
	}


	public virtual void AddFamily(Villager vil)
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

	public virtual void AddChild(Villager vil)
	{
		if (vil == this)
		{
			return;
		}

		//For the other Children, Add this Child to thier family
		for (int i = 0; i < children.Count; i++)
		{
			if (!children [i].family.Contains(vil))
			{
				children [i].AddFamily(vil);
			}
		}

		if (!children.Contains(vil))
		{
			children.Add(vil);
			AddFamily(vil);
		}

		if (!vil.family.Contains(this))
		{
			vil.AddFamily(this);
		}
	}

	public virtual void AddFamily(List<Villager> vilFam)
	{
		for (int i = 0; i < vilFam.Count; i++)
		{
			AddFamily(vilFam [i]);
		}
	}

	public virtual bool CanAddPartner(Villager vil)
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

	public virtual bool AddPartner(Villager vil)
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

	public virtual void Update(float villageTime)
	{
		if (!setup)
		{
			return;
		}

		stats.age += (villageTime);

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
	}

	public virtual void Kill()
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
