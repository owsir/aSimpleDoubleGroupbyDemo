using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGroupBy
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            var flights = new List<Flight>
            {
                new Flight{FlightA = 1, FlightB = 1,Price = 200},
                new Flight{FlightA = 1, FlightB = 2,Price = 200},
                new Flight{FlightA = 1, FlightB = 3,Price = 200},
                new Flight{FlightA = 1, FlightB = 4,Price = 200},
                new Flight{FlightA = 2, FlightB = 1,Price = 200},
                new Flight{FlightA = 2, FlightB = 2,Price = 200},
                new Flight{FlightA = 2, FlightB = 3,Price = 200},
                new Flight{FlightA = 2, FlightB = 4,Price = 200},
            };

            var groupStep1 = from flight in flights
                group flight by new {flight.Price, flight.FlightA}
                into g
                select new {key = g.Key, inboundGroup = g};

            var flightStepOneGroup= groupStep1.Select(item => new FlightCombineStepOne
            {
                Price = item.key.Price,
                FlightA = item.key.FlightA,
                FlightBGroup = item.inboundGroup.Select(x => x.FlightB).OrderBy(x => x).ToList(),
                FlightBGroupKey = string.Join(",", item.inboundGroup.Select(x => x.FlightB).OrderBy(x => x).ToList())
            }).ToList();

            var groupStep2 = from flight in flightStepOneGroup
                group flight by new { flight.Price, flight.FlightBGroupKey }
                into g
                select new { key = g.Key, outboundGroup = g };

            var flightStepTwoGroup = groupStep2.Select(item => new FlightCombineStepTwo
            {
                Price = item.key.Price,
                FlightAGroup = item.outboundGroup.Select(x => x.FlightA).OrderBy(x => x).ToList(),
                FlightBGroup = item.key.FlightBGroupKey.Split(',').Select(x=>Convert.ToInt32(x)).ToList()
            }).ToList();

            foreach (var item in flightStepTwoGroup)
            {
                Console.WriteLine(string.Join(",", item.FlightAGroup));
                Console.WriteLine(string.Join(",", item.FlightBGroup));
                Console.WriteLine(item.Price);
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }


    public class Flight
    {
        public int FlightA { get; set; }
        public int FlightB { get; set; }
        public int Price { get; set; }
    }

    public class FlightCombineStepOne
    {
        public int FlightA { get; set; }
        public List<int> FlightBGroup { get; set; }
        public string FlightBGroupKey { get; set; }
        public int Price { get; set; }
    }

    public class FlightCombineStepTwo
    {
        public List<int> FlightAGroup { get; set; }
        public List<int> FlightBGroup { get; set; }
        public int Price { get; set; }
    }
}
