namespace Sdl.Community.SDLBatchAnonymize.Interface
{
	public interface IBatchAnonymizerSettings
	{
		bool AnonymizeComplete { get; set; }
		bool AnonymizeTmMatch { get; set; }
		string FuzzyScore { get; set; }
		string TmName { get; set; }
	}
}
