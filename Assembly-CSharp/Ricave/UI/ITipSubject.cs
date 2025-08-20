using System;
using UnityEngine;

namespace Ricave.UI
{
    public interface ITipSubject
    {
        string LabelCap { get; }

        string Description { get; }

        Texture2D Icon { get; }

        Color IconColor { get; }
    }
}