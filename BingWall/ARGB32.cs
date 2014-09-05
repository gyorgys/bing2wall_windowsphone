using System;
using System.Runtime.InteropServices;

namespace BingWall
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ARGB32
    {
        [FieldOffset(0)]
        public byte B;

        [FieldOffset(1)]
        public byte G;

        [FieldOffset(2)]
        public byte R;

        [FieldOffset(3)]
        public byte A;

        [FieldOffset(0)]
        public int argb32;

        public ARGB32(byte r, byte g, byte b)
        {
            this.argb32 = 0;
            this.A = 255;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public ARGB32(byte r, byte g, byte b, byte a)
        {
            this.argb32 = 0;
            this.A = a;

            if (this.A == 255)
            {
                this.R = r;
                this.G = g;
                this.B = b;
            }
            else
            {
                float ratio = this.A / 255f;
                this.R = (byte)(r * ratio);
                this.G = (byte)(g * ratio);
                this.B = (byte)(b * ratio);
            }
        }

        public static explicit operator Int32(ARGB32 argb32)
        {
            return argb32.argb32;
        }

        public static explicit operator ARGB32(Int32 int32)
        {
            ARGB32 retVal = new ARGB32();
            retVal.argb32 = int32;
            return retVal;
        }

        public ARGB32 GetOpaque()
        {
            if (this.A == 255) return this;

            float ratio = 255f / this.A;
            return new ARGB32((byte)(this.R * ratio), (byte)(this.G * ratio), (byte)(this.B * ratio));
        }

        public enum Component
        {
            Blue = 0,
            Green = 1,
            Red = 2,
            Alpha = 3
        }

        public byte this[int component]
        {
            get
            {
                switch (component)
                {
                    case (int)Component.Blue:
                        return this.B;
                    case (int)Component.Red:
                        return this.R;
                    case (int)Component.Green:
                        return this.G;
                    case (int)Component.Alpha:
                        return this.A;
                }
                throw new ArgumentException("Invalid component", "component");
            }
            set
            {
                switch (component)
                {
                    case (int)Component.Blue:
                        this.B = value; break;
                    case (int)Component.Red:
                        this.R = value; break;
                    case (int)Component.Green:
                        this.G = value; break;
                    case (int)Component.Alpha:
                        this.A = value; break;
                    default:
                        throw new ArgumentException("Invalid component", "component");
                }

            }

        }

        public byte GetValue()
        {
            return (byte)Math.Max(R, Math.Max(G, B));
        }

        public byte GetBrightness()
        {
            return (byte)(Math.Min(R, Math.Min(G, B)) + Math.Max(R, Math.Max(G, B)) / 2);
        }

        public byte GetLuma()
        {
            return (byte)((7471 * B + 38470 * G + 19595 * R) >> 16);
        }

    }

}
