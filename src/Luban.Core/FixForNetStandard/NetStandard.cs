#if !NET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

namespace System.Diagnostics.CodeAnalysis
{
  /// <summary>Specifies that <see langword="null" /> is disallowed as an input even if the corresponding type allows it.</summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
  public sealed class DisallowNullAttribute : Attribute
  {
  }
}

public static class KeyValuePairExtensions
{
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
    {
        key = kvp.Key;
        value = kvp.Value;
    }
}

public static class EnumerableExtensions
{
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new HashSet<T>(source);
    }

    public static bool TryPop<T>(this Stack<T> stack, out T value)
    {
        if (stack == null)
            throw new ArgumentNullException(nameof(stack));

        if (stack.Count == 0)
        {
            value = default;
            return false;
        }

        value = stack.Pop();
        return true;
    }
    
    public static bool TryPeek<T>(this Stack<T> stack, out T value)
    {
        if (stack == null)
            throw new ArgumentNullException(nameof(stack));

        if (stack.Count == 0)
        {
            value = default;
            return false;
        }

        value = stack.Peek();
        return true;
    }
    
    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));

        if (dictionary.ContainsKey(key))
            return false;

        dictionary.Add(key, value);
        return true;
    }

}


public static class StringExtensions
{
    public static bool StartsWith(this string str, char value)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        return str[0] == value;
    }

    public static bool EndsWith(this string str, char value)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        return str[str.Length - 1] == value;
    }

    public static string ReplaceLineEndings(this string input, string replacement)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
        if (replacement == null)
            throw new ArgumentNullException(nameof(replacement));

        // 定义环境特定的行结束符
        string lineEnding = Environment.NewLine;
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        // 在 .NET Standard 1.x 中，Environment.NewLine 可能不是最准确的行结束符
        // 因此，我们可以考虑使用 "\r\n" 和 "\n" 作为可能的行结束符
        input = input.Replace("\r\n", replacement);
        input = input.Replace("\n", replacement);
#else
        // 在 .NET Standard 2.0 或更高版本中，Environment.NewLine 通常足够
        input = input.Replace(lineEnding, replacement);
#endif
        return input;
    }
    public static string Join<T>(this char separator, IEnumerable<T> values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        // 将值转换为字符串并使用 String.Join 方法
        return String.Join(separator.ToString(), ConvertToStrings(values));
    }

    private static IEnumerable<string> ConvertToStrings<T>(IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            // 检查是否为 null，然后转换为字符串
            yield return value?.ToString();
        }
    }

    public static string[] Split(this string str, string separator, StringSplitOptions options = StringSplitOptions.None)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        if (separator == null)
            throw new ArgumentNullException(nameof(separator));

        // .NET Standard 中的 Split 方法不支持 StringSplitOptions 参数
        // 因此我们首先调用 .NET Standard 的 Split 方法，然后根据 options 进行处理
        string[] result = str.Split(new[] { separator }, StringSplitOptions.None);

        // 如果需要去除空元素，则进行过滤
        if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
        {
            List<string> filteredResult = new List<string>();
            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    filteredResult.Add(item);
                }
            }
            return filteredResult.ToArray();
        }

        return result;
    }

    public static string TrimEndingDirectorySeparator(this string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        // 检查路径末尾是否有目录分隔符
        if (path.EndsWith("/") || path.EndsWith("\\"))
        {
            // 移除路径末尾的目录分隔符
            return path.Substring(0, path.Length - 1);
        }

        return path;
    }

}

public partial class String
{ 
    public static string Join<T,TS>(TS separator, IEnumerable<T> values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        // 将值转换为字符串并使用 String.Join 方法
        var sb = new StringBuilder();
        bool isFirst = true;

        foreach (T value in values)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                sb.Append(separator);
            }

            if (value != null)
            {
                sb.Append(value.ToString());
            }
        }

        return sb.ToString();
    }

    private static IEnumerable<string> ConvertToStrings<T>(IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            // 检查是否为 null，然后转换为字符串
            yield return value?.ToString();
        }
    }
}

public class ReadOnlyMemory<T> : IEquatable<ReadOnlyMemory<T>>, IEnumerable<T>
{
    private readonly T[] _array;
    private readonly int _offset;
    private readonly int _count;

    public ReadOnlyMemory(T[] array)
    {
        _array = array ?? throw new ArgumentNullException(nameof(array));
        _offset = 0;
        _count = array.Length;
    }

    public ReadOnlyMemory(T[] array, int offset, int count)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (offset < 0 || offset >= array.Length) throw new ArgumentOutOfRangeException(nameof(offset));
        if (count < 0 || (offset + count) > array.Length) throw new ArgumentOutOfRangeException(nameof(count));

        _array = array;
        _offset = offset;
        _count = count;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _array[_offset + index];
        }
    }

    public int Count => _count;

    public ArraySegment<T> ToArraySegment()
    {
        return new ArraySegment<T>(_array, _offset, _count);
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    public bool Equals(ReadOnlyMemory<T> other)
    {
        if (ReferenceEquals(other, null)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (_count != other._count) return false;

        int length = _count;
        for (int i = 0; i < length; i++)
        {
            if (!(_array[_offset + i].Equals(other._array[other._offset + i])))
                return false;
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ReadOnlyMemory<T>);
    }

    public override int GetHashCode()
    {
        int hashCode = -2027230473;
        for (int i = _offset, end = _offset + _count; i < end; i++)
        {
            hashCode = hashCode * -1521134295 + _array[i].GetHashCode();
        }
        return hashCode;
    }

    public struct Enumerator : IEnumerator<T>
    {
        private readonly ReadOnlyMemory<T> _segment;
        private int _index;
        private T _current;

        public Enumerator(ReadOnlyMemory<T> segment)
        {
            _segment = segment;
            _index = -1;
            _current = default;
        }

        public T Current
        {
            get
            {
                if (_index < 0 || _index >= _segment._count)
                    throw new InvalidOperationException();
                return _current;
            }
        }

        object IEnumerator.Current => _current;

        public bool MoveNext()
        {
            _index++;
            if (_index < _segment._count)
            {
                _current = _segment._array[_segment._offset + _index];
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _index = -1;
            _current = default;
        }

        public void Dispose()
        {
        }
    }
}

public static class ReadOnlyMemoryExtensions
{
    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this T[] array)
    {
        return new ReadOnlyMemory<T>(array);
    }

    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this T[] array, int offset, int count)
    {
        return new ReadOnlyMemory<T>(array, offset, count);
    }
}
#endif
