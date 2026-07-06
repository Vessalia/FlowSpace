using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public class Singleton<T> where T : Singleton<T>
{
	protected Singleton() { }

	private static readonly Lazy<T> instance = new Lazy<T>(() => Create());

	public static T Instance
	{
		get
		{
			return instance.Value;
		}
	}

	private static T Create()
	{
		try
		{
			var constructors = typeof(T).GetConstructors(BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Instance);
			return (T)constructors.Single().Invoke(null);
		}
		catch
		{
			throw new Exception(typeof(T) + "");
		}
	}
}
