#region license

// Copyright (c) 2021, andreakarasho
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 3. All advertising materials mentioning features or use of this software
//    must display the following acknowledgement:
//    This product includes software developed by andreakarasho - https://github.com/andreakarasho
// 4. Neither the name of the copyright holder nor the
//    names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS ''AS IS'' AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using SDL2;
using ClassicUO.Network;
using System.Timers;
using System;
using ClassicUO.Game;

using Timer = System.Timers.Timer;
using ClassicUO.Game.Scenes;
using ClassicUO.Game.GameObjects;

namespace ClassicUO.Input
{
    internal static class Keyboard
    {
        private static SDL.SDL_Keycode _code;

        public static SDL.SDL_Keymod IgnoreKeyMod { get; } = SDL.SDL_Keymod.KMOD_CAPS | SDL.SDL_Keymod.KMOD_NUM | SDL.SDL_Keymod.KMOD_MODE | SDL.SDL_Keymod.KMOD_RESERVED;

        public static bool Alt { get; private set; }
        public static bool Shift { get; private set; }
        public static bool Ctrl { get; private set; }

        public static int KeyCount { get; private set; }
        public static bool Return { get; private set; }
        public static bool Whisper { get; private set; }
        public static bool Command { get; private set; }

        public static bool PartyKey { get; private set; }


        public static bool Backspace { get; private set; }

        //public static bool IsKeyPressed(SDL.SDL_Keycode code)
        //{
        //    return code != SDL.SDL_Keycode.SDLK_UNKNOWN && _code == code;
        //}

        //public static bool IsModPressed(SDL.SDL_Keymod mod, SDL.SDL_Keymod tocheck)
        //{
        //    mod ^= mod & IgnoreKeyMod;

        //    return tocheck == mod || mod != SDL.SDL_Keymod.KMOD_NONE && (mod & tocheck) != 0;
        //}

        private static Timer messageCooldownTimer = new Timer(5000); // 5 seconds

        private static bool canSendMessage = true;
        public static void OnKeyUp(SDL.SDL_KeyboardEvent e)
        {

            SDL.SDL_Keymod mod = e.keysym.mod & ~IgnoreKeyMod;

            if ((mod & (SDL.SDL_Keymod.KMOD_RALT | SDL.SDL_Keymod.KMOD_LCTRL)) == (SDL.SDL_Keymod.KMOD_RALT | SDL.SDL_Keymod.KMOD_LCTRL))
            {
                e.keysym.sym = SDL.SDL_Keycode.SDLK_UNKNOWN;
                e.keysym.mod = SDL.SDL_Keymod.KMOD_NONE;
            }

            Shift = (e.keysym.mod & SDL.SDL_Keymod.KMOD_SHIFT) != SDL.SDL_Keymod.KMOD_NONE;
            Alt = (e.keysym.mod & SDL.SDL_Keymod.KMOD_ALT) != SDL.SDL_Keymod.KMOD_NONE;
            Ctrl = (e.keysym.mod & SDL.SDL_Keymod.KMOD_CTRL) != SDL.SDL_Keymod.KMOD_NONE;

            _code = SDL.SDL_Keycode.SDLK_UNKNOWN;
        }

