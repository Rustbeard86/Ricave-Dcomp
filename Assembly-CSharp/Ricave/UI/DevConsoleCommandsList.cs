using System;
using Ricave.Core;

namespace Ricave.UI
{
    public static class DevConsoleCommandsList
    {
        public static readonly DevConsoleCommands.Command[] Commands = new DevConsoleCommands.Command[]
        {
            new DevConsoleCommands.Command("help", new DevConsoleCommands.Command.Arg[0], "show all available commands", new Action<string[]>(DevConsoleCommandsImpl_General.Help)),
            new DevConsoleCommands.Command("log_specs", new DevConsoleCommands.Command.Arg[0], "show spec count loaded", new Action<string[]>(DevConsoleCommandsImpl_General.Log_Specs)),
            new DevConsoleCommands.Command("log_specs", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("type", null)
            }, "show specs loaded of type", new Action<string[]>(DevConsoleCommandsImpl_General.Log_Specs_OfType)),
            new DevConsoleCommands.Command("log_actors", new DevConsoleCommands.Command.Arg[0], "show actors stats", new Action<string[]>(DevConsoleCommandsImpl_General.Log_Actors)),
            new DevConsoleCommands.Command("log_bodyparts", new DevConsoleCommands.Command.Arg[0], "show body parts stats", new Action<string[]>(DevConsoleCommandsImpl_General.Log_BodyParts)),
            new DevConsoleCommands.Command("log_leveling", new DevConsoleCommands.Command.Arg[0], "show leveling data", new Action<string[]>(DevConsoleCommandsImpl_General.Log_Leveling)),
            new DevConsoleCommands.Command("log_permanent", new DevConsoleCommands.Command.Arg[0], "show which entities are permanent", new Action<string[]>(DevConsoleCommandsImpl_General.Log_Permanent)),
            new DevConsoleCommands.Command("get_pref", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("name", null)
            }, "get PlayerPref", new Action<string[]>(DevConsoleCommandsImpl_General.Get_Pref)),
            new DevConsoleCommands.Command("set_pref", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("name", null),
                new DevConsoleCommands.Command.Arg("value", null)
            }, "set PlayerPref", new Action<string[]>(DevConsoleCommandsImpl_General.Set_Pref)),
            new DevConsoleCommands.Command("get_uiscale", new DevConsoleCommands.Command.Arg[0], "get UI scale", new Action<string[]>(DevConsoleCommandsImpl_General.Get_UIScale)),
            new DevConsoleCommands.Command("set_uiscale", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("scale", null)
            }, "set UI scale", new Action<string[]>(DevConsoleCommandsImpl_General.Set_UIScale)),
            new DevConsoleCommands.Command("set_fontscale", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("scale", null)
            }, "set font scale", new Action<string[]>(DevConsoleCommandsImpl_General.Set_FontScale)),
            new DevConsoleCommands.Command("set_ui", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("visible", null)
            }, "set UI visibility", new Action<string[]>(DevConsoleCommandsImpl_General.Set_UI)),
            new DevConsoleCommands.Command("set_trailermode", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("value", null)
            }, "set trailer mode", new Action<string[]>(DevConsoleCommandsImpl_General.Set_TrailerMode)),
            new DevConsoleCommands.Command("save", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("name", null)
            }, "save run", new Action<string[]>(DevConsoleCommandsImpl_General.Save)),
            new DevConsoleCommands.Command("load", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("name", null)
            }, "load run", new Action<string[]>(DevConsoleCommandsImpl_General.Load)),
            new DevConsoleCommands.Command("generateTranslationTemplate", new DevConsoleCommands.Command.Arg[0], "generate a translation template for the base game and all active mods", new Action<string[]>(DevConsoleCommandsImpl_General.GenerateTranslationTemplate)),
            new DevConsoleCommands.Command("setlanguage", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("name", null)
            }, "set current language (only call from main menu!)", new Action<string[]>(DevConsoleCommandsImpl_General.SetLanguage)),
            new DevConsoleCommands.Command("spawn", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("EntitySpec", typeof(EntitySpec))
            }, "spawn new Entity at player pos", new Action<string[]>(DevConsoleCommandsImpl_World.Spawn)),
            new DevConsoleCommands.Command("spawnat", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("EntitySpec", typeof(EntitySpec))
            }, "spawn new Entity at selected pos", new Action<string[]>(DevConsoleCommandsImpl_World.SpawnAt)),
            new DevConsoleCommands.Command("spawnblueprint", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("BlueprintSpec", typeof(BlueprintSpec))
            }, "spawn blueprint at selected pos", new Action<string[]>(DevConsoleCommandsImpl_World.SpawnBlueprint)),
            new DevConsoleCommands.Command("saveblueprint", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("name", null)
            }, "save area as blueprint starting at selected pos", new Action<string[]>(DevConsoleCommandsImpl_World.SaveBlueprint)),
            new DevConsoleCommands.Command("despawn", new DevConsoleCommands.Command.Arg[0], "despawn selected Entity", new Action<string[]>(DevConsoleCommandsImpl_World.DeSpawn)),
            new DevConsoleCommands.Command("destroy", new DevConsoleCommands.Command.Arg[0], "destroy selected Entity", new Action<string[]>(DevConsoleCommandsImpl_World.Destroy)),
            new DevConsoleCommands.Command("spawnboss", new DevConsoleCommands.Command.Arg[0], "spawn random boss at selected pos", new Action<string[]>(DevConsoleCommandsImpl_World.SpawnBoss)),
            new DevConsoleCommands.Command("spawnboss", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("EntitySpec", typeof(EntitySpec))
            }, "spawn given boss at selected pos", new Action<string[]>(DevConsoleCommandsImpl_World.SpawnBoss_WithActorSpec)),
            new DevConsoleCommands.Command("spawnbaby", new DevConsoleCommands.Command.Arg[0], "spawn random baby at selected pos", new Action<string[]>(DevConsoleCommandsImpl_World.SpawnBaby)),
            new DevConsoleCommands.Command("run", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("RunSpec", typeof(RunSpec))
            }, "start new run", new Action<string[]>(DevConsoleCommandsImpl_World.Run)),
            new DevConsoleCommands.Command("run", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("RunSpec", typeof(RunSpec)),
                new DevConsoleCommands.Command.Arg("seed", null)
            }, "start new run using RunSpec and seed", new Action<string[]>(DevConsoleCommandsImpl_World.Run_WithRunSpecAndSeed)),
            new DevConsoleCommands.Command("gen", new DevConsoleCommands.Command.Arg[0], "generate new World", new Action<string[]>(DevConsoleCommandsImpl_World.Gen)),
            new DevConsoleCommands.Command("gen", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("WorldSpec", typeof(WorldSpec))
            }, "generate new World using WorldSpec", new Action<string[]>(DevConsoleCommandsImpl_World.Gen_WithWorldSpecOrRoomSpecOrFloor)),
            new DevConsoleCommands.Command("gen", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("RoomSpec", typeof(RoomSpec))
            }, "generate new World using only given RoomSpec", new Action<string[]>(DevConsoleCommandsImpl_World.Gen_WithWorldSpecOrRoomSpecOrFloor)),
            new DevConsoleCommands.Command("gen", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("floor", null)
            }, "generate new World for the given floor", new Action<string[]>(DevConsoleCommandsImpl_World.Gen_WithWorldSpecOrRoomSpecOrFloor)),
            new DevConsoleCommands.Command("gen", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("WorldSpec", typeof(WorldSpec)),
                new DevConsoleCommands.Command.Arg("seed", null)
            }, "generate new World using WorldSpec and seed", new Action<string[]>(DevConsoleCommandsImpl_World.Gen_WithWorldSpecAndSeed)),
            new DevConsoleCommands.Command("killall", new DevConsoleCommands.Command.Arg[0], "kill all actors", new Action<string[]>(DevConsoleCommandsImpl_World.KillAll)),
            new DevConsoleCommands.Command("give", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("EntitySpec", typeof(EntitySpec))
            }, "give new Item to player", new Action<string[]>(DevConsoleCommandsImpl_World.Give)),
            new DevConsoleCommands.Command("giveto", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("EntitySpec", typeof(EntitySpec))
            }, "give new Item to selected Actor", new Action<string[]>(DevConsoleCommandsImpl_World.GiveTo)),
            new DevConsoleCommands.Command("addcondition", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("ConditionSpec", typeof(ConditionSpec))
            }, "add new Condition to player", new Action<string[]>(DevConsoleCommandsImpl_World.AddCondition)),
            new DevConsoleCommands.Command("removecondition", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("ConditionSpec", typeof(ConditionSpec))
            }, "remove Condition from player", new Action<string[]>(DevConsoleCommandsImpl_World.RemoveCondition)),
            new DevConsoleCommands.Command("addconditionto", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("ConditionSpec", typeof(ConditionSpec))
            }, "add new Condition to selected Actor", new Action<string[]>(DevConsoleCommandsImpl_World.AddConditionTo)),
            new DevConsoleCommands.Command("removeconditionfrom", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("ConditionSpec", typeof(ConditionSpec))
            }, "remove Condition from selected Actor", new Action<string[]>(DevConsoleCommandsImpl_World.RemoveConditionFrom)),
            new DevConsoleCommands.Command("adduseeffect", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("UseEffectSpec", typeof(UseEffectSpec))
            }, "add new UseEffect to player's weapon", new Action<string[]>(DevConsoleCommandsImpl_World.AddUseEffect)),
            new DevConsoleCommands.Command("removeuseeffect", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("UseEffectSpec", typeof(UseEffectSpec))
            }, "remove UseEffect from player's weapon", new Action<string[]>(DevConsoleCommandsImpl_World.RemoveUseEffect)),
            new DevConsoleCommands.Command("adduseeffectto", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("UseEffectSpec", typeof(UseEffectSpec))
            }, "add new UseEffect to selected Actor's weapon", new Action<string[]>(DevConsoleCommandsImpl_World.AddUseEffectTo)),
            new DevConsoleCommands.Command("removeuseeffectfrom", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("UseEffectSpec", typeof(UseEffectSpec))
            }, "remove UseEffect from selected Actor's weapon", new Action<string[]>(DevConsoleCommandsImpl_World.RemoveUseEffectFrom)),
            new DevConsoleCommands.Command("fly", new DevConsoleCommands.Command.Arg[0], "make player fly", new Action<string[]>(DevConsoleCommandsImpl_World.Fly)),
            new DevConsoleCommands.Command("rampup", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("newValue", null)
            }, "set ramp up for the selected entity", new Action<string[]>(DevConsoleCommandsImpl_World.RampUp)),
            new DevConsoleCommands.Command("setgravity", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("direction", null)
            }, "set player's gravity", new Action<string[]>(DevConsoleCommandsImpl_World.SetGravity)),
            new DevConsoleCommands.Command("setgravityfor", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("direction", null)
            }, "set gravity for the selected Actor", new Action<string[]>(DevConsoleCommandsImpl_World.SetGravityFor)),
            new DevConsoleCommands.Command("setplayerscale", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("scale", null)
            }, "set player's scale", new Action<string[]>(DevConsoleCommandsImpl_World.SetPlayerScale)),
            new DevConsoleCommands.Command("setscale", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("scale", null)
            }, "set scale for the selected Entity", new Action<string[]>(DevConsoleCommandsImpl_World.SetScale)),
            new DevConsoleCommands.Command("control", new DevConsoleCommands.Command.Arg[0], "control the selected Actor", new Action<string[]>(DevConsoleCommandsImpl_World.Control)),
            new DevConsoleCommands.Command("addparty", new DevConsoleCommands.Command.Arg[0], "add the selected Actor to player party", new Action<string[]>(DevConsoleCommandsImpl_World.AddParty)),
            new DevConsoleCommands.Command("removeparty", new DevConsoleCommands.Command.Arg[0], "remove the selected Actor from player party", new Action<string[]>(DevConsoleCommandsImpl_World.RemoveParty)),
            new DevConsoleCommands.Command("rewind", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("turns", null)
            }, "rewind time", new Action<string[]>(DevConsoleCommandsImpl_World.Rewind)),
            new DevConsoleCommands.Command("replay", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("turns", null)
            }, "replay last turns", new Action<string[]>(DevConsoleCommandsImpl_World.Replay)),
            new DevConsoleCommands.Command("unfog", new DevConsoleCommands.Command.Arg[0], "unfog everything", new Action<string[]>(DevConsoleCommandsImpl_World.Unfog)),
            new DevConsoleCommands.Command("level", new DevConsoleCommands.Command.Arg[0], "level up", new Action<string[]>(DevConsoleCommandsImpl_World.Level)),
            new DevConsoleCommands.Command("media", new DevConsoleCommands.Command.Arg[0], "set random player stats for screenshots and videos", new Action<string[]>(DevConsoleCommandsImpl_World.Media)),
            new DevConsoleCommands.Command("debug", new DevConsoleCommands.Command.Arg[0], "toggle logging AI debug info for the selected actor", new Action<string[]>(DevConsoleCommandsImpl_World.Debug)),
            new DevConsoleCommands.Command("teleportto", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("EntitySpec", typeof(EntitySpec))
            }, "teleport to a random entity of the given EntitySpec", new Action<string[]>(DevConsoleCommandsImpl_World.TeleportTo)),
            new DevConsoleCommands.Command("addworldsituation", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("WorldSituationSpec", typeof(WorldSituationSpec))
            }, "add new WorldSituation", new Action<string[]>(DevConsoleCommandsImpl_World.AddWorldSituation)),
            new DevConsoleCommands.Command("removeworldsituation", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("WorldSituationSpec", typeof(WorldSituationSpec))
            }, "remove WorldSituation", new Action<string[]>(DevConsoleCommandsImpl_World.RemoveWorldSituation)),
            new DevConsoleCommands.Command("event", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("WorldEventSpec", typeof(WorldEventSpec))
            }, "invoke a world event", new Action<string[]>(DevConsoleCommandsImpl_World.DoWorldEvent)),
            new DevConsoleCommands.Command("chainattach", new DevConsoleCommands.Command.Arg[0], "attach actor to chain post", new Action<string[]>(DevConsoleCommandsImpl_World.AttachToChainPost)),
            new DevConsoleCommands.Command("test", new DevConsoleCommands.Command.Arg[0], "custom debug behavior", new Action<string[]>(DevConsoleCommandsImpl_World.Test)),
            new DevConsoleCommands.Command("shake", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("amount", null)
            }, "shake camera", new Action<string[]>(DevConsoleCommandsImpl_Visual.Shake)),
            new DevConsoleCommands.Command("wobble", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("amount", null)
            }, "wobble effect", new Action<string[]>(DevConsoleCommandsImpl_Visual.Wobble)),
            new DevConsoleCommands.Command("vignette", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("amount", null)
            }, "do vignette effect", new Action<string[]>(DevConsoleCommandsImpl_Visual.Vignette)),
            new DevConsoleCommands.Command("strike", new DevConsoleCommands.Command.Arg[0], "do strike effect", new Action<string[]>(DevConsoleCommandsImpl_Visual.Strike)),
            new DevConsoleCommands.Command("fall", new DevConsoleCommands.Command.Arg[0], "do fall effect", new Action<string[]>(DevConsoleCommandsImpl_Visual.Fall)),
            new DevConsoleCommands.Command("effect", new DevConsoleCommands.Command.Arg[]
            {
                new DevConsoleCommands.Command.Arg("VisualEffectSpec", typeof(VisualEffectSpec))
            }, "do visual effect", new Action<string[]>(DevConsoleCommandsImpl_Visual.Effect)),
            new DevConsoleCommands.Command("cubemap", new DevConsoleCommands.Command.Arg[0], "make a cubemap from the current scene", new Action<string[]>(DevConsoleCommandsImpl_Visual.MakeCubemap))
        };
    }
}