#region [R# naming]
// ReSharper disable ArrangeTypeModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using NSpectator;
using NSpectator.Domain;
using NUnit.Framework;

using DescribeOtherNamespace;
using DescribeSomeNamespace;
using Moq;
using FluentAssertions;

namespace NSpectator.Specs
{
    public class SpecClass : Spec
    {
        public void public_method() { }

        private void private_method() { }
    }

    public class AnotherSpecClass : Spec
    {
        void public_method() { }
    }

    public class NonSpecClass { }

    public class SpecClassWithNoVoidMethods : Spec
    {
        string parameter_less_method()
        {
            return string.Empty;
        }
    }

    public class SpecClassWithNoParameterLessMethods : Spec
    {
        void private_method(string parameter) { }

        public void public_method(string parameter) { }
    }

    [TestFixture]
    [Category("SpecFinder")]
    public class Without_filtering : When_finding_specs
    {
        [SetUp]
        public void Setup()
        {
            GivenDllContains();
        }

        [Test]
        public void Should_get_types_from_reflection()
        {
            reflectorMock.Verify(r => r.GetTypesFrom());
        }

        [Test]
        public void Should_include_classes_that_implement_nspec_and_have_paramterless_void_methods()
        {
            GivenDllContains(typeof(SpecClass));

            finder.SpecClasses().Should().Contain(typeof(SpecClass));
        }

        [Test]
        public void Should_exclude_classes_that_inherit_from_nspec_but_have_no_parameterless_methods()
        {
            GivenDllContains(typeof(SpecClassWithNoParameterLessMethods));

            finder.SpecClasses().Should().BeEmpty();
        }

        [Test]
        public void Should_exclude_classes_that_do_not_inherit_from_nspec()
        {
            GivenDllContains(typeof(NonSpecClass));

            finder.SpecClasses().Should().BeEmpty();
        }

        [Test]
        public void Should_exclude_classes_that_have_no_void_methods()
        {
            GivenDllContains(typeof(SpecClassWithNoVoidMethods));

            finder.SpecClasses().Should().BeEmpty();
        }
    }

    [TestFixture]
    [Category("SpecFinder")]
    public class When_filtering_specs : When_finding_specs
    {
        [Test]
        public void Should_filter_in()
        {
            GivenDllContains(typeof(AnotherSpecClass));

            GivenFilter(typeof(AnotherSpecClass).Name);

            finder.SpecClasses().Should().Contain(typeof(AnotherSpecClass));
        }

        [Test]
        public void Should_filter_out()
        {
            GivenDllContains(typeof(SpecClass));

            GivenFilter(typeof(AnotherSpecClass).Name);

            finder.SpecClasses().Should().BeEmpty();
        }
    }

    [TestFixture]
    [Category("SpecFinder")]
    public class When_finding_specs_based_on_regex : When_finding_specs
    {
        [SetUp]
        public void Setup()
        {
            GivenDllContains(typeof(SomeClass),
                typeof(SomeDerivedClass),
                typeof(SomeClassInOtherNameSpace),
                typeof(SomeDerivedDerivedClass));
        }

        [Test]
        public void Should_find_all_specs_if_regex_is_not_specified()
        {
            GivenFilter("");

            TheSpecClasses()
                .Should().Contain(typeof(SomeClass)).And
                .Contain(typeof(SomeDerivedClass)).And
                .Contain(typeof(SomeClassInOtherNameSpace));
        }

        [Test]
        public void Should_find_specs_for_derived_class_and_include_base_class()
        {
            GivenFilter("DerivedClass$");

            TheSpecClasses()
                .Should().Contain(typeof(SomeClass)).And
                .Contain(typeof(SomeDerivedClass)).And
                .Contain(typeof(SomeDerivedDerivedClass)).And
                .NotContain(typeof(SomeClassInOtherNameSpace));

            TheSpecClasses().Should().HaveCount(3);
        }

        [Test]
        public void Should_find_specs_that_contain_namespace()
        {
            GivenFilter("DescribeSomeNamespace");

            TheSpecClasses()
                .Should().Contain(typeof(SomeClass)).And
                .Contain(typeof(SomeDerivedClass)).And
                .NotContain(typeof(SomeClassInOtherNameSpace));
        }

        [Test]
        public void Should_find_distinct_specs()
        {
            GivenFilter("Derived");

            TheSpecClasses()
                .Should().Contain(typeof(SomeClass)).And
                .Contain(typeof(SomeDerivedClass)).And
                .Contain(typeof(SomeDerivedDerivedClass));

            TheSpecClasses().Should().HaveCount(3);
        }
    }

    public class When_finding_specs
    {
        protected void GivenDllContains(params Type[] types)
        {
            reflectorMock = new Mock<IReflector>();
            reflectorMock.Setup(r => r.GetTypesFrom()).Returns(types);

            someDLL = "some specs project dll";

            finder = new SpecFinder(reflectorMock.Object);
        }

        protected void GivenFilter(string filter)
        {
            finder = new SpecFinder(reflectorMock.Object, filter);
        }
        
        protected IEnumerable<Type> TheSpecClasses()
        {
            return finder.SpecClasses();
        }

        protected ISpecFinder finder;
        
        protected Mock<IReflector> reflectorMock;
        protected string someDLL;
    }
}

namespace DescribeSomeNamespace
{
    class SomeClass : Spec
    {
        void context_method()
        {
            
        }
    }

    class SomeDerivedClass : SomeClass
    {
        void context_method()
        {

        }
    }

    class SomeDerivedDerivedClass : SomeClass
    {
        void context_method()
        {

        }
    }
}

namespace DescribeOtherNamespace
{
    class SomeClassInOtherNameSpace : Spec
    {
        void context_method()
        {

        }
    }
}