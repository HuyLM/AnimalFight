using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct SecuredLong : IEquatable<SecuredLong>
{
	private static long staticKey = 246357;

	[SerializeField] private long key;
	[SerializeField] private long masked;

	public SecuredLong (long value=0) : this(staticKey, value) {}

	public SecuredLong (long key, long value)
	{
		this.key = key;
		this.masked = value ^ key;
	}

	public long Value {
		get { return masked ^ key; }
	}

	#if UNITY_EDITOR
	public long Masked {
		get { return masked; }
	}

	public long Key {
		get { return key; }
	}
	#endif

	public void ApplyNewKey (long key)
	{
		long value = Value;
		this.key = key;
		this.masked = value ^ key;
	}

	#if UNITY_EDITOR
	public static long Encrypt (long key, long value)
	{
		return value ^ key;
	}
	#endif

	// Implicit convert long to SecuredLong
	public static implicit operator SecuredLong (long value)
	{
		return new SecuredLong (value);
	}

	// Implicit convert SecuredLong to long
	public static implicit operator long (SecuredLong value)
	{
		return value.Value;
	}

	// Implicit convert int to SecuredLong
	public static implicit operator SecuredLong (int value)
	{
		return new SecuredLong (value);
	}

	// Implicit convert SecuredInt to SecuredLong
	public static implicit operator SecuredLong (SecuredInt value)
	{
		return new SecuredLong (value.Value);
	}

	public static SecuredLong operator ++ (SecuredLong number)
	{
		return new SecuredLong (number.Value + 1);
	}

	public static SecuredLong operator -- (SecuredLong number)
	{
		return new SecuredLong (number.Value - 1);
	}

	public static SecuredLong operator + (SecuredLong a, SecuredLong b)
	{
		return new SecuredLong (a.Value + b.Value);	
	}

	public static SecuredLong operator - (SecuredLong a, SecuredLong b)
	{
		return new SecuredLong (a.Value - b.Value);	
	}

	public static SecuredLong operator * (SecuredLong a, SecuredLong b)
	{
		return new SecuredLong (a.Value * b.Value);
	}

	public static SecuredLong operator / (SecuredLong a, SecuredLong b)
	{
		return new SecuredLong (a.Value / b.Value);
	}

	public static bool operator > (SecuredLong a, SecuredLong b)
	{
		return a.Value > b.Value;
	}

	public static bool operator >= (SecuredLong a, SecuredLong b)
	{
		return a.Value >= b.Value;
	}

	public static bool operator < (SecuredLong a, SecuredLong b)
	{
		return a.Value < b.Value;
	}

	public static bool operator <= (SecuredLong a, SecuredLong b)
	{
		return a.Value <= b.Value;
	}

	public static bool operator == (SecuredLong a, SecuredLong b)
	{
		return a.Value == b.Value;
	}

	public static bool operator != (SecuredLong a, SecuredLong b)
	{
		return a.Value != b.Value;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SecuredLong))
		{
			return false;
		}
		SecuredLong n = (SecuredLong)obj;
		return this.Value == n.Value;
	}

	public bool Equals(SecuredLong obj)
	{
		return this.Value == obj.Value;
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
		if (key != 0) 
		{
			staticKey = key;	
		}
	}

	public static SecuredLong Max (SecuredLong a, SecuredLong b)
	{
		return a > b ? a : b;
	}

	public static SecuredLong Min (SecuredLong a, SecuredLong b)
	{
		return a > b ? b : a;
	}

	public static SecuredLong Clamp (SecuredLong value, SecuredLong min, SecuredLong max)
	{
		return value < min ? min : (value > max ? max : value);
	}
}
