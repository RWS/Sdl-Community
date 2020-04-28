namespace Sdl.Community.SDLBatchAnonymize.Interface
{
	public interface IBatchAnonymizerSettings
	{
		bool AnonymizeComplete { get; set; }
		bool UseGeneral { get; set; }
		bool CreatedByChecked { get; set; }
		bool ModifyByChecked { get; set; }
		bool CommentChecked { get; set; }
		bool TrackedChecked { get; set; }
		bool ChangeMtChecked { get; set; }
		bool ChangeTmChecked { get; set; }
		bool SetSpecificResChecked { get; set; }
		bool ClearSettings { get; set; }
		string CreatedByName { get; set; }
		string ModifyByName { get; set; }
		string CommentAuthorName { get; set; }
		string TrackedName { get; set; }
		decimal FuzzyScore { get; set; }
		string TmName { get; set; }
	}
}
