// Generated code -- CC0 -- No Rights Reserved -- http://www.redblobgames.com/grids/hexagons/

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;

namespace HexLibrary
{
	[Serializable]
	public class Hex
	{
		public Hex(int q, int r, int s)
		{
			this.q = q;
			this.r = r;
			this.s = s;
			if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
		}

		[SerializeField]
		public int q;

		[SerializeField]
		public int r;

		[SerializeField]
		public int s;

		public Hex Add(Hex b)
		{
			return new Hex(q + b.q, r + b.r, s + b.s);
		}


		public Hex Subtract(Hex b)
		{
			return new Hex(q - b.q, r - b.r, s - b.s);
		}


		public Hex Scale(int k)
		{
			return new Hex(q * k, r * k, s * k);
		}


		public Hex RotateLeft()
		{
			return new Hex(-s, -q, -r);
		}


		public Hex RotateRight()
		{
			return new Hex(-r, -s, -q);
		}

		static public List<Hex> directions = new List<Hex>
		{
			new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1)
		};

		static public Hex Direction(int direction)
		{
			return Hex.directions[direction];
		}


		public Hex Neighbor(int direction)
		{
			return Add(Hex.Direction(direction));
		}

		public List<Hex> GetNeighbors()
		{
			return directions.Select(Add).ToList();
		}

		static public List<Hex> diagonals = new List<Hex>
		{
			new Hex(2, -1, -1), new Hex(1, -2, 1), new Hex(-1, -1, 2), new Hex(-2, 1, 1), new Hex(-1, 2, -1),
			new Hex(1, 1, -2)
		};

		public Hex DiagonalNeighbor(int direction)
		{
			return Add(Hex.diagonals[direction]);
		}


