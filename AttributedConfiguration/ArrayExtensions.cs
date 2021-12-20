using System;

namespace AttributedConfiguration;

public static class ArrayExtensions {
	public static Array ToTypeArray<T>(this T[] values, Type type) {
		var typeArray = Array.CreateInstance(type, values.Length);
		Array.Copy(values, typeArray, values.Length);
		return typeArray;
	}
}
