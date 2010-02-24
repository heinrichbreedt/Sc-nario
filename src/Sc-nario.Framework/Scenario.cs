using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace Sc_nario.Framework
{
    [TestFixture]
    public class Scenario
    {
        const BindingFlags flagsForCurrentOnly = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public delegate void given();

        public delegate void and();

        public delegate void when();

        public delegate void cleanup();

        [SetUp]
        [DebuggerStepThrough]
        public virtual void Setup()
        {
            InvokeDelegates(GetType(), "given", "and", "when");
        }

        [DebuggerStepThrough]
        public virtual void InvokeDelegates(Type type, params string[] delegateNames)
        {
            if (type.BaseType != typeof(Scenario))
                InvokeDelegates(type.BaseType, delegateNames);

            var fields = type.GetFields(flagsForCurrentOnly).Where(f => delegateNames.Contains(f.FieldType.Name)).ToList();
            fields.Sort(new DeclarationOrderComparator());

            foreach (var field in fields)
            {
                var value = (Delegate)field.GetValue(this);
                value.Method.Invoke(this, null);
            }
        }

        [TearDown]
        [DebuggerStepThrough]
        public virtual void Reset()
        {
            InvokeDelegates(GetType(), "cleanup");

            const BindingFlags flagsIncludeInherited = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            var type = GetType();
            var fields = type.GetFields(flagsIncludeInherited);

            foreach (var field in fields)
            {
                field.SetValue(this, null);
            }
        }

        public static TU GenerateStub<TU>() where TU : class
        {
            return MockRepository.GenerateStub<TU>();
        }
    }

    public class ThenAttribute : TestAttribute
    {
    }

    public class DeclarationOrderComparator : IComparer<FieldInfo>
    {
        public int Compare(FieldInfo x, FieldInfo y)
        {
            if (x.MetadataToken < y.MetadataToken)
                return -1;

            if (x.MetadataToken > y.MetadataToken)
                return 1;

            return 0;
        }
    }
}
