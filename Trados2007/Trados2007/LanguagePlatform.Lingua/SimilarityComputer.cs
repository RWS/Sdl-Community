using System;

namespace Sdl.LanguagePlatform.Lingua
{
	/// <summary>
	/// Return the simliarity between two objects, as a float value between
	/// 0 and 1. A larger value indicates higher simliarity. 1 indicates
	/// equality, 0 indicates inequality. 
	/// <para>
	/// An implementation may return a proper value for null parameters or may 
	/// throw an exception. An implementation may also choose either
	/// if the object types are not comparable. 
	/// </para>
	/// <para>
	/// The following conditions must be met by any impementation:
	/// <list type="bullet">
	/// <item><c>GetSimilarity(x, x) == 1.0</c> (reflexive)</item> 
	/// <item><c>GetSimilarity(x, y) == GetSimilarity(y, x)</c> (symmetrical)</item>
	/// </list>
	/// </para>
	/// </summary>
	/// <param name="a">The first object to compare</param>
	/// <param name="b">The second object to compare</param>
	/// <returns>A similarity measure between the two objects, which is
	/// a floating point number between 0.0 and 1.0</returns>
	public delegate double SimilarityComputer<T>(T a, T b);

	/// <summary>
	/// Return the costs of inserting or deleting the object in some underlying sequence.
	/// </summary>
	/// <returns>The costs, which are between 0 and 1, where 0 means lowest costs and 1 means
	/// highest costs.</returns>
	public delegate double InsertDeleteCostComputer<T>(T a);
}
