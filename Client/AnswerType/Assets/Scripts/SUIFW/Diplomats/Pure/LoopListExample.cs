using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataModule;
using UnityEngine;
using UnityEngine.UI;

public class LoopListExample : MonoBehaviour
{
	public List<int> _dictionData;
    public DynamicInfinityListRenderer m_Dl;
    private Transform _choicebar;
    private int _countId	=0 ;

    private int _index =1;
    // Use this for initialization
    void Start () {
	    _dictionData=new List<int>();
	    foreach (var VARIABLE in DataModuleManager._instance.TableAnswerInfoData_Dictionary.Values.ToList())
	    {
		    _dictionData.Add(_index);
		    _index++;
	    }
	    m_Dl.InitRendererList(OnSelectHandler,null); 
	    m_Dl.SetDataProvider(_dictionData);
    }

    void OnSelectHandler(DynamicInfinityItem item)
    {
        // print("on select "+item.ToString());
    }

    // Update is called once per frame
    void Update () {
		
	}
}
