using UnityEngine;

namespace RuntimeHandle
{
	public static class HandleMathUtils
	{
		public const float PRECISION_THRESHOLD = 0.001f;
		
		public static float ClosestPointOnRay(Ray ray, Ray other)
		{
			// based on: https://math.stackexchange.com/questions/1036959/midpoint-of-the-shortest-distance-between-2-rays-in-3d
			// note: directions of both rays must be normalized
			// ray.origin -> a
			// ray.direction -> b
			// other.origin -> c
			// other.direction -> d

			float bd = Vector3.Dot(ray.direction, other.direction);
			float cd = Vector3.Dot(other.origin,  other.direction);
			float ad = Vector3.Dot(ray.origin,    other.direction);
			float bc = Vector3.Dot(ray.direction, other.origin);
			float ab = Vector3.Dot(ray.origin,    ray.direction);
			
			float bottom = bd * bd - 1f;
			if (Mathf.Abs(bottom) < PRECISION_THRESHOLD)
			{
				return 0;
			}

			float top = ab - bc + bd * (cd - ad);
			return top / bottom;
		}
	}
}