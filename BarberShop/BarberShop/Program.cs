using System;
using System.Threading;

namespace BarberShop
{
    class Program
    {
        static void Main(string[] args)
        {

            Thread barber = new Thread(BarberShop.Barber);
            barber.Start();
            for (int i = 1; i < 11; i++)
            {
                Thread customer = new Thread(BarberShop.Customer);
                customer.Name = $"Customer {i.ToString()}"; ;
                customer.Start();
            }

            Console.ReadKey();

        }
    }
    static class BarberShop
    {
        static Semaphore barberReady = new Semaphore(0, 1);
        static Semaphore accessWRSeats = new Semaphore(1, 1);
        static Semaphore customerReady = new Semaphore(0, 5);
        static int numberOfFreeWRSeats = 5;

        public static void Barber()
        {
            while (true)
            {
                customerReady.WaitOne();
                accessWRSeats.WaitOne();
                numberOfFreeWRSeats += 1;
                ConsoleHelper.WriteCut();
                barberReady.Release();
                accessWRSeats.Release();
                
            }
        }
        public static void Customer()
        {
            while (true)
            {
                accessWRSeats.WaitOne();
                ConsoleHelper.WriteCustomerComeIn();
                if (numberOfFreeWRSeats > 0)
                {
                    ConsoleHelper.WriteCustomerWait();
                    numberOfFreeWRSeats -= 1;
                    accessWRSeats.Release();
                    customerReady.Release();
                    barberReady.WaitOne();
                    ConsoleHelper.WriteCustomerPay();
                }
                else
                {
                    accessWRSeats.Release();
                    ConsoleHelper.WriteCustomerGone();
                }
            }
        }
        public static class ConsoleHelper
        {
            public static object LockObject = new Object();
            public static void WriteCut()
            {
                lock (LockObject)
                {
                    Console.WriteLine("Barber is cutting hairs.");
                }

            }
            public static void WriteCustomerComeIn()
            {
                lock (LockObject)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} has come in the barber shop.");
                }

            }
            public static void WriteCustomerGone()
            {
                lock (LockObject)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} has left the barber shop.");
                }

            }
            public static void WriteCustomerPay()
            {
                lock (LockObject)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} has paid and left.");
                }

            }
            public static void WriteCustomerWait()
            {
                lock (LockObject)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} is waiting the barber.");
                }

            }
        }
    }
}
