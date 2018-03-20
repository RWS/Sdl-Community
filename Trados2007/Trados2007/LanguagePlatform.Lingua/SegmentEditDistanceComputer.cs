using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core;
using System.Linq;

namespace Sdl.LanguagePlatform.Lingua
{
	public class SegmentEditDistanceComputer
	{
		/// <summary>
		/// Costs of inserting or deleting a word
		/// </summary>
		private static readonly double _InsertDeleteCosts = 1.0d;
		/// <summary>
		/// Costs of moving a word somewhere else in the string
		/// </summary>
		private static readonly double _MoveCosts = 1.1d; // should be slightly higher than I/D costs
        ///// <summary>
        ///// Additional costs of moving a word somewhere else in the string
        ///// </summary>
        //private static readonly double _BlockMoveCostSurcharge = 0.1d;
		/// <summary>
		/// Similarity threshold at which a move operation may be triggered, instead of an 
		/// insert/delete pair of operations
		/// </summary>
		// NOTE if this is set to a value < 1, changed items may become part of a move block
		//  and will need to be treated separately. 
		private static readonly double _MoveSimThreshold = 0.95d;

		private static readonly double _InvalidAssignmentCosts = 100000.0d;
			
		/// <summary>
		/// Controls whether to apply string edit distance during computation of the similarity matrix
		/// (has performance impact).
		/// </summary>
		private static readonly bool _UseStringEditDistance = false;

		private class PositionRange
		{
			public PositionRange(int start, int into)
			{
				Start = start;
				Into = into;
			}

			public int Start;
			public int Into;
		}

		private bool _ComputeMoves;

		public SegmentEditDistanceComputer()
		{
			_ComputeMoves = true;
		}

		public SegmentEditDistanceComputer(bool computeMoves)
		{
			_ComputeMoves = computeMoves;
		}

		/// <summary>
		/// Computes the ED
		/// </summary>
		/// <param name="sourceTokens"></param>
		/// <param name="targetTokens"></param>
		/// <param name="computeDiagonalOnly">If number of tokens is equivalent, only the diagonal's similarities are computed.</param>
		/// <param name="alignedTags"></param>
		/// <returns></returns>
		public Core.EditDistance.EditDistance ComputeEditDistance(
			IList<Core.Tokenization.Token> sourceTokens,
			IList<Core.Tokenization.Token> targetTokens, 
			bool computeDiagonalOnly,
			Core.Tokenization.BuiltinRecognizers disabledAutoSubstitutions,
			out TagAssociations alignedTags)
		{
			Core.EditDistance.EditDistance result = null;

			result = ComputeEditDistanceImpl_Original(sourceTokens, targetTokens, computeDiagonalOnly, disabledAutoSubstitutions, out alignedTags);

#if DEBUG
			{
				bool ok = VerifyEditDistance(result.Items, sourceTokens.Count, targetTokens.Count);
				if (!ok)
					System.Diagnostics.Debug.Assert(ok, "ED error - let Oli know and provide test data");
			}
#endif
			return result;
		}

#if false
#region unused
		private EditDistance ComputeEditDistance(double[,] sim,
			PositionRange srcRange,
			PositionRange trgRange)
		{
			const double invalidAssignmentCosts = 100000.0d;

			// TODO handle special cases (one/both of the arrays being empty/having no elements)
			// TODO use diagonal algorithm

			int i;
			int j;

			int sourceObjectsCount = srcRange.Into - srcRange.Start + 1;
			int targetObjectsCount = trgRange.Into - trgRange.Start + 1;

			EditDistance result = new EditDistance(sourceObjectsCount, targetObjectsCount, 0.0d);

			MatrixItem[,] matrix = new MatrixItem[sourceObjectsCount + 1, targetObjectsCount + 1];

			// initialize matrix
			matrix[0, 0] = new MatrixItem(0.0d, EditOperation.Identity, 0.0d);

			for (i = 1; i <= sourceObjectsCount; ++i)
				matrix[i, 0] = new MatrixItem((double)i * _InsertDeleteCosts, EditOperation.Delete, 0.0d);

			for (j = 1; j <= targetObjectsCount; ++j)
				matrix[0, j] = new MatrixItem((double)j * _InsertDeleteCosts, EditOperation.Insert, 0.0d);

			for (i = 1; i <= sourceObjectsCount; ++i)
				for (j = 1; j <= targetObjectsCount; ++j)
					matrix[i, j] = new MatrixItem(0.0d, EditOperation.Identity, 0.0d);

			// populate edit distance matrix

			for (i = 1; i <= sourceObjectsCount; ++i)
			{
				for (j = 1; j <= targetObjectsCount; ++j)
				{
					double similarity = sim[srcRange.Start + i - 1, trgRange.Start + j - 1];

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
						matrix[i, j].Operation = EditOperation.Delete;
					}
					else if (min == insertCosts)
					{
						matrix[i, j].Operation = EditOperation.Insert;
					}
					else if (min == changeCosts)
					{
						if (similarity == 1.0d)
							matrix[i, j].Operation = EditOperation.Identity;
						else
							matrix[i, j].Operation = EditOperation.Change;
					}
				}
			}

