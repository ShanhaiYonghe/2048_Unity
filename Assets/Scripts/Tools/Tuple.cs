using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public interface IStructuralEquatable
{
    bool Equals(object other, IEqualityComparer comparer);
    int GetHashCode(IEqualityComparer comparer);
}

public interface IStructuralComparable
{
    int CompareTo(object other, IComparer comparer);
}

public interface IComparable
{
    int CompareTo(object obj);
}
internal interface ITuple
{
    int GetHashCode(IEqualityComparer comparer);
    string ToString(StringBuilder sb);

    int Size { get; }
}

public static class Tuple
{
    internal static int CombineHashCodes(int h1, int h2)
    {
        return (((h1 << 5) + h1) ^ h2);
    }
    internal static int CombineHashCodes(int h1, int h2, int h3)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2), h3);
    }
    internal static int CombineHashCodes(int h1, int h2, int h3, int h4)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
    }
    internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
    }
    internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6));
    }
    internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7));
    }
    internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
    {
        return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7, h8));
    }

    //public static Tuple<T1> Create<T1>(T1 item1);
    //public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2);
    public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
    {
        return new Tuple<T1, T2, T3>(item1, item2, item3);
    }
    //public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4);
    //public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5);
    //public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6);
    //public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7);
    //public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8);
}

public class Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
{
    // Fields
    private readonly T1 m_Item1;
    private readonly T2 m_Item2;

    // Properties
    public T1 Item1
    {
        get
        {
            return this.m_Item1;
        }
    }
    public T2 Item2
    {
        get
        {
            return this.m_Item2;
        }
    }
    int ITuple.Size
    {
        get
        {
            return 2;
        }
    }

    // Methods
    public Tuple(T1 item1, T2 item2)
    {
        this.m_Item1 = item1;
        this.m_Item2 = item2;
    }
    public override bool Equals(object obj)
    {
        return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
    }
    public override int GetHashCode()
    {
        return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default);
    }
    int IStructuralComparable.CompareTo(object other, IComparer comparer)
    {
        if (other == null)
        {
            return 1;
        }
        Tuple<T1, T2> tuple = other as Tuple<T1, T2>;
        if (tuple == null)
        {
            //throw new ArgumentException(Environment.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
        }
        int num = 0;
        num = comparer.Compare(this.m_Item1, tuple.m_Item1);
        if (num != 0)
        {
            return num;
        }
        return comparer.Compare(this.m_Item2, tuple.m_Item2);
    }
    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
    {
        if (other == null)
        {
            return false;
        }
        Tuple<T1, T2> tuple = other as Tuple<T1, T2>;
        if (tuple == null)
        {
            return false;
        }
        return (comparer.Equals(this.m_Item1, tuple.m_Item1) && comparer.Equals(this.m_Item2, tuple.m_Item2));
    }
    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
        return Tuple.CombineHashCodes(comparer.GetHashCode(this.m_Item1), comparer.GetHashCode(this.m_Item2));
    }
    int IComparable.CompareTo(object obj)
    {
        return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
    }
    int ITuple.GetHashCode(IEqualityComparer comparer)
    {
        return ((IStructuralEquatable)this).GetHashCode(comparer);
    }
    string ITuple.ToString(StringBuilder sb)
    {
        sb.Append(this.m_Item1);
        sb.Append(", ");
        sb.Append(this.m_Item2);
        sb.Append(")");
        return sb.ToString();
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("(");
        return ((ITuple)this).ToString(sb);
    }
    public bool IsEualsValue(Tuple<T1, T2> tuple)
    {
        return Comparer<object>.Default.Compare(this.m_Item1, tuple.Item1) == 0
               && Comparer<object>.Default.Compare(this.m_Item2, tuple.Item2) == 0;
    }
}



public class Tuple<T1, T2, T3> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
{
    private readonly T1 m_Item1;
    private readonly T2 m_Item2;
    private readonly T3 m_Item3;

    // Properties
    public T1 Item1
    {
        get
        {
            return this.m_Item1;
        }
    }
    public T2 Item2
    {
        get
        {
            return this.m_Item2;
        }
    }
    public T3 Item3
    {
        get
        {
            return this.m_Item3;
        }
    }
    int ITuple.Size
    {
        get
        {
            return 3;
        }
    }

    public Tuple(T1 item1, T2 item2, T3 item3)
    {
        this.m_Item1 = item1;
        this.m_Item2 = item2;
        this.m_Item3 = item3;
    }
    public override bool Equals(object obj)
    {
        return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
    }
    public override int GetHashCode()
    {
        return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default);
    }
    int IStructuralComparable.CompareTo(object other, IComparer comparer)
    {
        if (other == null)
        {
            return 1;
        }
        Tuple<T1, T2, T3> tuple = other as Tuple<T1, T2, T3>;
        if (tuple == null)
        {
            //throw new ArgumentException(Environment.GetResourceString("ArgumentException_TupleIncorrectType", new object[] { base.GetType().ToString() }), "other");
        }
        int num = 0;
        num = comparer.Compare(this.m_Item1, tuple.m_Item1);
        if (num != 0)
        {
            return num;
        }
        num = comparer.Compare(this.m_Item2, tuple.m_Item2);
        if (num != 0)
        {
            return num;
        }
        return comparer.Compare(this.m_Item3, tuple.m_Item3);
    }
    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
    {
        if (other == null)
        {
            return false;
        }
        Tuple<T1, T2, T3> tuple = other as Tuple<T1, T2, T3>;
        if (tuple == null)
        {
            return false;
        }
        return ((comparer.Equals(this.m_Item1, tuple.m_Item1) && comparer.Equals(this.m_Item2, tuple.m_Item2)) && comparer.Equals(this.m_Item3, tuple.m_Item3));
    }
    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
        return Tuple.CombineHashCodes(comparer.GetHashCode(this.m_Item1), comparer.GetHashCode(this.m_Item2), comparer.GetHashCode(this.m_Item3));
    }
    int IComparable.CompareTo(object obj)
    {
        return ((IStructuralComparable)this).CompareTo(obj, Comparer<object>.Default);
    }
    int ITuple.GetHashCode(IEqualityComparer comparer)
    {
        return ((IStructuralEquatable)this).GetHashCode(comparer);
    }
    string ITuple.ToString(StringBuilder sb)
    {
        sb.Append(this.m_Item1);
        sb.Append(", ");
        sb.Append(this.m_Item2);
        sb.Append(", ");
        sb.Append(this.m_Item3);
        sb.Append(")");
        return sb.ToString();
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("(");
        return ((ITuple)this).ToString(sb);
    }

    public bool IsEualsValue(T1 item1, T2 item2, T3 item3)
    {
        return Comparer<object>.Default.Compare(this.m_Item1, item1) == 0
            && Comparer<object>.Default.Compare(this.m_Item2, item2) == 0
            && Comparer<object>.Default.Compare(this.m_Item3, item3) == 0;
    }
    public bool IsEualsValue(Tuple<T1, T2, T3> tuple)
    {
        return Comparer<object>.Default.Compare(this.m_Item1, tuple.Item1) == 0
               && Comparer<object>.Default.Compare(this.m_Item2, tuple.Item2) == 0
               && Comparer<object>.Default.Compare(this.m_Item3, tuple.Item3) == 0;
    }
}


