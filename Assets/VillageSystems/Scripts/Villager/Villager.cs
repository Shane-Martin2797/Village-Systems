using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Villager : MonoBehaviour
{

	//public string serialKey { get; private set; }
	//Serial Key should be generated via: (Something like this)
	// Village Number (0000-9999)
	// Villager Aspirations (00-99)
	// Villager Name
	// Villager Number
	//Village Number - 24, Aspirations - Guard, Name - Villager, Village Gen - 14536
	//E.G. 002405Villager14536
	//



	public enum Gender
	{
		Male,
		Female
	}

	//Non-gendered Goals
	public enum Goals
	{
		//Lead either as a village head or guard leader, etc.
		Lead,
		//Create a family.
		Family,
		//Try to become any job/class/etc.
		Job,
		//Tries to earn this title
		Title
	}

	public List<string> names = new List<string> { "Villager" };

	public string villagerName = "Villager";

	public Section sectionAssigned { get; set; }

	public Stats stats;

	public bool setup = false;

	public VillageManager homeVillage;
	public VillageManager currentVillage;

	/// <summary>
	/// X Scale: -1 = Chaotic  --  0 = Neutral  --  1 = Lawful \n
	/// Y Scale: -1 = Evil     --  0 = Neutral  --  1 = Good \n
	/// Determines the Actions of Villagers to Situations
	/// Neutral Neutral means that their relationship with the person will affect descisions.
	/// </summary>
	public Vector2 personality = Vector2.zero;

	public virtual void Start()
	{
		gameObject.name = villagerName;
	}


	//Villager Family

	#region VillagerStatusControls

	public List<Villager> family { get; set; }

	public List<Villager> partners { get; set; }

	public List<Villager> children { get; set; }

	//String is the Person's Serial Key, Float is the Affinity Towards them.
	public Dictionary<string, float> knownPeople { get; set; }

	public Villager dad { get; set; }

	public Villager mum { get; set; }

	public Gender gender;

	public virtual void Initialise(Villager _dad, Villager _mum)
	{
		villagerName = names [Random.Range(0, names.Count)];

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

		//For the other Children, Add this Child to their family
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
		if (Mathf.Abs(vil.stats.age - stats.age) > currentVillage.maxAgeGap)
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

	#endregion





	public virtual void Initialise()
	{
		villagerName = names [Random.Range(0, names.Count)];

		family = new List<Villager>();
		partners = new List<Villager>();
		children = new List<Villager>();

		stats = new Stats();
		stats.Initialise();

		setup = true;
	}


	public virtual void UpdateVillager(float villageTime)
	{
		if (!setup)
		{
			return;
		}

		stats.age += (villageTime);

		if (stats.age > stats.lifeTime)
		{
			if (currentVillage != null)
			{
				currentVillage.AddToKillList(this);
			}
			else
			{
				Kill();
			}
		}
	}

	public virtual void Kill()
	{
		//Remove themselves from their children's family lists
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

		//Remove themselves from their family lists
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

		//Remove themselves from their partners lists
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

		//Remove themselves from their section
		if (sectionAssigned != null)
		{
			sectionAssigned.GetVillagers().Remove(this);
		}

		//Remove themselves from the Villagers List
		if (currentVillage != null)
		{
			currentVillage.villagers.Remove(this);
		}
	}

	public virtual bool CanHaveChildren()
	{
		if (!(children.Count < stats.maxChildren))
		{
			return false;
		}
		return true;
	}
}
