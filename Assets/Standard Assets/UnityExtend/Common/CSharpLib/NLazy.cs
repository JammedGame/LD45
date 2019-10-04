/// <summary>
/// Structure that provides easier creation of lazy-get fields,
/// storing within itself both bool that tells is value assigned and value itself.
/// </summary>
public struct NLazy<T>
{
	public bool IsAssigned;
	public T Value;

	NLazy(bool isAssigned, T value)
	{
		IsAssigned = isAssigned;
		Value = value;
	}

	/// <summary>
	/// Reset value, i.e. unassign value.
	/// </summary>
	public void ClearValue()
	{
		IsAssigned = false;
		Value = default(T);
	}

	/// <summary>
	/// Convert to value
	/// </summary>
	public static implicit operator T(NLazy<T> lazyT) => lazyT.Value;

	/// <summary>
	/// Convert value to lazy
	/// </summary>
	public static implicit operator NLazy<T>(T value) => new NLazy<T>(true, value);
}