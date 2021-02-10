using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
	/**
     * Created by Peter @sHTiF Stefcek 20.10.2020, some functions based on Unity wiki
     */
	public class MeshUtils
	{
		static public Mesh CreateArc(Vector3 p_center, Vector3 p_startPoint, Vector3 p_axis, float p_radius, float p_angle, int p_segmentCount)
		{
			Mesh mesh = new Mesh();
			
			Vector3[] vertices = new Vector3[p_segmentCount+2];

			Vector3 startVector = (p_startPoint - p_center).normalized * p_radius;
			for (int i = 0; i<=p_segmentCount; i++)
			{
				float rad = (float) i / p_segmentCount * p_angle;
				Vector3 v = Quaternion.AngleAxis(rad*180f/Mathf.PI, p_axis) * startVector;
				vertices[i] = v + p_center;
			}
			vertices[p_segmentCount+1] = p_center;
			
			Vector3[] normals = new Vector3[vertices.Length];
			for( int n = 0; n < normals.Length; n++ )
				normals[n] = Vector3.up;

			Vector2[] uvs = new Vector2[vertices.Length];
			for (int i = 0; i<=p_segmentCount; i++)
			{
				float rad = (float) i / p_segmentCount * p_angle;
				uvs[i] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
			}
			uvs[p_segmentCount + 1] = Vector2.one / 2f;
			
			int[] triangles = new int[ p_segmentCount * 3 ];
			for (int i = 0; i < p_segmentCount; i++)
			{
				int index = i * 3;
				triangles[index] = p_segmentCount+1;
				triangles[index+1] = i;
				triangles[index+2] = i + 1;
			}
			
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;
 
			mesh.RecalculateBounds();
			mesh.Optimize();
			
			return mesh;
		}
		
		static public Mesh CreateArc(float p_radius, float p_angle, int p_segmentCount)
		{
			Mesh mesh = new Mesh();
			
			Vector3[] vertices = new Vector3[p_segmentCount+2];
			
			for (int i = 0; i<=p_segmentCount; i++)
			{
				float rad = (float) i / p_segmentCount * p_angle;
				vertices[i] = new Vector3(Mathf.Cos(rad) * p_radius, 0f, Mathf.Sin(rad) * p_radius);
			}
			vertices[p_segmentCount+1] = Vector3.zero;
			
			Vector3[] normals = new Vector3[vertices.Length];
			for( int n = 0; n < normals.Length; n++ )
				normals[n] = Vector3.up;

			Vector2[] uvs = new Vector2[vertices.Length];
			for (int i = 0; i<=p_segmentCount; i++)
			{
				float rad = (float) i / p_segmentCount * p_angle;
				uvs[i] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
			}
			uvs[p_segmentCount + 1] = Vector2.one / 2f;
			
			int[] triangles = new int[ p_segmentCount * 3 ];
			for (int i = 0; i < p_segmentCount; i++)
			{
				int index = i * 3;
				triangles[index] = p_segmentCount+1;
				triangles[index+1] = i;
				triangles[index+2] = i + 1;
			}
			
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;
 
			mesh.RecalculateBounds();
			mesh.Optimize();
			
			return mesh;
		}
		
		static public Mesh CreateGrid(float p_width, float p_height, int p_segmentsX = 1, int p_segmentsY = 1)
		{
			Mesh mesh = new Mesh();

			int resX = p_segmentsX + 1;
			int resZ = p_segmentsY + 1;
			
			Vector3[] vertices = new Vector3[ resX * resZ ];
			for(int z = 0; z < resZ; z++)
			{
				float zPos = ((float)z / (resZ - 1) - .5f) * p_height;
				for(int x = 0; x < resX; x++)
				{
					float xPos = ((float)x / (resX - 1) - .5f) * p_width;
					vertices[ x + z * resX ] = new Vector3( xPos, 0f, zPos );
				}
			}

			
			Vector3[] normals = new Vector3[ vertices.Length ];
			for( int n = 0; n < normals.Length; n++ )
				normals[n] = Vector3.up;

			
			Vector2[] uvs = new Vector2[ vertices.Length ];
			for(int v = 0; v < resZ; v++)
			{
				for(int u = 0; u < resX; u++)
				{
					uvs[ u + v * resX ] = new Vector2( (float)u / (resX - 1), (float)v / (resZ - 1) );
				}
			}


			int faceCount = (resX - 1) * (resZ - 1);
			int[] triangles = new int[ faceCount * 6 ];
			int t = 0;
			for(int face = 0; face < faceCount; face++ )
			{
				// Retrieve lower left corner from face ind
				int i = face % (resX - 1) + (face / (resZ - 1) * resX);
 
				triangles[t++] = i + resX;
				triangles[t++] = i + 1;
				triangles[t++] = i;
 
				triangles[t++] = i + resX;	
				triangles[t++] = i + resX + 1;
				triangles[t++] = i + 1; 
			}

 
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;
 
			mesh.RecalculateBounds();
			mesh.Optimize();

			return mesh;
		}
		
		static public Mesh CreateBox(float p_width, float p_height, float p_depth)
		{
			Mesh mesh = new Mesh();

			Vector3 v0 = new Vector3(-p_depth * .5f, -p_width * .5f, p_height * .5f);
			Vector3 v1 = new Vector3(p_depth * .5f, -p_width * .5f, p_height * .5f);
			Vector3 v2 = new Vector3(p_depth * .5f, -p_width * .5f, -p_height * .5f);
			Vector3 v3 = new Vector3(-p_depth * .5f, -p_width * .5f, -p_height * .5f);

			Vector3 v4 = new Vector3(-p_depth * .5f, p_width * .5f, p_height * .5f);
			Vector3 v5 = new Vector3(p_depth * .5f, p_width * .5f, p_height * .5f);
			Vector3 v6 = new Vector3(p_depth * .5f, p_width * .5f, -p_height * .5f);
			Vector3 v7 = new Vector3(-p_depth * .5f, p_width * .5f, -p_height * .5f);

			Vector3[] vertices = new Vector3[]
			{
				// Bottom
				v0, v1, v2, v3,

				// Left
				v7, v4, v0, v3,

				// Front
				v4, v5, v1, v0,

				// Back
				v6, v7, v3, v2,

				// Right
				v5, v6, v2, v1,

				// Top
				v7, v6, v5, v4
			};

			Vector3 up = Vector3.up;
			Vector3 down = Vector3.down;
			Vector3 front = Vector3.forward;
			Vector3 back = Vector3.back;
			Vector3 left = Vector3.left;
			Vector3 right = Vector3.right;

			Vector3[] normals = new Vector3[]
			{
				down, down, down, down,

				left, left, left, left,

				front, front, front, front,

				back, back, back, back,

				right, right, right, right,

				up, up, up, up
			};

			Vector2 _00 = new Vector2(0f, 0f);
			Vector2 _10 = new Vector2(1f, 0f);
			Vector2 _01 = new Vector2(0f, 1f);
			Vector2 _11 = new Vector2(1f, 1f);

			Vector2[] uvs = new Vector2[]
			{
				// Bottom
				_11, _01, _00, _10,

				// Left
				_11, _01, _00, _10,

				// Front
				_11, _01, _00, _10,

				// Back
				_11, _01, _00, _10,

				// Right
				_11, _01, _00, _10,

				// Top
				_11, _01, _00, _10,
			};

			int[] triangles = new int[]
			{
				// Bottom
				3, 1, 0,
				3, 2, 1,

				// Left
				3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
				3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,

				// Front
				3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
				3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,

				// Back
				3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
				3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,

				// Right
				3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
				3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,

				// Top
				3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
				3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
			};

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			mesh.RecalculateBounds();
			mesh.Optimize();

			return mesh;
		}

		static public Mesh CreateCone(float p_height, float p_bottomRadius, float p_topRadius, int p_sideCount,
			int p_heightSegmentCount)
		{
			Mesh mesh = new Mesh();

			int vertexCapCount = p_sideCount + 1;

			// bottom + top + sides
			Vector3[] vertices =
				new Vector3[vertexCapCount + vertexCapCount + p_sideCount * p_heightSegmentCount * 2 + 2];
			int vert = 0;
			float _2pi = Mathf.PI * 2f;

			// Bottom cap
			vertices[vert++] = new Vector3(0f, 0f, 0f);
			while (vert <= p_sideCount)
			{
				float rad = (float) vert / p_sideCount * _2pi;
				vertices[vert] = new Vector3(Mathf.Cos(rad) * p_bottomRadius, 0f, Mathf.Sin(rad) * p_bottomRadius);
				vert++;
			}

			// Top cap
			vertices[vert++] = new Vector3(0f, p_height, 0f);
			while (vert <= p_sideCount * 2 + 1)
			{
				float rad = (float) (vert - p_sideCount - 1) / p_sideCount * _2pi;
				vertices[vert] = new Vector3(Mathf.Cos(rad) * p_topRadius, p_height, Mathf.Sin(rad) * p_topRadius);
				vert++;
			}

			// Sides
			int v = 0;
			while (vert <= vertices.Length - 4)
			{
				float rad = (float) v / p_sideCount * _2pi;
				vertices[vert] = new Vector3(Mathf.Cos(rad) * p_topRadius, p_height, Mathf.Sin(rad) * p_topRadius);
				vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * p_bottomRadius, 0, Mathf.Sin(rad) * p_bottomRadius);
				vert += 2;
				v++;
			}

			vertices[vert] = vertices[p_sideCount * 2 + 2];
			vertices[vert + 1] = vertices[p_sideCount * 2 + 3];


			// bottom + top + sides
			Vector3[] normals = new Vector3[vertices.Length];
			vert = 0;

			// Bottom cap
			while (vert <= p_sideCount)
			{
				normals[vert++] = Vector3.down;
			}

			// Top cap
			while (vert <= p_sideCount * 2 + 1)
			{
				normals[vert++] = Vector3.up;
			}

			// Sides
			v = 0;
			while (vert <= vertices.Length - 4)
			{
				float rad = (float) v / p_sideCount * _2pi;
				float cos = Mathf.Cos(rad);
				float sin = Mathf.Sin(rad);

				normals[vert] = new Vector3(cos, 0f, sin);
				normals[vert + 1] = normals[vert];

				vert += 2;
				v++;
			}

			normals[vert] = normals[p_sideCount * 2 + 2];
			normals[vert + 1] = normals[p_sideCount * 2 + 3];


			Vector2[] uvs = new Vector2[vertices.Length];

			// Bottom cap
			int u = 0;
			uvs[u++] = new Vector2(0.5f, 0.5f);
			while (u <= p_sideCount)
			{
				float rad = (float) u / p_sideCount * _2pi;
				uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
				u++;
			}

			// Top cap
			uvs[u++] = new Vector2(0.5f, 0.5f);
			while (u <= p_sideCount * 2 + 1)
			{
				float rad = (float) u / p_sideCount * _2pi;
				uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
				u++;
			}

			// Sides
			int u_sides = 0;
			while (u <= uvs.Length - 4)
			{
				float t = (float) u_sides / p_sideCount;
				uvs[u] = new Vector3(t, 1f);
				uvs[u + 1] = new Vector3(t, 0f);
				u += 2;
				u_sides++;
			}

			uvs[u] = new Vector2(1f, 1f);
			uvs[u + 1] = new Vector2(1f, 0f);

			int triangleCount = p_sideCount + p_sideCount + p_sideCount * 2;
			int[] triangles = new int[triangleCount * 3 + 3];

			// Bottom cap
			int tri = 0;
			int i = 0;
			while (tri < p_sideCount - 1)
			{
				triangles[i] = 0;
				triangles[i + 1] = tri + 1;
				triangles[i + 2] = tri + 2;
				tri++;
				i += 3;
			}

			triangles[i] = 0;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = 1;
			tri++;
			i += 3;

			// Top cap
			while (tri < p_sideCount * 2)
			{
				triangles[i] = tri + 2;
				triangles[i + 1] = tri + 1;
				triangles[i + 2] = vertexCapCount;
				tri++;
				i += 3;
			}

			triangles[i] = vertexCapCount + 1;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = vertexCapCount;
			tri++;
			i += 3;
			tri++;

			// Sides
			while (tri <= triangleCount)
			{
				triangles[i] = tri + 2;
				triangles[i + 1] = tri + 1;
				triangles[i + 2] = tri + 0;
				tri++;
				i += 3;

				triangles[i] = tri + 1;
				triangles[i + 1] = tri + 2;
				triangles[i + 2] = tri + 0;
				tri++;
				i += 3;
			}


			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			mesh.RecalculateBounds();
			mesh.Optimize();

			return mesh;
		}

		static public Mesh CreateTube(float p_height, int p_sideCount, float p_bottomRadius, float p_bottomThickness,
			float p_topRadius, float p_topThickness)
		{
			Mesh mesh = new Mesh();

			int vertexCapCount = p_sideCount * 2 + 2;
			int vertexSideCount = p_sideCount * 2 + 2;


			// bottom + top + sides
			Vector3[] vertices = new Vector3[vertexCapCount * 2 + vertexSideCount * 2];
			int vert = 0;
			float _2pi = Mathf.PI * 2f;

			// Bottom cap
			int sideCounter = 0;
			while (vert < vertexCapCount)
			{
				sideCounter = sideCounter == p_sideCount ? 0 : sideCounter;

				float r1 = (float) (sideCounter++) / p_sideCount * _2pi;
				float cos = Mathf.Cos(r1);
				float sin = Mathf.Sin(r1);
				vertices[vert] = new Vector3(cos * (p_bottomRadius - p_bottomThickness * .5f), 0f,
					sin * (p_bottomRadius - p_bottomThickness * .5f));
				vertices[vert + 1] = new Vector3(cos * (p_bottomRadius + p_bottomThickness * .5f), 0f,
					sin * (p_bottomRadius + p_bottomThickness * .5f));
				vert += 2;
			}

			// Top cap
			sideCounter = 0;
			while (vert < vertexCapCount * 2)
			{
				sideCounter = sideCounter == p_sideCount ? 0 : sideCounter;

				float r1 = (float) (sideCounter++) / p_sideCount * _2pi;
				float cos = Mathf.Cos(r1);
				float sin = Mathf.Sin(r1);
				vertices[vert] = new Vector3(cos * (p_topRadius - p_topThickness * .5f), p_height,
					sin * (p_topRadius - p_topThickness * .5f));
				vertices[vert + 1] = new Vector3(cos * (p_topRadius + p_topThickness * .5f), p_height,
					sin * (p_topRadius + p_topThickness * .5f));
				vert += 2;
			}

			// Sides (out)
			sideCounter = 0;
			while (vert < vertexCapCount * 2 + vertexSideCount)
			{
				sideCounter = sideCounter == p_sideCount ? 0 : sideCounter;

				float r1 = (float) (sideCounter++) / p_sideCount * _2pi;
				float cos = Mathf.Cos(r1);
				float sin = Mathf.Sin(r1);

				vertices[vert] = new Vector3(cos * (p_topRadius + p_topThickness * .5f), p_height,
					sin * (p_topRadius + p_topThickness * .5f));
				vertices[vert + 1] = new Vector3(cos * (p_bottomRadius + p_bottomThickness * .5f), 0,
					sin * (p_bottomRadius + p_bottomThickness * .5f));
				vert += 2;
			}

			// Sides (in)
			sideCounter = 0;
			while (vert < vertices.Length)
			{
				sideCounter = sideCounter == p_sideCount ? 0 : sideCounter;

				float r1 = (float) (sideCounter++) / p_sideCount * _2pi;
				float cos = Mathf.Cos(r1);
				float sin = Mathf.Sin(r1);

				vertices[vert] = new Vector3(cos * (p_topRadius - p_topThickness * .5f), p_height,
					sin * (p_topRadius - p_topThickness * .5f));
				vertices[vert + 1] = new Vector3(cos * (p_bottomRadius - p_bottomThickness * .5f), 0,
					sin * (p_bottomRadius - p_bottomThickness * .5f));
				vert += 2;
			}


			// bottom + top + sides
			Vector3[] normals = new Vector3[vertices.Length];
			vert = 0;

			// Bottom cap
			while (vert < vertexCapCount)
			{
				normals[vert++] = Vector3.down;
			}

			// Top cap
			while (vert < vertexCapCount * 2)
			{
				normals[vert++] = Vector3.up;
			}

			// Sides (out)
			sideCounter = 0;
			while (vert < vertexCapCount * 2 + vertexSideCount)
			{
				sideCounter = sideCounter == p_sideCount ? 0 : sideCounter;

				float r1 = (float) (sideCounter++) / p_sideCount * _2pi;

				normals[vert] = new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1));
				normals[vert + 1] = normals[vert];
				vert += 2;
			}

			// Sides (in)
			sideCounter = 0;
			while (vert < vertices.Length)
			{
				sideCounter = sideCounter == p_sideCount ? 0 : sideCounter;

				float r1 = (float) (sideCounter++) / p_sideCount * _2pi;

				normals[vert] = -(new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1)));
				normals[vert + 1] = normals[vert];
				vert += 2;
			}

			Vector2[] uvs = new Vector2[vertices.Length];

			vert = 0;
			// Bottom cap
			sideCounter = 0;
			while (vert < vertexCapCount)
			{
				float t = (float) (sideCounter++) / p_sideCount;
				uvs[vert++] = new Vector2(0f, t);
				uvs[vert++] = new Vector2(1f, t);
			}

			// Top cap
			sideCounter = 0;
			while (vert < vertexCapCount * 2)
			{
				float t = (float) (sideCounter++) / p_sideCount;
				uvs[vert++] = new Vector2(0f, t);
				uvs[vert++] = new Vector2(1f, t);
			}

			// Sides (out)
			sideCounter = 0;
			while (vert < vertexCapCount * 2 + vertexSideCount)
			{
				float t = (float) (sideCounter++) / p_sideCount;
				uvs[vert++] = new Vector2(t, 0f);
				uvs[vert++] = new Vector2(t, 1f);
			}

			// Sides (in)
			sideCounter = 0;
			while (vert < vertices.Length)
			{
				float t = (float) (sideCounter++) / p_sideCount;
				uvs[vert++] = new Vector2(t, 0f);
				uvs[vert++] = new Vector2(t, 1f);
			}

			int faceCount = p_sideCount * 4;
			int triangleCount = faceCount * 2;
			int indexCount = triangleCount * 3;
			int[] triangles = new int[indexCount];

			// Bottom cap
			int i = 0;
			sideCounter = 0;
			while (sideCounter < p_sideCount)
			{
				int current = sideCounter * 2;
				int next = sideCounter * 2 + 2;

				triangles[i++] = next + 1;
				triangles[i++] = next;
				triangles[i++] = current;

				triangles[i++] = current + 1;
				triangles[i++] = next + 1;
				triangles[i++] = current;

				sideCounter++;
			}

			// Top cap
			while (sideCounter < p_sideCount * 2)
			{
				int current = sideCounter * 2 + 2;
				int next = sideCounter * 2 + 4;

				triangles[i++] = current;
				triangles[i++] = next;
				triangles[i++] = next + 1;

				triangles[i++] = current;
				triangles[i++] = next + 1;
				triangles[i++] = current + 1;

				sideCounter++;
			}

			// Sides (out)
			while (sideCounter < p_sideCount * 3)
			{
				int current = sideCounter * 2 + 4;
				int next = sideCounter * 2 + 6;

				triangles[i++] = current;
				triangles[i++] = next;
				triangles[i++] = next + 1;

				triangles[i++] = current;
				triangles[i++] = next + 1;
				triangles[i++] = current + 1;

				sideCounter++;
			}


			// Sides (in)
			while (sideCounter < p_sideCount * 4)
			{
				int current = sideCounter * 2 + 6;
				int next = sideCounter * 2 + 8;

				triangles[i++] = next + 1;
				triangles[i++] = next;
				triangles[i++] = current;

				triangles[i++] = current + 1;
				triangles[i++] = next + 1;
				triangles[i++] = current;

				sideCounter++;
			}

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			mesh.RecalculateBounds();
			mesh.Optimize();

			return mesh;
		}

		static public Mesh CreateTorus(float p_radius, float p_thickness, int p_radiusSegmentCount, int p_sideCount)
		{
			Mesh mesh = new Mesh();


			Vector3[] vertices = new Vector3[(p_radiusSegmentCount + 1) * (p_sideCount + 1)];
			float _2pi = Mathf.PI * 2f;
			for (int seg = 0; seg <= p_radiusSegmentCount; seg++)
			{
				int currSeg = seg == p_radiusSegmentCount ? 0 : seg;

				float t1 = (float) currSeg / p_radiusSegmentCount * _2pi;
				Vector3 r1 = new Vector3(Mathf.Cos(t1) * p_radius, 0f, Mathf.Sin(t1) * p_radius);

				for (int side = 0; side <= p_sideCount; side++)
				{
					int currSide = side == p_sideCount ? 0 : side;

					Vector3 normale = Vector3.Cross(r1, Vector3.up);
					float t2 = (float) currSide / p_sideCount * _2pi;
					Vector3 r2 = Quaternion.AngleAxis(-t1 * Mathf.Rad2Deg, Vector3.up) *
					             new Vector3(Mathf.Sin(t2) * p_thickness, Mathf.Cos(t2) * p_thickness);

					vertices[side + seg * (p_sideCount + 1)] = r1 + r2;
				}
			}


			Vector3[] normals = new Vector3[vertices.Length];
			for (int seg = 0; seg <= p_radiusSegmentCount; seg++)
			{
				int currSeg = seg == p_radiusSegmentCount ? 0 : seg;

				float t1 = (float) currSeg / p_radiusSegmentCount * _2pi;
				Vector3 r1 = new Vector3(Mathf.Cos(t1) * p_radius, 0f, Mathf.Sin(t1) * p_radius);

				for (int side = 0; side <= p_sideCount; side++)
				{
					normals[side + seg * (p_sideCount + 1)] =
						(vertices[side + seg * (p_sideCount + 1)] - r1).normalized;
				}
			}


			Vector2[] uvs = new Vector2[vertices.Length];
			for (int seg = 0; seg <= p_radiusSegmentCount; seg++)
			for (int side = 0; side <= p_sideCount; side++)
				uvs[side + seg * (p_sideCount + 1)] =
					new Vector2((float) seg / p_radiusSegmentCount, (float) side / p_sideCount);


			int faceCount = vertices.Length;
			int triangleCount = faceCount * 2;
			int indexCount = triangleCount * 3;
			int[] triangles = new int[indexCount];

			int i = 0;
			for (int seg = 0; seg <= p_radiusSegmentCount; seg++)
			{
				for (int side = 0; side <= p_sideCount - 1; side++)
				{
					int current = side + seg * (p_sideCount + 1);
					int next = side + (seg < (p_radiusSegmentCount) ? (seg + 1) * (p_sideCount + 1) : 0);

					if (i < triangles.Length - 6)
					{
						triangles[i++] = current;
						triangles[i++] = next;
						triangles[i++] = next + 1;

						triangles[i++] = current;
						triangles[i++] = next + 1;
						triangles[i++] = current + 1;
					}
				}
			}

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			mesh.RecalculateBounds();
			mesh.Optimize();

			return mesh;
		}

		static public Mesh CreateSphere(float p_radius, int p_longitudeCount, int p_lattitudeCount)
		{
			Mesh mesh = new Mesh();
			mesh.Clear();
			
			Vector3[] vertices = new Vector3[(p_longitudeCount+1) * p_lattitudeCount + 2];
			float _pi = Mathf.PI;
			float _2pi = _pi * 2f;
			 
			vertices[0] = Vector3.up * p_radius;
			for( int lat = 0; lat < p_lattitudeCount; lat++ )
			{
				float a1 = _pi * (float)(lat+1) / (p_lattitudeCount+1);
				float sin1 = Mathf.Sin(a1);
				float cos1 = Mathf.Cos(a1);
			 
				for( int lon = 0; lon <= p_longitudeCount; lon++ )
				{
					float a2 = _2pi * (float)(lon == p_longitudeCount ? 0 : lon) / p_longitudeCount;
					float sin2 = Mathf.Sin(a2);
					float cos2 = Mathf.Cos(a2);
			 
					vertices[ lon + lat * (p_longitudeCount + 1) + 1] = new Vector3( sin1 * cos2, cos1, sin1 * sin2 ) * p_radius;
				}
			}
			vertices[vertices.Length-1] = Vector3.up * -p_radius;


			Vector3[] normals = new Vector3[vertices.Length];
			for( int n = 0; n < vertices.Length; n++ )
				normals[n] = vertices[n].normalized;

			
			Vector2[] uvs = new Vector2[vertices.Length];
			uvs[0] = Vector2.up;
			uvs[uvs.Length-1] = Vector2.zero;
			for( int lat = 0; lat < p_lattitudeCount; lat++ )
				for( int lon = 0; lon <= p_longitudeCount; lon++ )
					uvs[lon + lat * (p_longitudeCount + 1) + 1] = new Vector2( (float)lon / p_longitudeCount, 1f - (float)(lat+1) / (p_lattitudeCount+1) );
			
			int faceCount = vertices.Length;
			int triangleCount = faceCount * 2;
			int indexCount = triangleCount * 3;
			int[] triangles = new int[ indexCount ];
			 
			//Top Cap
			int i = 0;
			for( int lon = 0; lon < p_longitudeCount; lon++ )
			{
				triangles[i++] = lon+2;
				triangles[i++] = lon+1;
				triangles[i++] = 0;
			}
			 
			//Middle
			for( int lat = 0; lat < p_lattitudeCount - 1; lat++ )
			{
				for( int lon = 0; lon < p_longitudeCount; lon++ )
				{
					int current = lon + lat * (p_longitudeCount + 1) + 1;
					int next = current + p_longitudeCount + 1;
			 
					triangles[i++] = current;
					triangles[i++] = current + 1;
					triangles[i++] = next + 1;
			 
					triangles[i++] = current;
					triangles[i++] = next + 1;
					triangles[i++] = next;
				}
			}
			 
			//Bottom Cap
			for( int lon = 0; lon < p_longitudeCount; lon++ )
			{
				triangles[i++] = vertices.Length - 1;
				triangles[i++] = vertices.Length - (lon+2) - 1;
				triangles[i++] = vertices.Length - (lon+1) - 1;
			}

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			 
			mesh.RecalculateBounds();
			mesh.Optimize();

			return mesh;
		}
	}
}