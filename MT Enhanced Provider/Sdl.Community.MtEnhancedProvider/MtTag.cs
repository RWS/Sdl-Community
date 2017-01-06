/* Copyright 2015 Patrick Porter

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/

using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MtEnhancedProvider
{
    /// <summary>
    /// Used to add info to associate with an SDL tag object
    /// </summary>
    internal class MtTag
    {
        internal MtTag(Tag tag)
        {
            this.SdlTag = tag;
            PadLeft = string.Empty;
            PadRight = string.Empty;
        }

        internal string PadLeft { get; set; }

        internal string PadRight { get; set; }

        internal Tag SdlTag { get; }
    }
}
