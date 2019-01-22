using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VillagerM : Villager
{
	public List<VillagerF> partnersF;

	public List<string> names = new List<string>
	{
		{ "Daniel" },
		{ "Patrick" },
		{ "Jarred" },
		{ "Roland" }
	};

	public override void Initialise()
	{
		gender = Gender.Male;
		base.Initialise();
		partnersF = new List<VillagerF>();
		name = names [Random.Range(0, names.Count)];
	}

	public override void Initialise(Villager _dad, Villager _mum)
	{
		gender = Gender.Male;
		base.Initialise(_dad, _mum);
		partnersF = new List<VillagerF>();
		name = names [Random.Range(0, names.Count)];
	}

	public override void Update(float villageTime)
	{
		base.Update(villageTime);

//		if (stats.age >= VillageManager.Instance.adultAge)
//		{
//			if (partners.Count < stats.possiblePartnerCount)
//			{
//				Villager partner = sectionAssigned.PickPartner(this);
//				AddPartner(partner);
//			}
//		}
	}

	public bool AddPartner(VillagerF vil)
	{
		if (CanAddPartner(vil))
		{
			if (partnersF == null)
			{
				partnersF = new List<VillagerF>();
			}

			partnersF.Add(vil);
			partners.Add(vil);
			AddFamily(vil.family);

			vil.partners.Add(this);

			if (vil.partnersM == null)
			{
				vil.partnersM = new List<VillagerM>();
			}

			vil.partnersM.Add(this);
			vil.AddFamily(this.family);

			return true;
		}
		else
		{
			return false;
		}
	}

	public bool CanAddPartner(VillagerF vil)
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

	public bool CanHaveChildren()
	{
		if (!base.CanHaveChildren())
		{
			return false;
		}
		return true;
	}

}
