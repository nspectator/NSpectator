﻿#region [R# naming]
// ReSharper disable ArrangeTypeModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming
#endregion
using System.Collections.Generic;
using NSpectator;
using FluentAssertions;

namespace SampleSpecs.Bug
{
    public class Grandparents_run_first : Spec
    {
        List<int> ints = null;

        void describe_as_rspec() // describe RSpec do
        {
            before = () => ints = new List<int>(); //  before(:each) { @array = Array.new }

            context["something that works in rspec but not nspec"] = () => //  context "something that works in rspec but not nspec" do
            {
                before = () => ints.Add(1);

                describe["sibling context"] = () => //    context "sibling context" do
                {
                    before = () => ints.Add(1); //      before(:each) { @array << "sibling 1" }

                    specify = () => ints.Count.Expected().ToBe(1); //      it { @array.count.should == 1 }
                }; //    end

                describe["another sibling context"] = () => //    context "another sibling context" do
                {
                    before = () => ints.Add(1); //      before(:each) { @array << "sibling 2" }

                    specify = () => ints.Count.Expected().ToBe(1); //      it { @array.count.should == 1 }
                }; //    end
            }; //  end
        } //end
    }
}