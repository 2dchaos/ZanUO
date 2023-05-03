﻿using ClassicUO.Assets;
using ClassicUO.Configuration;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace ClassicUO.Game.UI.Gumps
{
    internal class CustomToolTip : Gump
    {
        private readonly Item item;
        private Control hoverReference;
        private readonly string prepend;
        private readonly string append;
        private TextBox text;
        private readonly uint hue = 0xFFFF;

        public event FinishedLoadingEvent OnOPLLoaded;

        public CustomToolTip(Item item, int x, int y, Control hoverReference, string prepend = "", string append = "") : base(0, 0)
        {
            this.item = item;
            this.hoverReference = hoverReference;
            this.prepend = prepend;
            this.append = append;
            X = x;
            Y = y;
            if (ProfileManager.CurrentProfile != null)
            {
                if (ProfileManager.CurrentProfile.TooltipTextHue != 0)
                    hue = ProfileManager.CurrentProfile.TooltipTextHue;
            }
            BuildGump();
        }

        public void RemoveHoverReference()
        {
            hoverReference = null;
        }

        private void BuildGump()
        {
            LoadOPLData(0);
        }

        private void LoadOPLData(int attempt)
        {
            if (attempt > 4 || IsDisposed)
                return;
            if (item == null)
            {
                Dispose();
                return;
            }

            Task.Factory.StartNew(() =>
            {
                if (World.OPL.TryGetNameAndData(item.Serial, out string name, out string data))
                {

                    text = new TextBox(
                        TextBox.ConvertHtmlToFontStashSharpCommand(FormatTooltip(name, data)),
                        ProfileManager.CurrentProfile.SelectedToolTipFont,
                        ProfileManager.CurrentProfile.SelectedToolTipFontSize,
                        ProfileManager.CurrentProfile.SelectedToolTipFontSize * 15,
                        (int)hue,
                        align: ProfileManager.CurrentProfile.LeftAlignToolTips ? FontStashSharp.RichText.TextHorizontalAlignment.Left : FontStashSharp.RichText.TextHorizontalAlignment.Center
                        );

                    Height = text.Height;
                    Width = text.Width;
                    OnOPLLoaded?.Invoke();
                }
                else
                {
                    World.OPL.Contains(item.Serial);
                    Task.Delay(1000).Wait();
                    LoadOPLData(attempt++);
                }
            });
        }

        private string FormatTooltip(string name, string data)
        {
            string text =
                prepend +
                "<basefont color=\"yellow\">" +
                name +
                "\n<basefont color=\"#FFFFFF\">" +
                data +
                append;

            return text;
        }

        public override bool Draw(UltimaBatcher2D batcher, int x, int y)
        {
            base.Draw(batcher, x, y);
            if (IsDisposed)
                return false;
            if (hoverReference != null && !hoverReference.MouseIsOver)
            {
                Dispose();
                return false;
            }
            //if (text == null) //Waiting for opl data to load the text tooltip
            //    return true;

            float alpha = 0.7f;

            if (ProfileManager.CurrentProfile != null)
            {
                alpha = ProfileManager.CurrentProfile.TooltipBackgroundOpacity / 100f;
                if (float.IsNaN(alpha))
                {
                    alpha = 0f;
                }
            }

            Vector3 hue_vec = ShaderHueTranslator.GetHueVector(0, false, alpha);

            batcher.Draw
            (
                SolidColorTextureCache.GetTexture(Color.Black),
                new Rectangle
                (
                    x - 4,
                    y - 2,
                    (int)(Width + 8),
                    (int)(Height + 8)
                ),
                hue_vec
            );


            batcher.DrawRectangle
            (
                SolidColorTextureCache.GetTexture(Color.Gray),
                x - 4,
                y - 2,
                (int)(Width + 8),
                (int)(Height + 8),
                hue_vec
            );

            if (text != null)
                text.Draw(batcher, x, y);

            return true;
        }
    }

    public delegate void FinishedLoadingEvent();
}
