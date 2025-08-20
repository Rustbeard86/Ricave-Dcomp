using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Steamworks;
using UnityEngine;

namespace Ricave.Core
{
    public static class DailyChallengeUtility
    {
        public static ValueTuple<int, string> CurrentDailyChallengeRunSeed
        {
            get
            {
                DateTime utcNow = DateTime.UtcNow;
                if (utcNow.Day == DailyChallengeUtility.cachedCurrentDailyChallengeRunSeedForDate)
                {
                    return DailyChallengeUtility.cachedCurrentDailyChallengeRunSeed;
                }
                string text = utcNow.ToString("yyyy-MM-dd");
                int runSeedForDateString = DailyChallengeUtility.GetRunSeedForDateString(text);
                DailyChallengeUtility.cachedCurrentDailyChallengeRunSeedForDate = utcNow.Day;
                DailyChallengeUtility.cachedCurrentDailyChallengeRunSeed = new ValueTuple<int, string>(runSeedForDateString, text);
                return new ValueTuple<int, string>(runSeedForDateString, text);
            }
        }

        public static ValueTuple<int, int, int> TimeToNextDailyChallenge
        {
            get
            {
                DateTime utcNow = DateTime.UtcNow;
                int num = Math.Max((int)Math.Round((utcNow.AddDays(1.0).Date - utcNow).TotalSeconds), 0);
                int num2 = num / 3600;
                int num3 = num % 3600 / 60;
                int num4 = num % 60;
                return new ValueTuple<int, int, int>(num2, num3, num4);
            }
        }

        public static HashSet<TraitSpec> TraitsForThisRun
        {
            get
            {
                if (DailyChallengeUtility.cachedTraitsForRun != null && DailyChallengeUtility.cachedTraitsForRunSeed == Get.RunConfig.RunSeed)
                {
                    return DailyChallengeUtility.cachedTraitsForRun;
                }
                Rand.PushState(Calc.CombineHashes<int, int>(Get.RunConfig.RunSeed, 453790907));
                DailyChallengeUtility.cachedTraitsForRun = Get.Specs.GetAll<TraitSpec>().InRandomOrder<TraitSpec>().Take<TraitSpec>(2)
                    .ToHashSet<TraitSpec>();
                Rand.PopState();
                DailyChallengeUtility.cachedTraitsForRunSeed = Get.RunConfig.RunSeed;
                return DailyChallengeUtility.cachedTraitsForRun;
            }
        }

        public static bool MostLikelyCheated(CSteamID userID, int score, int dateInt, int runSeed)
        {
            if (SteamManager.Initialized && userID == SteamUser.GetSteamID())
            {
                return false;
            }
            if (score < 0 || score >= 100000)
            {
                return true;
            }
            if (dateInt <= 0)
            {
                return true;
            }
            string text = dateInt.ToString();
            if (text.Length != 8)
            {
                return true;
            }
            int num;
            int num2;
            int num3;
            if (!int.TryParse(text.Substring(0, 4), out num) || !int.TryParse(text.Substring(4, 2), out num2) || !int.TryParse(text.Substring(6, 2), out num3))
            {
                return true;
            }
            if (num < 2024 || num > 3000 || num2 < 1 || num2 > 12 || num3 < 1 || num3 > 31)
            {
                return true;
            }
            try
            {
                new DateTime(num, num2, num3);
            }
            catch
            {
                return true;
            }
            int runSeedForDateString = DailyChallengeUtility.GetRunSeedForDateString(string.Format("{0:D4}-{1:D2}-{2:D2}", num, num2, num3));
            return runSeed != runSeedForDateString;
        }

