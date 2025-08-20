using System;

namespace Ricave.Core
{
    public class NoteSpec : Spec
    {
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        public NoteSpec.AuthorType Author
        {
            get
            {
                return this.author;
            }
        }

        public string AuthorLabel
        {
            get
            {
                NoteSpec.AuthorType authorType = this.author;
                if (authorType == NoteSpec.AuthorType.Author1)
                {
                    return "AuthorType_1".Translate();
                }
                if (authorType != NoteSpec.AuthorType.Author2)
                {
                    return "";
                }
                return "AuthorType_2".Translate();
            }
        }

        [Saved]
        [Translatable]
        private string text;

        [Saved]
        private NoteSpec.AuthorType author;

        public enum AuthorType
        {
            Author1,

            Author2
        }
    }
}