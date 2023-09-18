namespace OurAzure.Api.Extensions;

public static class ListExtensions {
	public static void Remove<T>(this IList<T> list, Type type)
	{
		var items = list.Where(x => x?.GetType() == type).ToList();
		items.ForEach(x => list.Remove(x));
	}

	// https://stackoverflow.com/a/30248074/11141271
	public static List<List<T>> SplitList<T>(this List<T> me, int size)
	{
		var list = new List<List<T>>();
		for (int i = 0; i < me.Count; i += size)
			list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
		return list;
	} 
}