using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VillagerF : Villager
{
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
		name = names [Random.Range(0, names.Count)];
	}

	public override void Initialise(Villager _dad, Villager _mum)
	{
		gender = Gender.Female;
		base.Initialise(_dad, _mum);
		name = names [Random.Range(0, names.Count)];
	}

	public override void Update(float villageTime)
	{
		base.Update(villageTime);
	}
}
