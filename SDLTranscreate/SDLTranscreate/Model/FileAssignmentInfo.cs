using System;
using System.Collections.Generic;

namespace Trados.Transcreate.Model
{
	public class FileAssignmentInfo
	{
		public FileAssignmentInfo()
		{
			Phase = string.Empty;
			AssignedAt = DateTime.MinValue;
			DueDate = DateTime.MaxValue;
			AssignedBy = string.Empty;
			IsCurrentAssignment = false;
			Assignees = new List<string>();
			FileStateInfo = new FileStateInfo();
		}

		/// <summary>
		/// Preparation, Translation, Review, Finalisation
		/// </summary>
		public string Phase { get; set; }

		public DateTime AssignedAt { get; set; }

		public DateTime DueDate { get; set; }

		public string AssignedBy { get; set; }

		public bool IsCurrentAssignment { get; set; }

		public List<string> Assignees { get; set; }

		public FileStateInfo FileStateInfo { get; set; }
	}
}
