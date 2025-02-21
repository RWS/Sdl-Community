using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Processors;
using Sdl.Community.NumberVerifier.Specifications;

namespace Sdl.Community.NumberVerifier.Composers
{
    public class TargetErrorMessageComposer
    {
        public IErrorMessageProcessor Compose()
        {
            return new ConditionalErrorMessageProcessor
            {
                Specification =
                    new OrVerifySpecification()
                    {
                        Specifications = new IVerifySpecification[]
                        {
                            new ReportAddedNumbersSpecification(),
                            new ReportRemovedNumbersSpecification(),
                            new ReportModifiedNumbersSpecification(),

                        }
                    },
                TruthProcessor = new AggregateTargetIssuesOnNewLineErrorMessageProcessor()
            };
        }
    }
}
