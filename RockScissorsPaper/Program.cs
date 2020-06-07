using System;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Xml;

namespace RockScissorsPaper
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                args = new string[] { "Rock", "Scissors", "Paper" };
            }

            int rngMove, playerInput;
            byte[] secretkey = new byte[32];
            string[] hmac;
            int plScore = 0, pcScore = 0;

            try
            {
                while (true)
                {
                    if (args.Length < 3 || inputValidation(args) || args.Length % 2 == 0)
                        throw new IOException();
                    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(secretkey);
                    }

                    Random rngNum = new Random();
                    rngMove = rngNum.Next(1, args.Length + 1);
                    hmac = ComputeHMAC(args[rngMove - 1], secretkey);

                    Console.WriteLine($"HMAC: {hmac[0]}\n");

                    while (true)
                    {
                        Console.WriteLine("Available moves:");
                        for (int i = 0; i < args.Length; i++)
                        {
                            Console.WriteLine($"{i + 1} - {args[i]}");
                        }

                        Console.Write("Enter your move: ");
                        int.TryParse(Console.ReadLine(), out playerInput);

                        if (playerInput > 0 && playerInput < args.Length + 1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine($"Your move: {args[playerInput - 1]}");
                    Console.WriteLine($"Computer move: {args[rngMove - 1]}");
                    if (rngMove == 1 && playerInput == args.Length)
                    {
                        Console.WriteLine("You win!");
                        plScore++;
                    }
                    else if (playerInput > rngMove || playerInput == 1 && rngMove == args.Length)
                    {
                        Console.WriteLine("You loose!");
                        pcScore++;
                    }
                    else if (playerInput < rngMove)
                    {
                        Console.WriteLine("You win!");
                        plScore++;
                    }
                    else
                        Console.WriteLine("Draw!");



                    Console.WriteLine();
                    Console.WriteLine($"HMAC key: {hmac[1]}\n");
                    Console.WriteLine($"Score: Player - {plScore}, Computer: - {pcScore}");
                    Console.Write("Play again? y\\n  ");
                    string key = Console.ReadLine();
                    if (key.ToLower() != "y" && key.Length != 0)
                        break;
                    Console.WriteLine("\n");
                }

            }
            catch (IOException e)
            {
                Console.WriteLine("Wrong arguments", e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        static string[] ComputeHMAC(string str, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return new string[] { builder.ToString(), BitConverter.ToString(hmac.Key).Replace("-", "") };
            }
        }

        static bool inputValidation(string[] args)
        {
            bool result = false;

            for (int i = 0; i < args.Length - 1; i++)
            {
                int j = i + 1;
                while (j < args.Length)
                {
                    if (args[i].ToLower() == args[j].ToLower())
                    {
                        result = true;
                        break;
                    }
                    else
                        j++;
                }
            }
            return result;
        }
    }

}
