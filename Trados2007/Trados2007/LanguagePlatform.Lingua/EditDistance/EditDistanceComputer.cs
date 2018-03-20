using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.LanguagePlatform.Core.EditDistance;

namespace Sdl.LanguagePlatform.Lingua.EditDistance
{
	// TODO computation of move operations doesn't work nicely, needs research and refinement
	// TODO optimization: Not the complete similarity matrix is needed, stick to near the diagonal
	// TODO document that change costs = 1 - similarity
	// TODO use a real move ED algorithm?

	/*
	 * More ideas:
	 * - optionally; constant change costs instead of 1.0d - item similarity
	 * - optionally: move costs depend on move distance (need weighted function)
	 * */

	/// <summary>
	/// <para>
	/// This class provides methods to compute the edit distance between two item sequences of type <typeparamref name="T"/>. 
	/// The user
	/// needs to pass a similarity computation method for two items as well as specify 
	/// the costs of insert/delete and move operations. Insertion costs and deletion costs
	/// are always identical (which makes the computation symmetric, i.e. ed(a, b) = ed(b, a)).
	/// </para>
	/// <list type="bullet">
	/// <item>All costs and similarities are expressed as floating-point values between 0 and 1.</item>
	/// <item>Change costs are computed as 1 minus the similary value between the items (as returned
	/// by the user-supplied similarity computation method).</item>
	/// <item>Move operations are not computed by default and need to be explicitly activated. </item>
	/// </list>
	/// There are several differences to "standard" edit distance:
	/// <list type="bullet">
	/// <item>Costs of edit operations can be freely defined (in classic ED they default to 1)</item>
	/// <item>Tthe set of operations depends on the similarity between two items (in ED two items are either equal or not)</item>
	/// <item>This implementation optionally computes (limited) move operations</item>
	/// <item>The algorithm can be used for item sequences of arbitrary types (classic ED is only for character strings).</item>
	/// </list>
	/// </summary>
	/// <typeparam name="T">The item type of the sequence elements</typeparam>
	public class EditDistanceComputer<T>
	{
		private SimilarityComputer<T> _SimilarityComputer;

		private double _InsertDeleteCosts;
		private double _MoveCosts;

		private double _SimThreshold;
		private bool _ComputeMoveOperations = false;

#if DEBUG
		static private bool _DumpMatrix = false;

		/// <summary>
		/// Controls whether or not the similarity matrix is dumped to %TEMP%/SimMatrix.html after
		/// computation of the ED. This is for debugging purposes only, and this flag (as well as
		/// dumping the matrix) is only available in Debug builds. 
		/// </summary>
		public static bool DumpMatrix
		{
			get { return _DumpMatrix; }
			set { _DumpMatrix = value; }
		}
#endif

		/// <summary>
		/// Instantiates an edit distance computer with the specified similarity 
		/// computation function and the specified insert/delete and move costs. Note
		/// that no move operations are computed unless ComputeMoveOperations is set to
		/// true (the default is false).
		/// </summary>
		/// <param name="similarityComputer">The item similarity computation method</param>
		/// <param name="insertDeleteCosts">The costs of inserting/deleting an item</param>
		/// <param name="moveCosts">The costs of moving an item. If &gt;0, move operations will 
		/// be computed. Otherwise, no move operations will be computed. Move costs should be 
		/// between the insertion/deletion costs and two times the insertion/deletion costs.</param>
		public EditDistanceComputer(SimilarityComputer<T> similarityComputer,
			double insertDeleteCosts, double moveCosts)
		{
			_SimilarityComputer = similarityComputer;

			if (insertDeleteCosts < 0.0d)
				throw new ArgumentOutOfRangeException("Insert/delete costs must be larger than 0");
			if (moveCosts < 0.0d)
				throw new ArgumentOutOfRangeException("Move costs must be larger than 0");

			_InsertDeleteCosts = insertDeleteCosts;
			_MoveCosts = moveCosts;

			_SimThreshold = 0.85d;
			_ComputeMoveOperations = (moveCosts > 0.0d);
		}

