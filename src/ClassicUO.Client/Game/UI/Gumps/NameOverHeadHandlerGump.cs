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

using System;
using ClassicUO.Configuration;
using ClassicUO.Game.Managers;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Resources;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Gumps
{
    internal class NameOverHeadHandlerGump : Gump
    {
        public static Point? LastPosition;

        public override GumpType GumpType => GumpType.NameOverHeadHandler;

        private StbTextBox searchBox;

        public NameOverHeadHandlerGump() : base(0, 0)
        {
            CanMove = true;
            AcceptMouseInput = true;
            CanCloseWithRightClick = true;

            if (LastPosition == null)
            {
                X = 100;
                Y = 100;
            }
            else
            {
                X = LastPosition.Value.X;
                Y = LastPosition.Value.Y;
            }

            WantUpdateSize = false;

            LayerOrder = UILayer.Over;

            Checkbox stayActive;
            RadioButton all, mobiles, items, mobilesCorpses;
            AlphaBlendControl alpha;

            Add
            (
                alpha = new AlphaBlendControl(0.7f)
                {
                    Hue = 34
                }
            );

            Add
            (
                stayActive = new Checkbox
                (
                    0x00D2,
                    0x00D3,
                    "Stay active",
                    color: 0xFFFF
                )
                {
                    IsChecked = ProfileManager.CurrentProfile.NameOverheadToggled,
                }
            );
            stayActive.ValueChanged += (sender, e) => { ProfileManager.CurrentProfile.NameOverheadToggled = stayActive.IsChecked; CanCloseWithRightClick = false; };


            Checkbox hideFullHp;
            Add
            (
                hideFullHp = new Checkbox
                (
                    0x00D2,
                    0x00D3,
                    color: 0xFFFF
                )
                {
                    IsChecked = ProfileManager.CurrentProfile.NamePlateHideAtFullHealth,
                    X = stayActive.Width + stayActive.X + 5
                }
            );
            hideFullHp.SetTooltip("Hide nameplates above 100% health.");
            hideFullHp.ValueChanged += (sender, e) => { ProfileManager.CurrentProfile.NamePlateHideAtFullHealth = hideFullHp.IsChecked; };


            Checkbox hideInWarmode;
            Add
            (
                hideInWarmode = new Checkbox
                (
                    0x00D2,
                    0x00D3,
                    color: 0xFFFF
                )
                {
                    IsChecked = ProfileManager.CurrentProfile.NamePlateHideAtFullHealthInWarmode,
                    X = hideFullHp.Width + hideFullHp.X + 5
                }
            );
            hideInWarmode.SetTooltip("Only hide 100% hp nameplates in warmode.");
            hideInWarmode.ValueChanged += (sender, e) => { ProfileManager.CurrentProfile.NamePlateHideAtFullHealthInWarmode = hideInWarmode.IsChecked; };



            Add
            (
                all = new RadioButton
                (
                    0,
                    0x00D0,
                    0x00D1,
                    ResGumps.All,
                    color: 0xFFFF
                )
                {
                    Y = stayActive.Height + stayActive.Y,
                    IsChecked = NameOverHeadManager.TypeAllowed == NameOverheadTypeAllowed.All
                }
            );

            Add
            (
                mobiles = new RadioButton
                (
                    0,
                    0x00D0,
                    0x00D1,
                    ResGumps.MobilesOnly,
                    color: 0xFFFF
                )
                {
                    Y = all.Y + all.Height,
                    IsChecked = NameOverHeadManager.TypeAllowed == NameOverheadTypeAllowed.Mobiles
                }
            );

            Add
            (
                items = new RadioButton
                (
                    0,
                    0x00D0,
                    0x00D1,
                    ResGumps.ItemsOnly,
                    color: 0xFFFF
                )
                {
                    Y = mobiles.Y + mobiles.Height,
                    IsChecked = NameOverHeadManager.TypeAllowed == NameOverheadTypeAllowed.Items
                }
            );

            Add
            (
                mobilesCorpses = new RadioButton
                (
                    0,
                    0x00D0,
                    0x00D1,
                    ResGumps.MobilesAndCorpsesOnly,
                    color: 0xFFFF
                )
                {
                    Y = items.Y + items.Height,
                    IsChecked = NameOverHeadManager.TypeAllowed == NameOverheadTypeAllowed.MobilesCorpses
                }
            );

            Add(new AlphaBlendControl() { Y = mobilesCorpses.Height + mobilesCorpses.Y, Width = 150, Height = 20, Hue = 0x0481 });
            Add(searchBox = new StbTextBox(0, -1, 150, hue: 0xFFFF) { Y = mobilesCorpses.Height + mobilesCorpses.Y, Width = 150, Height = 20 });
            searchBox.Text = NameOverHeadManager.Search;
            searchBox.TextChanged += (s, e) => { NameOverHeadManager.Search = searchBox.Text; };

            alpha.Width = Math.Max(mobilesCorpses.Width, Math.Max(items.Width, Math.Max(all.Width, mobiles.Width)));
            alpha.Height = stayActive.Height + all.Height + mobiles.Height + items.Height + mobilesCorpses.Height + searchBox.Height;

            Width = alpha.Width;
            Height = alpha.Height;

            all.ValueChanged += (sender, e) =>
            {
                if (all.IsChecked)
                {
                    NameOverHeadManager.TypeAllowed = NameOverheadTypeAllowed.All;
                }
            };

            mobiles.ValueChanged += (sender, e) =>
            {
                if (mobiles.IsChecked)
                {
                    NameOverHeadManager.TypeAllowed = NameOverheadTypeAllowed.Mobiles;
                }
            };

            items.ValueChanged += (sender, e) =>
            {
                if (items.IsChecked)
                {
                    NameOverHeadManager.TypeAllowed = NameOverheadTypeAllowed.Items;
                }
            };

            mobilesCorpses.ValueChanged += (sender, e) =>
            {
                if (mobilesCorpses.IsChecked)
                {
                    NameOverHeadManager.TypeAllowed = NameOverheadTypeAllowed.MobilesCorpses;
                }
            };
        }

        public override void Dispose()
        {
            NameOverHeadManager.Search = "";
            base.Dispose();
        }

        protected override void OnDragEnd(int x, int y)
        {
            LastPosition = new Point(ScreenCoordinateX, ScreenCoordinateY);

            SetInScreen();

            base.OnDragEnd(x, y);
        }
    }
}
