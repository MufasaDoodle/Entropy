using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

public class WeightedList<T> : IEnumerable<WeightedValue<T>>, ISerializable
{
	private List<WeightedValue<T>> _valueList;

	/// <summary>
	/// Total weights of the list.
	/// </summary>
	public double TotalWeight { get; private set; }

	/// <summary>
	/// Constructor.
	/// </summary>
	public WeightedList()
	{
		_valueList = new List<WeightedValue<T>>();
	}

	/// <summary>
	/// Deep copy consturctor
	/// </summary>
	public WeightedList(WeightedList<T> weightedList)
	{
		_valueList = new List<WeightedValue<T>>(weightedList._valueList);
		TotalWeight = weightedList.TotalWeight;
	}

	/// <summary>
	/// Adds a value to the weighted list.
	/// </summary>
	/// <param name="weight">Weight of this value in the list.</param>
	public void Add(double weight, T value)
	{
		var listEntry = new WeightedValue<T> { Weight = weight, Value = value };
		_valueList.Add(listEntry);
		TotalWeight += weight;
	}

	public void Add(WeightedValue<T> value)
	{
		Add(value.Weight, value.Value);
	}
	/// <summary>
	/// Adds the contents of another weighted list to this one.
	/// </summary>
	/// <param name="otherList">The list to add.</param>
	public void AddRange(WeightedList<T> otherList)
	{
		_valueList.AddRange(otherList._valueList);
		TotalWeight += otherList.TotalWeight;
	}

	/// <summary>
	/// Removes the specified value from the list.
	/// </summary>
	public void Remove(T value)
	{
		int removeAtIndex = -1;
		for (int i = 0; i < _valueList.Count; i++)
		{
			if (_valueList[i].Value.Equals(value))
			{
				removeAtIndex = i;
				break;
			}
		}

		RemoveAt(removeAtIndex);
	}

	/// <summary>
	/// Remove the value at the specified index.
	/// </summary>
	public void RemoveAt(int index)
	{
		double weight = _valueList[index].Weight;
		_valueList.RemoveAt(index);

		TotalWeight -= weight;
	}

	public bool ContainsValue(T Value)
	{
		return _valueList.Contains(new WeightedValue<T> { Value = Value });
	}

	public int IndexOf(T Value)
	{
		return _valueList.IndexOf(new WeightedValue<T> { Value = Value });
	}

	public IEnumerator<WeightedValue<T>> GetEnumerator()
	{
		return _valueList.GetEnumerator();
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/// <summary>
	/// Selects a value from the list based on the input.
	/// </summary>
	/// <param name="rngValue">Value 0.0 to 1.0 represending the random value selected by the RNG.</param>
	/// <returns></returns>
	public T Select(double rngValue)
	{
		double cumulativeChance = 0;
		foreach (WeightedValue<T> listEntry in _valueList)
		{
			double realChance = listEntry.Weight / TotalWeight;
			cumulativeChance += realChance;

			if (rngValue < cumulativeChance)
			{
				return listEntry.Value;
			}
		}
		throw new InvalidOperationException("Failed to choose a random value.");
	}

	/// <summary>
	/// Selects the value at the specified index.
	/// </summary>
	public T SelectAt(int index)
	{
		return _valueList[index].Value;
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Values", _valueList);
	}

	public WeightedList(SerializationInfo info, StreamingContext context)
	{
		_valueList = (List<WeightedValue<T>>)info.GetValue("Values", typeof(List<WeightedValue<T>>));

		// rebuild total weight:
		TotalWeight = 0;
		foreach (var w in _valueList)
		{
			TotalWeight += w.Weight;
		}
	}

	public WeightedValue<T> this[int index]
	{
		get { return _valueList[index]; }
		set { RemoveAt(index); Add(value); }
	}
}

public class WeightedValue<T>
{
	public double Weight { get; set; }
	public T Value { get; set; }

	protected bool Equals(WeightedValue<T> other)
	{
		return EqualityComparer<T>.Default.Equals(Value, other.Value);
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}
		if (ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((WeightedValue<T>)obj);
	}

	public override int GetHashCode()
	{
		return EqualityComparer<T>.Default.GetHashCode(Value);
	}
}

