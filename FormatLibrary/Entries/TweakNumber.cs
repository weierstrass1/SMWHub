using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace FormatLibrary.Entries;

public class TweakNumber : INumber<TweakNumber>
{
    public virtual byte Value { get; set; }
    public static readonly TweakNumber MaxTweakNumber = new() { Value = 255 };
    public static TweakNumber One => new() { Value = 1 };
    public static int Radix => 2;
    public static TweakNumber Zero => new() { Value = 0 };
    public static TweakNumber AdditiveIdentity => Zero;
    public static TweakNumber MultiplicativeIdentity => One;
    public static TweakNumber Abs(TweakNumber value)
    {
        return value;
    }
    public static bool IsCanonical(TweakNumber value)
    {
        return true;
    }
    public static bool IsComplexNumber(TweakNumber value)
    {
        return false;
    }
    public static bool IsEvenInteger(TweakNumber value)
    {
        return value.Value % 2 == 0;
    }
    public static bool IsFinite(TweakNumber value)
    {
        return true;
    }
    public static bool IsImaginaryNumber(TweakNumber value)
    {
        return false;
    }
    public static bool IsInfinity(TweakNumber value)
    {
        return false;
    }
    public static bool IsInteger(TweakNumber value)
    {
        return true;
    }
    public static bool IsNaN(TweakNumber value)
    {
        return false;
    }
    public static bool IsNegative(TweakNumber value)
    {
        return false;
    }
    public static bool IsNegativeInfinity(TweakNumber value)
    {
        return false;
    }
    public static bool IsNormal(TweakNumber value)
    {
        return true;
    }
    public static bool IsOddInteger(TweakNumber value)
    {
        return value.Value % 2 != 0;
    }
    public static bool IsPositive(TweakNumber value)
    {
        return true;
    }
    public static bool IsPositiveInfinity(TweakNumber value)
    {
        return false;
    }
    public static bool IsRealNumber(TweakNumber value)
    {
        return true;
    }
    public static bool IsSubnormal(TweakNumber value)
    {
        return false;
    }
    public static bool IsZero(TweakNumber value)
    {
        return value == 0;
    }
    public static TweakNumber MaxMagnitude(TweakNumber x, TweakNumber y)
    {
        return x > y ? x : y;
    }
    public static TweakNumber MaxMagnitudeNumber(TweakNumber x, TweakNumber y)
    {
        return MaxMagnitude(x,y);
    }
    public static TweakNumber MinMagnitude(TweakNumber x, TweakNumber y)
    {
        return x < y ? x : y;
    }
    public static TweakNumber MinMagnitudeNumber(TweakNumber x, TweakNumber y)
    {
        return MinMagnitude(x, y);
    }
    public static TweakNumber Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        return new()
        {
            Value = byte.Parse(s, style, provider)
        };
    }
    public static TweakNumber Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        return new()
        {
            Value = byte.Parse(s, style, provider)
        };
    }
    public static TweakNumber Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return new()
        {
            Value = byte.Parse(s, provider)
        };
    }
    public static TweakNumber Parse(string s, IFormatProvider? provider)
    {
        return new()
        {
            Value = byte.Parse(s, provider)
        };
    }
    public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out TweakNumber result) where TOther : INumberBase<TOther>
    {
        try
        {
            result = new()
            {
                Value = byte.CreateChecked(value)
            };
            return true;
        }
        catch
        {
            result = Zero;
            return false;
        }
    }
    public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out TweakNumber result) where TOther : INumberBase<TOther>
    {
        try
        {
            result = new()
            {
                Value = byte.CreateSaturating(value)
            };
            return true;
        }
        catch
        {
            result = Zero;
            return false;
        }
    }
    public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out TweakNumber result) where TOther : INumberBase<TOther>
    {
        try
        {
            result = new()
            {
                Value = byte.CreateTruncating(value)
            };
            return true;
        }
        catch
        {
            result = Zero;
            return false;
        }
    }
    public static bool TryConvertToChecked<TOther>(TweakNumber value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        if(value is TOther v)
            return TOther.TryConvertFromChecked(v, out result);
        result = default;
        return false;
    }
    public static bool TryConvertToSaturating<TOther>(TweakNumber value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        if (value is TOther v)
            return TOther.TryConvertToSaturating(v, out result);
        result = default;
        return false;
    }
    public static bool TryConvertToTruncating<TOther>(TweakNumber value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        if (value is TOther v)
            return TOther.TryConvertToTruncating(v, out result);
        result = default;
        return false;
    }
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TweakNumber result)
    {
        bool r = byte.TryParse(s, style, provider, out byte res);
        result = new()
        {
            Value = res
        };
        return r;
    }
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TweakNumber result)
    {
        bool r = byte.TryParse(s, style, provider, out byte res);
        result = new()
        {
            Value = res
        };
        return r;
    }
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out TweakNumber result)
    {
        bool r = byte.TryParse(s, provider, out byte res);
        result = new()
        {
            Value = res
        };
        return r;
    }
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TweakNumber result)
    {
        bool r = byte.TryParse(s, provider, out byte res);
        result = new()
        {
            Value = res
        };
        return r;
    }
    public int CompareTo(object? obj)
    {
        return Value.CompareTo(obj);
    }
    public int CompareTo(TweakNumber? other)
    {
        other ??= new()
        {
            Value = 0
        };
        return Value.CompareTo(other.Value);
    }
    public bool Equals(TweakNumber? other)
    {
        return this == (other ?? 0);
    }
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return Value.TryFormat(destination, out charsWritten, format, provider);
    }
    public static TweakNumber operator +(TweakNumber value)
    {
        return new()
        {
            Value = value.Value,
        };
    }
    public static TweakNumber operator +(TweakNumber left, TweakNumber right)
    {
        return new()
        {
            Value = (byte)(left.Value + right.Value)
        };
    }
    public static TweakNumber operator -(TweakNumber value)
    {
        return new()
        {
            Value = (byte)(-value.Value)
        };
    }
    public static TweakNumber operator -(TweakNumber left, TweakNumber right)
    {
        return new()
        {
            Value = (byte)(left.Value - right.Value)
        };
    }
    public static TweakNumber operator ++(TweakNumber value)
    {
        return new()
        {
            Value = (byte)(value.Value + 1)
        };
    }
    public static TweakNumber operator --(TweakNumber value)
    {
        return new()
        {
            Value = (byte)(value.Value-1)
        };
    }
    public static TweakNumber operator *(TweakNumber left, TweakNumber right)
    {
        return new()
        {
            Value = (byte)(left.Value * right.Value)
        };
    }
    public static TweakNumber operator /(TweakNumber left, TweakNumber right)
    {
        return new()
        {
            Value = (byte)(left.Value / right.Value)
        };
    }
    public static TweakNumber operator %(TweakNumber left, TweakNumber right)
    {
        return new()
        {
            Value = (byte)(left.Value % right.Value)
        };
    }
    public static bool operator ==(TweakNumber? left, TweakNumber? right)
    {
        if (left == null)
            return 0.CompareTo(right ?? 0) == 0;
        return left.CompareTo(right) == 0;
    }
    public static bool operator !=(TweakNumber? left, TweakNumber? right)
    {
        if (left == null)
            return 0.CompareTo(right ?? 0) != 0;
        return left.CompareTo(right) != 0;
    }
    public static bool operator <(TweakNumber left, TweakNumber right)
    {
        return left.CompareTo(right) < 0;
    }
    public static bool operator >(TweakNumber left, TweakNumber right)
    {
        return left.CompareTo(right) > 0;
    }
    public static bool operator <=(TweakNumber left, TweakNumber right)
    {
        return left.CompareTo(right) <= 0;
    }
    public static bool operator >=(TweakNumber left, TweakNumber right)
    {
        return left.CompareTo(right) >= 0;
    }
    public override bool Equals(object? obj)
    {
        if (obj is TweakNumber n)
            return Equals(n);
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return Value;
    }
    public override string ToString()
    {
        return Value.ToString();
    }
    public static implicit operator byte(TweakNumber t)
    {
        return t.Value;
    }
}
