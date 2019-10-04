//
// Perlin noise generator for Unity
// Keijiro Takahashi, 2013, 2015
// https://github.com/keijiro/PerlinNoise
//
// Based on the original implementation by Ken Perlin
// http://mrl.nyu.edu/~perlin/noise/
//
using Unity.Collections;
using UnityEngine;

public static class Perlin
{
	#region Noise functions

	public static float Noise(float x)
	{
		var X = Mathf.FloorToInt(x) & 0xff;
		x -= Mathf.Floor(x);
		var u = Fade(x);
		return Lerp(u, Grad(perm(X), x), Grad(perm(X+1), x-1)) * 2;
	}

	public static float Noise(float x, float y)
	{
		var X = Mathf.FloorToInt(x) & 0xff;
		var Y = Mathf.FloorToInt(y) & 0xff;
		x -= Mathf.Floor(x);
		y -= Mathf.Floor(y);
		var u = Fade(x);
		var v = Fade(y);
		var A = (perm(X  ) + Y) & 0xff;
		var B = (perm(X+1) + Y) & 0xff;
		return Lerp(v, Lerp(u, Grad(perm(A  ), x, y  ), Grad(perm(B  ), x-1, y  )),
					   Lerp(u, Grad(perm(A+1), x, y-1), Grad(perm(B+1), x-1, y-1)));
	}

	public static float Noise(Vector2 coord)
	{
		return Noise(coord.x, coord.y);
	}

	public static float Noise(float x, float y, float z)
	{
		var X = Mathf.FloorToInt(x) & 0xff;
		var Y = Mathf.FloorToInt(y) & 0xff;
		var Z = Mathf.FloorToInt(z) & 0xff;
		x -= Mathf.Floor(x);
		y -= Mathf.Floor(y);
		z -= Mathf.Floor(z);
		var u = Fade(x);
		var v = Fade(y);
		var w = Fade(z);
		var A  = (perm(X  ) + Y) & 0xff;
		var B  = (perm(X+1) + Y) & 0xff;
		var AA = (perm(A  ) + Z) & 0xff;
		var BA = (perm(B  ) + Z) & 0xff;
		var AB = (perm(A+1) + Z) & 0xff;
		var BB = (perm(B+1) + Z) & 0xff;
		return Lerp(w, Lerp(v, Lerp(u, Grad(perm(AA  ), x, y  , z  ), Grad(perm(BA  ), x-1, y  , z  )),
							   Lerp(u, Grad(perm(AB  ), x, y-1, z  ), Grad(perm(BB  ), x-1, y-1, z  ))),
					   Lerp(v, Lerp(u, Grad(perm(AA+1), x, y  , z-1), Grad(perm(BA+1), x-1, y  , z-1)),
							   Lerp(u, Grad(perm(AB+1), x, y-1, z-1), Grad(perm(BB+1), x-1, y-1, z-1))));
	}

	public static float Noise(Vector3 coord)
	{
		return Noise(coord.x, coord.y, coord.z);
	}

	#endregion

	#region fBm functions

	public static float Fbm(float x, int octave)
	{
		var f = 0.0f;
		var w = 0.5f;
		for (var i = 0; i < octave; i++) {
			f += w * Noise(x);
			x *= 2.0f;
			w *= 0.5f;
		}
		return f;
	}

	public static float Fbm(Vector2 coord, int octave)
	{
		var f = 0.0f;
		var w = 0.5f;
		for (var i = 0; i < octave; i++) {
			f += w * Noise(coord);
			coord *= 2.0f;
			w *= 0.5f;
		}
		return f;
	}

	public static float Fbm(float x, float y, int octave)
	{
		return Fbm(new Vector2(x, y), octave);
	}

	public static float Fbm(Vector3 coord, int octave)
	{
		var f = 0.0f;
		var w = 0.5f;
		for (var i = 0; i < octave; i++) {
			f += w * Noise(coord);
			coord *= 2.0f;
			w *= 0.5f;
		}
		return f;
	}

	public static float Fbm(float x, float y, float z, int octave)
	{
		return Fbm(new Vector3(x, y, z), octave);
	}

	#endregion

	#region Private functions

	static float Fade(float t)
	{
		return t * t * t * (t * (t * 6 - 15) + 10);
	}

	static float Lerp(float t, float a, float b)
	{
		return a + t * (b - a);
	}

	static float Grad(int hash, float x)
	{
		return (hash & 1) == 0 ? x : -x;
	}

	static float Grad(int hash, float x, float y)
	{
		return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
	}

	static float Grad(int hash, float x, float y, float z)
	{
		var h = hash & 15;
		var u = h < 8 ? x : y;
		var v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
		return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
	}

