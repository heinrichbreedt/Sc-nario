#pragma warning disable 169
// ReSharper disable ConvertToLambdaExpression

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Sc_nario.Framework.Tests
{
    public class OrderOfExcecutionScenarios
    {
        public class CustomBaseScenario : Scenario
        {
            protected static List<string> order = new List<string>();

            given given_from_the_base = () =>
            {
                order.Add("given_from_the_base");
            };
        }

        public class InheritsScenario : CustomBaseScenario
        {
            given given_from_superclass = () =>
            {
                order.Add("given_from_superclass");
            };

            //[Test]
            [Then]
            public void the_execution_order_should_be_correct()
            {
                order.IndexOf("given_from_the_base").ShouldBe(0);
                order.IndexOf("given_from_superclass").ShouldBe(1);
            }
        }
    }
}