		public int Length()
		{
			return (int)((Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s)) / 2);
		}


		public int Distance(Hex b)
		{
			return Subtract(b).Length();
		}

		public override string ToString()
		{
			return $"Hex : ({q}, {r}, {s})";
		}

		public override int GetHashCode()
		{
			Hash128 hash = new Hash128();
			hash.Append(q);
			hash.Append(r);
			hash.Append(s);
			return hash.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Hex);
		}

		public bool Equals(Hex obj)
		{
			if (obj is null)
			{
				return false;
			}

			return q == obj.q && r == obj.r && s == obj.s;
		}

		public static bool operator ==(Hex a, Hex b)
		{
			if (a is null || b is null)
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(Hex a, Hex b)
		{
			return !(a == b);
		}
	}

	public struct FractionalHex
	{
		public FractionalHex(float q, float r, float s)
		{
			this.q = q;
			this.r = r;
			this.s = s;
			if (Mathf.Round(q + r + s) != 0) throw new ArgumentException("q + r + s must be 0");
		}

		public readonly float q;
		public readonly float r;
		public readonly float s;

		public Hex HexRound()
		{
			int qi = (int)(Mathf.Round(q));
			int ri = (int)(Mathf.Round(r));
			int si = (int)(Mathf.Round(s));
			float q_diff = Mathf.Abs(qi - q);
			float r_diff = Mathf.Abs(ri - r);
			float s_diff = Mathf.Abs(si - s);
			if (q_diff > r_diff && q_diff > s_diff)
			{
				qi = -ri - si;
			}
			else if (r_diff > s_diff)
			{
				ri = -qi - si;
			}
			else
			{
				si = -qi - ri;
			}

			return new Hex(qi, ri, si);
		}


		public FractionalHex HexLerp(FractionalHex b, float t)
		{
			return new FractionalHex(q * (1.0f - t) + b.q * t, r * (1.0f - t) + b.r * t, s * (1.0f - t) + b.s * t);
		}


		static public List<Hex> HexLinedraw(Hex a, Hex b)
		{
			int N = a.Distance(b);
			FractionalHex a_nudge = new FractionalHex(a.q + 1e-06f, a.r + 1e-06f, a.s - 2e-06f);
			FractionalHex b_nudge = new FractionalHex(b.q + 1e-06f, b.r + 1e-06f, b.s - 2e-06f);
			List<Hex> results = new List<Hex> { };
			float step = 1.0f / Mathf.Max(N, 1);
			for (int i = 0; i <= N; i++)
			{
				results.Add(a_nudge.HexLerp(b_nudge, step * i).HexRound());
			}

			return results;
		}
	}

	public struct OffsetCoord
	{
		public OffsetCoord(int col, int row)
		{
			this.col = col;
			this.row = row;
		}

		public readonly int col;
		public readonly int row;
		static public int EVEN = 1;
		static public int ODD = -1;

		static public OffsetCoord QoffsetFromCube(int offset, Hex h)
		{
			int col = h.q;
			int row = h.r + (int)((h.q + offset * (h.q & 1)) / 2);
			if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
			{
				throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
			}

			return new OffsetCoord(col, row);
		}


		static public Hex QoffsetToCube(int offset, OffsetCoord h)
		{
			int q = h.col;
			int r = h.row - (int)((h.col + offset * (h.col & 1)) / 2);
			int s = -q - r;
			if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
			{
				throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
			}

			return new Hex(q, r, s);
		}


		static public OffsetCoord RoffsetFromCube(int offset, Hex h)
		{
			int col = h.q + (int)((h.r + offset * (h.r & 1)) / 2);
			int row = h.r;
			if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
			{
				throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
			}

			return new OffsetCoord(col, row);
		}


		static public Hex RoffsetToCube(int offset, OffsetCoord h)
		{
			int q = h.col - (int)((h.row + offset * (h.row & 1)) / 2);
			int r = h.row;
			int s = -q - r;
			if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
			{
				throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
			}

			return new Hex(q, r, s);
		}
	}

	public struct floatdCoord
	{
		public floatdCoord(int col, int row)
		{
			this.col = col;
			this.row = row;
		}

		public readonly int col;
		public readonly int row;

		static public floatdCoord QfloatdFromCube(Hex h)
		{
			int col = h.q;
			int row = 2 * h.r + h.q;
			return new floatdCoord(col, row);
		}


		public Hex QfloatdToCube()
		{
			int q = col;
			int r = (int)((row - col) / 2);
			int s = -q - r;
			return new Hex(q, r, s);
		}


		static public floatdCoord RfloatdFromCube(Hex h)
		{
			int col = 2 * h.q + h.r;
			int row = h.r;
			return new floatdCoord(col, row);
		}


		public Hex RfloatdToCube()
		{
			int q = (int)((col - row) / 2);
			int r = row;
			int s = -q - r;
			return new Hex(q, r, s);
		}
	}

	public struct Orientation
	{
		public Orientation(float f0, float f1, float f2, float f3, float b0, float b1, float b2, float b3,
			float start_angle)
		{
			this.f0 = f0;
			this.f1 = f1;
			this.f2 = f2;
			this.f3 = f3;
			this.b0 = b0;
			this.b1 = b1;
			this.b2 = b2;
			this.b3 = b3;
			this.start_angle = start_angle;
		}

		public readonly float f0;
		public readonly float f1;
		public readonly float f2;
		public readonly float f3;
		public readonly float b0;
		public readonly float b1;
		public readonly float b2;
		public readonly float b3;
		public readonly float start_angle;
	}

	public struct Layout
	{
		public Layout(Orientation orientation, Vector2 size, Vector2 origin)
		{
			this.orientation = orientation;
			this.size = size;
			this.origin = origin;
		}

		public readonly Orientation orientation;
		public readonly Vector2 size;
		public readonly Vector2 origin;

		static public Orientation Vector2y = new Orientation(Mathf.Sqrt(3.0f), Mathf.Sqrt(3.0f) / 2.0f, 0.0f, 3.0f / 2.0f,
			Mathf.Sqrt(3.0f) / 3.0f, -1.0f / 3.0f, 0.0f, 2.0f / 3.0f, 0.5f);

		static public Orientation flat = new Orientation(3.0f / 2.0f, 0.0f, Mathf.Sqrt(3.0f) / 2.0f, Mathf.Sqrt(3.0f), 2.0f / 3.0f,
			0.0f, -1.0f / 3.0f, Mathf.Sqrt(3.0f) / 3.0f, 0.0f);

		public Vector2 HexToPixel(Hex h)
		{
			Orientation M = orientation;
			float x = (M.f0 * h.q + M.f1 * h.r) * size.x;
			float y = (M.f2 * h.q + M.f3 * h.r) * size.y;
			return new Vector2(x + origin.x, y + origin.y);
		}

		public Vector3 HexToWorld(Hex h)
		{
			Vector2 pixel = HexToPixel(h);
			return new Vector3(pixel.x, 0, pixel.y);
		}


		public FractionalHex PixelToHex(Vector2 p)
		{
			Orientation M = orientation;
			Vector2 pt = new Vector2((p.x - origin.x) / size.x, (p.y - origin.y) / size.y);
			float q = M.b0 * pt.x + M.b1 * pt.y;
			float r = M.b2 * pt.x + M.b3 * pt.y;
			return new FractionalHex(q, r, -q - r);
		}


		public Vector2 HexCornerOffset(int corner)
		{
			Orientation M = orientation;
			float angle = 2.0f * Mathf.PI * (M.start_angle - corner) / 6.0f;
			return new Vector2(size.x * Mathf.Cos(angle), size.y * Mathf.Sin(angle));
		}


		public List<Vector2> PolygonCorners(Hex h)
		{
			List<Vector2> corners = new List<Vector2> { };
			Vector2 center = HexToPixel(h);
			for (int i = 0; i < 6; i++)
			{
				Vector2 offset = HexCornerOffset(i);
				corners.Add(new Vector2(center.x + offset.x, center.y + offset.y));
			}

			return corners;
		}
	}


