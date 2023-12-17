using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IShapeContract
{
    public static class CreateRightPointsForDraw
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        /// <summary>
        /// Đưa điểm đầu và điểm cuối về dạng đúng cho việc vẽ hình vuông, hình elipse,...
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void Change(ref Point start, ref Point end)
        {
            if (end.Y < start.Y && end.X > start.X)
            {
                var temp = start;
                start.Y = end.Y;
                end.Y = temp.Y;
            }
            else if (end.X <= start.X && end.Y <= start.Y)
            {
                Swap(ref start, ref end);
            }
            else if (end.X < start.X && end.Y > start.Y)
            {
                var temp = start;
                start.X = end.X;
                end.X = temp.X;
            }
        }
    }
}
