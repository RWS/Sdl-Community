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
    public class AlphanumericErrorComposer
    {
        public IVerifyProcessor Compose()
        {
            return new ConditionalVerifyProcessor
            {
                Specification = new AndVerifySpecification
                {
                    Specifications = new IVerifySpecification[]
                    {
                        new OrVerifySpecification
                        {
                            Specifications = new IVerifySpecification[]
                            {
                                new NumberExistsInSourceSpecification(),
                                new NumberExistsInTargetSpecification()
                            }
                        },
                        new ReportModifiedAlphanumericsSpecification()
                    }
                },
                TruthProcessor = new VerifyProcessor()
                {
                   ErrorMessage = PluginResources.Error_AlphanumericsModified
                }

            };
        }
    }
}
