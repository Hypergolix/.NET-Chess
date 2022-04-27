using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DChessCore
{
    class HelperMethods
    {
        public static Vector2u Clamp(Vector2i values, uint maxWidth, uint maxHeight)
        {
            Vector2u newValues;
            if (values.X < 0) newValues.X = 0;

            else if (values.X > maxWidth) newValues.X = maxWidth;

            else newValues.X = (uint)values.X;

            if (values.Y < 0) newValues.Y = 0;

            else if (values.Y > maxHeight) newValues.Y = maxHeight;

            else newValues.Y = (uint)values.Y;

            return newValues;
        }
    }
}
