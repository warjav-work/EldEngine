using EldEngine.Core.Application.Interfaces;

namespace EldEngine.GameTest.Inputs
{
    /// <summary>
    /// Mapeo completo entre Windows Forms Keys y nuestro KeyCode enum.
    /// Basado en la tabla oficial de teclas Windows Forms.
    /// </summary>
    public static class KeyMapping
    {
        private static readonly Dictionary<System.Windows.Forms.Keys, KeyCode> FormsToKeyCode =
            new Dictionary<System.Windows.Forms.Keys, KeyCode>()
            {
                // ==================== MODIFICADORES ====================
                { System.Windows.Forms.Keys.LShiftKey, KeyCode.LeftShift },
                { System.Windows.Forms.Keys.RShiftKey, KeyCode.RightShift },
                { System.Windows.Forms.Keys.LControlKey, KeyCode.LeftControl },
                { System.Windows.Forms.Keys.RControlKey, KeyCode.RightControl },
                { System.Windows.Forms.Keys.LMenu, KeyCode.LeftAlt },
                { System.Windows.Forms.Keys.RMenu, KeyCode.RightAlt },
                { System.Windows.Forms.Keys.LWin, KeyCode.LeftWin },
                { System.Windows.Forms.Keys.RWin, KeyCode.RightWin },
                { System.Windows.Forms.Keys.ShiftKey, KeyCode.Shift },
                { System.Windows.Forms.Keys.ControlKey, KeyCode.Control },
                { System.Windows.Forms.Keys.Menu, KeyCode.Alt },

                // ==================== LETRAS A-Z ====================
                { System.Windows.Forms.Keys.A, KeyCode.A },     { System.Windows.Forms.Keys.B, KeyCode.B },
                { System.Windows.Forms.Keys.C, KeyCode.C },     { System.Windows.Forms.Keys.D, KeyCode.D },
                { System.Windows.Forms.Keys.E, KeyCode.E },     { System.Windows.Forms.Keys.F, KeyCode.F },
                { System.Windows.Forms.Keys.G, KeyCode.G },     { System.Windows.Forms.Keys.H, KeyCode.H },
                { System.Windows.Forms.Keys.I, KeyCode.I },     { System.Windows.Forms.Keys.J, KeyCode.J },
                { System.Windows.Forms.Keys.K, KeyCode.K },     { System.Windows.Forms.Keys.L, KeyCode.L },
                { System.Windows.Forms.Keys.M, KeyCode.M },     { System.Windows.Forms.Keys.N, KeyCode.N },
                { System.Windows.Forms.Keys.O, KeyCode.O },     { System.Windows.Forms.Keys.P, KeyCode.P },
                { System.Windows.Forms.Keys.Q, KeyCode.Q },     { System.Windows.Forms.Keys.R, KeyCode.R },
                { System.Windows.Forms.Keys.S, KeyCode.S },     { System.Windows.Forms.Keys.T, KeyCode.T },
                { System.Windows.Forms.Keys.U, KeyCode.U },     { System.Windows.Forms.Keys.V, KeyCode.V },
                { System.Windows.Forms.Keys.W, KeyCode.W },     { System.Windows.Forms.Keys.X, KeyCode.X },
                { System.Windows.Forms.Keys.Y, KeyCode.Y },     { System.Windows.Forms.Keys.Z, KeyCode.Z },

                // ==================== NÚMEROS ====================
                { System.Windows.Forms.Keys.D0, KeyCode.D0 },   { System.Windows.Forms.Keys.D1, KeyCode.D1 },
                { System.Windows.Forms.Keys.D2, KeyCode.D2 },   { System.Windows.Forms.Keys.D3, KeyCode.D3 },
                { System.Windows.Forms.Keys.D4, KeyCode.D4 },   { System.Windows.Forms.Keys.D5, KeyCode.D5 },
                { System.Windows.Forms.Keys.D6, KeyCode.D6 },   { System.Windows.Forms.Keys.D7, KeyCode.D7 },
                { System.Windows.Forms.Keys.D8, KeyCode.D8 },   { System.Windows.Forms.Keys.D9, KeyCode.D9 },

                // ==================== TECLADO NUMÉRICO ====================
                { System.Windows.Forms.Keys.NumPad0, KeyCode.NumPad0 },
                { System.Windows.Forms.Keys.NumPad1, KeyCode.NumPad1 },
                { System.Windows.Forms.Keys.NumPad2, KeyCode.NumPad2 },
                { System.Windows.Forms.Keys.NumPad3, KeyCode.NumPad3 },
                { System.Windows.Forms.Keys.NumPad4, KeyCode.NumPad4 },
                { System.Windows.Forms.Keys.NumPad5, KeyCode.NumPad5 },
                { System.Windows.Forms.Keys.NumPad6, KeyCode.NumPad6 },
                { System.Windows.Forms.Keys.NumPad7, KeyCode.NumPad7 },
                { System.Windows.Forms.Keys.NumPad8, KeyCode.NumPad8 },
                { System.Windows.Forms.Keys.NumPad9, KeyCode.NumPad9 },
                { System.Windows.Forms.Keys.Multiply, KeyCode.Multiply },
                { System.Windows.Forms.Keys.Add, KeyCode.Add },
                { System.Windows.Forms.Keys.Separator, KeyCode.Separator },
                { System.Windows.Forms.Keys.Subtract, KeyCode.Subtract },
                { System.Windows.Forms.Keys.Decimal, KeyCode.Decimal },
                { System.Windows.Forms.Keys.Divide, KeyCode.Divide },

                // ==================== NAVEGACIÓN ====================
                { System.Windows.Forms.Keys.Home, KeyCode.Home },
                { System.Windows.Forms.Keys.End, KeyCode.End },
                { System.Windows.Forms.Keys.Prior, KeyCode.PageUp },    // PageUp
                { System.Windows.Forms.Keys.Next, KeyCode.PageDown },   // PageDown
                { System.Windows.Forms.Keys.PageUp, KeyCode.PageUp },
                { System.Windows.Forms.Keys.PageDown, KeyCode.PageDown },
                { System.Windows.Forms.Keys.Up, KeyCode.Up },
                { System.Windows.Forms.Keys.Down, KeyCode.Down },
                { System.Windows.Forms.Keys.Left, KeyCode.Left },
                { System.Windows.Forms.Keys.Right, KeyCode.Right },
                { System.Windows.Forms.Keys.Insert, KeyCode.Insert },
                { System.Windows.Forms.Keys.Delete, KeyCode.Delete },

                // ==================== ACCIONES ====================
                { System.Windows.Forms.Keys.Space, KeyCode.Space },
                { System.Windows.Forms.Keys.Enter, KeyCode.Enter },
                { System.Windows.Forms.Keys.Return, KeyCode.Return },
                { System.Windows.Forms.Keys.Escape, KeyCode.Escape },
                { System.Windows.Forms.Keys.Tab, KeyCode.Tab },
                { System.Windows.Forms.Keys.Back, KeyCode.Back },
                { System.Windows.Forms.Keys.Clear, KeyCode.Clear },

                // ==================== TECLAS DE FUNCIÓN ====================
                { System.Windows.Forms.Keys.F1, KeyCode.F1 },    { System.Windows.Forms.Keys.F2, KeyCode.F2 },
                { System.Windows.Forms.Keys.F3, KeyCode.F3 },    { System.Windows.Forms.Keys.F4, KeyCode.F4 },
                { System.Windows.Forms.Keys.F5, KeyCode.F5 },    { System.Windows.Forms.Keys.F6, KeyCode.F6 },
                { System.Windows.Forms.Keys.F7, KeyCode.F7 },    { System.Windows.Forms.Keys.F8, KeyCode.F8 },
                { System.Windows.Forms.Keys.F9, KeyCode.F9 },    { System.Windows.Forms.Keys.F10, KeyCode.F10 },
                { System.Windows.Forms.Keys.F11, KeyCode.F11 },  { System.Windows.Forms.Keys.F12, KeyCode.F12 },
                { System.Windows.Forms.Keys.F13, KeyCode.F13 },  { System.Windows.Forms.Keys.F14, KeyCode.F14 },
                { System.Windows.Forms.Keys.F15, KeyCode.F15 },  { System.Windows.Forms.Keys.F16, KeyCode.F16 },
                { System.Windows.Forms.Keys.F17, KeyCode.F17 },  { System.Windows.Forms.Keys.F18, KeyCode.F18 },
                { System.Windows.Forms.Keys.F19, KeyCode.F19 },  { System.Windows.Forms.Keys.F20, KeyCode.F20 },
                { System.Windows.Forms.Keys.F21, KeyCode.F21 },  { System.Windows.Forms.Keys.F22, KeyCode.F22 },
                { System.Windows.Forms.Keys.F23, KeyCode.F23 },  { System.Windows.Forms.Keys.F24, KeyCode.F24 },

                // ==================== BLOQUEOS ====================
                { System.Windows.Forms.Keys.Capital, KeyCode.CapsLock },
                { System.Windows.Forms.Keys.CapsLock, KeyCode.CapsLock },
                { System.Windows.Forms.Keys.NumLock, KeyCode.NumLock },
                { System.Windows.Forms.Keys.Scroll, KeyCode.Scroll },

                // ==================== MULTIMEDIA ====================
                { System.Windows.Forms.Keys.VolumeUp, KeyCode.VolumeUp },
                { System.Windows.Forms.Keys.VolumeDown, KeyCode.VolumeDown },
                { System.Windows.Forms.Keys.VolumeMute, KeyCode.VolumeMute },
                { System.Windows.Forms.Keys.MediaPlayPause, KeyCode.MediaPlayPause },
                { System.Windows.Forms.Keys.MediaStop, KeyCode.MediaStop },
                { System.Windows.Forms.Keys.MediaPreviousTrack, KeyCode.MediaPreviousTrack },
                { System.Windows.Forms.Keys.MediaNextTrack, KeyCode.MediaNextTrack },
                { System.Windows.Forms.Keys.SelectMedia, KeyCode.SelectMedia },
                { System.Windows.Forms.Keys.LaunchMail, KeyCode.LaunchMail },
                { System.Windows.Forms.Keys.LaunchApplication1, KeyCode.LaunchApplication1 },
                { System.Windows.Forms.Keys.LaunchApplication2, KeyCode.LaunchApplication2 },
                { System.Windows.Forms.Keys.BrowserHome, KeyCode.BrowserHome },
                { System.Windows.Forms.Keys.BrowserBack, KeyCode.BrowserBack },
                { System.Windows.Forms.Keys.BrowserForward, KeyCode.BrowserForward },
                { System.Windows.Forms.Keys.BrowserRefresh, KeyCode.BrowserRefresh },
                { System.Windows.Forms.Keys.BrowserStop, KeyCode.BrowserStop },
                { System.Windows.Forms.Keys.BrowserSearch, KeyCode.BrowserSearch },
                { System.Windows.Forms.Keys.BrowserFavorites, KeyCode.BrowserFavorites },

                // ==================== ESPECIALES OEM ====================
                { System.Windows.Forms.Keys.Oem1, KeyCode.Oem1 },
                { System.Windows.Forms.Keys.Oemplus, KeyCode.Oemplus },
                { System.Windows.Forms.Keys.Oemcomma, KeyCode.Oemcomma },
                { System.Windows.Forms.Keys.OemMinus, KeyCode.OemMinus },
                { System.Windows.Forms.Keys.OemPeriod, KeyCode.OemPeriod },
                { System.Windows.Forms.Keys.Oem2, KeyCode.Oem2 },
                { System.Windows.Forms.Keys.Oem3, KeyCode.Oem3 },
                { System.Windows.Forms.Keys.Oem4, KeyCode.Oem4 },
                { System.Windows.Forms.Keys.Oem5, KeyCode.Oem5 },
                { System.Windows.Forms.Keys.Oem6, KeyCode.Oem6 },
                { System.Windows.Forms.Keys.OemQuotes, KeyCode.OemQuotes },
                { System.Windows.Forms.Keys.Oem8, KeyCode.Oem8 },
                { System.Windows.Forms.Keys.OemBackslash, KeyCode.OemBackslash },

                // ==================== SISTEMA ====================
                { System.Windows.Forms.Keys.Apps, KeyCode.Apps },
                { System.Windows.Forms.Keys.PrintScreen, KeyCode.PrintScreen },
                { System.Windows.Forms.Keys.Snapshot, KeyCode.PrintScreen },  // Alias
                { System.Windows.Forms.Keys.Pause, KeyCode.Pause },
                { System.Windows.Forms.Keys.Print, KeyCode.Print },
                { System.Windows.Forms.Keys.Help, KeyCode.Help },
                { System.Windows.Forms.Keys.Execute, KeyCode.Execute },

                // ==================== MOUSE ====================
                { System.Windows.Forms.Keys.LButton, KeyCode.LButton },
                { System.Windows.Forms.Keys.RButton, KeyCode.RButton },
                { System.Windows.Forms.Keys.MButton, KeyCode.MButton },
                { System.Windows.Forms.Keys.XButton1, KeyCode.XButton1 },
                { System.Windows.Forms.Keys.XButton2, KeyCode.XButton2 },

                // ==================== IME ====================
                { System.Windows.Forms.Keys.IMEConvert, KeyCode.IMEConvert },
                { System.Windows.Forms.Keys.IMENonconvert, KeyCode.IMENonconvert },
                { System.Windows.Forms.Keys.IMEAccept, KeyCode.IMEAccept },
                { System.Windows.Forms.Keys.IMEModeChange, KeyCode.IMEModeChange },

                // ==================== OTROS ====================
                { System.Windows.Forms.Keys.Sleep, KeyCode.Sleep },
                { System.Windows.Forms.Keys.Zoom, KeyCode.Zoom },
                { System.Windows.Forms.Keys.Play, KeyCode.Play },
                { System.Windows.Forms.Keys.Select, KeyCode.Select },
                { System.Windows.Forms.Keys.Cancel, KeyCode.Cancel },
                { System.Windows.Forms.Keys.ProcessKey, KeyCode.ProcessKey },
                { System.Windows.Forms.Keys.Attn, KeyCode.Attn },
                { System.Windows.Forms.Keys.Crsel, KeyCode.Crsel },
                { System.Windows.Forms.Keys.Exsel, KeyCode.Exsel },
                { System.Windows.Forms.Keys.EraseEof, KeyCode.EraseEof },
                { System.Windows.Forms.Keys.Pa1, KeyCode.Pa1 },
                { System.Windows.Forms.Keys.OemClear, KeyCode.OemClear },
            };

