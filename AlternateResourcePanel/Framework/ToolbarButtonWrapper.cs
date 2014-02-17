////
////  ToolbarWrapper.cs
////
////  Author:
////       toadicus <>
////
////  Generalised a little bit so it stands by itself:
////      TriggerAu
////
////  Copyright (c) 2013 toadicus
////
////  This program is free software: you can redistribute it and/or modify
////  it under the terms of the GNU General Public License as published by
////  the Free Software Foundation, either version 3 of the License, or
////  (at your option) any later version.
////
////  This program is distributed in the hope that it will be useful,
////  but WITHOUT ANY WARRANTY; without even the implied warranty of
////  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////  GNU General Public License for more details.
////
////  You should have received a copy of the GNU General Public License
////  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Linq;
//using System.Reflection;
//using UnityEngine;

//namespace Toolbar
//{
//    /// <summary>
//    /// Wraps a Toolbar clickable button, after fetching it from a foreign assembly.
//    /// </summary>
//    internal class ToolbarButtonWrapper
//    {
//        protected static System.Type ToolbarManager;
//        protected static object TBManagerInstance;
//        protected static MethodInfo TBManagerAdd;

//        /// <summary>
//        /// Wraps the ToolbarManager class, if present.
//        /// </summary>
//        /// <returns><c>true</c>, if ToolbarManager is wrapped, <c>false</c> otherwise.</returns>
//        protected static bool TryWrapToolbarManager()
//        {
//            if (ToolbarManager == null)
//            {
//                LogFormatted("Loading ToolbarManager.");

//                ToolbarManager = AssemblyLoader.loadedAssemblies
//                    .Select(a => a.assembly.GetExportedTypes())
//                        .SelectMany(t => t)
//                        .FirstOrDefault(t => t.FullName == "Toolbar.ToolbarManager");

//                if (ToolbarManager == null)
//                {
//                    return false;
//                }

//                LogFormatted_DebugOnly("Loaded ToolbarManager.  Getting Instance.");

//                TBManagerInstance = ToolbarManager.GetProperty(
//                    "Instance",
//                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
//                    )
//                    .GetValue(null, null);

//                LogFormatted_DebugOnly("Got ToolbarManager Instance '{0}'.  Getting 'add' method.",
//                    TBManagerInstance
//                    );

//                TBManagerAdd = ToolbarManager.GetMethod("add");

//                LogFormatted_DebugOnly( "Got ToolbarManager Instance 'add' method.");
//            }

//            return true;
//        }

//        /// <summary>
//        /// Gets a value indicating whether <see cref="Toolbar.ToolbarManager"/> is present.
//        /// </summary>
//        /// <value><c>true</c>, if ToolbarManager is wrapped, <c>false</c> otherwise.</value>
//        public static bool ToolbarManagerPresent
//        {
//            get
//            {
//                return TryWrapToolbarManager();
//            }
//        }

//        /// <summary>
//        /// If ToolbarManager is present, initializes a new instance of the <see cref="VOID.ToolbarButtonWrapper"/> class.
//        /// </summary>
//        /// <param name="ns">Namespace, usually the plugin name.</param>
//        /// <param name="id">Identifier, unique per namespace.</param>
//        /// <returns>If ToolbarManager is present, a new <see cref="Toolbar.IButton"/> object, <c>null</c> otherwise.</returns>
//        public static ToolbarButtonWrapper TryWrapToolbarButton(string ns, string id)
//        {
//            if (ToolbarManagerPresent)
//            {
//                object button = TBManagerAdd.Invoke(TBManagerInstance, new object[] { ns, id });

//                LogFormatted_DebugOnly("Added Button '{0}' with ToolbarManager.  Getting 'Text' property",
//                    button.ToString()
//                );

//                return new ToolbarButtonWrapper(button);
//            }
//            else
//            {
//                return null;
//            }
//        }

//        protected System.Type IButton;
//        protected object Button;
//        protected PropertyInfo ButtonText;
//        protected PropertyInfo ButtonTextColor;
//        protected PropertyInfo ButtonTexturePath;
//        protected PropertyInfo ButtonToolTip;
//        protected PropertyInfo ButtonVisible;
//        protected PropertyInfo ButtonVisibility;
//        protected PropertyInfo ButtonEffectivelyVisible;
//        protected PropertyInfo ButtonEnabled;
//        protected PropertyInfo ButtonImportant;
//        protected EventInfo ButtonOnClick;
//        protected EventInfo ButtonOnMouseEnter;
//        protected EventInfo ButtonOnMouseLeave;
//        protected MethodInfo ButtonDestroy;
//        protected System.Type GameScenesVisibilityType;

