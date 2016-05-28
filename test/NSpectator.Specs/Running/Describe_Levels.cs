﻿#region [R# naming]
// ReSharper disable ArrangeTypeModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming
#endregion
using NUnit.Framework;

namespace NSpectator.Specs.Running
{
    public class Describe_Levels : When_running_specs
    {
        class Describe_numbers : Spec
        {
            void method_level_context()
            {
                It["1 is 1"] = () => 1.Expected().ToBe(1);

                Context["except in crazy world"] = () =>
                {
                    It["1 is 2"] = () => 1.Expected().ToBe(2);
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            Run(typeof(Describe_numbers));
        }

        [Test]
        public void classes_that_directly_inherit_nspec_have_level_1()
        {
            TheContext("describe numbers").Level.Expected().ToBe(1);
        }

        [Test]
        public void method_level_contexts_have_one_level_deeper()
        {
            TheContext("method level context").Level.Expected().ToBe(2);
        }

        [Test]
        public void and_nested_contexts_one_more_deep()
        {
            TheContext("except in crazy world").Level.Expected().ToBe(3);
        }
    }
}