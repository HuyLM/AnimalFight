using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

[System.Serializable]
public struct SecuredFloat 
{
	[StructLayout(LayoutKind.Explicit)]
	private struct Union
	{
		[FieldOffset(0)]
		public float d;

		[FieldOffset(0)]
		public long l;
	}

	private static long staticKey = 0x12faba9e7d653e8f;

	[SerializeField] private long key;
	[SerializeField] private float masked;

	public SecuredFloat (float value) : this(staticKey, value) {}

	public SecuredFloat (long key, float value)
	{
		this.key = key;
		Union u = default(Union);
		u.d = value;
		u.l ^= key;
		masked = u.d;
	}

	public float Value {
		get {
			Union u = default(Union);
			u.d = masked;
			u.l ^= key;
			return u.d;
		}
	}

	#if UNITY_EDITOR
	public float Masked {
		get { return masked; }
	}
	#endif

	public void ApplyNewKey (long key)
	{
		float value = this.Value;
		this.key = key;
		Union u = default(Union);
		u.d = value;
		u.l ^= key;
		masked = u.d;
	}

	// Implicit convert double to SecuredFloat
	public static implicit operator SecuredFloat (float value)
	{
		return new SecuredFloat (value);
	}

	// Implicit convert SecuredFloat to double
	public static implicit operator float (SecuredFloat value)
	{
		return value.Value;
	}

	public static SecuredFloat operator + (SecuredFloat a, SecuredFloat b)
	{
		return new SecuredFloat (a.Value + b.Value);	
	}

	public static SecuredFloat operator - (SecuredFloat a, SecuredFloat b)
	{
		return new SecuredFloat (a.Value - b.Value);	
	}

	public static SecuredFloat operator * (SecuredFloat a, SecuredFloat b)
	{
		return new SecuredFloat (a.Value * b.Value);
	}

	public static SecuredFloat operator / (SecuredFloat a, SecuredFloat b)
	{
		return new SecuredFloat (a.Value / b.Value);
	}

	public static bool operator > (SecuredFloat a, SecuredFloat b)
	{
		return a.Value > b.Value;
	}

	public static bool operator >= (SecuredFloat a, SecuredFloat b)
	{
		return a.Value >= b.Value;
	}

	public static bool operator < (SecuredFloat a, SecuredFloat b)
	{
		return a.Value < b.Value;
	}

	public static bool operator <= (SecuredFloat a, SecuredFloat b)
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

	public SecuredFloat Pow (float p)
	{
		return new SecuredFloat (Mathf.Pow (Value, p));
	}

	public static SecuredFloat Max (SecuredFloat a, SecuredFloat b)
	{
		return a > b ? a : b;
	}

	public static SecuredFloat Min (SecuredFloat a, SecuredFloat b)
	{
		return a > b ? b : a;
	}

	public static SecuredFloat Clamp (SecuredFloat value, SecuredFloat min, SecuredFloat max)
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
}
