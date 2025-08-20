using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;

namespace Ricave.Core
{
    public static class SteamWorkshop
    {
        public static List<ValueTuple<PublishedFileId_t, string>> SubscribedItems
        {
            get
            {
                return SteamWorkshop.subscribedItems;
            }
        }

        public static bool Uploading
        {
            get
            {
                return SteamWorkshop.uploadingItem != null;
            }
        }

        public static void Init()
        {
            SteamWorkshop.installedCallback = Callback<ItemInstalled_t>.Create(new Callback<ItemInstalled_t>.DispatchDelegate(SteamWorkshop.InstalledCallback));
            SteamWorkshop.subscribedCallback = Callback<RemoteStoragePublishedFileSubscribed_t>.Create(new Callback<RemoteStoragePublishedFileSubscribed_t>.DispatchDelegate(SteamWorkshop.SubscribedCallback));
            SteamWorkshop.unsubscribedCallback = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(new Callback<RemoteStoragePublishedFileUnsubscribed_t>.DispatchDelegate(SteamWorkshop.UnsubscribedCallback));
            SteamWorkshop.RecalculateSubscribedItems();
        }

        public static void UploadOrUpdate(ISteamWorkshopItem item)
        {
            if (SteamWorkshop.uploadingItem != null)
            {
                return;
            }
            SteamWorkshop.uploadingItem = item;
            if (SteamWorkshop.uploadingItem.PublishedId != PublishedFileId_t.Invalid)
            {
                SteamWorkshop.updateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), SteamWorkshop.uploadingItem.PublishedId);
                SteamWorkshop.FillUpdateHandleData(SteamWorkshop.updateHandle, SteamWorkshop.uploadingItem, false);
                SteamWorkshop.submitResult = CallResult<SubmitItemUpdateResult_t>.Create(new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(SteamWorkshop.SubmittedCallback));
                SteamWorkshop.submitResult.Set(SteamUGC.SubmitItemUpdate(SteamWorkshop.updateHandle, "Update"), null);
                return;
            }
            SteamWorkshop.createResult = CallResult<CreateItemResult_t>.Create(new CallResult<CreateItemResult_t>.APIDispatchDelegate(SteamWorkshop.CreatedCallback));
            SteamWorkshop.createResult.Set(SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeFirst), null);
        }

        public static void Unsubscribe(ISteamWorkshopItem item)
        {
            SteamUGC.UnsubscribeItem(item.PublishedId);
        }

        private static void SubscribedCallback(RemoteStoragePublishedFileSubscribed_t result)
        {
            if (result.m_nAppID != SteamUtils.GetAppID())
            {
                return;
            }
            SteamWorkshop.RecalculateSubscribedItems();
        }

        private static void InstalledCallback(ItemInstalled_t result)
        {
            if (result.m_unAppID != SteamUtils.GetAppID())
            {
                return;
            }
            SteamWorkshop.RecalculateSubscribedItems();
        }

        private static void UnsubscribedCallback(RemoteStoragePublishedFileUnsubscribed_t result)
        {
            if (result.m_nAppID != SteamUtils.GetAppID())
            {
                return;
            }
            SteamWorkshop.RecalculateSubscribedItems();
        }

        private static void CreatedCallback(CreateItemResult_t result, bool fail)
        {
            if (fail || result.m_eResult != EResult.k_EResultOK)
            {
                SteamWorkshop.uploadingItem = null;
                Get.WindowManager.OpenMessageWindow("SteamUploadFailed".Translate(result.m_eResult.ToString()), null);
                return;
            }
            SteamWorkshop.uploadingItem.PublishedId = result.m_nPublishedFileId;
            SteamWorkshop.updateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), SteamWorkshop.uploadingItem.PublishedId);
            SteamWorkshop.FillUpdateHandleData(SteamWorkshop.updateHandle, SteamWorkshop.uploadingItem, true);
            SteamWorkshop.submitResult = CallResult<SubmitItemUpdateResult_t>.Create(new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(SteamWorkshop.SubmittedCallback));
            SteamWorkshop.submitResult.Set(SteamUGC.SubmitItemUpdate(SteamWorkshop.updateHandle, "Upload"), null);
            SteamWorkshop.createResult = null;
        }

        private static void SubmittedCallback(SubmitItemUpdateResult_t result, bool fail)
        {
            if (fail || result.m_eResult != EResult.k_EResultOK)
            {
                Get.WindowManager.OpenMessageWindow("SteamUploadFailed".Translate(result.m_eResult.ToString()), null);
            }
            else
            {
                Get.WindowManager.OpenMessageWindow("SteamUploadSucceeded".Translate(), null);
            }
            SteamWorkshop.uploadingItem = null;
            SteamWorkshop.submitResult = null;
        }

        private static void FillUpdateHandleData(UGCUpdateHandle_t updateHandle, ISteamWorkshopItem item, bool creating)
        {
            SteamUGC.SetItemTitle(updateHandle, item.Name);
            if (creating)
            {
                SteamUGC.SetItemDescription(updateHandle, item.Description);
            }
            if (File.Exists(item.PreviewImage))
            {
                SteamUGC.SetItemPreview(updateHandle, item.PreviewImage);
            }
            SteamUGC.SetItemContent(updateHandle, item.UploadDirectory);
            SteamUGC.SetItemVisibility(updateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
        }

        private static void RecalculateSubscribedItems()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }
            SteamWorkshop.subscribedItems.Clear();
            uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
            PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
            numSubscribedItems = SteamUGC.GetSubscribedItems(array, numSubscribedItems);
            int num = 0;
            while ((long)num < (long)((ulong)numSubscribedItems))
            {
                ulong num2;
                string text;
                uint num3;
                if (SteamUGC.GetItemInstallInfo(array[num], out num2, out text, 250U, out num3) && Directory.Exists(text))
                {
                    SteamWorkshop.subscribedItems.Add(new ValueTuple<PublishedFileId_t, string>(array[num], text));
                }
                num++;
            }
        }

        private static ISteamWorkshopItem uploadingItem;

        private static UGCUpdateHandle_t updateHandle;

        private static List<ValueTuple<PublishedFileId_t, string>> subscribedItems = new List<ValueTuple<PublishedFileId_t, string>>();

        private static Callback<ItemInstalled_t> installedCallback;

        private static Callback<RemoteStoragePublishedFileSubscribed_t> subscribedCallback;

        private static Callback<RemoteStoragePublishedFileUnsubscribed_t> unsubscribedCallback;

        private static CallResult<SubmitItemUpdateResult_t> submitResult;

        private static CallResult<CreateItemResult_t> createResult;
    }
}