			// readout the cheapest path

			i = sourceObjectsCount;
			j = targetObjectsCount;

			// TODO we may rather need to find the end point at the borders
			result.Distance += matrix[i, j].Score;

			while (i > 0 || j > 0)
			{
				EditDistanceItem item = new EditDistanceItem();
				item.Resolution = EditDistanceResolution.None;

				MatrixItem m = matrix[i, j];

				item.Operation = m.Operation;
				switch (item.Operation)
				{
				case EditOperation.Identity:
					item.Costs = 0.0d;
					--i;
					--j;
					break;
				case EditOperation.Change:
					item.Costs = (1.0d - m.Similarity);
					--i;
					--j;
					break;
				case EditOperation.Insert:
					item.Costs = _InsertDeleteCosts;
					--j;
					break;
				case EditOperation.Delete:
					item.Costs = _InsertDeleteCosts;
					--i;
					break;
				}

				System.Diagnostics.Debug.Assert(i >= 0 && j >= 0);

				System.Diagnostics.Debug.Assert(sourceObjectsCount == 0
					|| item.Operation == EditOperation.Insert
					|| i < sourceObjectsCount);

				System.Diagnostics.Debug.Assert(targetObjectsCount == 0
					|| item.Operation == EditOperation.Delete
					|| j < targetObjectsCount);

				// TODO shift only for certain ops?
				item.Source = srcRange.Start + i;
				item.Target = trgRange.Start + j;

				result.AddAtStart(item);
			}

			return result;
		}
#endregion
#endif

		/// <summary>
		/// Verifies that no ED position is used twice, and all positions in the input vector are covered
		/// </summary>
		private bool VerifyEditDistance(IList<EditDistanceItem> ed,
			int sourceObjectCount,
			int targetObjectCount)
		{
			bool[] srcCovered = new bool[sourceObjectCount];
			bool[] trgCovered = new bool[targetObjectCount];

			foreach (EditDistanceItem item in ed)
			{
				switch (item.Operation)
				{
				case EditOperation.Identity:
				case EditOperation.Change:
				case EditOperation.Move:
					if (srcCovered[item.Source])
						return false;
					if (trgCovered[item.Target])
						return false;
					srcCovered[item.Source] = true;
					trgCovered[item.Target] = true;
					break;
				case EditOperation.Insert:
					if (trgCovered[item.Target])
						return false;
					trgCovered[item.Target] = true;
					break;
				case EditOperation.Delete:
					if (srcCovered[item.Source])
						return false;
					srcCovered[item.Source] = true;
					break;
				default:
					throw new Exception("Unexpected case");
				}
			}

			for (int p = 0; p < sourceObjectCount; ++p)
				if (!srcCovered[p])
					return false;
			for (int p = 0; p < targetObjectCount; ++p)
				if (!trgCovered[p])
					return false;

			return true;
		}

