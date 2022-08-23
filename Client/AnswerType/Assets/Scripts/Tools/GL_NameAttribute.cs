//2018.08.30    关林
//名称属性

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_NameAttribute : PropertyAttribute {

	public string _name;
    public GL_NameAttribute(string name)
	{
		_name = name;
	}
}
