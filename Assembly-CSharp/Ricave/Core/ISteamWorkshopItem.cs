using System;
using Steamworks;

namespace Ricave.Core
{
    public interface ISteamWorkshopItem
    {
        string Name { get; }

        string Description { get; }

        string PreviewImage { get; }

        string UploadDirectory { get; }

        SteamWorkshopItemData ItemData { get; }

        PublishedFileId_t PublishedId { get; set; }
    }
}