using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    public static class HelperMethods
    {
        public static (int, int) GetRandomNubmers(int arrayCount1, int arrayCount2)
        {
            var rnd = new Random();
            int randomNumber1 = rnd.Next(0, arrayCount1);
            int randomNumber2 = rnd.Next(0, arrayCount2);
            return (randomNumber1, randomNumber2);
        }
    }
}