		/// <summary>
		/// Instantiates an edit distance computer with the specified similarity 
		/// computation function and the default insert/delete (1.0) costs, change
		/// costs of 0.9, and zero move (0.0) costs (the latter meaning that no 
		/// move operations are computed).
		/// </summary>
		/// <param name="similarityComputer">The item similarity computation method</param>
		public EditDistanceComputer(SimilarityComputer<T> similarityComputer)
			: this(similarityComputer, 1.0d, 0.0d)
		{
		}

		/// <summary>
		/// Gets or sets the similarity threshold which is used to detect move operations. If the similarity
		/// between items participating in two "compensating operations" is >= the threshold, a "move" may be
		/// recorded since the items are considered "sufficiently similar". The default threshold
		/// is 0.7. Note that no move operations will be computed if ComputeMoveOperations is false 
		/// (which is the default).
		/// </summary>
		public double SimilarityThreshold
		{
			get { return _SimThreshold; }
			set { _SimThreshold = value; }
		}

		/// <summary>
		/// Gets or sets the costs for inserting or deleting an item. Insert/delete costs must be between 0 and 1. The
		/// default is 1.
		/// </summary>
		public double InsertDeleteCosts
		{
			get { return _InsertDeleteCosts; }
			set
			{
				if (value < 0.0d)
					throw new ArgumentOutOfRangeException("Insertion/deletion costs must be >= 0");
				_InsertDeleteCosts = value;
			}
		}

		/// <summary>
		/// Gets or sets the costs of moving an item. Move costs should be between the insert/deletion
		/// costs and two times the insertion/deletion costs. The default is 1.
		/// </summary>
		public double MoveCosts
		{
			get { return _MoveCosts; }
			set
			{
				if (value < 0.0d)
					throw new ArgumentOutOfRangeException("Move costs must be  >= 0");
				_MoveCosts = value;
				_ComputeMoveOperations = (_MoveCosts > 0.0d);
			}
		}

		/// <summary>
		/// Gets or sets a flag which controls whether or not move operations are computed. 
		/// The default is false.
		/// </summary>
		public bool ComputeMoveOperations
		{
			get { return _ComputeMoveOperations; }
			set { _ComputeMoveOperations = value; }
		}

		private struct MatrixItem
		{
			public MatrixItem(double score, Core.EditDistance.EditOperation op, double similarity)
			{
				Score = score;
				Operation = op;
				Similarity = similarity;
			}

			public double Score;
			public Core.EditDistance.EditOperation Operation;
			public double Similarity;
		}

		/// <summary>
		/// Computes and returns the edit distance between two sequences of type <typeparamref name="T"/>, using 
		/// the similarity computer and cost values specified in the constructor. If <see cref="ComputeMoveOperations"/>
		/// is <c>true</c>, simple moves will be detected. Otherwise, moves will (typically) result in two independent
		/// insert/delete operations.
		/// </summary>
		/// <param name="sourceObjects">The first input sequence ("source")</param>
		/// <param name="targetObjects">The second input sequence ("target")</param>
		/// <returns>The edit distance between the two sequences</returns>
		public Core.EditDistance.EditDistance ComputeEditDistance(IList<T> sourceObjects,
			IList<T> targetObjects)
		{
			return ComputeEditDistance(sourceObjects, targetObjects, null);
		}

