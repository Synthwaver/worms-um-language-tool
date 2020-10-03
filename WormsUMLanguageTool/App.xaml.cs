using System.Windows;

namespace WormsUMLanguageTool
{
    public partial class App : Application
    {
        public const WormsLanguage DefaultChosenLanguage = WormsLanguage.English;
        public const string WormsExeFileName = "WormsMayhem.exe";
        public const string WormsSteamDirRelativePath = @"steamapps\common\WormsXHD";
        public const int LanguageBlockOffset = 0x44FC30;
    }
}