		private void ComputeEditDistanceMatrix_Full(MatrixItem[,] matrix,
			SimilarityMatrix sim,
			TagAssociations alignedTags)
		{
			for (int i = 1; i <= sim.SourceTokens.Count; ++i)
			{
				for (int j = 1; j <= sim.TargetTokens.Count; ++j)
				{
					// current cell must not yet be computed:
					System.Diagnostics.Debug.Assert(matrix[i, j].Operation == EditOperation.Undefined);
					// predecessors must be valid:
					System.Diagnostics.Debug.Assert(matrix[i - 1, j - 1].Operation != EditOperation.Undefined);
					System.Diagnostics.Debug.Assert(matrix[i, j - 1].Operation != EditOperation.Undefined);
					System.Diagnostics.Debug.Assert(matrix[i - 1, j].Operation != EditOperation.Undefined);

					double similarity = sim[i - 1, j - 1];

					System.Diagnostics.Debug.Assert((similarity >= 0.0d && similarity <= 1.0d)
						|| similarity == -1.0d);

					// low similarity means high "change costs" and vice versa:
					double changeCosts = (similarity < 0)
						? _InvalidAssignmentCosts
						: matrix[i - 1, j - 1].Score + (1.0d - similarity);

					double insertCosts = matrix[i, j - 1].Score + _InsertDeleteCosts;
					double deleteCosts = matrix[i - 1, j].Score + _InsertDeleteCosts;

					double min = Math.Min(Math.Min(changeCosts, deleteCosts), insertCosts);

					// verify the shortcut condition:
					System.Diagnostics.Debug.Assert(similarity < 1.0d || min == changeCosts);

					EditOperation op = EditOperation.Undefined;
					if (min == deleteCosts)
					{
						op = EditOperation.Delete;
					}
					else if (min == insertCosts)
					{
						op = EditOperation.Insert;
					}
					else if (min == changeCosts)
					{
						if (similarity == 1.0d)
							op = EditOperation.Identity;
						else
							op = EditOperation.Change;
					}

					if (alignedTags != null && alignedTags.Count > 0)
					{
						// check whether tag alignment overrides ED result:
						// TODO do this during population or during readout?

						EditOperation srcTagOp = alignedTags.GetOperationBySourcePosition(i - 1);
						EditOperation trgTagOp = alignedTags.GetOperationByTargetPosition(j - 1);

						// changes/identity of tags are through ED, while the tag alignment 
						//  defines deletions, insertions
						if ((srcTagOp == EditOperation.Insert || srcTagOp == EditOperation.Delete)
							&& op != srcTagOp)
						{
							// this is where the pre-alignment of tags supersedes the ED result
							op = srcTagOp;
						}
						else if ((trgTagOp == EditOperation.Insert || trgTagOp == EditOperation.Delete)
							&& op != trgTagOp)
						{
							op = trgTagOp;
						}
					}

					matrix[i, j].Similarity = similarity;
					matrix[i, j].Operation = op;

					if (op == EditOperation.Delete)
					{
						matrix[i, j].Score = deleteCosts;
					}
					else if (op == EditOperation.Insert)
					{
						matrix[i, j].Score = insertCosts;
					}
					else
					{
						matrix[i, j].Score = changeCosts;
					}
				}
			}
		}

		private EditOperation GetOperation(double changeCosts, double insertCosts, double deleteCosts, double similarity)
		{
			double min = Math.Min(changeCosts, Math.Min(insertCosts, deleteCosts));
			if (min == changeCosts)
			{
				return similarity == 1.0d ? EditOperation.Identity : EditOperation.Change;
			}
			else if (min == deleteCosts)
				return EditOperation.Delete;
			else
				return EditOperation.Insert;
		}