//        /// <summary>
//        /// The text displayed on the button. Set to null to hide text.
//        /// </summary>
//        /// <remarks>
//        /// The text can be changed at any time to modify the button's appearance. Note that since this will also
//        /// modify the button's size, this feature should be used sparingly, if at all.
//        /// </remarks>
//        /// <seealso cref="TexturePath"/>
//        public string Text
//        {
//            get
//            {
//                return this.ButtonText.GetValue(this.Button, null) as String;
//            }
//            set
//            {
//                this.ButtonText.SetValue(this.Button, value, null);
//            }
//        }

//        /// <summary>
//        /// The color the button text is displayed with. Defaults to Color.white.
//        /// </summary>
//        /// <remarks>
//        /// The text color can be changed at any time to modify the button's appearance.
//        /// </remarks>
//        public Color TextColor
//        {
//            get
//            {
//                return (Color)this.ButtonTextColor.GetValue(this.Button, null);
//            }
//            set
//            {
//                this.ButtonTextColor.SetValue(this.Button, value, null);
//            }
//        }

//        /// <summary>
//        /// The path of a texture file to display an icon on the button. Set to null to hide icon.
//        /// </summary>
//        /// <remarks>
//        /// <para>
//        /// A texture path on a button will have precedence over text. That is, if both text and texture path
//        /// have been set on a button, the button will show the texture, not the text.
//        /// </para>
//        /// <para>
//        /// The texture size must not exceed 24x24 pixels.
//        /// </para>
//        /// <para>
//        /// The texture path must be relative to the "GameData" directory, and must not specify a file name suffix.
//        /// Valid example: MyAddon/Textures/icon_mybutton
//        /// </para>
//        /// <para>
//        /// The texture path can be changed at any time to modify the button's appearance.
//        /// </para>
//        /// </remarks>
//        /// <seealso cref="Text"/>
//        public string TexturePath
//        {
//            get
//            {
//                return this.ButtonTexturePath.GetValue(this.Button, null) as string;
//            }
//            set
//            {
//                this.ButtonTexturePath.SetValue(this.Button, value, null);
//            }
//        }

//        /// <summary>
//        /// The button's tool tip text. Set to null if no tool tip is desired.
//        /// </summary>
//        /// <remarks>
//        /// Tool Tip Text Should Always Use Headline Style Like This.
//        /// </remarks>
//        public string ToolTip
//        {
//            get
//            {
//                return this.ButtonToolTip.GetValue(this.Button, null) as string;
//            }
//            set
//            {
//                this.ButtonToolTip.SetValue(this.Button, value, null);
//            }
//        }

//        /// <summary>
//        /// Whether this button is currently visible or not. Can be used in addition to or as a replacement for <see cref="Visibility"/>.
//        /// </summary>
//        public bool Visible
//        {
//            get
//            {
//                return (bool)this.ButtonVisible.GetValue(this.Button, null);
//            }
//            set
//            {
//                this.ButtonVisible.SetValue(this.Button, value, null);
//            }
//        }

//        /// <summary>
//        /// Whether this button is currently effectively visible or not. This is a combination of
//        /// <see cref="Visible"/> and <see cref="Visibility"/>.
//        /// </summary>
//        /// <remarks>
//        /// Note that the toolbar is not visible in certain game scenes, for example the loading screens. This property
//        /// does not reflect button invisibility in those scenes.
//        /// </remarks>
//        public bool EffectivelyVisible
//        {
//            get
//            {
//                return (bool)this.ButtonEffectivelyVisible.GetValue(this.Button, null);
//            }
//        }

//        /// <summary>
//        /// Whether this button is currently enabled (clickable) or not. This will not affect the player's ability to
//        /// position the button on their screen.
//        /// </summary>
//        public bool Enabled
//        {
//            get
//            {
//                return (bool)this.ButtonEnabled.GetValue(this.Button, null);
//            }
//            set
//            {
//                this.ButtonEnabled.SetValue(this.Button, value, null);
//            }
//        }

