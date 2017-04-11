using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sdl.Community.TermInjector
{
    public class Trie
    {
        /// <summary>
        /// Each node consists of a dictionary of child nodes and a possible translation
        /// </summary>
        private Dictionary<char, Trie> characterTransitions;
        private String translation;

        //The optional string that will be replaced with the translation in the fuzzy match
        private String replaces;

        public Trie()
        {
            this.characterTransitions = new Dictionary<char,Trie>();
            this.translation = "";
            this.replaces = "";
        }

        public string GetTranslation()
        {
            return this.translation;
        }

        public string GetReplaces()
        {
            return this.replaces;
        }

        public Trie GetChildNode(char c)
        {
            return (this.characterTransitions.ContainsKey(c)) ? this.characterTransitions[c] : null;
        }

        public void AddToTrie(string source, string translation, string replaces)
        {
            //Assign the first letter of the source to a variable
            char firstletter = source.First();

            //If this level of the trie does not contain firstletter as key, add it
            if (!this.characterTransitions.ContainsKey(firstletter))
            {
                this.characterTransitions.Add(firstletter, new Trie());
            }
            //If source length is 1, add the translation and replaces fields to the child
            if (source.Length == 1)
            {
                this.characterTransitions[firstletter].translation = translation;
                this.characterTransitions[firstletter].replaces = replaces;

            }
            //Else call the AddToTrie function of the child node with rest of the source
            else
            {
                this.characterTransitions[firstletter].AddToTrie(source.Substring(1), translation, replaces);
            }
        }

        public Trie Clone()
        {
            return (Trie)this.MemberwiseClone();
        } 
    }
}