		private void ComputeCell(MatrixItem[,] matrix,
			SimilarityMatrix sim, int i, int j)
		{
			if (matrix[i, j].Operation != EditOperation.Undefined)
				// cell already computed - no further processing required
				return;

			// ensure that the diagonal cell is computed (always needed)
			ComputeCell(matrix, sim, i - 1, j - 1);
			System.Diagnostics.Debug.Assert(matrix[i - 1, j - 1].Operation != EditOperation.Undefined);

			double similarity = sim[i - 1, j - 1];
			// low similarity means high "change costs" and vice versa:
			double changeCosts = (similarity < 0.0d)
				? _InvalidAssignmentCosts
				: matrix[i - 1, j - 1].Score + (1.0d - similarity);

			EditOperation op = EditOperation.Undefined;
			double insertCosts = 0.0d;
			double deleteCosts = 0.0d;

			// i == j: main diagonal. i < j: below, i > j: above.

			/*
			if (similarity == 1.0d)
			{
				// this seems to assume that the costs are minimal in the diagonal - not
				//  sure that's true in the general case (only if insert/delete/change
				//  costs are equal = 1)
				op = EditOperation.Identity;
			}
			else 
			 */
			if (i < j)
			{
				// below main diagonal.
				ComputeCell(matrix, sim, i, j - 1);
				insertCosts = matrix[i, j - 1].Score + _InsertDeleteCosts;
				if (insertCosts >= changeCosts || changeCosts == _InvalidAssignmentCosts)
				{
					// need to get the deletion costs as well
					ComputeCell(matrix, sim, i - 1, j);
					deleteCosts = matrix[i - 1, j].Score + _InsertDeleteCosts;

					op = GetOperation(changeCosts, insertCosts, deleteCosts, similarity);
				}
				else
				{
					if (insertCosts < changeCosts)
						op = EditOperation.Insert;
					else
						op = EditOperation.Change;
				}
			}
			else
			{
				// on or above main diagonal
				ComputeCell(matrix, sim, i - 1, j);
				deleteCosts = matrix[i - 1, j].Score + _InsertDeleteCosts;

				if (deleteCosts >= changeCosts || changeCosts == _InvalidAssignmentCosts)
				{
					// need to get the insert costs as well
					ComputeCell(matrix, sim, i, j - 1);
					insertCosts = matrix[i, j - 1].Score + _InsertDeleteCosts;

					op = GetOperation(changeCosts, insertCosts, deleteCosts, similarity);
				}
				else
				{
					if (deleteCosts < changeCosts)
						op = EditOperation.Delete;
					else
						op = EditOperation.Change;
				}
			}

			matrix[i, j].Similarity = similarity;
			matrix[i, j].Operation = op;

			System.Diagnostics.Debug.Assert(op != EditOperation.Undefined);

			if (op == EditOperation.Delete)
			{
				matrix[i, j].Score = deleteCosts;
			}
			else if (op == EditOperation.Insert)
			{
				matrix[i, j].Score = insertCosts;
			}
			else
			{
				matrix[i, j].Score = changeCosts;
			}
		}

		private void ComputeEditDistanceMatrix_Lazy(MatrixItem[,] matrix,
			SimilarityMatrix sim)
		{
			ComputeCell(matrix, sim, sim.SourceTokens.Count, sim.TargetTokens.Count);
		}

		/// <summary>
		/// Creates a new edit distance matrix, and initializes the border elements.
		/// </summary>
		private MatrixItem[,] CreateEditDistanceMatrix(IList<Core.Tokenization.Token> sourceTokens,
			IList<Core.Tokenization.Token> targetTokens)
		{
			MatrixItem[,] matrix = new MatrixItem[sourceTokens.Count + 1, targetTokens.Count + 1];

			int i;
			int j;

			matrix[0, 0] = new MatrixItem(0.0d, EditOperation.Identity, 0.0d);

			for (i = 1; i <= sourceTokens.Count; ++i)
				matrix[i, 0] = new MatrixItem((double)i * _InsertDeleteCosts, EditOperation.Delete, 0.0d);

			for (j = 1; j <= targetTokens.Count; ++j)
				matrix[0, j] = new MatrixItem((double)j * _InsertDeleteCosts, EditOperation.Insert, 0.0d);

			for (i = 1; i <= sourceTokens.Count; ++i)
				for (j = 1; j <= targetTokens.Count; ++j)
					matrix[i, j] = new MatrixItem(0.0d, EditOperation.Undefined, 0.0d);

			return matrix;
		}

