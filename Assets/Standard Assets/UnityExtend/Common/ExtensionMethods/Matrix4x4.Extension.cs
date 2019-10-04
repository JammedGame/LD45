using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Matrix4x4Extensions
{
	public static void Multiply(this ref Matrix4x4 lhs, ref Matrix4x4 rhs, out Matrix4x4 result)
	{
		result.m00 = (lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30);
		result.m01 = (lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31);
		result.m02 = (lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32);
		result.m03 = (lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33);
		result.m10 = (lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30);
		result.m11 = (lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31);
		result.m12 = (lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32);
		result.m13 = (lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33);
		result.m20 = (lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30);
		result.m21 = (lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31);
		result.m22 = (lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32);
		result.m23 = (lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33);
		result.m30 = (lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30);
		result.m31 = (lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31);
		result.m32 = (lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32);
		result.m33 = (lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33);
	}

	/// <summary>
	/// Transforms a position by this matrix (fast and inlined).
	/// </summary>
	public static void MultiplyPoint3x4(this ref Matrix4x4 lhs, ref Vector3 rhs, out Vector3 result)
	{
		result.x = (lhs.m00 * rhs.x + lhs.m01 * rhs.y + lhs.m02 * rhs.z) + lhs.m03;
		result.y = (lhs.m10 * rhs.x + lhs.m11 * rhs.y + lhs.m12 * rhs.z) + lhs.m13;
		result.z = (lhs.m20 * rhs.x + lhs.m21 * rhs.y + lhs.m22 * rhs.z) + lhs.m23;
	}
}