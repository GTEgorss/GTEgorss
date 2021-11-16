using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
using Banks.Tools;

namespace Banks.Controllers
{
    public class UICentralBank
    {
        private static CentralBank _centralBank;

        public UICentralBank()
        {
            _centralBank = CentralBank.GetInstance();
        }

        public static void CentralBankMenu()
        {
            while (true)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine("Central Bank:");
                Console.WriteLine("0 - Create bank");
                Console.WriteLine("1 - Show banks");
                Console.WriteLine("2 - Go to Bank");
                Console.WriteLine("3 - Do daily action");
                Console.WriteLine("4 - Do monthly action");
                Console.WriteLine("5 - Transfer money");
                Console.WriteLine("6 - exit");
                Action(Convert.ToInt32(Console.ReadLine()));
            }
        }

        public static void Action(int command)
        {
            switch (command)
            {
                case 0:
                    BasicBankBuilder bankBuilder = new BasicBankBuilder();

                    try
                    {
                        Console.WriteLine("Set debit interest (>0, %):");
                        bankBuilder.SetDebitInterest(Convert.ToDecimal(Console.ReadLine()));

                        Console.WriteLine(
                            "Set deposit interest (range from to, interest; default; >0, %) (type / to stop):");
                        string line = Console.ReadLine();
                        List<InterestRange> interestRanges = new List<InterestRange>();
                        while (line != "/")
                        {
                            Console.WriteLine("Type from(>=0): ");
                            line = Console.ReadLine();
                            if (line == "/") break;
                            decimal from = Convert.ToDecimal(line);
                            Console.WriteLine("Type to(>=0): ");
                            line = Console.ReadLine();
                            if (line == "/") break;
                            decimal to = Convert.ToDecimal(line);
                            Console.WriteLine("Type interest(>=0): ");
                            line = Console.ReadLine();
                            if (line == "/") break;
                            decimal interest = Convert.ToDecimal(line);
                            interestRanges.Add(new InterestRange(from, to, interest));
                        }

                        Console.WriteLine("Type default interest: ");
                        decimal defaultInterest = Convert.ToDecimal(Console.ReadLine());
                        bankBuilder.SetDepositInterest(interestRanges, defaultInterest);

                        Console.WriteLine("Set deposit days till expiry(>=0, int):");
                        bankBuilder.SetDepositDaysTillExpiry(Convert.ToUInt32(Console.ReadLine()));

                        Console.WriteLine("Set credit commission(>=0,)");
                        bankBuilder.SetCreditCommission(Convert.ToDecimal(Console.ReadLine()));

                        Console.WriteLine("Set credit limit(<=0)");
                        bankBuilder.SetCreditLimit(Convert.ToDecimal(Console.ReadLine()));

                        Console.WriteLine("Set transfer limit(>=0)");
                        bankBuilder.SetTransferLimit(Convert.ToDecimal(Console.ReadLine()));
                        _centralBank.CreateBank(bankBuilder.GetBank());
                    }
                    catch (BanksException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }

                    break;

                case 1:
                    Console.WriteLine("Banks:");
                    Console.WriteLine(
                        "ID   DebitInterest   DepositInterest    CreditCommission  CreditLimit DepositDaysTillExpiry  TransferLimit");
                    _centralBank.Banks.ToList().ForEach(b =>
                    {
                        Console.WriteLine(b.BankId.Id + " " + b.DebitInterest + " " + b.DepositInterest.ToString() +
                                          " " + b.CreditCommission + " " + b.CreditLimit + " " +
                                          b.DepositDaysTillExpiry + " " + b.TransferLimit);
                    });
                    break;
                case 2:
                    Console.WriteLine("Enter ID:");
                    uint id = Convert.ToUInt32(Console.ReadLine());

                    try
                    {
                        Bank bank = _centralBank.GetBank(new BankId(id));
                        UIBank uiBank = new UIBank(bank);
                        uiBank.BankMenu();
                    }
                    catch (BanksException e)
                    {
                        Console.WriteLine(e);
                        Console.ReadLine();
                    }

                    break;
                case 3:
                    _centralBank.DoDailyActions();
                    break;
                case 4:
                    _centralBank.DoMonthlyActions();
                    break;
                case 5:
                    Console.WriteLine("Enter ID of sender:");
                    uint fromUserBank = Convert.ToUInt32(Console.ReadLine());
                    uint fromUserClient = Convert.ToUInt32(Console.ReadLine());
                    uint fromUserAccount = Convert.ToUInt32(Console.ReadLine());
                    Console.WriteLine("Enter ID of receiver:");
                    uint toUserBank = Convert.ToUInt32(Console.ReadLine());
                    uint toUserClient = Convert.ToUInt32(Console.ReadLine());
                    uint toUserAccount = Convert.ToUInt32(Console.ReadLine());
                    Console.WriteLine("Enter money:");
                    decimal money = Convert.ToDecimal(Console.ReadLine());
                    try
                    {
                        _centralBank.TransferMoney(new AccountId(fromUserBank, fromUserClient, fromUserAccount), money, new AccountId(toUserBank, toUserClient, toUserAccount));
                    }
                    catch (BanksException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }

                    break;
                case 6:
                    Environment.Exit(0);
                    break;
                default:
                    Console.ReadLine();
                    break;
            }
        }
    }
}