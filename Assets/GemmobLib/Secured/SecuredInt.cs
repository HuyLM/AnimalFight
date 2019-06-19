using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct SecuredInt : IEquatable<SecuredInt>
{
	private static int staticKey = 43098;
		
	[SerializeField] private int key;
	[SerializeField] private int masked;

	public SecuredInt (int value=0) : this(staticKey, value) {}

	public SecuredInt (int key, int value)
	{
		this.key = key;
		this.masked = value ^ key;
	}

    public SecuredInt(string serializedString)
    {
        try
        {
            var cs = serializedString.Split('@');
            this.key = int.Parse(cs[0]);
            this.masked = int.Parse(cs[1]);
        }
        catch (Exception ex)
        {
            this.key = staticKey;
            this.masked = 0 ^ this.key;
        }
    }

	public int Value {
		get { return masked ^ key; }
	}

	#if UNITY_EDITOR
	public int Masked {
		get { return masked; }
	}

	public int Key {
		get { return key; }
	}
	#endif

	public void ApplyNewKey (int key)
	{
		int value = Value;
		this.key = key;
		this.masked = value ^ key;
	}

	// Implicit convert int to SecuredInt
	public static implicit operator SecuredInt (int value)
	{
		return new SecuredInt (RandomKey(), value);
	}

	// Implicit convert SecuredInt to int
	public static implicit operator int (SecuredInt value)
	{
		return value.Value;
	}

	public static SecuredInt operator ++ (SecuredInt number)
	{
		return new SecuredInt (number.Value + 1);
	}

	public static SecuredInt operator -- (SecuredInt number)
	{
		return new SecuredInt (number.Value - 1);
	}

	public static SecuredInt operator + (SecuredInt a, SecuredInt b)
	{
		return new SecuredInt (a.Value + b.Value);	
	}

	public static SecuredInt operator - (SecuredInt a, SecuredInt b)
	{
		return new SecuredInt (a.Value - b.Value);	
	}

	public static SecuredInt operator * (SecuredInt a, SecuredInt b)
	{
		return new SecuredInt (a.Value * b.Value);
	}

	public static SecuredInt operator / (SecuredInt a, SecuredInt b)
	{
		return new SecuredInt (a.Value / b.Value);
	}

	public static bool operator > (SecuredInt a, SecuredInt b)
	{
		return a.Value > b.Value;
	}

	public static bool operator >= (SecuredInt a, SecuredInt b)
	{
		return a.Value >= b.Value;
	}

	public static bool operator < (SecuredInt a, SecuredInt b)
	{
		return a.Value < b.Value;
	}

	public static bool operator <= (SecuredInt a, SecuredInt b)
	{
		return a.Value <= b.Value;
	}

	public static bool operator == (SecuredInt a, SecuredInt b)
	{
		return a.Value == b.Value;
	}

	public static bool operator != (SecuredInt a, SecuredInt b)
	{
		return a.Value != b.Value;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SecuredInt))
		{
			return false;
		}
		SecuredInt n = (SecuredInt)obj;
		return this.Value == n.Value;
	}

	public bool Equals(SecuredInt obj)
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

    public string ToSerializedString()
    {
        return key.ToString() + "@" + masked.ToString();
    }

	public static void SetNewKey (int key)
	{
		if (key != 0) 
		{
			staticKey = key;	
		}
	}

	public static SecuredInt Max (SecuredInt a, SecuredInt b)
	{
		return a > b ? a : b;
	}

	public static SecuredInt Min (SecuredInt a, SecuredInt b)
	{
		return a > b ? b : a;
	}

	public static SecuredInt Clamp (SecuredInt value, SecuredInt min, SecuredInt max)
	{
		return value < min ? min : (value > max ? max : value);
	}

	public static int RandomKey ()
	{
		int key = 0;
		for (int i = 0; i < sizeof(int); i++) {
			int b = (int)(UnityEngine.Random.value * byte.MaxValue);
			key += b << (i * 8);
		}
		return key;
	}
}
