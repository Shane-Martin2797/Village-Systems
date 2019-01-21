using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Requirement
{
	People,
	Job,
	Attribute,
	Material,
	Technology
}

public class Action
{
	public List<Prerequisite> requirements;

}


public class Prerequisite
{
	public Requirement req;

	public int numberOfPeopleRequired = 1;
	public Job requiredJob;
	public Stat requiredAttributes;
	public BuildSupplies requiredMaterials;
	public Technology requiredTechnology;
}