//        /// <summary>
//        /// Whether this button is currently "important."  Set to false to return to normal button behaviour.
//        /// </summary>
//        /// <remarks>
//        /// <para>
//        /// This can be used to temporarily force the button to be shown on the screen regardless of the toolbar being
//        /// currently in auto-hidden mode.  For example, a button that signals the arrival of a private message in a
//        /// chat room could mark itself as "important" as long as the message has not been read.
//        /// </para>
//        /// <para>
//        /// Setting this property does not change the appearance of the button.  use <see cref="TexturePath"/>  to
//        /// change the button's icon.
//        /// </para>
//        /// <para>
//        /// This feature should be used only sparingly, if at all, since it forces the button to be displayed on screen
//        /// even when it normally wouldn't.
//        /// </para>
//        /// </remarks>
//        /// <value><c>true</c> if important; otherwise, <c>false</c>.</value>
//        public bool Important
//        {
//            get
//            {
//                return (bool)this.ButtonImportant.GetValue(this.Button, null);
//            }
//            set
//            {
//                this.ButtonImportant.SetValue(this.Button, value, null);
//            }
//        }

//        private ToolbarButtonWrapper()
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="VOID.ToolbarButtonWrapper"/> class.
//        /// </summary>
//        /// <param name="ns">Namespace, usually the plugin name.</param>
//        /// <param name="id">Identifier, unique per namespace.</param>
//        protected ToolbarButtonWrapper(object button)
//        {
//            this.Button = button;

//            this.IButton = AssemblyLoader.loadedAssemblies
//                .Select(a => a.assembly.GetExportedTypes())
//                .SelectMany(t => t)
//                .FirstOrDefault(t => t.FullName == "Toolbar.IButton");
//            LogFormatted_DebugOnly("Loaded IButton.  Adding Button with ToolbarManager.");

//            this.ButtonText = this.IButton.GetProperty("Text");
//            LogFormatted_DebugOnly("Got 'Text' property.  Getting 'TextColor' property.");

//            this.ButtonTextColor = this.IButton.GetProperty("TextColor");
//            LogFormatted_DebugOnly("Got 'TextColor' property.  Getting 'TexturePath' property.");

//            this.ButtonTexturePath = this.IButton.GetProperty("TexturePath");
//            LogFormatted_DebugOnly("Got 'TexturePath' property.  Getting 'ToolTip' property.");

//            this.ButtonToolTip = this.IButton.GetProperty("ToolTip");
//            LogFormatted_DebugOnly("Got 'ToolTip' property.  Getting 'Visible' property.");

//            this.ButtonVisible = this.IButton.GetProperty("Visible");
//            LogFormatted_DebugOnly("Got 'Visible' property.  Getting 'Visibility' property.");

//            this.ButtonVisibility = this.IButton.GetProperty("Visibility");
//            LogFormatted_DebugOnly("Got 'Visibility' property.  Getting 'EffectivelyVisible' property.");

//            this.ButtonEffectivelyVisible = this.IButton.GetProperty("EffectivelyVisible");
//            LogFormatted_DebugOnly("Got 'Visibility' property.  Getting 'Enabled' property.");

//            this.ButtonEnabled = this.IButton.GetProperty("Enabled");
//            LogFormatted_DebugOnly("Got 'Enabled' property.  Getting 'Important' event.");

//            this.ButtonImportant = this.IButton.GetProperty("Important");
//            LogFormatted_DebugOnly("Got 'Important' property.  Getting 'OnClick' event.");

//            this.ButtonOnClick = this.IButton.GetEvent("OnClick");
//            LogFormatted_DebugOnly("Got 'OnClick' event '{0}'.  Getting 'OnMouseEnter' event.",
//                this.ButtonOnClick.ToString()
//            );

//            this.ButtonOnMouseEnter = this.IButton.GetEvent("OnMouseEnter");
//            LogFormatted_DebugOnly("Got 'OnMouseEnter' event '{0}'.  Getting 'OnMouseLeave' event.",
//                this.ButtonOnMouseEnter.ToString()
//                );

//            this.ButtonOnMouseLeave = this.IButton.GetEvent("OnMouseLeave");
//            LogFormatted_DebugOnly("Got 'OnMouseLeave' event '{0}'.  Getting 'Destroy' method.",
//                this.ButtonOnMouseLeave.ToString()
//               );

//            this.ButtonDestroy = this.IButton.GetMethod("Destroy");
//            LogFormatted_DebugOnly("Got 'Destroy' property '{0}'.  Loading GameScenesVisibility class.",
//                this.ButtonDestroy.ToString()
//            );

//            this.GameScenesVisibilityType = AssemblyLoader.loadedAssemblies
//                .Select(a => a.assembly.GetExportedTypes())
//                    .SelectMany(t => t)
//                    .FirstOrDefault(t => t.FullName == "Toolbar.GameScenesVisibility");

