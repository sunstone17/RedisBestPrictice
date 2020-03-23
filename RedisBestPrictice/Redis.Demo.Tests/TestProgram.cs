using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Demo.Tests
{
    class TestProgram
    {
        [SetUp]
        public static void Main(string[] args)
        {
            Demo.Program.Main(args);

            //new Locks().TestLockV2();
            //Console.Read();
        }
    }
}
