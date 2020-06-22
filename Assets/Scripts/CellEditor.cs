using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[ExecuteInEditMode]
public class CellEditor : MonoBehaviour
{
	public Transform[] anchorTransforms;
	public Vector3 offset;
	public Vector3[] anchorVectors;
	public bool updateAnchorVectors=true;
	public bool updateAnchorTransforms=false;

	// Start is called before the first frame update
	void Start()
	{
	}
	Mesh mesh;
	Vector3[] points;

	Vector2[] uvItr=new Vector2[]{new Vector2(0.5f,0.5f),new Vector2(0.5f,1),new Vector2(1,1),new Vector2(1,0.5f)};

	// Update is called once per frame
	void Update()
	{
		{
			points=new Vector3[anchorTransforms.Length+(anchorTransforms.Length%2==1?1:0)];
			for(int p=0;p<anchorTransforms.Length;p++)
				points[p]=anchorTransforms[p].localPosition;
			if(points.Length>anchorTransforms.Length)
				points[points.Length-1]=points[points.Length-2]+new Vector3(0.00001f,0.00001f,0.00001f);
		}
		if(mesh==null)
			mesh = new Mesh();
		Vector3[] verts = new Vector3[points.Length * 2*2];
		int[] tris=new int[(points.Length*2+(points.Length-2)*2)*3];
		Vector2[] uvs=new Vector2[points.Length * 2*2];
		int tr=0;
		for (int p = 0; p < points.Length; p++)
		{
			verts[points.Length * 0 + p] = verts[points.Length * 2 + p] = points[p]-offset;
			verts[points.Length * 1 + p] = verts[points.Length * 3 + p] = points[p]+offset;

			uvs[points.Length * 0 + p]=new Vector2(p%2,0);
			uvs[points.Length * 1 + p]=new Vector2(p%2,1);

			uvs[points.Length * 2 + p]=uvs[points.Length * 3 + p]=false?uvItr[p%uvItr.Length]:uvItr[p==0?0:(p==1?1:(p!=points.Length-1?2:3))];

			tris[tr++]=points.Length * 0 + (p	)%points.Length;
			tris[tr++]=points.Length * 1 + (p	)%points.Length;
			tris[tr++]=points.Length * 1 + (p+1	)%points.Length;
			tris[tr++]=points.Length * 0 + (p	)%points.Length;
			tris[tr++]=points.Length * 1 + (p+1	)%points.Length;
			tris[tr++]=points.Length * 0 + (p+1	)%points.Length;

			if(p>1)
			{
				tris[tr++]=points.Length * 2 + (0	)%points.Length;
				tris[tr++]=points.Length * 2 + (p-1	)%points.Length;
				tris[tr++]=points.Length * 2 + (p	)%points.Length;

				tris[tr++]=points.Length * 3 + (p	)%points.Length;
				tris[tr++]=points.Length * 3 + (p-1	)%points.Length;
				tris[tr++]=points.Length * 3 + (0	)%points.Length;
			}
		}
		mesh.vertices=verts;
		mesh.triangles=tris;
		mesh.uv=uvs;
		mesh.RecalculateNormals();
		GetComponent<MeshFilter>().mesh=mesh;
		GetComponent<MeshCollider>().sharedMesh=null;
		GetComponent<MeshCollider>().sharedMesh=mesh;
	}
}
