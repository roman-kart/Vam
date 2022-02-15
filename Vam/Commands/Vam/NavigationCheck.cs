using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vam.Commands
{
    public class NavigationCheck
    {
        public static bool IsCursorInBuffer(int leftDifference = 0, int topDifference = 0)
        {
            // если каким-либо образом выходит за границы буфера консоли
            if (
                Console.CursorLeft + leftDifference < 0 ||
                Console.CursorLeft + leftDifference >= Console.BufferWidth ||
                Console.CursorTop + topDifference < 0 ||
                Console.CursorTop + topDifference >= Console.BufferHeight
                )
            {
                return false;
            }
            return true;
        }
    }
}
