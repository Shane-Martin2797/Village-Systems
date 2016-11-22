using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Section
{

	private List<Villager> villagers = new List<Villager>();

	public List<Villager> GetVillagers()
	{
		return villagers;
	}

	public Bounds box;

	public void Initialise()
	{
	}

	public void Initialise(int x, int y, Vector2 size)
	{
		Bounds b = new Bounds(new Vector3(x, y, 0), (Vector3)size);
		box = b;
	}

	public Villager PickPartner(Villager vil)
	{
		for (int i = 0; i < villagers.Count; i++)
		{
			if (villagers [i] == vil)
			{
				continue;
			}

			if (vil.CanAddPartner(villagers [i]))
			{
				return villagers [i];
			}
		}
		return null;
	}
}