		private Core.EditDistance.EditDistance ComputeEditDistanceImpl_Original(
			IList<Core.Tokenization.Token> sourceTokens,
			IList<Core.Tokenization.Token> targetTokens,
			bool computeDiagonalOnly,
			Core.Tokenization.BuiltinRecognizers disabledAutoSubstitutions,
			out TagAssociations alignedTags)
		{
			/*
			 * The "classic" ED approach has the problem that it doesn't detect moves
			 * reliably, particularly block moves. Patching up insert/delete pairs as 
			 * moves also won't catch moves which appear as changes in the ED.
			 */

			if (sourceTokens == null)
				throw new ArgumentNullException("sourceTokens");
			if (targetTokens == null)
				throw new ArgumentNullException("targetTokens");

			alignedTags = null;

			int i, j;

			// TODO handle special cases (one/both of the arrays being empty/having no elements)
			// TODO use diagonal algorithm

			bool enforceFullMatrixComputation = false;

			Core.EditDistance.EditDistance result =
				new Core.EditDistance.EditDistance(sourceTokens.Count, targetTokens.Count, 0.0d);

			// matrix which captures the similarity between two tokens as well as preassignments
			SimilarityMatrix sim = new SimilarityMatrix(sourceTokens, targetTokens, 
				_UseStringEditDistance, disabledAutoSubstitutions);
			if (enforceFullMatrixComputation)
			{
				// this will be fully computed by the tag aligner in most cases, but we may save a bit
				// on plain text segments
				sim.Compute(computeDiagonalOnly);
			}

			MatrixItem[,] matrix = CreateEditDistanceMatrix(sourceTokens, targetTokens);

			alignedTags = TagAligner.AlignPairedTags(sourceTokens, targetTokens, sim);
			if (alignedTags != null && alignedTags.Count > 0)
			{
				// Patch the sim matrix so that non-aligned tags can't be assigned to each other
				PatchSimilarityMatrix(sim, sourceTokens, targetTokens, alignedTags);
				ComputeEditDistanceMatrix_Full(matrix, sim, alignedTags);
			}
			else if (enforceFullMatrixComputation)
			{
				ComputeEditDistanceMatrix_Full(matrix, sim, alignedTags);
			}
			else
			{
				ComputeEditDistanceMatrix_Lazy(matrix, sim);
			}

			// readout the cheapest path

			i = sourceTokens.Count;
			j = targetTokens.Count;
			result.Distance = matrix[i, j].Score;

			while (i > 0 || j > 0)
			{
				EditDistanceItem item = new EditDistanceItem();
				item.Resolution = EditDistanceResolution.None;

				MatrixItem m = matrix[i, j];

				item.Operation = m.Operation;

				switch (item.Operation)
				{
				case EditOperation.Identity:
					item.Costs = 0.0d;
					--i;
					--j;
					break;
				case EditOperation.Change:
					item.Costs = _UseStringEditDistance
						? (1.0d - m.Similarity)
						: (1.0d - SimilarityComputers.GetTokenSimilarity(sourceTokens[i - 1], targetTokens[j - 1],
								true, disabledAutoSubstitutions));
					// item.Costs = (1.0d - m.Similarity);
					--i;
					--j;
					break;
				case EditOperation.Insert:
					item.Costs = _InsertDeleteCosts;
					--j;
					break;
				case EditOperation.Delete:
					item.Costs = _InsertDeleteCosts;
					--i;
					break;
				case EditOperation.Undefined:
					throw new Exception("Internal ED computation error");
				}

				item.Source = i;
				item.Target = j;
				result.AddAtStart(item);
			}

			if (alignedTags != null && alignedTags.Count > 0)
			{
				// should happen before move detection
				FixTagActions(sourceTokens, targetTokens, result, alignedTags);
			}

			// identify move operations which are pairs of insert/delete operations in the shortest path.
			// Note that the comparision result is already in the matrix and we only care about identity.
			// TODO we may rather use a configurable threshold than identity (1.0) to catch move operations
			//  of sufficiently similar items (e.g. case-insensitive)

			if (_ComputeMoves)
			{
				int moves = DetectMoves(result, matrix);
				if (moves > 0)
				{
					// adjust score: substract moves * (deletionCosts + insertionCosts), add moves * moveCosts
					// TODO take moveDistance into account, i.e. penalty depends on distance? 
					result.Distance -= (double)moves * (2.0d * _InsertDeleteCosts);
					result.Distance += (double)moves * _MoveCosts;
				}
			}

#if DEBUG
			// a stream for logging. Will always be null in non-Debug builds
			System.IO.TextWriter logStream = null;
			bool log = false;
			if (log)
			{
				logStream = new System.IO.StreamWriter(System.IO.Path.GetTempPath() + "/ed.log",
					false, System.Text.Encoding.UTF8);

				logStream.WriteLine("Source objects:");
				for (int p = 0; p < sourceTokens.Count; ++p)
					logStream.WriteLine("\t{0}:\t{1}", p, sourceTokens[p].ToString());
				logStream.WriteLine();
				logStream.WriteLine("Target objects:");
				for (int p = 0; p < targetTokens.Count; ++p)
					logStream.WriteLine("\t{0}:\t{1}", p, targetTokens[p].ToString());
				logStream.WriteLine();
				logStream.WriteLine();

				if (alignedTags != null)
				{
					logStream.WriteLine("Tag Alignment:");
					foreach (TagAssociation ta in alignedTags)
					{
						logStream.WriteLine("\t{0}", ta.ToString());
					}
					logStream.WriteLine();
					logStream.WriteLine();
				}

				result.Dump(logStream, "Final ED");

				logStream.Close();
				logStream.Dispose();
				logStream = null;
			}
#endif

#if DEBUG

			// write matrix to a temp file in HTML format
			_DumpMatrix = false; //  typeof(T) != typeof(char);
			if (_DumpMatrix)
			{
				System.IO.StreamWriter wtr = new System.IO.StreamWriter(System.IO.Path.GetTempPath() + "/SimMatrix.html",
					false, System.Text.Encoding.UTF8);
				System.Web.UI.Html32TextWriter htmlWriter = new System.Web.UI.Html32TextWriter(wtr);

				htmlWriter.WriteFullBeginTag("html");
				htmlWriter.WriteFullBeginTag("body");
				htmlWriter.WriteBeginTag("table");
				htmlWriter.WriteAttribute("border", "1");

				for (j = -1; j <= targetTokens.Count; ++j)
				{
					htmlWriter.WriteFullBeginTag("tr");

					for (i = -1; i <= sourceTokens.Count; ++i)
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
									htmlWriter.Write(targetTokens[j - 1].ToString());
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
								htmlWriter.Write(sourceTokens[i - 1].ToString());
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

		/// <summary>
		/// Patch the similarity matrix so that tags which are not aligned can't be associated
		/// by the ED
		/// </summary>
		private void PatchSimilarityMatrix(SimilarityMatrix sim, 
			IList<Core.Tokenization.Token> srcTokens,
			IList<Core.Tokenization.Token> trgTokens,
			TagAssociations tagAlignment)
		{
			if (tagAlignment == null || tagAlignment.Count == 0)
				return;

			for (int s = 0; s < srcTokens.Count; ++s)
			{
				if (!(srcTokens[s] is Core.Tokenization.TagToken))
					// not a tag
					continue;

				Core.Tag st = ((Core.Tokenization.TagToken)srcTokens[s]).Tag;
				if (!(st.Type == TagType.Start || st.Type == TagType.End))
					// not a paired tag
					continue;

				for (int t = 0; t < trgTokens.Count; ++t)
				{
					if (sim.IsAssigned(s, t) && sim[s, t] < 0.0d)
						// invalid assignment anyway, no need to check further
						continue;

					if (!(trgTokens[t] is Core.Tokenization.TagToken))
					{
						// should't really be the case as then sim[s, t] < 0
						System.Diagnostics.Debug.Assert(false, "Shouldn't be");
						continue;
					}

					Core.Tag tt = ((Core.Tokenization.TagToken)trgTokens[t]).Tag;
					if (!(tt.Type == TagType.Start || tt.Type == TagType.End))
					{
						// should't really be the case as then sim[s, t] < 0
						System.Diagnostics.Debug.Assert(false, "Shouldn't be");
						continue;
					}

					if (!tagAlignment.AreAssociated(s, t))
					{
						sim[s, t] = -1.0d;
					}
				}
			}
		}

		/// <summary>
		/// If the tag alignment suggests action "Change", but the ED can't find this, 
		/// we need to patch the corresponding ED item for the corresponding start or end tag as well.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="tagAlignment"></param>
		private void FixTagActions(IList<Core.Tokenization.Token> sourceTokens,
			IList<Core.Tokenization.Token> targetTokens,
			Core.EditDistance.EditDistance result, TagAssociations tagAlignment)
		{
			// not yet working
			return;

			// NOTE a single ED item may suggest "D" or "I" for a tag. This does not necessarily conflict
			//  with a "C" suggested by the alignment, as a following ED item may suggest
			//  a compensating "I" or "D" which would result in a "M", which is still 
			//  compatible with the alignment's "C".

			TagAssociation srcAssoc = null;
			TagAssociation trgAssoc = null;

			foreach (EditDistanceItem edi in result.Items)
			{
				switch (edi.Operation)
				{
				case EditOperation.Identity:
				case EditOperation.Change:
					srcAssoc = tagAlignment.GetBySourcePosition(edi.Source);
					trgAssoc = tagAlignment.GetByTargetPosition(edi.Target);
					if (srcAssoc != null || trgAssoc != null)
					{
						// assignment is only valid between tags
						System.Diagnostics.Debug.Assert(srcAssoc != null && trgAssoc != null);
						// should also be the same association, otherwise incompatible tags are
						//  associated
						System.Diagnostics.Debug.Assert(object.ReferenceEquals(srcAssoc, trgAssoc));
						System.Diagnostics.Debug.Assert(srcAssoc.SourceTag != null && srcAssoc.TargetTag != null);

						if (edi.Source == srcAssoc.SourceTag.Start)
						{
							System.Diagnostics.Debug.Assert(srcAssoc.SourceTag.StartTagOperation == EditOperation.Undefined);
							System.Diagnostics.Debug.Assert(srcAssoc.TargetTag.StartTagOperation == EditOperation.Undefined);
							// source tag start position
							srcAssoc.SourceTag.StartTagOperation = edi.Operation;
							srcAssoc.TargetTag.StartTagOperation = edi.Operation;
						}
						else
						{
							System.Diagnostics.Debug.Assert(edi.Source == srcAssoc.SourceTag.End);
							System.Diagnostics.Debug.Assert(srcAssoc.SourceTag.EndTagOperation == EditOperation.Undefined);
							System.Diagnostics.Debug.Assert(srcAssoc.TargetTag.EndTagOperation == EditOperation.Undefined);

							srcAssoc.SourceTag.EndTagOperation = edi.Operation;
							srcAssoc.TargetTag.EndTagOperation = edi.Operation;
						}
					}
					break;

				case EditOperation.Insert:
					trgAssoc = tagAlignment.GetByTargetPosition(edi.Target);
					if (trgAssoc != null)
					{
						if (edi.Target == trgAssoc.TargetTag.Start)
						{
							System.Diagnostics.Debug.Assert(trgAssoc.TargetTag.StartTagOperation == EditOperation.Undefined);
							trgAssoc.TargetTag.StartTagOperation = edi.Operation;
						}
						else
						{
							System.Diagnostics.Debug.Assert(edi.Target == trgAssoc.TargetTag.End);
							System.Diagnostics.Debug.Assert(trgAssoc.TargetTag.EndTagOperation == EditOperation.Undefined);
							trgAssoc.TargetTag.EndTagOperation = edi.Operation;
						}
					}
					break;

				case EditOperation.Delete:
					srcAssoc = tagAlignment.GetBySourcePosition(edi.Source);
					if (srcAssoc != null)
					{
						if (edi.Source == srcAssoc.SourceTag.Start)
						{
							System.Diagnostics.Debug.Assert(srcAssoc.SourceTag.StartTagOperation == EditOperation.Undefined);
							srcAssoc.SourceTag.StartTagOperation = edi.Operation;
						}
						else
						{
							System.Diagnostics.Debug.Assert(edi.Source == srcAssoc.SourceTag.End);
							System.Diagnostics.Debug.Assert(srcAssoc.SourceTag.EndTagOperation == EditOperation.Undefined);
							srcAssoc.SourceTag.EndTagOperation = edi.Operation;
						}
					}
					break;
				case EditOperation.Move:
				case EditOperation.Undefined:
				default:
					throw new Exception("Unexpected case");
				}
			}

			// phase 2: detect conflicts 

			foreach (TagAssociation ta in tagAlignment)
			{
				EditOperation startOp = EditOperation.Undefined;
				EditOperation endOp = EditOperation.Undefined;

				if ((ta.SourceTag.StartTagOperation == EditOperation.Insert)
					&& (ta.TargetTag.StartTagOperation == EditOperation.Delete))
				{
					startOp = EditOperation.Move;
				}
				else if ((ta.SourceTag.StartTagOperation == EditOperation.Delete)
					&& (ta.TargetTag.StartTagOperation == EditOperation.Insert))
				{
					startOp = EditOperation.Move;
				}
				else if (ta.SourceTag.StartTagOperation == ta.TargetTag.StartTagOperation)
				{
					startOp = ta.SourceTag.StartTagOperation;
				}
				else
				{
					System.Diagnostics.Debug.Assert(false, "Conflicting start tag operations");
					startOp = EditOperation.Undefined;
				}

				if ((ta.SourceTag.EndTagOperation == EditOperation.Insert)
					&& (ta.TargetTag.EndTagOperation == EditOperation.Delete))
				{
					endOp = EditOperation.Move;
				}
				else if ((ta.SourceTag.EndTagOperation == EditOperation.Delete)
					&& (ta.TargetTag.EndTagOperation == EditOperation.Insert))
				{
					endOp = EditOperation.Move;
				}
				else if (ta.SourceTag.EndTagOperation == ta.TargetTag.EndTagOperation)
				{
					endOp = ta.SourceTag.EndTagOperation;
				}
				else
				{
					System.Diagnostics.Debug.Assert(false, "Conflicting end tag operations");
					endOp = EditOperation.Undefined;
				}

				if (startOp != endOp)
				{
					System.Diagnostics.Debug.Assert(false, "Conflicting tag actions");
				}
			}
		}

		private int DetectMoves(Core.EditDistance.EditDistance result, MatrixItem[,] matrix)
		{
			int moves = 0;

			// try to detect moves in case the move penalty is smaller than the sum of insert/delete penalties

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
						if (op == EditOperation.Delete
							&& result[comp].Operation == EditOperation.Insert
							&& matrix[result[index].Source + 1, result[comp].Target + 1].Similarity >= _MoveSimThreshold)
						{
							// source[result[index].Source] was deleted
							// target[result[comp].Target] was inserted
							moveSource = result[index].Source;
							moveSourceTarget = result[index].Target;

							moveTarget = result[comp].Target;
							moveTargetSource = result[comp].Source;
							break;
						}
						else if (op == EditOperation.Insert
							&& result[comp].Operation == EditOperation.Delete
							&& matrix[result[comp].Source + 1, result[index].Target + 1].Similarity >= _MoveSimThreshold)
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

			return moves;
		}

		private struct TokenHashes
		{
			public int TextHash;
			public int CaseInsensitiveTextHash;
			public int StemHash;
		}

		private TokenHashes[] ComputeTokenHashes(IList<Core.Tokenization.Token> sourceObjects)
		{
			TokenHashes[] result = new TokenHashes[sourceObjects.Count];
			for (int p = 0; p < sourceObjects.Count; ++p)
			{
				Core.Tokenization.Token t = sourceObjects[p];
				Core.Tokenization.SimpleToken st = t as Core.Tokenization.SimpleToken;

				TokenHashes th = new TokenHashes();

				th.TextHash = t.Text == null ? 0 : t.Text.GetHashCode();
				th.CaseInsensitiveTextHash = t.Text == null ? 0 : t.Text.ToLowerInvariant().GetHashCode();
				th.StemHash = (st == null || st.Stem == null) 
					? 0 
					: st.Stem.ToLowerInvariant().GetHashCode();

				result[p] = th;
			}
			return result;
		}

		private bool AreAssociated(int st, int tt, List<Core.Pair<int>> precomputedAssociations)
		{
			return GetAssociationBySource(st, precomputedAssociations) == tt;
		}

		/// <summary>
		/// Returns the target item position if the source tag st is preassigned, and -1 otherwise.
		/// </summary>
		private int GetAssociationBySource(int st, List<Core.Pair<int>> associations)
		{
			if (associations == null || associations.Count == 0)
				return -1;

			// TODO use binary search for longer lists

			foreach (Core.Pair<int> p in associations)
			{
				if (p.Left == st)
					return p.Right;
			}
			return -1;
		}

		/// <summary>
		/// Returns the source item position if the target tag tt is preassigned, and -1 otherwise.
		/// </summary>
		private int GetAssociationByTarget(int tt, List<Core.Pair<int>> associations)
		{
			if (associations == null || associations.Count == 0)
				return -1;

			foreach (Core.Pair<int> p in associations)
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

		private struct MatrixItem
		{
			public MatrixItem(double score, EditOperation op, double similarity)
			{
				Score = score;
				Operation = op;
				Similarity = similarity;
			}

			public double Score;
			public EditOperation Operation;
			public double Similarity;
		}

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

	}


}