		/// <summary>
		/// Computes and returns the edit distance between two sequences of type <typeparamref name="T"/>, using 
		/// the similarity computer and cost values specified in the constructor. If <see cref="ComputeMoveOperations"/>
		/// is <c>true</c>, simple moves will be detected. Otherwise, moves will (typically) result in two independent
		/// insert/delete operations.
		/// </summary>
		/// <param name="sourceObjects">The first input sequence ("source")</param>
		/// <param name="targetObjects">The second input sequence ("target")</param>
		/// <param name="precomputedAssociations">A list of precomputed item index associations. If valid, item pairs
		/// in this list will be associated with each other, which will result in either an identity
		/// or a change operation.</param>
		/// <returns>The edit distance between the two sequences</returns>
		public Core.EditDistance.EditDistance ComputeEditDistance(IList<T> sourceObjects,
			IList<T> targetObjects, List<Core.Pair<int>> precomputedAssociations)
		{
			const double invalidAssignmentCosts = 100000.0d;

#if DEBUG
			if (typeof(T) != typeof(char))
			{
			}
#endif
			if (sourceObjects == null)
				throw new ArgumentNullException("sourceObjects");
			if (targetObjects == null)
				throw new ArgumentNullException("targetObjects");

			if (precomputedAssociations != null)
			{
				if (!SortAndValidate(precomputedAssociations, sourceObjects.Count, targetObjects.Count))
				{
					System.Diagnostics.Debug.Assert(false, "Invalid preassignments");
					precomputedAssociations = null;
				}
			}

			// TODO handle special cases (one/both of the arrays being empty/having no elements)
			// TODO use diagonal algorithm

			Core.EditDistance.EditDistance result = new Core.EditDistance.EditDistance(sourceObjects.Count, targetObjects.Count, 0.0d);

			MatrixItem[,] matrix = new MatrixItem[sourceObjects.Count + 1, targetObjects.Count + 1];
			int i, j;

			bool usePreassignments = precomputedAssociations != null;

			// initialize matrix
			matrix[0, 0] = new MatrixItem(0.0d, Core.EditDistance.EditOperation.Identity, 0.0d);

			for (i = 1; i <= sourceObjects.Count; ++i)
				matrix[i, 0] = new MatrixItem((double)i * _InsertDeleteCosts, Core.EditDistance.EditOperation.Delete, 0.0d);

			for (j = 1; j <= targetObjects.Count; ++j)
				matrix[0, j] = new MatrixItem((double)j * _InsertDeleteCosts, Core.EditDistance.EditOperation.Insert, 0.0d);

			for (i = 1; i <= sourceObjects.Count; ++i)
				for (j = 1; j <= targetObjects.Count; ++j)
					matrix[i, j] = new MatrixItem(0.0d, Core.EditDistance.EditOperation.Identity, 0.0d);

			// populate matrix

			for (i = 1; i <= sourceObjects.Count; ++i)
			{
				T s = sourceObjects[i - 1];

				int associatedTarget = usePreassignments
					? GetSourcePreassignment(i - 1, precomputedAssociations)
					: -1;

				for (j = 1; j <= targetObjects.Count; ++j)
				{
					T t = targetObjects[j - 1];

					double similarity = 0.0d;

					if (associatedTarget < 0 || associatedTarget == j - 1)
						// no preassignment or items are correlated - use std sim
						similarity = _SimilarityComputer(s, t);
					else
						// there is a correlation with another item - don't allow change/identity
						similarity = -1.0d;

					System.Diagnostics.Debug.Assert((similarity >= 0.0d && similarity <= 1.0d)
						|| similarity == -1.0d);

					// low similarity means high "change costs" and vice versa:
					double changeCosts = (similarity < 0)
						? invalidAssignmentCosts
						: matrix[i - 1, j - 1].Score + (1.0d - similarity);

					double insertCosts = matrix[i, j - 1].Score + _InsertDeleteCosts;
					double deleteCosts = matrix[i - 1, j].Score + _InsertDeleteCosts;

					double min = Math.Min(Math.Min(changeCosts, deleteCosts), insertCosts);

					matrix[i, j].Score = min;
					matrix[i, j].Similarity = similarity;

					if (min == deleteCosts)
					{
						matrix[i, j].Operation = Core.EditDistance.EditOperation.Delete;
					}
					else if (min == insertCosts)
					{
						matrix[i, j].Operation = Core.EditDistance.EditOperation.Insert;
					}
					else if (min == changeCosts)
					{
						if (similarity == 1.0d)
							matrix[i, j].Operation = Core.EditDistance.EditOperation.Identity;
						else
							matrix[i, j].Operation = Core.EditDistance.EditOperation.Change;
					}
				}
			}

			// readout the cheapest path

			i = sourceObjects.Count;
			j = targetObjects.Count;
			result.Distance = matrix[i, j].Score;

			while (i > 0 || j > 0)
			{
				EditDistanceItem item = new EditDistanceItem();
				item.Resolution = EditDistanceResolution.None;

				item.Operation = matrix[i, j].Operation;
				switch (item.Operation)
				{
				case EditOperation.Identity:
					--i;
					--j;
					item.Costs = 0.0d;
					break;
				case EditOperation.Change:
					item.Costs = 1.0d - matrix[i, j].Similarity;
					--i;
					--j;
					break;
				case EditOperation.Insert:
					--j;
					item.Costs = _InsertDeleteCosts;
					break;
				case EditOperation.Delete:
					item.Costs = _InsertDeleteCosts;
					--i;
					break;
				}

				item.Source = i;
				item.Target = j;
				result.AddAtStart(item);
			}

			// identify move operations which are pairs of insert/delete operations in the shortest path.
			// Note that the comparision result is already in the matrix and we only care about identity.
			// TODO we may rather use a configurable threshold than identity (1.0) to catch move operations
			//  of sufficiently similar items (e.g. case-insensitive)

			int moves = 0;

			// try to detect moves in case the move penalty is smaller than the sum of insert/delete penalties
			if (_ComputeMoveOperations)
			{
				// matrix[i, j].Similarity is the cached token similarity between source[i-1] and target[j-1]

				// TODO may need to restrict moves to undisputed corresponding items, i.e. those which 
				//  have only one row/column similarity maximum

				for (int index = 0; index < result.Items.Count; ++index)
				{
					EditOperation op = result[index].Operation;
					if (op == EditOperation.Delete || op == EditOperation.Insert)
					{
						int moveSource = 0;
						int moveTarget = 0;
						int moveSourceTarget = 0;
						int moveTargetSource = 0;

						// search in the remainder of the result list for a "compensating" operation
						int comp = 0;
						for (comp = index + 1; comp < result.Items.Count; ++comp)
						{
							if (result[comp].Operation == EditOperation.Insert
								&& op == EditOperation.Delete
								&& matrix[result[index].Source + 1, result[comp].Target + 1].Similarity >= _SimThreshold)
							{
								// source[result[index].Source] was deleted
								// target[result[comp].Target] was inserted
								moveSource = result[index].Source;
								moveSourceTarget = result[index].Target;

								moveTarget = result[comp].Target;
								moveTargetSource = result[comp].Source;
								break;
							}
							else if (result[comp].Operation == EditOperation.Delete
								&& op == EditOperation.Insert
								&& matrix[result[comp].Source + 1, result[index].Target + 1].Similarity >= _SimThreshold)
							{
								// source[result[comp].Source] was deleted
								// target[result[index].Target] was inserted
								moveSource = result[comp].Source;
								moveSourceTarget = result[comp].Target;

								moveTarget = result[index].Target;
								moveTargetSource = result[index].Source;
								break;
							}

						}

						// TODO take moveDistance into account, i.e. penalty depends on distance? Avoids
						//  long-distance moves.

						if (comp < result.Items.Count)
						{
							// compensating operation found
							// TODO backtrack to find other compensating items?
							EditDistanceItem item = result[index];
							item.Operation = EditOperation.Move;
							item.Source = moveSource;
							item.Target = moveTarget;
							item.MoveSourceTarget = moveSourceTarget;
							item.MoveTargetSource = moveTargetSource;
							// TODO update item similarity
							result.Items[index] = item;
							result.Items.RemoveAt(comp);
							++moves;
						}
					}
				}
			}

			if (moves > 0)
			{
				// adjust score: substract moves * (deletionCosts + insertionCosts), add moves * moveCosts
				// TODO take moveDistance into account, i.e. penalty depends on distance? 
				result.Distance -= (double)moves * (2.0d * _InsertDeleteCosts);
				result.Distance += (double)moves * _MoveCosts;
			}

#if DEBUG && !SILVERLIGHT

			_DumpMatrix = false; //  typeof(T) != typeof(char);
			if (_DumpMatrix)
			{
				// in debug mode, write matrix to a temp file in HTML format
				System.Environment.GetEnvironmentVariable("TEMP");
				System.IO.StreamWriter wtr = new System.IO.StreamWriter(System.Environment.GetEnvironmentVariable("TEMP") + "/SimMatrix.html",
					false, System.Text.Encoding.UTF8);
				System.Web.UI.Html32TextWriter htmlWriter = new System.Web.UI.Html32TextWriter(wtr);

				htmlWriter.WriteFullBeginTag("html");
				htmlWriter.WriteFullBeginTag("body");
				htmlWriter.WriteBeginTag("table");
				htmlWriter.WriteAttribute("border", "1");

				for (j = -1; j <= targetObjects.Count; ++j)
				{
					htmlWriter.WriteFullBeginTag("tr");

					for (i = -1; i <= sourceObjects.Count; ++i)
					{
						htmlWriter.WriteFullBeginTag("td");

						if (i < 0)
						{
							// caption row
							if (j >= 0)
							{
								htmlWriter.Write("j={0}", j);
								if (j > 0)
								{
									htmlWriter.WriteFullBeginTag("br");
									htmlWriter.WriteFullBeginTag("b");
									htmlWriter.Write(targetObjects[j - 1].ToString());
									htmlWriter.WriteEndTag("b");
								}
							}
						}
						else if (j < 0)
						{
							// j < 0 but i >= 0 --> 
							htmlWriter.Write("i={0}", i);
							if (i > 0)
							{
								htmlWriter.WriteFullBeginTag("br");
								htmlWriter.WriteFullBeginTag("b");
								htmlWriter.Write(sourceObjects[i - 1].ToString());
								htmlWriter.WriteEndTag("b");
							}
						}
						else
						{
							// content cell
							htmlWriter.Write("d={0}", matrix[i, j].Score);
							htmlWriter.WriteFullBeginTag("br");
							htmlWriter.Write("s={0}", matrix[i, j].Similarity);
							htmlWriter.WriteFullBeginTag("br");
							htmlWriter.Write("o={0}", matrix[i, j].Operation.ToString());
						}

						htmlWriter.WriteEndTag("td");
					}

					htmlWriter.WriteEndTag("tr");
				}

				htmlWriter.WriteEndTag("table");

				htmlWriter.WriteFullBeginTag("h2");
				htmlWriter.Write("Result");
				htmlWriter.WriteEndTag("h2");

				htmlWriter.Write("Score = {0}", result.Distance);

				htmlWriter.WriteFullBeginTag("ol");

				for (i = 0; i < result.Items.Count; ++i)
				{
					htmlWriter.WriteFullBeginTag("li");
					htmlWriter.Write("{0}: s={1} t={2}",
						result[i].Operation.ToString(), result[i].Source, result[i].Target);
				}

				htmlWriter.WriteEndTag("ol");

				htmlWriter.WriteEndTag("body");
				htmlWriter.WriteEndTag("html");

				htmlWriter.Close();
			}
#endif

			return result;
		}

