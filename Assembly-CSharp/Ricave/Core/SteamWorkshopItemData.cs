using System;
using Steamworks;

namespace Ricave.Core
{
    public class SteamWorkshopItemData
    {
        public ISteamWorkshopItem Parent
        {
            get
            {
                return this.parent;
            }
        }

        public CSteamID Author
        {
            get
            {
                return this.author;
            }
        }

        public SteamWorkshopItemData(ISteamWorkshopItem parent)
        {
            this.parent = parent;
            if (parent.PublishedId != PublishedFileId_t.Invalid && SteamManager.Initialized)
            {
                this.authorCallResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create(delegate (SteamUGCRequestUGCDetailsResult_t result, bool _)
                {
                    this.author = (CSteamID)result.m_details.m_ulSteamIDOwner;
                });
                this.authorCallResult.Set(SteamUGC.RequestUGCDetails(parent.PublishedId, 99999U), null);
            }
        }

        public bool DifferentAuthor
        {
            get
            {
                return !(this.parent.PublishedId == PublishedFileId_t.Invalid) && (this.author == CSteamID.Nil || this.author != SteamUser.GetSteamID());
            }
        }

        private ISteamWorkshopItem parent;

        private CSteamID author = CSteamID.Nil;

        private CallResult<SteamUGCRequestUGCDetailsResult_t> authorCallResult;
    }
}