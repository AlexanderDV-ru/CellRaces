using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public enum CellSource
{
	Transforms,
	Vectors,
	Pattern
}
public enum Pattern
{
	Cube,
	Angle,
	A,
	B,
	Diagonal,
	DiagonalX,
	DiagonalY,
	DiagonalXY,
	DiagonalAngle
}
[ExecuteInEditMode]
public class CellEditor : MonoBehaviour
{
	public Transform[] anchorTransforms;
	public Vector3 offset;
	public Vector3[] anchorVectors;
	public CellSource from=CellSource.Transforms;
	public Pattern patternName;
	public Vector3 patternScale=new Vector3(1,1,1);
	static Dictionary<Pattern,Vector3[]> getPatterns()
	{
		Dictionary<Pattern,Vector3[]> patterns=new Dictionary<Pattern,Vector3[]>();
		patterns.Add(Pattern.Cube,new Vector3[]{
			new Vector3(-0.5f,0,-0.5f),new Vector3(-0.5f,0,+0.5f),new Vector3(+0.5f,0,+0.5f),new Vector3(+0.5f,0,-0.5f)});
		patterns.Add(Pattern.Angle,new Vector3[]{
			new Vector3(-0.5f,0,-0.5f),new Vector3(-0.5f,0,+0.5f),new Vector3(+0.5f,0,-0.5f)});
		patterns.Add(Pattern.A,new Vector3[]{
			new Vector3(-0.5f,0,-0.5f),new Vector3(-0.5f,0,+0.5f),new Vector3(+0.5f,0,+0.5f),new Vector3(+1.5f,0,-0.5f)});
		patterns.Add(Pattern.B,new Vector3[]{
			new Vector3(-0.5f,0,-0.5f),new Vector3(-0.5f,0,+0.5f),new Vector3(+0.5f,0,+0.5f),new Vector3(+1.5f,0,-0.5f)});
		patterns.Add(Pattern.Diagonal,new Vector3[]{
			new Vector3(-0.5f,0,-0.5f),new Vector3(-1.5f,0,+0.5f),new Vector3(-0.5f,0,+0.5f),new Vector3(+0.5f,0,-0.5f)});
		patterns.Add(Pattern.DiagonalX,new Vector3[]{
			new Vector3(-0.5f,0,-0.5f),new Vector3(-1.5f,0,+0.5f),new Vector3(+0.5f,0,+0.5f),new Vector3(+1.5f,0,-0.5f)});
		patterns.Add(Pattern.DiagonalY,new Vector3[]{
			new Vector3(-0.5f,0,-0.5f),new Vector3(-1.5f,0,+1.5f),new Vector3(-0.5f,0,+1.5f),new Vector3(+0.5f,0,-0.5f)});
		patterns.Add(Pattern.DiagonalXY,new Vector3[]{
			new Vector3(-0.5f,0,-0.5f),new Vector3(-2.5f,0,+1.5f),new Vector3(-0.5f,0,+1.5f),new Vector3(+1.5f,0,-0.5f)});
		patterns.Add(Pattern.DiagonalAngle,new Vector3[]{
			new Vector3(-0.5f,0,-0.5f),new Vector3(-1.5f,0,+0.5f),new Vector3(-1.5f,0,+1.5f),new Vector3(+0.5f,0,-0.5f)});
		return patterns;
	}
	public static Dictionary<Pattern,Vector3[]> patterns=getPatterns();
	public bool updateTransforms=true;
	public bool updateVectors=true;
	public bool invert=true;

	// Start is called before the first frame update
	void Start()
	{
	}
	Mesh mesh;
	Vector3[] points;
static float fp=0,sp=1;
	static Vector2[] uvItr=new Vector2[]{new Vector2(fp,fp),new Vector2(fp,sp),new Vector2(sp,sp),new Vector2(sp,fp)};
	public static Vector3 Scaled(Vector3 a,Vector3 b)
	{
		return new Vector3(a.x*b.x,a.y*b.y,a.z*b.z);
	}
	// Update is called once per frame
	void Update()
	{
		{
			int length=from==CellSource.Transforms?anchorTransforms.Length:(from==CellSource.Vectors?anchorVectors.Length:patterns[patternName].Length);
			points=new Vector3[length+(length%2==1?1:0)];
			if(updateTransforms)
				System.Array.Resize(ref anchorTransforms,length);
			if(updateVectors)
				System.Array.Resize(ref anchorVectors,length);
			for(int p=0;p<length;p++)
			{
				points[p]=from==CellSource.Transforms?anchorTransforms[p].localPosition:(from==CellSource.Vectors?anchorVectors[p]:Scaled(patterns[patternName][p],patternScale));
				if(updateTransforms)
					anchorTransforms[p].localPosition=points[p];
				if(updateVectors)
					anchorVectors[p]=points[p];
			}
			if(points.Length>length)
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

			tris[tr+(invert?2:0)]=points.Length * 0 + (p	)%points.Length;
			tris[tr+(invert?1:1)]=points.Length * 1 + (p	)%points.Length;
			tris[tr+(invert?0:2)]=points.Length * 1 + (p+1	)%points.Length;

			tr+=3;

			tris[tr+(invert?2:0)]=points.Length * 0 + (p	)%points.Length;
			tris[tr+(invert?1:1)]=points.Length * 1 + (p+1	)%points.Length;
			tris[tr+(invert?0:2)]=points.Length * 0 + (p+1	)%points.Length;

			tr+=3;

			if(p>1)
			{
				tris[tr+(invert?2:0)]=points.Length * 2 + (0	)%points.Length;
				tris[tr+(invert?1:1)]=points.Length * 2 + (p-1	)%points.Length;
				tris[tr+(invert?0:2)]=points.Length * 2 + (p	)%points.Length;

				tr+=3;

				tris[tr+(invert?2:0)]=points.Length * 3 + (p	)%points.Length;
				tris[tr+(invert?1:1)]=points.Length * 3 + (p-1	)%points.Length;
				tris[tr+(invert?0:2)]=points.Length * 3 + (0	)%points.Length;

				tr+=3;
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
