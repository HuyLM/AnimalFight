using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

[System.Serializable]
public struct SecuredDouble 
{
	[StructLayout(LayoutKind.Explicit)]
	private struct Union
	{
		[FieldOffset(0)]
		public double d;

		[FieldOffset(0)]
		public long l;
	}

	private static long staticKey = 0x12faba9e7d653e8f;

	[SerializeField] private long key;
	[SerializeField] private double masked;
	static System.Func<double, Unit> unitFinder;

	public SecuredDouble (double value=0) : this(staticKey, value) {}

	static SecuredDouble ()
	{
		unitFinder = Unit0.Find;
	}

	public static void SetUnit (int u)
	{
		if (u == 0) {
			unitFinder = Unit0.Find;
		} else {
			unitFinder = Unit1.Find;
		}
	}

	public SecuredDouble (long key, double value=0)
	{
		this.key = key;
		Union u = default(Union);
		u.d = value;
		u.l ^= key;
		masked = u.d;
	}

	public double Value {
		get {
			Union u = default(Union);
			u.d = masked;
			u.l ^= key;
			return u.d;
		}
	}

	public SecuredDouble Floor ()
	{
		return new SecuredDouble (System.Math.Floor (Value));
	}

	public SecuredDouble Ceiling ()
	{
		return new SecuredDouble (System.Math.Ceiling (Value));
	}

	public SecuredDouble Round ()
	{
		return new SecuredDouble (System.Math.Round (Value));
	}

	#if UNITY_EDITOR
	public double Masked {
		get { return masked; }
	}

	public static double Encrypt (long key, double value)
	{
		Union u = default(Union);
		u.d = value;
		u.l ^= key;
		return u.d;
	}
	#endif

	public void ApplyNewKey (long key)
	{
		double value = this.Value;
		this.key = key;
		Union u = default(Union);
		u.d = value;
		u.l ^= key;
		masked = u.d;
	}

	// Implicit convert double to SecuredDouble
	public static implicit operator SecuredDouble (double value = 0)
	{
		return new SecuredDouble (value);
	}

	// Implicit convert SecuredDouble to double
	public static implicit operator double (SecuredDouble value)
	{
		return value.Value;
	}

	public static SecuredDouble operator + (SecuredDouble a, SecuredDouble b)
	{
		return new SecuredDouble (a.Value + b.Value);	
	}

	public static SecuredDouble operator - (SecuredDouble a, SecuredDouble b)
	{
		return new SecuredDouble (a.Value - b.Value);	
	}

	public static SecuredDouble operator * (SecuredDouble a, SecuredDouble b)
	{
		return new SecuredDouble (a.Value * b.Value);
	}

	public static SecuredDouble operator / (SecuredDouble a, SecuredDouble b)
	{
		return new SecuredDouble (a.Value / b.Value);
	}

	public static bool operator > (SecuredDouble a, SecuredDouble b)
	{
		return a.Value > b.Value;
	}

	public static bool operator >= (SecuredDouble a, SecuredDouble b)
	{
		return a.Value >= b.Value;
	}

	public static bool operator < (SecuredDouble a, SecuredDouble b)
	{
		return a.Value < b.Value;
	}

