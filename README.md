<h1>
    <img src="docs/media/logo.png" align="center" hspace="10">
    Worms Ultimate Mayhem Language Tool
</h1>

[![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/Synthwaver/worms-um-language-tool?label=latest)](https://github.com/Synthwaver/worms-um-language-tool/releases/latest)
[![GitHub all releases](https://img.shields.io/github/downloads/Synthwaver/worms-um-language-tool/total)](https://github.com/Synthwaver/worms-um-language-tool/releases)

This tool allows you to modify the Steam version of Worms Ultimate Mayhem to unlock inaccessible languages, the files of which are in the game directory.

## Supported languages

- Czech
- English
- French
- German
- Italian
- Polish
- Russian
- Spanish

The game files also contain American and Slovak localization files. But American files are completely the same as English, and the Slovak ones are not fully translated, so I did not include them in the list.

## How to use

It's very easy! Just specify the path to your `WormsMayhem.exe` (if the application does not automatically detect it), select desired language from the list and click `Modify`.

If `Make backup` is checked, a folder named `LanguageToolBackups` will be created and the current `WormsMayhem.exe` will be saved inside. 

<div align="center"><img src="docs/media/app-interface-screenshot.png" width="512"></div>

The selected language will be setted.<br>
If not, make sure that folder `WormsXHD\Data\Language\PC\` contains the required localization `.xom` files.

<div align="center"><img src="docs/media/in-game-menu-screenshot.png" width="512"></div>