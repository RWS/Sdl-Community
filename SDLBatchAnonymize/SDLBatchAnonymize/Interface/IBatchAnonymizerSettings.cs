namespace Sdl.Community.SDLBatchAnonymize.Interface
{
	public interface IBatchAnonymizerSettings
	{
		bool AnonymizeComplete { get; set; }
		bool AnonymizeTmMatch { get; set; }
		decimal FuzzyScore { get; set; }
		string TmName { get; set; }
	}
}
