using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Processors;
using Sdl.Community.NumberVerifier.Processors.CompositionProcessors;
using Sdl.Community.NumberVerifier.Processors.LocalizationProcessors;
using Sdl.Community.NumberVerifier.Specifications;
using Sdl.Community.NumberVerifier.Specifications.LocalizationSpecification;

namespace Sdl.Community.NumberVerifier.Composers
{
    public class TargetDecimalSeparatorsExtractComposer
    {
        public IExtractProcessor Compose()
        {

            return new ApostrophSeparatorsCompositeExtractProcessor
            {
                Nodes = new IExtractProcessor[]
                {
                    new UniqueSeparatorsCompositeExtractProcessor
                    {
                        Nodes = new IExtractProcessor[]
                        {

                            new ConditionalSeparatorsExtractProcessor
                            {
                                Specification = new AllowLocalizationSpecification(),
                                TruthProcessor = new DecimalAllowLocalizationExtractProcessor()

                            },
                            new ConditionalSeparatorsExtractProcessor
                            {
                                Specification = new RequireLocalizationSpecification(),
                                TruthProcessor = new TargetDecimalRequireLocalizationExtractProcessor()

                            },
                            new ConditionalSeparatorsExtractProcessor
                            {
                                Specification = new PreventLocalizationSpecification(),
                                TruthProcessor = new DecimalPreventLocalizationExtractProcessor()

                            }
                        }
                    }
                }
            };
        }
    }
}
