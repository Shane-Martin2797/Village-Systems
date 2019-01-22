using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VillagerF : Villager
{
	public bool pregnant = false;
	public float timeSincePregnant;
	public float timePregnancyLasts = 0.75f;

	public List<VillagerM> partnersM;

	public List<string> names = new List<string>
	{
		{ "Abby" },
		{ "Ali" },
		{ "Jasmine" },
		{ "Cinderella" }
	};



	public override void Initialise()
	{
		gender = Gender.Female;
		base.Initialise();
		partnersM = new List<VillagerM>();
		name = names [Random.Range(0, names.Count)];
		stats.possiblePartnerCount = 1;
	}

	public override void Initialise(Villager _dad, Villager _mum)
	{
		gender = Gender.Female;
		base.Initialise(_dad, _mum);
		partnersM = new List<VillagerM>();
		name = names [Random.Range(0, names.Count)];
		stats.possiblePartnerCount = 1;
	}

	public override void Update(float villageTime)
	{
		base.Update(villageTime);
		if (pregnant)
		{
			timeSincePregnant += Time.deltaTime;
		}
	}

	public bool AddPartner(VillagerM vil)
	{
		if (CanAddPartner(vil))
		{
			partners.Add(vil);
			partnersM.Add(vil);
			AddFamily(vil.family);

			vil.partners.Add(this);
			vil.partnersF.Add(this);
			vil.AddFamily(this.family);

			return true;
		}
		else
		{
			return false;
		}
	}

	public bool CanAddPartner(VillagerM vil)
	{
		//Null Check
		if (vil == null)
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
		if (Mathf.Abs(vil.stats.age - stats.age) > VillageManager.Instance.maxAgeGap)
		{
			return false;
		}

		return true;
	}

	public override bool CanHaveChildren()
	{
		if (!base.CanHaveChildren())
		{
			return false;
		}

		if (pregnant)
		{
			return false;
		}

		return true;
	}
}
