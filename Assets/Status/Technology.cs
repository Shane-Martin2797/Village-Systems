using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnumAction
{
	Add,
	Replace,
	Remove
}

public class Technology
{
	public List<Prerequisite> requirements;

	public EnumAction technologyAction;

	//Technology Adds/Replaces/Removes
	//It can cause wood houses to become stone houses
	//It can cause a cure for the plague to be found
}
