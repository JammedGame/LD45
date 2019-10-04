using System.Collections;
using System.Collections.Generic;

public abstract class ReusableEnumerator<T, K> : IEnumerator<T>, IEnumerable<T>
	where K : ReusableEnumerator<T, K>, new()
{
	/* Supports pooling. */
	static readonly Stack<K> pool = new Stack<K>();
	protected static K Fetch() => pool.Count > 0 ? pool.Pop() : new K();

	public void Dispose()
	{
		Current = default(T);
		OnDisposed();
		pool.Push((K)this);
	}

	/* for implementations. */
	public abstract void OnDisposed();
	public abstract bool MoveNext();

	/* Current state */
	public T Current { get; protected set; }
	object IEnumerator.Current => Current;

	/* Interface crap */
	public void Reset() => Dispose();
	IEnumerator IEnumerable.GetEnumerator() => this;
	public IEnumerator<T> GetEnumerator() => this;
}