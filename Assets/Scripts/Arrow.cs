using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	public bool bothDirs;
	private MapPoint _from,_into;
	public MapPoint from{
		get=>_from;
		set{_from=value;fromIntoUpd();}
	}
	public MapPoint into{
		get=>_into;
		set{_into=value;fromIntoUpd();}
	}
	public float scale=0;
	void fromIntoUpd()
	{
		if(!from||!into||scale==0)
			return;
		transform.parent=from.transform;
		transform.localScale=new Vector3(0.5f,0.5f,Vector3.Distance(from.transform.position,into.transform.position)/scale*0.9f);
		transform.localPosition=new Vector3(0,0,0);
		transform.LookAt(into.transform);
		transform.localPosition=transform.forward*Vector3.Distance(from.transform.position,into.transform.position)/scale*0.9f/2;
	}
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