	public static bool operator <= (SecuredDouble a, SecuredDouble b)
	{
		return a.Value <= b.Value;
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string ToString()
	{
		return Value.ToString();
	}

	public string ToString(string format)
	{
		return Value.ToString(format);
	}

	public string ToString(IFormatProvider provider)
	{
		return Value.ToString(provider);
	}

	public string ToString(string format, IFormatProvider provider)
	{
		return Value.ToString(format, provider);
	}

	public static void SetNewKey (long key)
	{
		if (key != 0) {
			staticKey = key;
		}
	}
		
    public SecuredDouble Ceil ()
    {
        return new SecuredDouble (System.Math.Ceiling (Value));
    }
        
	public SecuredDouble Pow (double p)
	{
		return new SecuredDouble (System.Math.Pow (Value, p));
	}

	public static SecuredDouble Max (SecuredDouble a, SecuredDouble b)
	{
		return a > b ? a : b;
	}

	public static SecuredDouble Min (SecuredDouble a, SecuredDouble b)
	{
		return a > b ? b : a;
	}

	public static SecuredDouble Clamp (SecuredDouble value, SecuredDouble min, SecuredDouble max)
	{
		return value < min ? min : (value > max ? max : value);
	}

	public static long RandomKey ()
	{
		long key = 0;
		for (int i = 0; i < sizeof(long); i++) {
			long b = (long)(UnityEngine.Random.value * byte.MaxValue);
			key += b << (i * 8);
		}
		return key;
	}

	public string FormattedString {
		get { 
			return ToString (FindUnit ());
		}
	}

	public string RoundedString {
		get { 
			return ToString (FindUnit (), "0.");
		} 
	}

//	public override string ToString ()
//	{
//		return ToString(FindUnit());
//	}

	public string ToString (Unit unit, string format="0.##")
	{
		if (double.IsInfinity (Value) || double.IsNaN (Value)) {
			return "Infinity or NaN";
		}

		return (Value / System.Math.Pow (10, unit.exponent)).ToString (format) + unit.name;
	}

	public Unit FindUnit ()
	{
		return unitFinder.Invoke (Value);
	}

	public class Unit0
	{
		static Unit[] units;
		static Unit Infinity;
		static Unit Zero;

		static Unit0 ()
		{
			Infinity.exponent = 0;
			Infinity.name = "(VeryBIG)";
			Zero.exponent = 0;
			Zero.name = "";

			units = new Unit[120];
			int i = 0;

			units[i++].name = "";
			units[i-1].exponent = (i - 1) * 3;

			units[i++].name = "K";
			units[i-1].exponent = (i - 1) * 3;

			units[i++].name = "M";
			units[i-1].exponent = (i - 1) * 3;

			units[i++].name = "B";
			units[i-1].exponent = (i - 1) * 3;

			units[i++].name = "T";
			units[i-1].exponent = (i - 1) * 3;

			for (char c0 = 'a'; c0 <= 'z'; c0++) {
				for (char c1 = c0; c1 <= 'z'; c1++) {
					if (i >= units.Length) {
						break;
					}

					units[i++].name = c0.ToString() + c1.ToString();
					units[i-1].exponent = (i - 1) * 3;
				}
			}
		}
		public static Unit Find(double value)
		{
			//extract
			long exponent;

			double e = System.Math.Log10(System.Math.Abs(value));
			double fe = System.Math.Floor(e);

			long remainder;
			exponent = System.Math.DivRem((long)fe, 3, out remainder) * 3;

			//find
			if (exponent < 0)
				return Zero;
			return exponent / 3 < units.Length ? units[exponent / 3] : Infinity;
		}
	}

	public class Unit1
	{
		static Unit[] units;
		static Unit Infinity;
		static Unit Zero;

		static Unit1()
		{
			Infinity.exponent = 0;
			Infinity.name = "(VeryBIG)";
			Zero.exponent = 0;
			Zero.name = "";

			units = new Unit[304];
			int i = 0;

			units[i++].name = "";
			units[i-1].exponent = (i - 1) * 3;

			units[i++].name = "K";
			units[i-1].exponent = (i - 1) * 3;

			units[i++].name = "M";
			units[i-1].exponent = (i - 1) * 3;

			units[i++].name = "B";
			units[i-1].exponent = (i - 1) * 3;

			units[i++].name = "T";
			units[i-1].exponent = (i - 1) * 3;

			int exp = 14;
			for(i = i; i < units.Length; i++)
			{
				units[i].name = "e" + (++exp);
				units[i].exponent = exp;
			}
		}

		public static Unit Find (double value)
		{
			//extract
			long exponent;

			double e = System.Math.Log10 (System.Math.Abs(value));
			double fe = System.Math.Floor (e);

			long remainder = 0;
			if (fe < 15)
			{
				exponent = System.Math.DivRem((long)fe, 3, out remainder) * 3;
			}
			else
				exponent = (long)fe;

			//find
			if (exponent < 0)
				return Zero;
			if(exponent < 15)
				return units [exponent / 3];
			else
				return exponent < units.Length + 5 ? units [15 / 3 + exponent - 15] : Infinity;
		}
	}

	public struct Unit
	{
		public int exponent;
		public string name;
	}


}