        /// <summary>
        /// Convierte de Windows Forms Keys a nuestro KeyCode
        /// </summary>
        public static KeyCode Convert(System.Windows.Forms.Keys key)
        {
            if (FormsToKeyCode.TryGetValue(key, out var keyCode))
                return keyCode;

            System.Diagnostics.Debug.WriteLine($"⚠️ Tecla no mapeada: {key}");
            return KeyCode.None;
        }

        /// <summary>
        /// Obtiene el nombre legible de una tecla.
        /// </summary>
        public static string GetKeyName(KeyCode keyCode)
        {
            return keyCode switch
            {
                // Modificadores
                KeyCode.LeftShift => "Shift (Izq)",
                KeyCode.RightShift => "Shift (Der)",
                KeyCode.LeftControl => "Ctrl (Izq)",
                KeyCode.RightControl => "Ctrl (Der)",
                KeyCode.LeftAlt => "Alt (Izq)",
                KeyCode.RightAlt => "Alt (Der)",
                KeyCode.LeftWin => "Win (Izq)",
                KeyCode.RightWin => "Win (Der)",

                // Letras
                KeyCode.A => "A",
                KeyCode.B => "B",
                KeyCode.C => "C",
                KeyCode.D => "D",
                KeyCode.E => "E",
                KeyCode.F => "F",
                KeyCode.G => "G",
                KeyCode.H => "H",
                KeyCode.I => "I",
                KeyCode.J => "J",
                KeyCode.K => "K",
                KeyCode.L => "L",
                KeyCode.M => "M",
                KeyCode.N => "N",
                KeyCode.O => "O",
                KeyCode.P => "P",
                KeyCode.Q => "Q",
                KeyCode.R => "R",
                KeyCode.S => "S",
                KeyCode.T => "T",
                KeyCode.U => "U",
                KeyCode.V => "V",
                KeyCode.W => "W",
                KeyCode.X => "X",
                KeyCode.Y => "Y",
                KeyCode.Z => "Z",

                // Números
                KeyCode.D0 => "0",
                KeyCode.D1 => "1",
                KeyCode.D2 => "2",
                KeyCode.D3 => "3",
                KeyCode.D4 => "4",
                KeyCode.D5 => "5",
                KeyCode.D6 => "6",
                KeyCode.D7 => "7",
                KeyCode.D8 => "8",
                KeyCode.D9 => "9",

                // Teclado numérico
                KeyCode.NumPad0 => "NumPad 0",
                KeyCode.NumPad1 => "NumPad 1",
                KeyCode.NumPad2 => "NumPad 2",
                KeyCode.NumPad3 => "NumPad 3",
                KeyCode.NumPad4 => "NumPad 4",
                KeyCode.NumPad5 => "NumPad 5",
                KeyCode.NumPad6 => "NumPad 6",
                KeyCode.NumPad7 => "NumPad 7",
                KeyCode.NumPad8 => "NumPad 8",
                KeyCode.NumPad9 => "NumPad 9",
                KeyCode.Multiply => "NumPad *",
                KeyCode.Add => "NumPad +",
                KeyCode.Subtract => "NumPad -",
                KeyCode.Decimal => "NumPad .",
                KeyCode.Divide => "NumPad /",

                // Navegación
                KeyCode.Home => "Home",
                KeyCode.End => "End",
                KeyCode.PageUp => "PgUp",
                KeyCode.PageDown => "PgDn",
                KeyCode.Up => "↑",
                KeyCode.Down => "↓",
                KeyCode.Left => "←",
                KeyCode.Right => "→",
                KeyCode.Insert => "Ins",
                KeyCode.Delete => "Del",

                // Acciones
                KeyCode.Space => "Space",
                KeyCode.Enter => "Enter",
                KeyCode.Escape => "Esc",
                KeyCode.Tab => "Tab",
                KeyCode.Back => "Backspace",

                // Funciones
                KeyCode.F1 => "F1",
                KeyCode.F2 => "F2",
                KeyCode.F3 => "F3",
                KeyCode.F4 => "F4",
                KeyCode.F5 => "F5",
                KeyCode.F6 => "F6",
                KeyCode.F7 => "F7",
                KeyCode.F8 => "F8",
                KeyCode.F9 => "F9",
                KeyCode.F10 => "F10",
                KeyCode.F11 => "F11",
                KeyCode.F12 => "F12",

                // Multimedia
                KeyCode.VolumeUp => "🔊 Subir",
                KeyCode.VolumeDown => "🔉 Bajar",
                KeyCode.VolumeMute => "🔇 Mute",
                KeyCode.MediaPlayPause => "▶⏸",
                KeyCode.MediaStop => "⏹",

                _ => keyCode.ToString()
            };
        }

        /// <summary>
        /// Obtiene todas las teclas mapeadas.
        /// </summary>
        public static int GetMappedKeyCount() => FormsToKeyCode.Count;

        /// <summary>
        /// Verifica si una tecla está mapeada.
        /// </summary>
        public static bool IsMapped(System.Windows.Forms.Keys key) => FormsToKeyCode.ContainsKey(key);
    }
}