        public static void UploadScore(int score, string dateString, int runSeed)
        {
            if (!SteamManager.Initialized)
            {
                return;
            }
            CallResult<LeaderboardFindResult_t> callResult = DailyChallengeUtility.activeFindLeaderboardResult;
            if (callResult != null)
            {
                callResult.Dispose();
            }
            DailyChallengeUtility.activeFindLeaderboardResult = null;
            CallResult<LeaderboardScoreUploaded_t> callResult2 = DailyChallengeUtility.activeUploadScoreResult;
            if (callResult2 != null)
            {
                callResult2.Dispose();
            }
            DailyChallengeUtility.activeUploadScoreResult = null;
            try
            {
                CallResult<LeaderboardScoreUploaded_t>.APIDispatchDelegate<>9__1;
                DailyChallengeUtility.activeFindLeaderboardResult = CallResult<LeaderboardFindResult_t>.Create(delegate (LeaderboardFindResult_t result, bool ioFailure)
                {
                    if (ioFailure || result.m_bLeaderboardFound == 0)
                    {
                        Log.Error("Failed to find or create leaderboard for daily challenge " + dateString + ".", false);
                        CallResult<LeaderboardFindResult_t> callResult5 = DailyChallengeUtility.activeFindLeaderboardResult;
                        if (callResult5 != null)
                        {
                            callResult5.Dispose();
                        }
                        DailyChallengeUtility.activeFindLeaderboardResult = null;
                        return;
                    }
                    SteamLeaderboard_t hSteamLeaderboard = result.m_hSteamLeaderboard;
                    int num = int.Parse(dateString.Replace("-", ""));
                    int[] array = new int[] { num, runSeed };
                    CallResult<LeaderboardScoreUploaded_t>.APIDispatchDelegate apidispatchDelegate;
                    if ((apidispatchDelegate = <> 9__1) == null)
                    {
                        apidispatchDelegate = (<> 9__1 = delegate (LeaderboardScoreUploaded_t uploadResult, bool uploadIoFailure)
                        {
                            if (uploadIoFailure)
                            {
                                Log.Error("Failed to upload score to daily challenge leaderboard " + dateString + ".", false);
                                CallResult<LeaderboardScoreUploaded_t> callResult7 = DailyChallengeUtility.activeUploadScoreResult;
                                if (callResult7 != null)
                                {
                                    callResult7.Dispose();
                                }
                                DailyChallengeUtility.activeUploadScoreResult = null;
                                return;
                            }
                            if (uploadResult.m_bSuccess == 0)
                            {
                                Log.Error("Score upload unsuccessful for daily challenge " + dateString + ".", false);
                                CallResult<LeaderboardScoreUploaded_t> callResult8 = DailyChallengeUtility.activeUploadScoreResult;
                                if (callResult8 != null)
                                {
                                    callResult8.Dispose();
                                }
                                DailyChallengeUtility.activeUploadScoreResult = null;
                                return;
                            }
                            Log.Message(string.Format("Successfully uploaded daily challenge score: {0} for date {1}.", score, dateString));
                            CallResult<LeaderboardScoreUploaded_t> callResult9 = DailyChallengeUtility.activeUploadScoreResult;
                            if (callResult9 != null)
                            {
                                callResult9.Dispose();
                            }
                            DailyChallengeUtility.activeUploadScoreResult = null;
                        });
                    }
                    DailyChallengeUtility.activeUploadScoreResult = CallResult<LeaderboardScoreUploaded_t>.Create(apidispatchDelegate);
                    SteamAPICall_t steamAPICall_t2 = SteamUserStats.UploadLeaderboardScore(hSteamLeaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, array, array.Length);
                    DailyChallengeUtility.activeUploadScoreResult.Set(steamAPICall_t2, null);
                    CallResult<LeaderboardFindResult_t> callResult6 = DailyChallengeUtility.activeFindLeaderboardResult;
                    if (callResult6 != null)
                    {
                        callResult6.Dispose();
                    }
                    DailyChallengeUtility.activeFindLeaderboardResult = null;
                });
                SteamAPICall_t steamAPICall_t = SteamUserStats.FindOrCreateLeaderboard(DailyChallengeUtility.GetLeaderboardName(dateString), ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
                DailyChallengeUtility.activeFindLeaderboardResult.Set(steamAPICall_t, null);
            }
            catch (Exception ex)
            {
                Log.Error("Exception while uploading daily challenge score for " + dateString + ".", ex);
                CallResult<LeaderboardFindResult_t> callResult3 = DailyChallengeUtility.activeFindLeaderboardResult;
                if (callResult3 != null)
                {
                    callResult3.Dispose();
                }
                DailyChallengeUtility.activeFindLeaderboardResult = null;
                CallResult<LeaderboardScoreUploaded_t> callResult4 = DailyChallengeUtility.activeUploadScoreResult;
                if (callResult4 != null)
                {
                    callResult4.Dispose();
                }
                DailyChallengeUtility.activeUploadScoreResult = null;
            }
        }

        public static ValueTuple<string, int>[] GetLeaderboard(string dateString, bool friendsOnly)
        {
            DailyChallengeUtility.<> c__DisplayClass16_0 CS$<> 8__locals1 = new DailyChallengeUtility.<> c__DisplayClass16_0();
            CS$<> 8__locals1.dateString = dateString;
            if (!SteamManager.Initialized)
            {
                return new ValueTuple<string, int>[0];
            }
            CS$<> 8__locals1.validEntries = new List<ValueTuple<string, int>>();
            CS$<> 8__locals1.playerIDDuplicates = new HashSet<CSteamID>();
            CS$<> 8__locals1.activeCallResults = new List<CallResult<LeaderboardScoresDownloaded_t>>();
            CallResult<LeaderboardFindResult_t> callResult = null;
            CS$<> 8__locals1.isComplete = false;
            CS$<> 8__locals1.dataRequest = (friendsOnly ? ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends : ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal);
            try
            {
                callResult = CallResult<LeaderboardFindResult_t>.Create(delegate (LeaderboardFindResult_t result, bool ioFailure)
                {
                    DailyChallengeUtility.<> c__DisplayClass16_1 CS$<> 8__locals2 = new DailyChallengeUtility.<> c__DisplayClass16_1();
                    CS$<> 8__locals2.CS$<> 8__locals1 = CS$<> 8__locals1;
                    if (ioFailure || result.m_bLeaderboardFound == 0)
                    {
                        Log.Warning("Failed to find leaderboard for daily challenge " + CS$<> 8__locals1.dateString + ".", false);
                        CS$<> 8__locals1.isComplete = true;
                        return;
                    }
                    CS$<> 8__locals2.leaderboardHandle = result.m_hSteamLeaderboard;
                    CS$<> 8__locals2.totalEntries = SteamUserStats.GetLeaderboardEntryCount(CS$<> 8__locals2.leaderboardHandle);
                    Math.Min(50, CS$<> 8__locals2.totalEntries);
                    CS$<> 8__locals2.currentOffset = 0;
                    CS$<> 8__locals2.< GetLeaderboard > g__FetchNextBatch | 1();
                });
                SteamAPICall_t steamAPICall_t = SteamUserStats.FindLeaderboard(DailyChallengeUtility.GetLeaderboardName(CS$<> 8__locals1.dateString));
                callResult.Set(steamAPICall_t, null);
                float realtimeSinceStartup = Time.realtimeSinceStartup;
                while (!CS$<> 8__locals1.isComplete && Time.realtimeSinceStartup - realtimeSinceStartup < 4f)
				{
                    SteamAPI.RunCallbacks();
                    Thread.Sleep(1);
                }
                if (!CS$<> 8__locals1.isComplete)
				{
                    Log.Error("Timeout while fetching leaderboard for daily challenge " + CS$<> 8__locals1.dateString + ".", false);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception while fetching leaderboard for daily challenge " + CS$<> 8__locals1.dateString + ".", ex);
            }
            finally
            {
                if (callResult != null)
                {
                    callResult.Dispose();
                }
                foreach (CallResult<LeaderboardScoresDownloaded_t> callResult2 in CS$<> 8__locals1.activeCallResults)
				{
                    if (callResult2 != null)
                    {
                        callResult2.Dispose();
                    }
                }
            }
            return CS$<> 8__locals1.validEntries.ToArray();
        }

        public static int GetMyLeaderboardPlace(string dateString, bool friendsOnly)
        {
            if (!SteamManager.Initialized)
            {
                return -1;
            }
            int myRank = -1;
            bool isComplete = false;
            CallResult<LeaderboardFindResult_t> callResult = null;
            CallResult<LeaderboardScoresDownloaded_t> downloadResult = null;
            try
            {
                CallResult<LeaderboardScoresDownloaded_t>.APIDispatchDelegate<>9__4;
                callResult = CallResult<LeaderboardFindResult_t>.Create(delegate (LeaderboardFindResult_t result, bool ioFailure)
                {
                    try
                    {
                        if (ioFailure || result.m_bLeaderboardFound == 0)
                        {
                            Log.Warning("Failed to find leaderboard for daily challenge " + dateString + ".", false);
                            isComplete = true;
                        }
                        else
                        {
                            SteamLeaderboard_t hSteamLeaderboard = result.m_hSteamLeaderboard;
                            CSteamID mySteamID = SteamUser.GetSteamID();
                            if (friendsOnly)
                            {
                                ELeaderboardDataRequest eleaderboardDataRequest = ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends;
                                SteamAPICall_t steamAPICall_t2 = SteamUserStats.DownloadLeaderboardEntries(hSteamLeaderboard, eleaderboardDataRequest, 1, 100);
                                downloadResult = CallResult<LeaderboardScoresDownloaded_t>.Create(delegate (LeaderboardScoresDownloaded_t downloadRes, bool downloadFailure)
                                {
                                    if (downloadFailure)
                                    {
                                        Log.Error("Failed to download friends leaderboard entries for daily challenge " + dateString + ".", false);
                                        isComplete = true;
                                        return;
                                    }
                                    List<ValueTuple<CSteamID, int>> list = new List<ValueTuple<CSteamID, int>>();
                                    int num = -1;
                                    for (int i = 0; i < downloadRes.m_cEntryCount; i++)
                                    {
                                        int[] array = new int[2];
                                        LeaderboardEntry_t entry;
                                        if (SteamUserStats.GetDownloadedLeaderboardEntry(downloadRes.m_hSteamLeaderboardEntries, i, out entry, array, 2) && !list.Any<ValueTuple<CSteamID, int>>(([TupleElementNames(new string[] { "steamID", "score" })] ValueTuple<CSteamID, int> x) => x.Item1 == entry.m_steamIDUser))
                                        {
                                            list.Add(new ValueTuple<CSteamID, int>(entry.m_steamIDUser, entry.m_nScore));
                                            if (entry.m_steamIDUser == mySteamID)
                                            {
                                                num = entry.m_nScore;
                                            }
                                        }
                                    }
                                    if (num >= 0)
                                    {
                                        list.Sort(([TupleElementNames(new string[] { "steamID", "score" })] ValueTuple<CSteamID, int> a, [TupleElementNames(new string[] { "steamID", "score" })] ValueTuple<CSteamID, int> b) => b.Item2.CompareTo(a.Item2));
                                        for (int j = 0; j < list.Count; j++)
                                        {
                                            if (list[j].Item1 == mySteamID)
                                            {
                                                myRank = j + 1;
                                                break;
                                            }
                                        }
                                    }
                                    isComplete = true;
                                });
                                downloadResult.Set(steamAPICall_t2, null);
                            }
                            else
                            {
                                SteamAPICall_t steamAPICall_t3 = SteamUserStats.DownloadLeaderboardEntriesForUsers(hSteamLeaderboard, new CSteamID[] { mySteamID }, 1);
                                CallResult<LeaderboardScoresDownloaded_t>.APIDispatchDelegate apidispatchDelegate;
                                if ((apidispatchDelegate = <> 9__4) == null)
                                {
                                    apidispatchDelegate = (<> 9__4 = delegate (LeaderboardScoresDownloaded_t downloadRes, bool downloadFailure)
                                    {
                                        if (downloadFailure)
                                        {
                                            Log.Error("Failed to download user's leaderboard entry for daily challenge " + dateString + ".", false);
                                            isComplete = true;
                                            return;
                                        }
                                        if (downloadRes.m_cEntryCount > 0)
                                        {
                                            int[] array2 = new int[2];
                                            LeaderboardEntry_t leaderboardEntry_t;
                                            if (SteamUserStats.GetDownloadedLeaderboardEntry(downloadRes.m_hSteamLeaderboardEntries, 0, out leaderboardEntry_t, array2, 2))
                                            {
                                                myRank = leaderboardEntry_t.m_nGlobalRank;
                                            }
                                        }
                                        isComplete = true;
                                    });
                                }
                                downloadResult = CallResult<LeaderboardScoresDownloaded_t>.Create(apidispatchDelegate);
                                downloadResult.Set(steamAPICall_t3, null);
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Log.Error("Exception while processing leaderboard results for daily challenge " + dateString + ".", ex2);
                        isComplete = true;
                    }
                });
                SteamAPICall_t steamAPICall_t = SteamUserStats.FindLeaderboard(DailyChallengeUtility.GetLeaderboardName(dateString));
                callResult.Set(steamAPICall_t, null);
                float realtimeSinceStartup = Time.realtimeSinceStartup;
                while (!isComplete && Time.realtimeSinceStartup - realtimeSinceStartup < 4f)
                {
                    SteamAPI.RunCallbacks();
                    Thread.Sleep(1);
                }
                if (!isComplete)
                {
                    Log.Error("Timeout while fetching player's leaderboard rank for daily challenge " + dateString + ".", false);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception while fetching player's leaderboard rank for daily challenge " + dateString + ".", ex);
            }
            finally
            {
                if (callResult != null)
                {
                    callResult.Dispose();
                }
                CallResult<LeaderboardScoresDownloaded_t> downloadResult2 = downloadResult;
                if (downloadResult2 != null)
                {
                    downloadResult2.Dispose();
                }
            }
            return myRank;
        }

        public static string GetLeaderboardName(string dateString)
        {
            return "DailyChallenge_" + dateString;
        }

        private static int GetRunSeedForDateString(string dateString)
        {
            return Calc.CombineHash(dateString.StableHashCode(), 842564778);
        }

        public static ClassSpec GetClassForSeed(int seed)
        {
            Rand.PushState(Calc.CombineHashes<int, int>(seed, 923841671));
            ClassSpec classSpec;
            Get.Specs.GetAll<ClassSpec>().TryGetRandomElement<ClassSpec>(out classSpec);
            Rand.PopState();
            return classSpec;
        }

        public static ChooseablePartyMemberSpec GetChooseablePartyMemberForSeed(int seed)
        {
            Rand.PushState(Calc.CombineHashes<int, int>(seed, 517509076));
            if (Rand.Chance(0.75f))
            {
                Rand.PopState();
                return null;
            }
            ChooseablePartyMemberSpec chooseablePartyMemberSpec;
            Get.Specs.GetAll<ChooseablePartyMemberSpec>().TryGetRandomElement<ChooseablePartyMemberSpec>(out chooseablePartyMemberSpec);
            Rand.PopState();
            return chooseablePartyMemberSpec;
        }

        public const int ParticipationStardustReward = 20;

        public const int UnlockedAtLevel = 4;

        private static int cachedCurrentDailyChallengeRunSeedForDate = -1;

        private static ValueTuple<int, string> cachedCurrentDailyChallengeRunSeed;

        private static CallResult<LeaderboardFindResult_t> activeFindLeaderboardResult;

        private static CallResult<LeaderboardScoreUploaded_t> activeUploadScoreResult;

        private static HashSet<TraitSpec> cachedTraitsForRun;

        private static int cachedTraitsForRunSeed;
    }
}