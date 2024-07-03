using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAttributedConfiguration {
	public static class IEnumerableExtensions {
		public static Array ToTypeArray<T>(this IEnumerable<T> enumerable, Type type)
			=> enumerable.ToArray().ToTypeArray(type);
	}
}
