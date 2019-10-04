using GameConsole;
using Unity.Mathematics;
using UnityEngine;

public static class math2D
{
	[ExecutableCommand]
	public static float lineDistance(float from_x, float from_y, float to_x, float to_y, float circle_x, float circle_y)
	{
		// line end to line start
		var d_x = to_x - from_x;
		var d_y = to_y - from_y;

		// circle to line start
		var c_x = circle_x - from_x;
		var c_y = circle_y - from_y;

		// d_dist normalized
		var d_dist = Mathf.Sqrt(d_x*d_x + d_y*d_y);

		var d_x_n = d_x / d_dist;
		var d_y_n = d_y / d_dist;

		// project circle to start on a line
		var c_proj = c_x * d_x_n + c_y * d_y_n;
		if (c_proj < 0f || d_dist <= 0f)
		{
			return Mathf.Sqrt(c_x*c_x + c_y*c_y);
		}
		else if (c_proj > d_dist)
		{
			return Mathf.Sqrt((to_x-circle_x)*(to_x-circle_x) + (to_y-circle_y)*(to_y-circle_y));
		}

		// pythagoras
		var dist_to_line_squared = c_x*c_x + c_y*c_y - c_proj*c_proj;

		// when point is exactly on the line, dist could dip into negative values due to floating point errors - just return zero.
		return dist_to_line_squared > 0 ? Mathf.Sqrt(dist_to_line_squared) : 0;
	}

	public static float lineDistanceSqr(float from_x, float from_y, float to_x, float to_y, float circle_x, float circle_y)
	{
		// line end to line start
		var d_x = to_x - from_x;
		var d_y = to_y - from_y;

		// circle to line start
		var c_x = circle_x - from_x;
		var c_y = circle_y - from_y;

		// d_dist normalized
		var d_dist = math.sqrt(d_x*d_x + d_y*d_y);
		var d_x_n = d_x / d_dist;
		var d_y_n = d_y / d_dist;

		// project circle to start on a line
		var c_proj = c_x * d_x_n + c_y * d_y_n;
		if (c_proj < 0f)
		{
			return c_x*c_x + c_y*c_y;
		}
		else if (c_proj > d_dist)
		{
			return (to_x-circle_x)*(to_x-circle_x) + (to_y-circle_y)*(to_y-circle_y);
		}

		// pythagoras
		var dist_to_line_squared = c_x*c_x + c_y*c_y - c_proj*c_proj;

		// when point is exactly on the line, dist could dip into negative values due to floating point errors - just return zero.
		return dist_to_line_squared > 0 ? dist_to_line_squared : 0;
	}

	public static float lineDistanceSqr(float2 from, float2 to, float2 circle)
	{
		// line end to line start
		var d = to - from;

		// circle to line start
		var c = circle - from;

		// d_dist normalized
		var d_dist = math.length(d);
		var dist_n = d / d_dist;

		// project circle to start on a line
		var c_proj = c.x * dist_n.x + c.y * dist_n.y;
		if (c_proj < 0f)
		{
			return math.lengthsq(c);
		}
		else if (c_proj > d_dist)
		{
			return math.distancesq(to, circle);
		}

		// pythagoras
		var dist_to_line_squared = math.lengthsq(c) - c_proj*c_proj;

		// when point is exactly on the line, dist could dip into negative values due to floating point errors - just return zero.
		return dist_to_line_squared > 0 ? dist_to_line_squared : 0;
	}
}