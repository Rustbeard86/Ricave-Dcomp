using System;
using System.IO;
using Ricave.Rendering;
using Steamworks;
using UnityEngine;

namespace Ricave.Core
{
    public class ModInfo : ISteamWorkshopItem, ISaveableEventsReceiver
    {
        public string ModId
        {
            get
            {
                return this.modId;
            }
        }

        public string Directory
        {
            get
            {
                return this.directory;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string Author
        {
            get
            {
                return this.author;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public string EventsHandler
        {
            get
            {
                return this.eventsHandler;
            }
        }

        public Texture2D PreviewImage
        {
            get
            {
                return this.previewImage;
            }
        }

        public SteamWorkshopItemData SteamWorkshopItemData
        {
            get
            {
                if (this.steamWorkshopItemData == null)
                {
                    this.steamWorkshopItemData = new SteamWorkshopItemData(this);
                }
                return this.steamWorkshopItemData;
            }
        }

        string ISteamWorkshopItem.PreviewImage
        {
            get
            {
                return FilePaths.ModPreviewImage(this);
            }
        }

        string ISteamWorkshopItem.UploadDirectory
        {
            get
            {
                return this.directory;
            }
        }

        SteamWorkshopItemData ISteamWorkshopItem.ItemData
        {
            get
            {
                return this.SteamWorkshopItemData;
            }
        }

        PublishedFileId_t ISteamWorkshopItem.PublishedId
        {
            get
            {
                return new PublishedFileId_t(this.workshopItemId);
            }
            set
            {
                if (this.workshopItemId == value.m_PublishedFileId)
                {
                    return;
                }
                this.workshopItemId = value.m_PublishedFileId;
                this.Save();
            }
        }

        public Mod Mod
        {
            get
            {
                if (this.cachedMod == null)
                {
                    this.cachedMod = Get.ModManager.GetMod(this.modId);
                }
                return this.cachedMod;
            }
        }

        public ModInfo(string directory)
        {
            this.directory = directory;
        }

        public ModInfo(string directory, PublishedFileId_t workshopItemId)
        {
            this.directory = directory;
            this.workshopItemId = workshopItemId.m_PublishedFileId;
        }

        public override string ToString()
        {
            if (!this.name.NullOrEmpty())
            {
                return this.name;
            }
            if (!this.modId.NullOrEmpty())
            {
                return this.modId;
            }
            return "Unnamed mod";
        }

        public void OnLoaded()
        {
            string text = FilePaths.ModPreviewImage(this);
            Object @object;
            if (File.Exists(text) && RuntimeAssetLoader.TryLoad(text, out @object))
            {
                Texture2D texture2D = @object as Texture2D;
                if (texture2D != null)
                {
                    this.previewImage = texture2D;
                    return;
                }
            }
            Log.Error("ModInfo " + this.modId + " has no preview image.", false);
        }

        public void OnSaved()
        {
        }

        private void Save()
        {
            SaveLoadManager.Save(this, Path.Combine(this.directory, "Info.xml"), "ModInfo");
        }

        private string directory;

        [Saved]
        private string modId;

        [Saved]
        private string name;

        [Saved]
        private string author;

        [Saved]
        private string description;

        [Saved(Default.PublishedFiledId_Invalid, false)]
        private ulong workshopItemId = PublishedFileId_t.Invalid.m_PublishedFileId;

        [Saved("ModEventsHandler", false)]
        private string eventsHandler = "ModEventsHandler";

        private Mod cachedMod;

        private Texture2D previewImage;

        private SteamWorkshopItemData steamWorkshopItemData;
    }
}