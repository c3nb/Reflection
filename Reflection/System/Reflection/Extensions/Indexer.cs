namespace System.Reflection.Extensions
{
	public class Indexer
	{
		public Indexer(PropertyInfo item, object instance)
		{
			ItemProp = item;
			if (item.GetAccessors()[0].IsStatic)
				Instance = null;
			else
				Instance = instance;
		}
		public PropertyInfo ItemProp { get; private set; }
		public object Instance { get; private set; }
		public object this[params object[] index]
		{
			get => ItemProp.GetValue(Instance, index);
			set => ItemProp.SetValue(Instance, value, index);
		}
	}
	public class Indexer<T> : Indexer
	{
		public Indexer(PropertyInfo item, object instance) : base(item, instance) { }
		public new T this[params object[] index] => (T)base[index];
	}
}
