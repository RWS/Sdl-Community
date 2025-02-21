﻿using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Processors;
using Sdl.Community.NumberVerifier.Processors.CompositionProcessors;
using Sdl.Community.NumberVerifier.Processors.LocalizationProcessors;
using Sdl.Community.NumberVerifier.Specifications.LocalizationSpecification;

namespace Sdl.Community.NumberVerifier.Composers
{
	public class SourceDecimalSeparatorsExtractComposer
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
                                TruthProcessor = new SourceDecimalRequireLocalizationExtractProcessor()

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
