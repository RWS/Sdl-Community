using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	/// <summary>
	/// Represents an alignment between ranges in a source and target sequence.
	/// </summary>
	public struct AlignmentItem
	{
		/// <summary>
		/// The type of alignment between the ranges.
		/// </summary>
		public AlignmentOperation Op;
		/// <summary>
		/// The start index of the alignment in the source sequence. Depending on the 
		/// alignment type, this value may be invalid or undefined.
		/// </summary>
		public int SourceFrom;
		/// <summary>
		/// The end index of the alignment in the source sequence (exclusive). Depending on the 
		/// alignment type, this value may be invalid or undefined.
		/// </summary>
		public int SourceUpto;
		/// <summary>
		/// The start index of the alignment in the target sequence. Depending on the 
		/// alignment type, this value may be invalid or undefined.
		/// </summary>
		public int TargetFrom;
		/// <summary>
		/// The end index of the alignment in the target sequence (exclusive). Depending on the 
		/// alignment type, this value may be invalid or undefined.
		/// </summary>
		public int TargetUpto;
	}

	/// <summary>
	/// Implements a simple, generic cost-based alignment computer with externally specified alignment costs
	/// for the basic alignment operations.
	/// </summary>
	/// <typeparam name="T">The type of elements to be aligned.</typeparam>
	public class CostBasedAlignmentComputer<T>
	{
		// TODO consider a "streaming model" where the matrix is successively filled
		//  in the lower right corner and emptied in the top left corner (research)
		// TODO factor out interface, i.e. a SimpleAligner and a StreamingAligner etc.
		// TODO try to first fill the diagonal or find fix points so that we don't drown
		//  in large matrices
		// TODO define alignment window and fix points

		private IAlignmentCostComputer<T> _AlignmentCostComputer;

		/// <summary>
		/// Constructs a new instance with the specified cost computer.
		/// </summary>
		/// <param name="alignmentCostComputer">The cost computer</param>
		public CostBasedAlignmentComputer(IAlignmentCostComputer<T> alignmentCostComputer)
		{
			if (alignmentCostComputer == null)
				throw new ArgumentNullException();
			_AlignmentCostComputer = alignmentCostComputer;
		}

		private struct Cell
		{
			public int Cost;
			public AlignmentOperation Op;
		}

		/// <summary>
		/// Computes the alignment between the two sequences, using the cost computer
		/// used in the constructor.
		/// </summary>
		/// <param name="srcItems">The non-empty sequence of source items.</param>
		/// <param name="trgItems">The non-empty sequence of target items.</param>
		/// <returns>An alignment between the input sequences.</returns>
		public List<AlignmentItem> Align(IList<T> srcItems, IList<T> trgItems)
		{
			return Align(srcItems, trgItems, null);
		}

		private List<AlignmentItem> Align(IList<T> srcItems, IList<T> trgItems,
			List<AlignmentItem> fixpoints)
		{
			int srcCnt = srcItems.Count;
			int trgCnt = trgItems.Count;
			int s;
			int t;

			Cell[,] matrix = new Cell[srcCnt + 1, trgCnt + 1];

			if (fixpoints != null)
			{
				// mark fixpoints in the alignment and insert into matrix as substitutions

				foreach (AlignmentItem p in fixpoints)
				{
					if (p.SourceFrom < 0 || p.SourceFrom >= srcCnt)
						throw new ArgumentOutOfRangeException("fixpoint out of range");
					if (p.SourceUpto < 0 || p.SourceUpto > srcCnt || p.SourceUpto <= p.SourceFrom)
						throw new ArgumentOutOfRangeException("fixpoint out of range");
					if (p.TargetFrom < 0 || p.TargetFrom >= srcCnt)
						throw new ArgumentOutOfRangeException("fixpoint out of range");
					if (p.TargetUpto < 0 || p.TargetUpto > srcCnt || p.TargetUpto <= p.TargetFrom)
						throw new ArgumentOutOfRangeException("fixpoint out of range");

					// mark all rows/colums with fix points as 'fixed' and insert the fix point
					for (s = 0; s < srcCnt; ++s)
					{
						for (t = 0; t < trgCnt; ++t)
						{
							if (s >= p.SourceFrom && s < p.SourceUpto
								&& t >= p.TargetFrom && t < p.TargetUpto)
							{
								matrix[s + 1, t + 1].Op = AlignmentOperation.Substitute;
								matrix[s + 1, t + 1].Cost = _AlignmentCostComputer.GetSubstitutionCosts(srcItems[s], trgItems[t]);
							}
							else
							{
								matrix[s + 1, t + 1].Op = AlignmentOperation.Invalid;
								// cost don't matter in this case since the point will never be in 
								//  an alignment path
							}
						}
					}
				}
			}

			Cell c;
			c.Cost = 0;
			c.Op = AlignmentOperation.None;
			matrix[0, 0] = c;

			// TODO only compute "near" diagonal

			for (t = 0; t <= trgCnt; ++t)
			{
				for (s = 0; s <= srcCnt; ++s)
				{
					// skip preassigned (forbidden) cells
					if (matrix[s, t].Op != AlignmentOperation.None)
						continue;

					int minCosts = int.MaxValue;

					AlignmentOperation minOp = AlignmentOperation.None;
					int costs = minCosts;

					if (s == 0 && t == 0)
					{
						costs = minCosts = 0;
						minOp = AlignmentOperation.None;
					}

					if (s > 0 && t > 0 && matrix[s - 1, t - 1].Op != AlignmentOperation.Invalid)
					{
						costs = matrix[s - 1, t - 1].Cost + _AlignmentCostComputer.GetSubstitutionCosts(srcItems[s - 1], trgItems[t - 1]);
						if (costs < minCosts)
						{
							minCosts = costs;
							minOp = AlignmentOperation.Substitute;
						}
					}

					if (s > 0 && matrix[s - 1, t].Op != AlignmentOperation.Invalid)
					{
						costs = matrix[s - 1, t].Cost + _AlignmentCostComputer.GetDeletionCosts(srcItems[s - 1]);
						if (costs < minCosts)
						{
							minCosts = costs;
							minOp = AlignmentOperation.Delete;
						}
					}

					if (t > 0 && matrix[s, t - 1].Op != AlignmentOperation.Invalid)
					{
						costs = matrix[s, t - 1].Cost + _AlignmentCostComputer.GetInsertionCosts(trgItems[t - 1]);
						if (costs < minCosts)
						{
							minCosts = costs;
							minOp = AlignmentOperation.Insert;
						}
					}

					if (s > 1 && t > 0 && matrix[s - 2, t - 1].Op != AlignmentOperation.Invalid)
					{
						costs = matrix[s - 2, t - 1].Cost + _AlignmentCostComputer.GetContractionCosts(srcItems[s - 2], srcItems[s - 1], trgItems[t - 1]);
						if (costs < minCosts)
						{
							minCosts = costs;
							minOp = AlignmentOperation.Contract;
						}
					}

					if (s > 0 && t > 1 && matrix[s - 1, t - 2].Op != AlignmentOperation.Invalid)
					{
						costs = matrix[s - 1, t - 2].Cost + _AlignmentCostComputer.GetExpansionCosts(srcItems[s - 1], trgItems[t - 2], trgItems[t - 1]);
						if (costs < minCosts)
						{
							minCosts = costs;
							minOp = AlignmentOperation.Expand;
						}
					}

					if (s > 1 && t > 1 && matrix[s - 2, t - 2].Op != AlignmentOperation.Invalid)
					{
						costs = matrix[s - 2, t - 2].Cost + _AlignmentCostComputer.GetMeldingCosts(srcItems[s - 2], srcItems[s - 1], trgItems[t - 2], trgItems[t - 1]);
						if (costs < minCosts)
						{
							minCosts = costs;
							minOp = AlignmentOperation.Merge;
						}
					}

					c.Cost = minCosts;
					c.Op = minOp;
					matrix[s, t] = c;
				}
			}

#if DEBUG
			bool dumpMatrix = false;
			if (dumpMatrix)
			{
				using (System.IO.TextWriter wtr = new System.IO.StreamWriter("D:/temp/alignment-matrix.html", false, System.Text.Encoding.UTF8))
				{
					wtr.WriteLine("<html><head><title>Alignment Matrix</title></head><body>");
					wtr.WriteLine("<h1>Alignment Matrix</h1><hr>");

					wtr.WriteLine("<h2>Source items</h2><ol>");
					foreach (T item in srcItems)
						wtr.WriteLine("<li> {0}", item);
					wtr.WriteLine("</ol>");

					wtr.WriteLine("<h2>Target items</h2><ol>");
					foreach (T item in trgItems)
						wtr.WriteLine("<li> {0}", item);
					wtr.WriteLine("</ol>");

					wtr.WriteLine("<h2>Alignment</h2><table border='1'>");

					// header line
					wtr.Write("<tr><td>&nbsp;</td><td>&nbsp;</td>");
					for (t = 1; t <= trgCnt; ++t)
					{
						string text = trgItems[t - 1].ToString();
						if (text.Length > 7)
						{
							text = text.Substring(0, 5) + "...";
						}
						wtr.WriteLine("<td>{0}</td>", text);
					}
					wtr.Write("</tr>");

					for (s = 0; s <= srcCnt; ++s)
					{
						wtr.WriteLine("<tr>");

						{
							string text = s > 0 ? srcItems[s - 1].ToString() : "&nbsp;";
							if (text.Length > 7)
							{
								text = text.Substring(0, 5) + "...";
							}
							wtr.WriteLine("<td>{0}</td>", text);
						}

						for (t = 0; t <= trgCnt; ++t)
						{
							wtr.Write("<td>{0}<br>{1}</td>", matrix[s, t].Op, matrix[s, t].Cost);
						}

						wtr.WriteLine("</tr>");
					}

					wtr.WriteLine("</table></body></html");
				}
			}

#endif

			// readout

			if (fixpoints != null)
			{
				// TODO Alignment path readout with fixpoints
				throw new NotImplementedException("Alignment path readout does not yet work with fixpoints");

				/*
				 * algorithm: find rectangles bound by fixpoints and align within those rectangles only
				 * */
			}

			List<AlignmentItem> result = new List<AlignmentItem>();

			s = srcCnt;
			t = trgCnt;

			while (s > 0 || t > 0)
			{
				AlignmentItem r;

				r.Op = matrix[s, t].Op;
				r.SourceUpto = s;
				r.TargetUpto = t;

				switch (matrix[s, t].Op)
				{
				case AlignmentOperation.Substitute:
					--s;
					--t;
					break;
				case AlignmentOperation.Insert:
					--t;
					break;
				case AlignmentOperation.Delete:
					--s;
					break;
				case AlignmentOperation.Contract:
					s -= 2;
					t -= 1;
					break;
				case AlignmentOperation.Expand:
					--s;
					t -= 2;
					break;
				case AlignmentOperation.Merge:
					s -= 2;
					t -= 2;
					break;
				case AlignmentOperation.None:
				case AlignmentOperation.Invalid:
				default:
					throw new Exception("Unexpected");
				}

				r.SourceFrom = s;
				r.TargetFrom = t;

				result.Insert(0, r);
			}

			return result;
		}
	}
}
