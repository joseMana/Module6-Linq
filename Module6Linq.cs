using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace ScratchPad
{
    #region more code
    public enum SalaryPayFrequency
    {
        Monthly,
        BiMonthly
    }
    public enum WagePayFrequency
    {
        Hourly,
        Daily,
        Weekly
    }
    public readonly struct Salary
    {
        private readonly decimal _salaryValue;
        private readonly SalaryPayFrequency _payFrequency;
        public Salary(decimal salaryValue)
            : this(salaryValue, SalaryPayFrequency.BiMonthly)
        {
        }
        public Salary(decimal salaryValue, SalaryPayFrequency payFrequency)
        {
            _salaryValue = salaryValue;
            _payFrequency = payFrequency;
        }
        public Salary SetSalaryValue(decimal salaryValue)
        {
            return new Salary(salaryValue, this._payFrequency);
        }
        public Salary SetPayFrequency(SalaryPayFrequency payFrequency)
        {
            return new Salary(this._salaryValue, payFrequency);
        }
        public decimal GetSalaryValue() => _salaryValue;
        public SalaryPayFrequency GetSalarySchedule() => _payFrequency;
    }
    public readonly struct Wage
    {
        private readonly decimal _payRate;
        private readonly WagePayFrequency _payFrequency;
        public Wage(decimal payRate, WagePayFrequency payFrequency)
        {
            _payRate = payRate;
            _payFrequency = payFrequency;
        }
        public Wage SetRate(decimal rate)
        {
            return new Wage(rate, this._payFrequency);
        }
        public Wage SetPayFrequency(WagePayFrequency payFrequency)
        {
            return new Wage(this._payRate, payFrequency);
        }
        public decimal GetPayRate() => _payRate;
        public WagePayFrequency GetPayFrequency() => _payFrequency;
    }
    public abstract class Employee
    {
        public int EmployeeId { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public Employee()
        {
        }
        ~Employee() { }
        public abstract void Work();
    }
    public abstract class Consultant : Employee
    {
        public string ProjectName { get; set; }
        public List<string> MastersEngagement { get; set; }
        public override void Work()
        {
            Console.WriteLine($"Working for {ProjectName}");
        }
    }
    public class EmployeeDecorator : Employee
    {
        Employee _employee = null;
        public EmployeeDecorator() { }
        public EmployeeDecorator(Employee employee)
        {
            this._employee = employee;
        }
        public virtual void Payout()
        {
            Console.WriteLine($"EmployeeId: {_employee.EmployeeId}\nEmployee type: {base.GetType().Name}");
        }
        public void SetEmployee(Employee employee)
        {
            this._employee = employee;
        }

        public override void Work()
        {
            _employee.Work();
        }
    }
    public class Developer : Consultant
    {
        public static int Id;
        public string ProgrammingLanguage { get; set; }
        private Developer()
        {

        }
        public override void Work()
        {
            base.Work();
            Console.WriteLine($"Writing code using {ProgrammingLanguage}\n");
        }
        public static Developer Create()//factory method
        {
            return new Developer();
        }
    }
    public class QualityEngineer : Consultant
    {
        public string TestingTool { get; set; }

        public override void Work()
        {
            base.Work();
            Console.WriteLine($"Doing tests using {TestingTool}\n");
        }
    }
    public class ContractualEmployee : EmployeeDecorator
    {
        private Wage _wage;
        public ContractualEmployee() { }
        public ContractualEmployee(Employee employee) : base(employee)
        {
        }
        public ContractualEmployee SetPayRate(decimal rate)
        {
            _wage = _wage.SetRate(rate);
            return this;
        }
        public ContractualEmployee SetPayFrequency(WagePayFrequency payFrequency)
        {
            _wage = _wage.SetPayFrequency(payFrequency);
            return this;
        }
        public override void Payout()
        {
            base.Payout();
            Console.WriteLine(
                $"{_wage.GetPayFrequency().GetType().Name} Rate : {_wage.GetPayFrequency()}\n" +
                $"Rate : {_wage.GetPayRate()}\n");
        }
    }
    public class PermanentEmployee : EmployeeDecorator
    {
        private Salary _salary = new Salary();
        public PermanentEmployee() { }
        public PermanentEmployee(Employee employee) : base(employee) { }
        public PermanentEmployee SetSalaryValue(decimal salaryValue)
        {
            _salary = _salary.SetSalaryValue(salaryValue);
            return this;
        }
        public PermanentEmployee SetPayFrequency(SalaryPayFrequency payFrequency)
        {
            _salary = _salary.SetPayFrequency(payFrequency);
            return this;
        }
        public override void Payout()
        {
            base.Payout();
            Console.WriteLine(
                $"Salary Schedule: {_salary.GetSalaryValue()}\n" +
                $"Salary Value: {_salary.GetSalarySchedule()}");
        }
    }
    public static class ConsultantHelper
    {
        public static void IntroduceConsultant(this Consultant c)
        {
            Console.WriteLine($"\tI'm {c.FirstName} {c.LastName}\n\tEngaged in project {c.ProjectName}\n\tI'm {c.GetType().Name}\n\n");
        }
    }
    public static class PrimitiveTypeHelper
    {
        public static void Write(this string c)
        {
            Console.WriteLine(c);
        }
        public static void Write(this int c)
        {
            Console.WriteLine(c);
        }
    }
    public static class ConsultantDecoratorBuilderExtension
    {
        public static TEmployeeDecorator DecorateAs<TEmployeeDecorator>(this Consultant c, Action<TEmployeeDecorator> typeInitializer) where TEmployeeDecorator : EmployeeDecorator, new()
        {
            var decorator = new TEmployeeDecorator();
            decorator.SetEmployee(c);

            typeInitializer(decorator);
            return decorator;
        }
    }

    //public static class OurOwnEnumebraleImpleemntation
    //{
    //    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    //    {
    //        foreach (var item in source)
    //            if (predicate(item))
    //                yield return item;
    //    }
    //}
    #endregion
    class Program
    {
        static void Main(string[] args)
        {
            #region more code
            Developer dev1 = Developer.Create();
            dev1.FirstName = "John";
            dev1.LastName = "Puruntong";
            dev1.ProjectName = "Fiserv Mainframe Offload";
            dev1.ProgrammingLanguage = "C#";
            dev1.MastersEngagement = new List<string> { "EF Core", "C#", "Xamarin" };
            var permanentDeveloper = dev1.DecorateAs<ContractualEmployee>(c =>
            {
                c.SetPayFrequency(WagePayFrequency.Daily);
                c.SetPayRate(500);
            });

            Developer dev2 = Developer.Create();
            dev2.FirstName = "Kevin";
            dev2.LastName = "Puruntong";
            dev2.ProjectName = "Fiserv Mainframe Offload";
            dev2.ProgrammingLanguage = "C#";
            dev2.MastersEngagement = new List<string> { "EF Core", "C#", "Xamarin" };

            Developer dev3 = Developer.Create();
            dev3.FirstName = "Kevin";
            dev3.LastName = "Puruntong";
            dev3.ProjectName = "BCG";
            dev3.ProgrammingLanguage = "C#";
            dev3.MastersEngagement = new List<string> { "EF Core", "C#", "Xamarin" };


            QualityEngineer qe = new QualityEngineer();
            qe.FirstName = "Billy";
            qe.LastName = "Puruntong";
            qe.ProjectName = "EY";
            qe.TestingTool = "Tricentis Tosca";
            qe.MastersEngagement = new List<string> { "Selenium Framework", "NeoLoad Automation" };
            var contractualQe = qe.DecorateAs<PermanentEmployee>(c =>
            {
                c.SetPayFrequency(SalaryPayFrequency.BiMonthly);
                c.SetSalaryValue(30000);
            });

            QualityEngineer qe2 = new QualityEngineer();
            qe2.FirstName = "Andres";
            qe2.LastName = "Puruntong";
            qe2.ProjectName = "EY";
            qe2.TestingTool = "Tricentis Tosca";
            qe2.MastersEngagement = new List<string> { "Selenium Framework", "NeoLoad Automation" };


            //ConsultantHelper.IntroduceConsultant(dev1);
            //ConsultantHelper.IntroduceConsultant(qe);

            //dev1.IntroduceConsultant();
            //qe.IntroduceConsultant();


            //"qwerty".Write();

            //1.Write();

            #endregion

            var consultants = new List<Consultant> { dev1, dev2, dev3, qe };

            #region filtering
            //using enumerable.where
            var fiservConsultant = from c in consultants
                                   where c.ProjectName.Contains("Fiserv")
                                   select c;

            var fiservConsultant2 = consultants
                                        .Where(c => c.ProjectName.Contains("Fiserv"))
                                        .Where(c => c.ProjectName.Contains("Fiserv"))
                                        .OrderBy(c=>c.ProjectName)
                                        .OrderBy(c=>c.ProjectName)
                                        .OrderBy(c=>c.ProjectName)
                                        .OrderBy(c=>c.ProjectName)
                                        .Where(c => c.ProjectName.Contains("Fiserv"))
                                        .Where(c => c.ProjectName.Contains("Fiserv"))
                                        .Where(c => c.ProjectName.Contains("Fiserv"))
                                        .Where(c => c.ProjectName.Contains("Fiserv"))
                                        .Where(c => c.ProjectName.Contains("Fiserv"))
                                        .Where(c => c.ProjectName.Contains("Fiserv"));

            var fiservConsultant3 = Enumerable.Where(consultants, c => c.ProjectName.Contains("Fiserv"));


            //using enumrable.oftype
            var fiservDevelopers = consultants.Where(c => c.ProjectName.Contains("Fiserv")).OfType<Developer>();
            var qeConsultants = consultants.OfType<QualityEngineer>();
            #endregion

            #region projection

            //using enumerable.select
            var consultantsNameToUpper = from c in consultants
                                         select c.MastersEngagement.ToUpper();
            var consultantsNameToUpper2 = consultants.Select(c => c.FirstName.ToUpper());
            var eyConsultantsNameToUpper3 = from c in consultants
                                            where c.ProjectName == "EY"
                                            select new { Name = c.FirstName.ToUpper() + " " + c.LastName.ToUpper(), c.ProjectName };


            //using selectmany
            var allMastersEngagement = from c in consultants
                                       from me in c.MastersEngagement
                                       select me;
            var allMastersEngagement2 = consultants.Where(c => c.ProjectName.Contains("Fiserv")).SelectMany(c => c.MastersEngagement);


            //using zip
            var myTuple = fiservConsultant.Zip(qeConsultants);

            #endregion

            #region sorting

            //using enumerable.orderby
            var consultantsOrderByFirstName = from c in consultants
                                              orderby c.FirstName
                                              select c;
            
            var consultantsOrderByFirstName2 = consultants.OrderBy(c => c.FirstName);

            //using enumerable.orderby desc

            var consultantsOrderByFirstNameDesc = from c in consultants
                                                  orderby c.FirstName descending
                                                  select c;
            var consultantsOrderByFirstNameDesc2 = consultants.OrderByDescending(c => c.FirstName);


            //using enumerable.orderby multiple
            var consultantsMultipleOrderBySample = from c in consultants
                                                   orderby c.FirstName, c.ProjectName
                                                   select c;
            var consultantsMultipleOrderBySample2 = consultants.OrderBy(c => c.FirstName).ThenBy(c => c.ProjectName);

            //using enumerable.reverse
            var reverseSortedConsultants = consultants.Reverse<Consultant>();
            #endregion

            #region grouping

            //using enumerable.groupby(deferred execution)
            var groupedConsultants = from c in consultants
                                     group c by c.ProjectName;

            //using enumerable.tolookup(immediate execution)
            var groupedConsultants2 = consultants.ToLookup(c => c.ProjectName);




            var enumerator1 = groupedConsultants.GetEnumerator();
            var enumerator2 = groupedConsultants2.GetEnumerator();



            //foreach (var item in groupedConsultants2)
            //{
            //    Console.WriteLine(item.Key);
            //    foreach (var consultant in item)
            //        consultant.IntroduceConsultant();
            //}
            //Console.WriteLine("============================================");

            //foreach (var item in groupedConsultants)
            //{
            //    Console.WriteLine(item.Key);
            //    foreach (var consultant in item)
            //        consultant.IntroduceConsultant();
            //}



            var clientNameAndNumberOfConsultant = from c in consultants
                                                  group c by c.ProjectName into grp
                                                  select new { ClientName = grp.Key, NumberOfConsultants = grp.Count() };


            var clientNameAndNumberOfConsultant2 = from c in consultants
                                                   group c by c.ProjectName into grp
                                                   let count = grp.Count()
                                                   orderby count
                                                   select new { ClientName = grp.Key, NumberOfConsultants = count };

            var clientNameAndNumberOfConsultant3 = consultants
                                                        .GroupBy(c => c.ProjectName)
                                                        .Select(grp => new { g = grp, count = grp.Count() })
                                                        .OrderBy(anon => anon.count)
                                                        .Select(anon => new { ClientName = anon.g.Key, NumberOfConsultants = anon.count });

            var clientNameAndNumberOfConsultant4
                = Enumerable.Select(
                    Enumerable.OrderBy(
                        Enumerable.Select(
                            Enumerable.GroupBy(
                                consultants,
                                c => c.ProjectName
                                ),
                            grp => new { g = grp, count = grp.Count() }
                        ),
                        anon => anon.count
                    ), anon => new { ClientName = anon.g.Key, NumberOfConsultants = anon.count }
                    );


            ///*
            // ClientName     NumberOfConsultants
            // */

            //foreach (var item in clientNameAndNumberOfConsultant4)
            //    Console.WriteLine(item.ClientName + "\t" + item.NumberOfConsultants);

            #endregion

            #region expression

            Func<int, bool> isNumberLessThan18 = qwodnlqkwd;
            Expression<Func<int, bool>> isNumberLessThan18Expression = i => i < 18;

            var parameterExpression = Expression.Parameter(typeof(int), "i");
            var constantExpression = Expression.Constant(18, typeof(int));
            var binaryExpression = Expression.MakeBinary(ExpressionType.LessThan, parameterExpression, constantExpression);
            var lambdaExpression = Expression.Lambda<Func<int, bool>>(binaryExpression, parameterExpression).Compile();

            //console.writeline("1")
            var methodCallExp = Expression.Call(null, typeof(Console).GetMethods().First(), Expression.Constant("1", typeof(string)));
            Expression.Lambda<Action>(methodCallExp).Compile().Invoke();

            #endregion

            Console.ReadLine();
        }

        private static bool qwodnlqkwd(int arg)
        {
            throw new NotImplementedException();
        }
    }
}