using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_StaircaseConfirm : UsePrompt_Confirm
    {
        protected override string GetPostProcessedDescription()
        {
            string text = base.GetPostProcessedDescription();
            bool flag = true;
            List<Actor> followers = Get.Player.Followers;
            for (int i = 0; i < followers.Count; i++)
            {
                Actor actor = followers[i];
                if (!Get.Player.FollowerCanFollowPlayerIntoNextFloor(actor, true))
                {
                    if (flag)
                    {
                        flag = false;
                        text = text.AppendedInDoubleNewLine(RichText.Red("{0}:".Formatted("TheseFollowersWillBeLeftBehind".Translate())));
                    }
                    text = text.AppendedInNewLine(RichText.Red(actor.LabelCap));
                }
            }
            return text;
        }
    }
}