// Tests


	struct Tests
	{
		static public void EqualHex(String name, Hex a, Hex b)
		{
			if (!(a.q == b.q && a.s == b.s && a.r == b.r))
			{
				Tests.Complain(name);
			}
		}


		static public void EqualOffsetcoord(String name, OffsetCoord a, OffsetCoord b)
		{
			if (!(a.col == b.col && a.row == b.row))
			{
				Tests.Complain(name);
			}
		}


		static public void Equalfloatdcoord(String name, floatdCoord a, floatdCoord b)
		{
			if (!(a.col == b.col && a.row == b.row))
			{
				Tests.Complain(name);
			}
		}


		static public void EqualInt(String name, int a, int b)
		{
			if (!(a == b))
			{
				Tests.Complain(name);
			}
		}


		static public void EqualHexArray(String name, List<Hex> a, List<Hex> b)
		{
			Tests.EqualInt(name, a.Count, b.Count);
			for (int i = 0; i < a.Count; i++)
			{
				Tests.EqualHex(name, a[i], b[i]);
			}
		}


		static public void TestHexArithmetic()
		{
			Tests.EqualHex("hex_add", new Hex(4, -10, 6), new Hex(1, -3, 2).Add(new Hex(3, -7, 4)));
			Tests.EqualHex("hex_subtract", new Hex(-2, 4, -2), new Hex(1, -3, 2).Subtract(new Hex(3, -7, 4)));
		}


		static public void TestHexDirection()
		{
			Tests.EqualHex("hex_direction", new Hex(0, -1, 1), Hex.Direction(2));
		}


		static public void TestHexNeighbor()
		{
			Tests.EqualHex("hex_neighbor", new Hex(1, -3, 2), new Hex(1, -2, 1).Neighbor(2));
		}


		static public void TestHexDiagonal()
		{
			Tests.EqualHex("hex_diagonal", new Hex(-1, -1, 2), new Hex(1, -2, 1).DiagonalNeighbor(3));
		}


		static public void TestHexDistance()
		{
			Tests.EqualInt("hex_distance", 7, new Hex(3, -7, 4).Distance(new Hex(0, 0, 0)));
		}


		static public void TestHexRotateRight()
		{
			Tests.EqualHex("hex_rotate_right", new Hex(1, -3, 2).RotateRight(), new Hex(3, -2, -1));
		}


		static public void TestHexRotateLeft()
		{
			Tests.EqualHex("hex_rotate_left", new Hex(1, -3, 2).RotateLeft(), new Hex(-2, -1, 3));
		}


		static public void TestHexRound()
		{
			FractionalHex a = new FractionalHex(0.0f, 0.0f, 0.0f);
			FractionalHex b = new FractionalHex(1.0f, -1.0f, 0.0f);
			FractionalHex c = new FractionalHex(0.0f, -1.0f, 1.0f);
			Tests.EqualHex("hex_round 1", new Hex(5, -10, 5),
				new FractionalHex(0.0f, 0.0f, 0.0f).HexLerp(new FractionalHex(10.0f, -20.0f, 10.0f), 0.5f).HexRound());
			Tests.EqualHex("hex_round 2", a.HexRound(), a.HexLerp(b, 0.499f).HexRound());
			Tests.EqualHex("hex_round 3", b.HexRound(), a.HexLerp(b, 0.501f).HexRound());
			Tests.EqualHex("hex_round 4", a.HexRound(),
				new FractionalHex(a.q * 0.4f + b.q * 0.3f + c.q * 0.3f, a.r * 0.4f + b.r * 0.3f + c.r * 0.3f,
					a.s * 0.4f + b.s * 0.3f + c.s * 0.3f).HexRound());
			Tests.EqualHex("hex_round 5", c.HexRound(),
				new FractionalHex(a.q * 0.3f + b.q * 0.3f + c.q * 0.4f, a.r * 0.3f + b.r * 0.3f + c.r * 0.4f,
					a.s * 0.3f + b.s * 0.3f + c.s * 0.4f).HexRound());
		}


		static public void TestHexLinedraw()
		{
			Tests.EqualHexArray("hex_linedraw",
				new List<Hex>
				{
					new Hex(0, 0, 0), new Hex(0, -1, 1), new Hex(0, -2, 2), new Hex(1, -3, 2), new Hex(1, -4, 3),
					new Hex(1, -5, 4)
				}, FractionalHex.HexLinedraw(new Hex(0, 0, 0), new Hex(1, -5, 4)));
		}


		static public void TestLayout()
		{
			Hex h = new Hex(3, 4, -7);
			Layout flat = new Layout(Layout.flat, new Vector2(10.0f, 15.0f), new Vector2(35.0f, 71.0f));
			Tests.EqualHex("layout", h, flat.PixelToHex(flat.HexToPixel(h)).HexRound());
			Layout Vector2y = new Layout(Layout.Vector2y, new Vector2(10.0f, 15.0f), new Vector2(35.0f, 71.0f));
			Tests.EqualHex("layout", h, Vector2y.PixelToHex(Vector2y.HexToPixel(h)).HexRound());
		}


		static public void TestOffsetRoundtrip()
		{
			Hex a = new Hex(3, 4, -7);
			OffsetCoord b = new OffsetCoord(1, -3);
			Tests.EqualHex("conversion_roundtrip even-q", a,
				OffsetCoord.QoffsetToCube(OffsetCoord.EVEN, OffsetCoord.QoffsetFromCube(OffsetCoord.EVEN, a)));
			Tests.EqualOffsetcoord("conversion_roundtrip even-q", b,
				OffsetCoord.QoffsetFromCube(OffsetCoord.EVEN, OffsetCoord.QoffsetToCube(OffsetCoord.EVEN, b)));
			Tests.EqualHex("conversion_roundtrip odd-q", a,
				OffsetCoord.QoffsetToCube(OffsetCoord.ODD, OffsetCoord.QoffsetFromCube(OffsetCoord.ODD, a)));
			Tests.EqualOffsetcoord("conversion_roundtrip odd-q", b,
				OffsetCoord.QoffsetFromCube(OffsetCoord.ODD, OffsetCoord.QoffsetToCube(OffsetCoord.ODD, b)));
			Tests.EqualHex("conversion_roundtrip even-r", a,
				OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, a)));
			Tests.EqualOffsetcoord("conversion_roundtrip even-r", b,
				OffsetCoord.RoffsetFromCube(OffsetCoord.EVEN, OffsetCoord.RoffsetToCube(OffsetCoord.EVEN, b)));
			Tests.EqualHex("conversion_roundtrip odd-r", a,
				OffsetCoord.RoffsetToCube(OffsetCoord.ODD, OffsetCoord.RoffsetFromCube(OffsetCoord.ODD, a)));
			Tests.EqualOffsetcoord("conversion_roundtrip odd-r", b,
				OffsetCoord.RoffsetFromCube(OffsetCoord.ODD, OffsetCoord.RoffsetToCube(OffsetCoord.ODD, b)));
		}


		static public void TestOffsetFromCube()
		{
			Tests.EqualOffsetcoord("offset_from_cube even-q", new OffsetCoord(1, 3),
				OffsetCoord.QoffsetFromCube(OffsetCoord.EVEN, new Hex(1, 2, -3)));
			Tests.EqualOffsetcoord("offset_from_cube odd-q", new OffsetCoord(1, 2),
				OffsetCoord.QoffsetFromCube(OffsetCoord.ODD, new Hex(1, 2, -3)));
		}


		static public void TestOffsetToCube()
		{
			Tests.EqualHex("offset_to_cube even-", new Hex(1, 2, -3),
				OffsetCoord.QoffsetToCube(OffsetCoord.EVEN, new OffsetCoord(1, 3)));
			Tests.EqualHex("offset_to_cube odd-q", new Hex(1, 2, -3),
				OffsetCoord.QoffsetToCube(OffsetCoord.ODD, new OffsetCoord(1, 2)));
		}


		static public void TestfloatdRoundtrip()
		{
			Hex a = new Hex(3, 4, -7);
			floatdCoord b = new floatdCoord(1, -3);
			Tests.EqualHex("conversion_roundtrip floatd-q", a, floatdCoord.QfloatdFromCube(a).QfloatdToCube());
			Tests.Equalfloatdcoord("conversion_roundtrip floatd-q", b, floatdCoord.QfloatdFromCube(b.QfloatdToCube()));
			Tests.EqualHex("conversion_roundtrip floatd-r", a, floatdCoord.RfloatdFromCube(a).RfloatdToCube());
			Tests.Equalfloatdcoord("conversion_roundtrip floatd-r", b, floatdCoord.RfloatdFromCube(b.RfloatdToCube()));
		}


		static public void TestfloatdFromCube()
		{
			Tests.Equalfloatdcoord("floatd_from_cube floatd-q", new floatdCoord(1, 5),
				floatdCoord.QfloatdFromCube(new Hex(1, 2, -3)));
			Tests.Equalfloatdcoord("floatd_from_cube floatd-r", new floatdCoord(4, 2),
				floatdCoord.RfloatdFromCube(new Hex(1, 2, -3)));
		}


		static public void TestfloatdToCube()
		{
			Tests.EqualHex("floatd_to_cube floatd-q", new Hex(1, 2, -3), new floatdCoord(1, 5).QfloatdToCube());
			Tests.EqualHex("floatd_to_cube floatd-r", new Hex(1, 2, -3), new floatdCoord(4, 2).RfloatdToCube());
		}


		static public void TestAll()
		{
			Tests.TestHexArithmetic();
			Tests.TestHexDirection();
			Tests.TestHexNeighbor();
			Tests.TestHexDiagonal();
			Tests.TestHexDistance();
			Tests.TestHexRotateRight();
			Tests.TestHexRotateLeft();
			Tests.TestHexRound();
			Tests.TestHexLinedraw();
			Tests.TestLayout();
			Tests.TestOffsetRoundtrip();
			Tests.TestOffsetFromCube();
			Tests.TestOffsetToCube();
			Tests.TestfloatdRoundtrip();
			Tests.TestfloatdFromCube();
			Tests.TestfloatdToCube();
		}


		static public void Main()
		{
			Tests.TestAll();
		}


		static public void Complain(String name)
		{
			Console.WriteLine("FAIL " + name);
		}
	}
}