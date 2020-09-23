using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WormsUMLanguageTool
{
    public class LanguageBlock
    {
        private class LanguageInsertionPositions
        {
            public readonly int Loading;
            public readonly int LS;
            public readonly int FE;
            public readonly int Name;

            public LanguageInsertionPositions(int loading, int ls, int fe, int name)
            {
                Loading = loading;
                LS = ls;
                FE = fe;
                Name = name;
            }
        }

        public const int BlockLength = 475;

        private readonly LanguageInsertionPositions desiredLangPos = new LanguageInsertionPositions(loading:  72, ls: 204, fe: 308, name: 412);
        private readonly LanguageInsertionPositions defaultLangPos = new LanguageInsertionPositions(loading: 144, ls: 252, fe: 356, name: 468);

        private byte[] bytes;
        public byte[] GetBytes()
        {
            return bytes;
        }

        public string AsciiString => Encoding.ASCII.GetString(bytes);
        public string DisplayableString => AsciiString.Replace('\0', ' ');

        private readonly WormsLanguage defaultLanguage = WormsLanguage.English;

        public LanguageBlock(byte[] bytes)
        {
            Validate(bytes);
            this.bytes = bytes;
        }

        private void Validate(byte[] bytes)
        {
            if (bytes is null)
            {
                throw new NullReferenceException(nameof(bytes));
            }
            if (bytes.Length != BlockLength)
            {
                throw new ArgumentException("The language block has the wrong length", nameof(bytes));
            }

            string text = Encoding.ASCII.GetString(bytes);
            string pattern = "^"
                    + $@"(?=.{{{defaultLangPos.Loading + 3}}}Loading)"
                    + $@"(?=.{{{defaultLangPos.LS + 3}}}LS)"
                    + $@"(?=.{{{defaultLangPos.FE + 3}}}FE)";

            if (!new Regex(pattern).IsMatch(text))
            {
                throw new ArgumentException("The language block has the wrong structure", nameof(bytes));
            }
        }

        public void SetLanguage(WormsLanguage language)
        {
            string newAsciiString = new string('\0', BlockLength);
            newAsciiString = InsertLangWithPos(newAsciiString, defaultLanguage, defaultLangPos);
            if (language != defaultLanguage)
            {
                newAsciiString = InsertLangWithPos(newAsciiString, language, desiredLangPos);
            }
            bytes = Encoding.ASCII.GetBytes(newAsciiString);

            string InsertLangWithPos(string str, WormsLanguage lang, LanguageInsertionPositions pos)
            {
                string langName = lang.ToString();
                string shortLangName = langName.Substring(0, 3);

                InsertWithReplacement(pos.Loading, $"{shortLangName}Loading");
                InsertWithReplacement(pos.LS, $"{shortLangName}LS");
                InsertWithReplacement(pos.FE, $"{shortLangName}FE");
                InsertWithReplacement(pos.Name, langName);

                return str;

                void InsertWithReplacement(int startIndex, string value)
                {
                    str = str.Remove(startIndex, value.Length).Insert(startIndex, value);
                }
            }
        }
    }
}
