﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Elysium_Diamond.Resource;
using Elysium_Diamond.Network;
using SharpDX;
using SharpDX.Direct3D9;
using Color = SharpDX.Color;

namespace Elysium_Diamond.DirectX {
    public class EngineNpc {
        public enum Direction {
            Up = 1,
            Down = 4,
            Left = 7,
            Right = 10
        }

        public Direction Dir { get; set; }
        public string Name { get; set; }
        public string Legion { get; set; }
        public int Sprite { get; set; }
        public Color Color { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int OffSetY { get; set; }
        public int OffSetX { get; set; }
        public Point Coordinate { get; set; }
        public Size2 Size { get; set; }
        public bool Visible { get; set; }
        public bool Enabled { get; set; }
        public byte Transparency { get; set; }
        public float ShadowAngle { get; set; }
        public Color ShadowColor { get; set; }
        public byte ShadowTransparency { get; set; }
        public SpriteFlags SpriteFlags { get; set; }
        public Rectangle SourceRect { get; set; }

        public int ID { get; set; }
        public int UniqueID { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Type { get; set; }
        public int Elite { get; set; }
        public string Shop { get; set; }
        public string Quest { get; set; }

        public Queue<byte> DirectionQueue { get; set; } = new Queue<byte>();


        public event EventHandler MouseUp, MouseDown, MouseMove, MouseLeave;

        private int anim, animTime, moveTime, step;
        private bool move, mov, click;

        public EngineNpc() {
            Visible = true;
            Enabled = true;
            Size = new Size2(32, 32);
            ShadowAngle = 0.7f;
            ShadowTransparency = 120;
            Transparency = 255;
            ShadowColor = Color.Black;
            SourceRect = new Rectangle(0, 0, Size.Width, Size.Height);
            SpriteFlags = SpriteFlags.AlphaBlend;
            Color = Color.White;
        }

        public EngineNpc(int id, int sprite, int level, string name, string legion, int type, int elite, string shop, string quest) {
            Visible = true;
            Enabled = true;
            Size = new Size2(32, 32);
            ShadowAngle = 0.7f;
            ShadowTransparency = 120;
            Transparency = 255;
            ShadowColor = Color.Black;
            SourceRect = new Rectangle(0, 0, Size.Width, Size.Height);
            SpriteFlags = SpriteFlags.AlphaBlend;
            Color = Color.White;

            ID = id;
            Sprite = sprite;
            Level = level;
            Name = name;
            Legion = legion;
            Type = type;
            Elite = elite;
            Shop = shop;
            Quest = quest;
        }

        public void Draw() {
            if (!Visible) { return; }
            if (Transparency == 0) { return; }
            if (Sprite == 0) { return; }

            MouseButtons();

            ProcessMovement();
            ProcessAnimation();

            EngineCore.SpriteDevice.Begin(SpriteFlags);
            EngineCore.SpriteDevice.Draw(EngineTexture.FindTextureByID(Sprite, EngineTextureType.Sprites), new Color(Color.R, Color.G, Color.B, Transparency), SourceRect, new Vector3(0, 0, 0), new Vector3(PositionX, PositionY, 0));
            EngineCore.SpriteDevice.End();

            EngineFont.DrawText(Name, new Size2(30, 0), new Point(PositionX, PositionY - 5), Color.White, EngineFontStyle.Regular, FontDrawFlags.Center);
            EngineFont.DrawText(Legion, new Size2(30, 0), new Point(PositionX, PositionY - 20), Color.BlueViolet, EngineFontStyle.Bold, FontDrawFlags.Center);
        }

        public void ProcessAnimation() {
            anim = (int)Dir;

            if (mov) {
                if (step == 0) {
                    if (Environment.TickCount >= animTime + 200) {
                        step = 1;
                        animTime = Environment.TickCount;
                    }

                    anim -= 1;
                }
                else {
                    if (Environment.TickCount >= animTime + 200) {
                        step = 0;
                        animTime = Environment.TickCount;
                    }

                    anim += 1;
                }
            }

            SourceRect = new Rectangle(anim * 32, 0, Size.Width, Size.Height);
        }

        public void ProcessMovement() {
            if (DirectionQueue.Count > 0) {
                if (!mov) {
                    Dir = (Direction)DirectionQueue.Dequeue();

                    if (Dir == Direction.Up) {
                        OffSetY = 16;
                    }

                    if (Dir == Direction.Down) {
                        OffSetY = -16;
                    }

                    if (Dir == Direction.Left) {
                        OffSetX = 16;
                    }

                    if (Dir == Direction.Right) {
                        OffSetX = -16;
                    }

                    mov = true;
                }
            }

            if (!mov) { return; }

            if (Environment.TickCount >= this.moveTime + 35) {
                moveTime = Environment.TickCount;

                switch (Dir) {
                    case Direction.Up:
                        PositionY -= 4;
                        OffSetY -= 4;
                        if (OffSetY <= 0) {
                            mov = false;
                            Coordinate = new Point(Coordinate.X, Coordinate.Y - 1);
                        }
                        break;

                    case Direction.Down:
                        PositionY += 4;
                        OffSetY += 4;
                        if (OffSetY >= 0) {
                            mov = false;
                            Coordinate = new Point(Coordinate.X, Coordinate.Y + 1);
                        }

                        break;

                    case Direction.Left:
                        PositionX -= 4;
                        OffSetX -= 4;
                        if (OffSetX <= 0) {
                            mov = false;
                            Coordinate = new Point(Coordinate.X - 1, Coordinate.Y);
                        }
                        break;

                    case Direction.Right:
                        PositionX += 4;
                        OffSetX += 4;
                        if (OffSetX >= 0) {
                            mov = false;
                            Coordinate = new Point(Coordinate.X + 1, Coordinate.Y);
                        }

                        break;
                }
            }
        }

        public void MouseButtons() {
            if (Enabled) {
                if (InsideButton()) {
                    if (!move) {
                        move = true;
                        MouseMove?.Invoke(this, EventArgs.Empty);
                    }

                    if (EngineCore.MouseLeft) {
                        if (!click) {
                            MouseDown?.Invoke(this, EventArgs.Empty);
                            click = true;
                        }
                    }
                    else {
                        if (click) {
                            MouseUp?.Invoke(this, EventArgs.Empty);
                        }

                        click = false;
                    }
                }
                else {
                    if (move) {
                        move = false;
                        MouseLeave?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Verifica se o mouse faz uma intersecção com o controle.
        /// </summary>
        public bool InsideButton() {
            if (!Enabled) { return false; }
            if (!Visible) { return false; }
            if (!Program.GraphicsDisplay.Focused) { return false; }
            if (Program.GraphicsDisplay.WindowState == FormWindowState.Minimized) { return false; }

            if ((EngineCore.MousePosition.X >= PositionX) && (EngineCore.MousePosition.X <= (Size.Width + PositionX))) {
                if ((EngineCore.MousePosition.Y >= PositionY) && (EngineCore.MousePosition.Y <= (PositionY + Size.Height))) { return true; }
            }

            return false;
        }
    }
}
