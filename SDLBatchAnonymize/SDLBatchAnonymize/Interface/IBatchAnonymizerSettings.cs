namespace Sdl.Community.SDLBatchAnonymize.Interface
{
	public interface IBatchAnonymizerSettings
	{
		bool AnonymizeComplete { get; set; }
		bool ChangeMtChecked { get; set; }
		bool ChangeTmChecked { get; set; }
		bool ClearSettings { get; set; }
		string CommentAuthorName { get; set; }
		bool CommentChecked { get; set; }
		bool CreatedByChecked { get; set; }
		string CreatedByName { get; set; }
		decimal FuzzyScore { get; set; }
		bool ModifyByChecked { get; set; }
		string ModifyByName { get; set; }
		bool RemoveMtCloudMetadata { get; set; }
		bool SetSpecificResChecked { get; set; }
		string TmName { get; set; }
		bool TrackedChecked { get; set; }
		string TrackedName { get; set; }
		bool UseGeneral { get; set; }
	}
}