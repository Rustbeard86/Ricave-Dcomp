using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class WindowSpec : Spec
    {
        public Type WindowType
        {
            get
            {
                return this.windowType;
            }
        }

        public Vector2 Size
        {
            get
            {
                return this.size;
            }
        }

        public MainTabSpec AutoOpenForMainTab
        {
            get
            {
                return this.autoOpenForMainTab;
            }
        }

        public bool AutoOpenInUIModeInLobby
        {
            get
            {
                return this.autoOpenInUIModeInLobby;
            }
        }

        public bool CanDragWindow
        {
            get
            {
                return this.canDragWindow;
            }
        }

        public Vector2 Offset
        {
            get
            {
                return this.offset;
            }
        }

        public float Padding
        {
            get
            {
                return this.padding;
            }
        }

        public float TitlePadding
        {
            get
            {
                return this.titlePadding;
            }
        }

        public bool MaxOne
        {
            get
            {
                return this.maxOne;
            }
        }

        public bool CloseOnEscapeKey
        {
            get
            {
                return this.closeOnEscapeKey;
            }
        }

        public bool CloseOnEnterKey
        {
            get
            {
                return this.closeOnEnterKey;
            }
        }

        public bool CloseOnLostFocus
        {
            get
            {
                return this.closeOnLostFocus;
            }
        }

        [Saved(typeof(Window), false)]
        private Type windowType = typeof(Window);

        [Saved]
        private Vector2 size;

        [Saved]
        private MainTabSpec autoOpenForMainTab;

        [Saved]
        private bool autoOpenInUIModeInLobby;

        [Saved]
        private bool canDragWindow;

        [Saved]
        private Vector2 offset;

        [Saved(20f, false)]
        private float padding = 20f;

        [Saved(20f, false)]
        private float titlePadding = 20f;

        [Saved(true, false)]
        private bool maxOne = true;

        [Saved]
        private bool closeOnEscapeKey;

        [Saved]
        private bool closeOnEnterKey;

        [Saved]
        private bool closeOnLostFocus;

        public const float DefaultPadding = 20f;
    }
}