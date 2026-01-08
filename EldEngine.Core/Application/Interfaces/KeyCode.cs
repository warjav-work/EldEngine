namespace EldEngine.Core.Application.Interfaces
{
    /// <summary>
    /// Enum completo de códigos de tecla basado en Windows Forms Keys enum
    /// Soporta 150+ teclas incluyendo letras, números, función, modificadores, etc.
    /// </summary>
    [Flags]
    public enum KeyCode
    {
        // ==================== ESPECIALES ====================
        None = 0,

        // ==================== MODIFICADORES ====================
        LeftShift = 160,      // Shift izquierda
        RightShift = 161,     // Shift derecha
        LeftControl = 162,    // Control izquierda
        RightControl = 163,   // Control derecha
        LeftAlt = 164,        // Alt izquierda
        RightAlt = 165,       // Alt derecha
        LeftWin = 91,         // Windows izquierda
        RightWin = 92,        // Windows derecha

        // Para compatibilidad (sin distinción L/R):
        Shift = 16,
        Control = 17,
        Alt = 18,

        // ==================== LETRAS A-Z ====================
        A = 65, B = 66, C = 67, D = 68, E = 69,
        F = 70, G = 71, H = 72, I = 73, J = 74,
        K = 75, L = 76, M = 77, N = 78, O = 79,
        P = 80, Q = 81, R = 82, S = 83, T = 84,
        U = 85, V = 86, W = 87, X = 88, Y = 89, Z = 90,

        // ==================== NÚMEROS ====================
        D0 = 48, D1 = 49, D2 = 50, D3 = 51, D4 = 52,
        D5 = 53, D6 = 54, D7 = 55, D8 = 56, D9 = 57,

        // ==================== TECLADO NUMÉRICO ====================
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,       // * (teclado numérico)
        Add = 107,            // + (teclado numérico)
        Separator = 108,      // Separator (teclado numérico)
        Subtract = 109,       // - (teclado numérico)
        Decimal = 110,        // . (teclado numérico)
        Divide = 111,         // / (teclado numérico)

        // ==================== NAVEGACIÓN ====================
        Home = 36,
        End = 35,
        PageUp = 33,
        PageDown = 34,
        Up = 38,
        Down = 40,
        Left = 37,
        Right = 39,
        Insert = 45,
        Delete = 46,

        // ==================== ACCIONES ====================
        Space = 32,           // Barra espaciadora
        Enter = 13,           // Intro/Retorno
        Escape = 27,          // Escape
        Tab = 9,              // Tabulación
        Back = 8,             // Retroceso/Backspace
        Clear = 12,           // Borrar
        Return = 13,          // Retorno (igual a Enter)

        // ==================== TECLAS DE FUNCIÓN ====================
        F1 = 112, F2 = 113, F3 = 114, F4 = 115,
        F5 = 116, F6 = 117, F7 = 118, F8 = 119,
        F9 = 120, F10 = 121, F11 = 122, F12 = 123,
        F13 = 124, F14 = 125, F15 = 126, F16 = 127,
        F17 = 128, F18 = 129, F19 = 130, F20 = 131,
        F21 = 132, F22 = 133, F23 = 134, F24 = 135,

        // ==================== BLOQUEOS ====================
        CapsLock = 20,        // Bloq Mayús
        NumLock = 144,        // Bloq Num
        Scroll = 145,         // Bloq Despl

        // ==================== MULTIMEDIA ====================
        VolumeUp = 175,       // Subir volumen
        VolumeDown = 174,     // Bajar volumen
        VolumeMute = 173,     // Silenciar volumen
        MediaPlayPause = 179, // Reproducir/Pausa
        MediaStop = 178,      // Detener
        MediaPreviousTrack = 177, // Pista anterior
        MediaNextTrack = 176, // Pista siguiente
        SelectMedia = 181,    // Seleccionar medio
        LaunchMail = 180,     // Abrir correo
        LaunchApplication1 = 182, // Abrir aplicación 1
        LaunchApplication2 = 183, // Abrir aplicación 2
        BrowserHome = 172,    // Inicio navegador
        BrowserBack = 166,    // Atrás navegador
        BrowserForward = 167, // Adelante navegador
        BrowserRefresh = 168, // Actualizar navegador
        BrowserStop = 169,    // Detener navegador
        BrowserSearch = 170,  // Buscar navegador
        BrowserFavorites = 171, // Favoritos navegador

        // ==================== ESPECIALES OEM ====================
        Oem1 = 186,           // ;: (Oem1)
        Oemplus = 187,        // =+ (OemPlus)
        Oemcomma = 188,       // ,< (OemComma)
        OemMinus = 189,       // -_ (OemMinus)
        OemPeriod = 190,      // .> (OemPeriod)
        Oem2 = 191,           // /? (Oem2)
        Oem3 = 192,           // `~ (Oem3)
        Oem4 = 219,           // [{ (Oem4)
        Oem5 = 220,           // \| (Oem5)
        Oem6 = 221,           // ]} (Oem6)
        OemQuotes = 222,      // '" (OemQuotes)
        Oem8 = 223,           // (Oem8)
        OemBackslash = 226,   // \| (OemBackslash)

        // ==================== SISTEMA ====================
        Apps = 93,            // Tecla de aplicación/menú contexto
        PrintScreen = 44,     // Imprimir pantalla
        Pause = 19,           // Pausa
        Print = 42,           // Print
        Help = 47,            // Ayuda
        Execute = 43,         // Ejecutar

        // ==================== MOUSE ====================
        LButton = 1,          // Botón izquierdo mouse
        RButton = 2,          // Botón derecho mouse
        MButton = 4,          // Botón central mouse
        XButton1 = 5,         // Botón X1 mouse
        XButton2 = 6,         // Botón X2 mouse

        // ==================== IME (Input Method Editor) ====================
        IMEConvert = 28,
        IMENonconvert = 29,
        IMEAccept = 30,
        IMEModeChange = 31,

        // ==================== OTROS ====================
        Sleep = 95,           // Modo sueño
        Zoom = 251,           // Zoom
        Play = 250,           // Play
        Select = 41,          // Select
        Cancel = 3,           // Cancel
        ProcessKey = 229,     // Process Key
        LineFeed = 10,        // Line Feed
        Attn = 246,           // ATTN
        Crsel = 247,          // CRSEL
        Exsel = 248,          // EXSEL
        EraseEof = 249,       // ERASE EOF
        Pa1 = 253,            // PA1
        OemClear = 254,       // OEM Clear
    }
}
