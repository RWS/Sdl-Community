using System;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Model
{
	public class Comment
    {	 
		public string Id { get; set;  }

	    public string Text { get; set; }

	    public string Author { get; set; }

		public string Version { get; set; }

		public DateTime Date { get; set; }	  
	}
}
