using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Processors;
using Sdl.Community.NumberVerifier.Processors.CompositionProcessors;
using Sdl.Community.NumberVerifier.Specifications;

namespace Sdl.Community.NumberVerifier.Composers
{
    public class NumberErrorComposer
    {
        public IVerifyProcessor Compose()
        {
            return new CompositeVerifyProcessor
            {
                Nodes = new IVerifyProcessor[]
                {
                    //processor for "Modified numbers" error
                    new ConditionalVerifyProcessor
                    {
                        Specification = new AndVerifySpecification
                        {
                            Specifications = new IVerifySpecification[]
                            {
                                new NumberExistsInSourceSpecification(),
                                new NumberExistsInTargetSpecification(),
                                new ReportModifiedNumbersSpecification()
                            }
                        },
                        TruthProcessor = new VerifyProcessor()
                        {
                            ErrorMessage = PluginResources.Error_NumbersNotIdentical
                        }
                    },

                    //processor for "Added numbers" error
                    new ConditionalVerifyProcessor
                    {
                        Specification = new AndVerifySpecification
                        {
                            Specifications = new IVerifySpecification[]
                            {
                                new NumberAddedSpecification(),
                                new ReportAddedNumbersSpecification()
                            }
                        },
                        TruthProcessor = new VerifyProcessor()
                        {
                            ErrorMessage = PluginResources.Error_NumbersAdded
                        }
                    },

                    //processor for "Removed numbers" error
                    new ConditionalVerifyProcessor
                    {
                        Specification = new AndVerifySpecification
                        {
                            Specifications = new IVerifySpecification[]
                            {
                                new NumberRemovedSpecification(),
                                new ReportRemovedNumbersSpecification()
                            }
                        },
                        TruthProcessor = new VerifyProcessor
                        {
                            ErrorMessage = PluginResources.Error_NumbersRemoved
                        }
                    }
                }
            };
        }
    }
}