//            LogFormatted_DebugOnly("Got 'GameScenesVisibility' class '{0}'.",
//                this.GameScenesVisibilityType.ToString()
//            );

//            LogFormatted_DebugOnly("ToolbarButtonWrapper built!");
//        }

//        /// <summary>
//        /// Adds event handler to receive "on click" events.
//        /// </summary>
//        /// <example>
//        /// <code>
//        /// ToolbarButtonWrapper button = ...
//        /// button.AddButtonClickHandler(
//        /// 	(e) =>
//        /// 	{
//        /// 		Debug.Log("button clicked, mouseButton: " + e.Mousebutton");
//        /// 	}
//        /// );
//        /// </code>
//        /// </example>
//        /// <param name="Handler">Delegate to handle "on click" events</param>
//        public void AddButtonClickHandler(Action<object> Handler)
//        {
//            this.AddButtonEventHandler(this.ButtonOnClick, Handler);
//        }

//        /// <summary>
//        /// Adds event handler that can be registered with to receive "on mouse enter" events.
//        /// </summary>
//        /// <example>
//        /// <code>
//        /// ToolbarWrapperButton button = ...
//        /// button.AddButtonOnMouseEnterHandler(
//        /// 	(e) =>
//        /// 	{
//        /// 		Debug.Log("mouse entered button");
//        /// 	}
//        /// );
//        /// </code>
//        /// </example>
//        /// <param name="Handler">Delegate to handle "OnMouseEnter" events.</param>
//        public void AddButtonOnMouseEnterHandler(Action<object> Handler)
//        {
//            this.AddButtonEventHandler(this.ButtonOnMouseEnter, Handler);
//        }

//        /// <summary>
//        /// Adds event handler that can be registered with to receive "on mouse leave" events.
//        /// </summary>
//        /// <example>
//        /// <code>
//        /// ToolbarWrapperButton button = ...
//        /// button.AddButtonOnMouseLeaveHandler(
//        /// 	(e) =>
//        /// 	{
//        /// 		Debug.Log("mouse left button");
//        /// 	}
//        /// );
//        /// </code>
//        /// </example>
//        /// <param name="Handler">Delegate to handle "OnMouseLeave" events.</param>
//        public void AddButtonOnMouseLeaveHandler(Action<object> Handler)
//        {
//            this.AddButtonEventHandler(this.ButtonOnMouseLeave, Handler);
//        }

//        /// <summary>
//        /// Sets this button's visibility. Can be used in addition to or as a replacement for <see cref="Visible"/>.
//        /// </summary>
//        /// <param name="gameScenes">Array of GameScene objects in which the button should be visible.</param>
//        public void SetButtonVisibility(params GameScenes[] gameScenes)
//        {
//            object GameScenesVisibilityObj = Activator.CreateInstance(this.GameScenesVisibilityType, gameScenes);
//            this.ButtonVisibility.SetValue(this.Button, GameScenesVisibilityObj, null);
//        }

//        /// <summary>
//        /// Permanently destroys this button so that it is no longer displayed.
//        /// Should be used when a plugin is stopped to remove leftover buttons.
//        /// </summary>
//        public void Destroy()
//        {
//            this.ButtonDestroy.Invoke(this.Button, null);
//        }

//        // Utility method for use with the AddButton<event>Handler API methods.
//        protected void AddButtonEventHandler(EventInfo Event, Action<object> Handler)
//        {
//            Delegate d = Delegate.CreateDelegate(Event.EventHandlerType, Handler.Target, Handler.Method);
//            MethodInfo addHandler = Event.GetAddMethod();
//            addHandler.Invoke(this.Button, new object[] { d });
//        }

//        #region Logging Stuff
//        [System.Diagnostics.Conditional("DEBUG")]
//        internal static void LogFormatted_DebugOnly(String Message, params object[] strParams)
//        {
//            LogFormatted(Message, strParams);
//        }

//        /// <summary>
//        /// Some Structured logging to the debug file
//        /// </summary>
//        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
//        /// <param name="strParams">Objects to feed into a String.format</param>
//        internal static void LogFormatted(String Message, params object[] strParams)
//        {
//            Message = String.Format(Message, strParams);
//            String strMessageLine = String.Format("{0},{2}-{3},{1}",
//                DateTime.Now, Message, Assembly.GetExecutingAssembly().GetName().Name,
//                MethodBase.GetCurrentMethod().DeclaringType.Name);
//            UnityEngine.Debug.Log(strMessageLine);
//        }
//        #endregion
//    }
//}
