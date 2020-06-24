using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
	private bool _selected;
	public bool selected{
		get=>_selected;
		set{_selected=value;GetComponent<MeshRenderer>().material=selected?selectedState:standardState;}
	}
	public Material standardState,selectedState;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
