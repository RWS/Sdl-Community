using System;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	/// <summary>
	/// Defines a set of methods for computing the costs of the six basic alignment operations.
	/// </summary>
	/// <typeparam name="T">The type of the sequence elements to be aligned.</typeparam>
	public interface IAlignmentCostComputer<T>
	{
		/// <summary>
		/// Returns the costs of the 1:1 alignment between the source and target item
		/// </summary>
		/// <param name="s">The source item</param>
		/// <param name="t">The target item</param>
		int GetSubstitutionCosts(T s, T t);
		/// <summary>
		/// Returns the costs of the 1:0 alignment of the source item (deletion)
		/// </summary>
		/// <param name="s">The source item</param>
		int GetDeletionCosts(T s);
		/// <summary>
		/// Returns the costs of the 0:1 alignment of the target item (insertion)
		/// </summary>
		/// <param name="t">The target item</param>
		int GetInsertionCosts(T t);
		/// <summary>
		/// Returns the costs of the 2:1 alignment between two source items and the target item
		/// </summary>
		/// <param name="s1">The first source item</param>
		/// <param name="s2">The second source item</param>
		/// <param name="t">The target item</param>
		int GetContractionCosts(T s1, T s2, T t);
		/// <summary>
		/// Returns the costs of the 1:2 alignment between the the source item and two target items
		/// </summary>
		/// <param name="s">The source item</param>
		/// <param name="t1">The first target item</param>
		/// <param name="t2">The second target item</param>
		int GetExpansionCosts(T s, T t1, T t2);
		/// <summary>
		/// Returns the costs of the 2:2 alignment between the two source and the two target items
		/// </summary>
		/// <param name="s1">The first source item</param>
		/// <param name="s2">The second source item</param>
		/// <param name="t1">The first target item</param>
		/// <param name="t2">The second target item</param>
		int GetMeldingCosts(T s1, T s2, T t1, T t2);
	}
}
