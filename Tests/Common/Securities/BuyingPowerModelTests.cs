/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using NUnit.Framework;
using QuantConnect.Securities;

namespace QuantConnect.Tests.Common.Securities
{
    [TestFixture]
    public class BuyingPowerModelTests
    {
											    // Current Order Margin 
        [TestCase(-40,25, -900, 1, -36)]   	    // -1000
        [TestCase(-36, 25, -880, 1, -35)]  	    // -900
        [TestCase(-35, 25, -900,1, -36)]   	    // -875
        [TestCase(-34, 25, -880, 1, -35)]       // -850
        [TestCase(48, 25, 1050, 1,42)]    	    // 1200
        [TestCase(49, 25, 1212, 1,  48)]   	    // 1225
        [TestCase(44, 25, 1200, 1, 48)]    	    // 1100
        [TestCase(45, 25, 1250,1, 50)]    	    // 1125
        [TestCase(80, 25, -1250, 1, -50)]       // 2000
        [TestCase(45.5, 25, 1240, 0.5, 49.5)]   // 1125
        [TestCase(45.75, 25, 1285, 0.25, 51.25)]// 1125
        [TestCase(-40, 25, 1500, 1, 60)]        // -1000
        [TestCase(-40.5, 12.5, 1505, .5, 120)]  // -506.25
        [TestCase(-40.5, 12.5, 1508, .5, 120.5)]// -506.25

        public void OrderAdjustmentCalculation(decimal currentOrderSize, decimal perUnitMargin, decimal targetMargin, decimal lotSize, decimal expectedOrderSize)
        {
            var currentOrderMargin = currentOrderSize * perUnitMargin;

            // Determine the adjustment to get us to our target margin and apply it
            // Use our GetAmountToOrder for determining adjustment to reach the end goal
            var orderAdjustment =
                BuyingPowerModel.GetAmountToOrder(currentOrderMargin, targetMargin, perUnitMargin, lotSize);

            // Apply the change in margin
            var resultMargin = currentOrderMargin - (orderAdjustment * perUnitMargin);

            // Assert after our adjustment we have met our target condition
            Assert.IsTrue(Math.Abs(resultMargin) <= Math.Abs(targetMargin));

            // Verify our adjustment meets our expected order size
            var adjustOrderSize = currentOrderSize - orderAdjustment;
            Assert.AreEqual(expectedOrderSize, adjustOrderSize);
        }
    }
}
