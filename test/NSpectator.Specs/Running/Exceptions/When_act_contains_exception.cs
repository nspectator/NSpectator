﻿#region [R# naming]
// ReSharper disable ArrangeTypeModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming
#endregion
using NSpectator.Domain;
using NUnit.Framework;
using FluentAssertions;

namespace NSpectator.Specs.Running.Exceptions
{
    [TestFixture]
    [Category("RunningSpecs")]
    public class When_act_contains_exception : When_running_specs
    {
        private class SpecClass : Spec
        {
            void method_level_context()
            {
                act = () => { throw new ActException(); };

                it["should fail this example because of act"] = () => "1".Should().Be("1");

                it["should also fail this example because of act"] = () => "1".Should().Be("1");

                it["overrides exception from same level it"] = () => { throw new ItException(); };

                context["exception thrown by both act and nested before"] = () =>
                {
                    before = () => { throw new BeforeException(); };

                    it["preserves exception from nested before"] = () => "1".Should().Be("1");
                };

                context["exception thrown by both act and nested act"] = () =>
                {
                    act = () => { throw new NestedActException(); };

                    it["overrides exception from nested act"] = () => "1".Should().Be("1");
                };

                context["exception thrown by both act and nested it"] = () =>
                {
                    it["overrides exception from nested it"] = () => { throw new ItException(); };
                };

                context["exception thrown by both act and nested after"] = () =>
                {
                    it["overrides exception from nested after"] = () => "1".Should().Be("1");

                    after = () => { throw new AfterException(); };
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            Run(typeof(SpecClass));
        }

        [Test]
        public void the_example_level_failure_should_indicate_a_context_failure()
        {
            TheExample("should fail this example because of act")
                .Exception.Expect<ExampleFailureException>();
            TheExample("should also fail this example because of act")
                .Exception.Expect<ExampleFailureException>();
            TheExample("overrides exception from same level it")
                .Exception.Expect<ExampleFailureException>();
            TheExample("preserves exception from nested before")
                .Exception.Expect<ExampleFailureException>();
            TheExample("overrides exception from nested act")
                .Exception.Expect<ExampleFailureException>();
            TheExample("overrides exception from nested it")
                .Exception.Expect<ExampleFailureException>();
            TheExample("overrides exception from nested after")
                .Exception.Expect<ExampleFailureException>();
        }

        [Test]
        public void examples_with_only_act_failure_should_fail_because_of_act()
        {
            TheExample("should fail this example because of act").Exception
                .InnerException.Expect<ActException>();
            TheExample("should also fail this example because of act").Exception
                .InnerException.Expect<ActException>();
        }

        [Test]
        public void it_should_throw_exception_from_act_not_from_same_level_it()
        {
            TheExample("overrides exception from same level it")
                .Exception.InnerException.Expect<ActException>();
        }

        [Test]
        public void it_should_throw_exception_from_nested_before_not_from_act()
        {
            TheExample("preserves exception from nested before")
                .Exception.InnerException.Expect<BeforeException>();
        }

        [Test]
        public void it_should_throw_exception_from_act_not_from_nested_act()
        {
            TheExample("overrides exception from nested act")
                .Exception.InnerException.Expect<ActException>();
        }

        [Test]
        public void it_should_throw_exception_from_act_not_from_nested_it()
        {
            TheExample("overrides exception from nested it")
                .Exception.InnerException.Expect<ActException>();
        }

        [Test]
        public void it_should_throw_exception_from_act_not_from_nested_after()
        {
            TheExample("overrides exception from nested after")
                .Exception.InnerException.Expect<ActException>();
        }
    }
}
