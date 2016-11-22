using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stat
{
	public int value { get; private set; }

	public void Initialise()
	{
		value = Random.Range(5, 10);
	}

	public void Initialise(Stat dadStat, Stat mumStat, bool male)
	{
		float split = (Random.value * 0.5f);

		if (male)
		{
			split += 0.5f;
		}

		value = Mathf.RoundToInt(Mathf.Lerp(0, ((float)dadStat.value), split) + Mathf.Lerp(0, ((float)mumStat.value), (1.0f - split)) + Random.Range(-2.0f, 3.0f));
	}
}

[System.Serializable]
public class Stats
{
	private Stat strength;

	public Stat GetStrength()
	{
		return strength;
	}

	private Stat vitality;


	public Stat GetVitality()
	{
		return vitality;
	}


	public int lifeTime { get; private set; }

	public int possiblePartnerCount { get; private set; }

	public float age = 0;

	public int maxChildren = 2;

	public void Initialise()
	{
		strength = new Stat();
		strength.Initialise();

		vitality = new Stat();
		vitality.Initialise();

		CalculateAllStatRefences();
	}

	public void Initialise(Villager dad, Villager mum, bool male)
	{
		strength = new Stat();
		strength.Initialise(dad.stats.strength, mum.stats.strength, male);

		vitality = new Stat();
		vitality.Initialise(dad.stats.vitality, mum.stats.vitality, male);

		CalculateAllStatRefences();
	}

	private void CalculateAllStatRefences()
	{
		possiblePartnerCount = Mathf.FloorToInt((1.4121f * Mathf.Log((float)strength.value)) - 1.2103f);
		lifeTime = Mathf.RoundToInt((20 * ((float)vitality.value - 5)) + 50);
	}
}
