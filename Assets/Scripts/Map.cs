using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[System.Serializable]
public class SerPoint
{
	public Vector3 pos;
	public Vector3[] links;
}

public class Map : MonoBehaviour
{
	public Camera draw;
	public Vector3 size=new Vector3(1024,0,1024),offset=new Vector3(-0.5f,0,-0.5f), scale=new Vector3(1024,0,1024),brushSize=new Vector3(10,0,10);
	// Start is called before the first frame update
	Texture2D tex;
	void Start()
	{
		GetComponent<Renderer>().material=new Material(GetComponent<Renderer>().material);
		tex=new Texture2D((int)size.x,(int)size.z);
		GetComponent<Renderer>().material.mainTexture=tex;
		for(int x=0;x<tex.width;x++)
			for(int y=0;y<tex.height;y++)
			{
				tex.SetPixel(x,y,new Color(1,1,1));
			}
		tex.Apply();
	}
	Ray ray;
	Vector3 lastDot;
	Dictionary<Vector3,List<Vector3>> points=new Dictionary<Vector3,List<Vector3>>();
	bool ld;
	public string mode="set";
	public Color paintColor=new Color(0,0,0);

	// Update is called once per frame
	Vector3 selectedDot;
	public Material stPoint,selPoint,arrow;
	public Vector3 scalePoint;
	MapPoint lPoint;
	public bool nextMode;
	public bool bothArrowDirs;
	void Update()
	{
		switch (mode)
		{
			case "set":
				{
					RaycastHit hit;
			        ray = draw.ScreenPointToRay(Input.mousePosition);

			        if (Physics.Raycast(ray, out hit))
					{
						Debug.Log(hit.transform);
						if(hit.transform==transform)
							if(!lPoint||!lPoint.selected)
								if(Input.GetMouseButtonDown(0))
								{
									GameObject obj=GameObject.CreatePrimitive(PrimitiveType.Cube);
									obj.transform.parent=transform;
									obj.transform.position=hit.point;
									obj.transform.localScale=scalePoint;
									obj.AddComponent<MapPoint>();
									obj.GetComponent<MapPoint>().standardState=stPoint;
									obj.GetComponent<MapPoint>().selectedState=selPoint;
									obj.GetComponent<MapPoint>().selected=false;
								}
						Debug.Log(hit.transform.GetComponent<MeshRenderer>().material==stPoint);
						if(hit.transform.GetComponent<MapPoint>())
						{
							MapPoint cur=hit.transform.GetComponent<MapPoint>();
							if(Input.GetMouseButtonDown(0))
							{
								if(lPoint!=cur)
								{
									GameObject obj=GameObject.CreatePrimitive(PrimitiveType.Cube);
									if(!bothArrowDirs)
										obj.GetComponent<MeshRenderer>().material=arrow;
									Arrow arr=obj.AddComponent<Arrow>();
									arr.bothDirs=bothArrowDirs;
									arr.scale=scalePoint.z;
									arr.from=lPoint;
									arr.into=cur;

									lPoint.selected=false;
								}
								else Destroy(lPoint.gameObject);
								lPoint=nextMode?cur:null;
								if(nextMode)
									cur.selected=true;
							}
							if(Input.GetMouseButtonDown(1))
							{
								if(lPoint&&lPoint!=cur)
									lPoint.selected=false;

								cur.selected=!cur.selected;
								lPoint=cur.selected?cur:null;

							}
						}
					}
				}
				break;
			case "paint":
				if(Input.GetMouseButton(0))
				{
					RaycastHit hit;
			        ray = draw.ScreenPointToRay(Input.mousePosition);

			        if (Physics.Raycast(ray, out hit))
						if(hit.transform==transform)
						{
							Vector3 dot=new Vector3(((hit.point.x-transform.position.x+offset.x)*scale.x*transform.lossyScale.x),0, ((hit.point.z-transform.position.z+offset.z)*scale.z*transform.lossyScale.z));
							for(int i=0;i<=Vector3.Distance((ld?lastDot:dot),dot);i++)
							{
								Vector3 d=(ld?lastDot:dot)+(dot-(ld?lastDot:dot))/Vector3.Distance((ld?lastDot:dot),dot)*i;
								for(float x=d.x-brushSize.x/2;x<d.x+brushSize.x/2;x++)
									for(float z=d.z-brushSize.z/2;z<d.z+brushSize.z/2;z++)
										tex.SetPixel((int)x, (int)z,paintColor);
							}
							//Debug.Log(dot);
							tex.Apply();
							lastDot=dot;
							ld=true;
				        }
				}
				else ld=false;
				break;
		}
	}
	void OnDrawGizmos()
	{
		Gizmos.DrawRay(ray);
	}
}