	public static int perm(int index)
	{
		switch(index)
		{
			case 0: return 151;
			case 1: return 160;
			case 2: return 137;
			case 3: return 91;
			case 4: return 90;
			case 5: return 15;
			case 6: return 131;
			case 7: return 13;
			case 8: return 201;
			case 9: return 95;
			case 10: return 96;
			case 11: return 53;
			case 12: return 194;
			case 13: return 233;
			case 14: return 7;
			case 15: return 225;
			case 16: return 140;
			case 17: return 36;
			case 18: return 103;
			case 19: return 30;
			case 20: return 69;
			case 21: return 142;
			case 22: return 8;
			case 23: return 99;
			case 24: return 37;
			case 25: return 240;
			case 26: return 21;
			case 27: return 10;
			case 28: return 23;
			case 29: return 190;
			case 30: return 6;
			case 31: return 148;
			case 32: return 247;
			case 33: return 120;
			case 34: return 234;
			case 35: return 75;
			case 36: return 0;
			case 37: return 26;
			case 38: return 197;
			case 39: return 62;
			case 40: return 94;
			case 41: return 252;
			case 42: return 219;
			case 43: return 203;
			case 44: return 117;
			case 45: return 35;
			case 46: return 11;
			case 47: return 32;
			case 48: return 57;
			case 49: return 177;
			case 50: return 33;
			case 51: return 88;
			case 52: return 237;
			case 53: return 149;
			case 54: return 56;
			case 55: return 87;
			case 56: return 174;
			case 57: return 20;
			case 58: return 125;
			case 59: return 136;
			case 60: return 171;
			case 61: return 168;
			case 62: return 68;
			case 63: return 175;
			case 64: return 74;
			case 65: return 165;
			case 66: return 71;
			case 67: return 134;
			case 68: return 139;
			case 69: return 48;
			case 70: return 27;
			case 71: return 166;
			case 72: return 77;
			case 73: return 146;
			case 74: return 158;
			case 75: return 231;
			case 76: return 83;
			case 77: return 111;
			case 78: return 229;
			case 79: return 122;
			case 80: return 60;
			case 81: return 211;
			case 82: return 133;
			case 83: return 230;
			case 84: return 220;
			case 85: return 105;
			case 86: return 92;
			case 87: return 41;
			case 88: return 55;
			case 89: return 46;
			case 90: return 245;
			case 91: return 40;
			case 92: return 244;
			case 93: return 102;
			case 94: return 143;
			case 95: return 54;
			case 96: return 65;
			case 97: return 25;
			case 98: return 63;
			case 99: return 161;
			case 100: return 1;
			case 101: return 216;
			case 102: return 80;
			case 103: return 73;
			case 104: return 209;
			case 105: return 76;
			case 106: return 132;
			case 107: return 187;
			case 108: return 208;
			case 109: return 89;
			case 110: return 18;
			case 111: return 169;
			case 112: return 200;
			case 113: return 196;
			case 114: return 135;
			case 115: return 130;
			case 116: return 116;
			case 117: return 188;
			case 118: return 159;
			case 119: return 86;
			case 120: return 164;
			case 121: return 100;
			case 122: return 109;
			case 123: return 198;
			case 124: return 173;
			case 125: return 186;
			case 126: return 3;
			case 127: return 64;
			case 128: return 52;
			case 129: return 217;
			case 130: return 226;
			case 131: return 250;
			case 132: return 124;
			case 133: return 123;
			case 134: return 5;
			case 135: return 202;
			case 136: return 38;
			case 137: return 147;
			case 138: return 118;
			case 139: return 126;
			case 140: return 255;
			case 141: return 82;
			case 142: return 85;
			case 143: return 212;
			case 144: return 207;
			case 145: return 206;
			case 146: return 59;
			case 147: return 227;
			case 148: return 47;
			case 149: return 16;
			case 150: return 58;
			case 151: return 17;
			case 152: return 182;
			case 153: return 189;
			case 154: return 28;
			case 155: return 42;
			case 156: return 223;
			case 157: return 183;
			case 158: return 170;
			case 159: return 213;
			case 160: return 119;
			case 161: return 248;
			case 162: return 152;
			case 163: return 44;
			case 164: return 154;
			case 165: return 163;
			case 166: return 70;
			case 167: return 221;
			case 168: return 153;
			case 169: return 101;
			case 170: return 155;
			case 171: return 167;
			case 172: return 43;
			case 173: return 172;
			case 174: return 9;
			case 175: return 129;
			case 176: return 22;
			case 177: return 39;
			case 178: return 253;
			case 179: return 19;
			case 180: return 98;
			case 181: return 108;
			case 182: return 110;
			case 183: return 79;
			case 184: return 113;
			case 185: return 224;
			case 186: return 232;
			case 187: return 178;
			case 188: return 185;
			case 189: return 112;
			case 190: return 104;
			case 191: return 218;
			case 192: return 246;
			case 193: return 97;
			case 194: return 228;
			case 195: return 251;
			case 196: return 34;
			case 197: return 242;
			case 198: return 193;
			case 199: return 238;
			case 200: return 210;
			case 201: return 144;
			case 202: return 12;
			case 203: return 191;
			case 204: return 179;
			case 205: return 162;
			case 206: return 241;
			case 207: return 81;
			case 208: return 51;
			case 209: return 145;
			case 210: return 235;
			case 211: return 249;
			case 212: return 14;
			case 213: return 239;
			case 214: return 107;
			case 215: return 49;
			case 216: return 192;
			case 217: return 214;
			case 218: return 31;
			case 219: return 181;
			case 220: return 199;
			case 221: return 106;
			case 222: return 157;
			case 223: return 184;
			case 224: return 84;
			case 225: return 204;
			case 226: return 176;
			case 227: return 115;
			case 228: return 121;
			case 229: return 50;
			case 230: return 45;
			case 231: return 127;
			case 232: return 4;
			case 233: return 150;
			case 234: return 254;
			case 235: return 138;
			case 236: return 236;
			case 237: return 205;
			case 238: return 93;
			case 239: return 222;
			case 240: return 114;
			case 241: return 67;
			case 242: return 29;
			case 243: return 24;
			case 244: return 72;
			case 245: return 243;
			case 246: return 141;
			case 247: return 128;
			case 248: return 195;
			case 249: return 78;
			case 250: return 66;
			case 251: return 215;
			case 252: return 61;
			case 253: return 156;
			case 254: return 180;
			case 255: return 151;
		}

		// fix if out of bounds.
		var indexModed = index % 255;
		while (indexModed < 0) indexModed += 255;
		return perm(indexModed);
	}

	#endregion
}
