using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
    class Program
    {
        static string LogPath = "Log.txt";
        static string InventoryPath = "Inventory.csv";
        static Dictionary<string, Item> Inventory = new Dictionary<string, Item>();
        static void Main(string[] args)
        {
            // Read inventory
            // Format in A,10,5.5
            using (StreamReader sr = new StreamReader(InventoryPath))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(',');
                    var item = new Item()
                    {
                        Name = line[0],
                        Count = int.Parse(line[1]),
                        Price = double.Parse(line[2]),
                    };
                    Inventory.Add(line[0], item);
                }
            }

            while (true)
            {
                Console.WriteLine("输入订单。格式为“物品名称-数量 物品名称2-数量2”");
                Console.WriteLine("Place order. Format is \"Item1-10 Item2-3”");
                var inputStr = Console.ReadLine();
                try
                {
                    var order = ReadOrder(inputStr);
                    PrintOrder(order);

                    Console.WriteLine("确认请按Y,重新输入请按其他键");
                    Console.WriteLine("Press 'Y' to confirm. Press others to skip.");
                    var needSkip = char.ToUpperInvariant(Console.ReadKey().KeyChar) != 'Y';
                    Console.WriteLine();
                    if (needSkip)
                    {
                        Console.WriteLine("Order skipped.");
                        continue;
                    }

                    ApplyOrder(order, inputStr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errored while placing order. Please retry.{Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}");
                }
            }
        }

        /// <summary>
        /// Input will be A-3 B-4
        /// Name and count seperated by dash.
        /// Two different item seperated by space.
        /// Using this to enable Chinese character
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        static List<Item> ReadOrder(string inputStr)
        {
            var result = new List<Item>();
            var inputPairs = inputStr.Split(' ');
            foreach (var inputPair in inputPairs)
            {
                var input = inputPair.Split('-');
                var item = new Item()
                {
                    Name = input[0],
                    Count = int.Parse(input[1]),
                };

                result.Add(item);
            }

            return result;
        }

        static void PrintOrder(List<Item> list)
        {
            Console.WriteLine("请检查购物车");
            Console.WriteLine("Please review your order.");
            double amount = 0;
            foreach(var item in list)
            {
                //Console.WriteLine($"'{item.Count}' 件 '{item.Name}'");
                Console.WriteLine($"'{item.Count}' '{item.Name}'");
                amount += (item.Count * Inventory[item.Name].Price);
            }
            Console.WriteLine($"总价: {amount}");
            Console.WriteLine($"Total amount: {amount}");
        }

        /// <summary>
        /// Apply cart
        /// </summary>
        /// <param name="list"></param>
        /// <param name="inputStr">Using to record log</param>
        static void ApplyOrder(List<Item> list, string inputStr)
        {
            foreach(var item in list)
            {
                Inventory[item.Name].Count -= item.Count;
            }

            using (StreamWriter sw = new StreamWriter(LogPath, true))
            {
                sw.WriteLine(inputStr);
            }

            using (StreamWriter sw = new StreamWriter(InventoryPath))
            {
                foreach(var item in Inventory)
                {
                    sw.WriteLine($"{item.Value.Name},{item.Value.Count},{item.Value.Price}");
                }
            }

            Console.WriteLine("Order applied");
        }
    }

    class Item
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }
    }
}
