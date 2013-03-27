﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceptionTest
{
    public static class Program
    {
        public static void Main(string[] args) {
            if (args.Length == 0 || args[0] != "noloop")
                StandaloneExceptionLoop();

            // first chance
            try {
                throw new ArgumentException();
            }
            catch (Exception ex) {
                // do nothing
            }

            // second chance
            //throw new ArgumentException();
        }

        private static void StandaloneExceptionLoop() {
            Console.WriteLine("Press any key to get an exception.");
            while (true) {
                Console.ReadKey();
                // first chance
                try {
                    throw new ArgumentException();
                }
                catch (Exception) {
                    // do nothing
                }

                Console.WriteLine("  Exception thrown.");
            }
        }
    }
}