		private bool AreAssociated(int st, int tt, List<Core.Pair<int>> precomputedAssociations)
		{
			return GetSourcePreassignment(st, precomputedAssociations) == tt;
		}

		/// <summary>
		/// Returns the target item position if the source tag st is preassigned, and -1 otherwise.
		/// </summary>
		private int GetSourcePreassignment(int st, List<Core.Pair<int>> precomputedAssociations)
		{
			if (precomputedAssociations == null || precomputedAssociations.Count == 0)
				return -1;

			// TODO use binary search for longer lists

			foreach (Core.Pair<int> p in precomputedAssociations)
			{
				if (p.Left > st)
					return -1;
				if (p.Left == st)
					return p.Right;
			}
			return -1;
		}

		/// <summary>
		/// Returns the source item position if the target tag tt is preassigned, and -1 otherwise.
		/// </summary>
		private int GetTargetPreassignment(int tt, List<Core.Pair<int>> precomputedAssociations)
		{
			if (precomputedAssociations == null || precomputedAssociations.Count == 0)
				return -1;

			foreach (Core.Pair<int> p in precomputedAssociations)
			{
				if (p.Right == tt)
					return p.Left;
			}
			return -1;
		}

		private bool SortAndValidate(List<Core.Pair<int>> precomputedAssociations,
			int srcObjCount, int trgObjCount)
		{
			bool isValid = !precomputedAssociations.Any(p => 
				// TODO additional verification checks, i.e. positions must 
				//  be unique in the respective half
				p.Left < 0 || p.Left >= srcObjCount 
				|| p.Right < 0 || p.Right >= trgObjCount);

			if (isValid)
			{
				precomputedAssociations.Sort(delegate(Core.Pair<int> a, Core.Pair<int> b)
				{
					int r = a.Left - b.Left;
					if (r == 0)
						r = a.Right - b.Right;
					return r;
				});
			}

			return isValid;
		}

	}
}

