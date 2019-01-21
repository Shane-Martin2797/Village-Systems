using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VillagerM : Villager
{
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
		name = names [Random.Range(0, names.Count)];
	}

	public override void Initialise(Villager _dad, Villager _mum)
	{
		gender = Gender.Male;
		base.Initialise(_dad, _mum);
		name = names [Random.Range(0, names.Count)];
	}

	public override void Update(float villageTime)
	{
		base.Update(villageTime);

		if (stats.age >= VillageManager.Instance.adultAge)
		{
			if (partners.Count < stats.possiblePartnerCount)
			{
				Villager partner = sectionAssigned.PickPartner(this);
				AddPartner(partner);
			}
		}


		//Change the rule for when children can be had.
		if (Mathf.RoundToInt(stats.age) % 5 == 0)
		{
			for (int i = 0; i < partners.Count; i++)
			{
				if (partners [i].children.Count < partners [i].stats.maxChildren)
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
