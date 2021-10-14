using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Interface;

namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser.Patterns
{
	public class IntegerNumber : AbstractPattern
    {
	    private Sequence Integer { get; } = new Sequence(
		    new Optional(new Text("-")),
		    new Choice(
			    new Character('0'),
			    new Choice(
				    new Sequence(
					    new Range('1', '9'),
					    new Many(
						    new Sequence(
							    new Character(' '),
							    new Many(
								    new Range('0', '9'), 3, 3)))),
				    new Sequence(
					    new Range('1', '9'),
					    new Many(new Range('0', '9'))))));

	    public override IMatch Match(TextToParse text) => Integer.Match(text);
    }
}