        public static void OnKeyDown(SDL.SDL_KeyboardEvent e)
        {
            SDL.SDL_Keymod mod = e.keysym.mod & ~IgnoreKeyMod;

            if ((mod & (SDL.SDL_Keymod.KMOD_RALT | SDL.SDL_Keymod.KMOD_LCTRL)) == (SDL.SDL_Keymod.KMOD_RALT | SDL.SDL_Keymod.KMOD_LCTRL))
            {
                e.keysym.sym = SDL.SDL_Keycode.SDLK_UNKNOWN;
                e.keysym.mod = SDL.SDL_Keymod.KMOD_NONE;
            }

            Shift = (e.keysym.mod & SDL.SDL_Keymod.KMOD_SHIFT) != SDL.SDL_Keymod.KMOD_NONE;
            Alt = (e.keysym.mod & SDL.SDL_Keymod.KMOD_ALT) != SDL.SDL_Keymod.KMOD_NONE;
            Ctrl = (e.keysym.mod & SDL.SDL_Keymod.KMOD_CTRL) != SDL.SDL_Keymod.KMOD_NONE;
            Return = e.keysym.sym == SDL.SDL_Keycode.SDLK_RETURN;
            Whisper = e.keysym.sym == SDL.SDL_Keycode.SDLK_SEMICOLON;
            Command = e.keysym.sym == SDL.SDL_Keycode.SDLK_KP_PERIOD;
            Backspace = e.keysym.sym == SDL.SDL_Keycode.SDLK_BACKSPACE;
            PartyKey = e.keysym.sym == SDL.SDL_Keycode.SDLK_SLASH;

            LoginScene ls = Client.Game.GetScene<LoginScene>();

            if (!Ctrl && !Alt && !Return)
            {
                if (ls != null || Command || PartyKey)
                {
                    return;
                }

                if ((World.Player.IsHidden))
                    return;

                if (Whisper)
                {
                    KeyCount = -200;
                    return;
                }

                if (Backspace)
                {
                    KeyCount = 0;
                    return;
                }

                switch (e.keysym.sym)
                {
                    case SDL.SDL_Keycode.SDLK_a:
                    case SDL.SDL_Keycode.SDLK_b:
                    case SDL.SDL_Keycode.SDLK_c:
                    case SDL.SDL_Keycode.SDLK_d:
                    case SDL.SDL_Keycode.SDLK_e:
                    case SDL.SDL_Keycode.SDLK_f:
                    case SDL.SDL_Keycode.SDLK_g:
                    case SDL.SDL_Keycode.SDLK_h:
                    case SDL.SDL_Keycode.SDLK_i:
                    case SDL.SDL_Keycode.SDLK_j:
                    case SDL.SDL_Keycode.SDLK_k:
                    case SDL.SDL_Keycode.SDLK_l:
                    case SDL.SDL_Keycode.SDLK_m:
                    case SDL.SDL_Keycode.SDLK_n:
                    case SDL.SDL_Keycode.SDLK_o:
                    case SDL.SDL_Keycode.SDLK_p:
                    case SDL.SDL_Keycode.SDLK_q:
                    case SDL.SDL_Keycode.SDLK_r:
                    case SDL.SDL_Keycode.SDLK_s:
                    case SDL.SDL_Keycode.SDLK_t:
                    case SDL.SDL_Keycode.SDLK_u:
                    case SDL.SDL_Keycode.SDLK_v:
                    case SDL.SDL_Keycode.SDLK_w:
                    case SDL.SDL_Keycode.SDLK_x:
                    case SDL.SDL_Keycode.SDLK_y:
                    case SDL.SDL_Keycode.SDLK_z:

                        if (ls == null)
                        {
                            KeyCount++;
                        }
                        break;
                }

                if (KeyCount >= 10) //Minimum amount of characters the player can type to start showing the typing indicator
                {
                    KeyCount = 0;
                    messageCooldownTimer.Elapsed += (sender, e) =>
                    {
                        canSendMessage = true; // Allow the user to send another message after the cooldown
                        messageCooldownTimer.Stop(); // Stop the timer
                    };

                    if (canSendMessage)
                    {

                        GameActions.Say(" . . . ", 0, Game.Data.MessageType.New);

                        canSendMessage = false; // Prevent the user from sending another message
                        messageCooldownTimer.Start(); // Start the cooldown timer
                    }

                }

            }

            if (Return)
            {
                KeyCount = 0;
                messageCooldownTimer.Start();
            }

            if (e.keysym.sym != SDL.SDL_Keycode.SDLK_UNKNOWN)
            {
                _code = e.keysym.sym;
            }
        }
    }
}