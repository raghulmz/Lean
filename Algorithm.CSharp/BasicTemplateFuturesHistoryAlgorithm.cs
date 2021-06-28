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
 *
*/

using System;
using System.Linq;
using QuantConnect.Data;
using QuantConnect.Orders;
using QuantConnect.Securities;
using QuantConnect.Data.UniverseSelection;
using QuantConnect.Interfaces;
using System.Collections.Generic;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// This example demonstrates how to get access to futures history for a given root symbol.
    /// It also shows how you can prefilter contracts easily based on expirations, and inspect the futures
    /// chain to pick a specific contract to trade.
    /// </summary>
    /// <meta name="tag" content="using data" />
    /// <meta name="tag" content="history and warm up" />
    /// <meta name="tag" content="history" />
    /// <meta name="tag" content="futures" />
    public class BasicTemplateFuturesHistoryAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        // S&P 500 EMini futures
        private string [] roots = new []
        {
            Futures.Indices.SP500EMini,
            Futures.Metals.Gold,
        };

        private int _successCount = 0;

        public override void Initialize()
        {
            SetStartDate(2013, 10, 8);
            SetEndDate(2013, 10, 9);
            SetCash(1000000);

            foreach (var root in roots)
            {
                // set our expiry filter for this futures chain
                AddFuture(root, Resolution.Minute).SetFilter(TimeSpan.Zero, TimeSpan.FromDays(182));
            }

            SetBenchmark(d => 1000000);

            Schedule.On(DateRules.EveryDay(), TimeRules.Every(TimeSpan.FromHours(1)), MakeHistoryCall);
        }

        private void MakeHistoryCall()
        {
            var history = History(10, Resolution.Minute);
            if (history.Count() < 10)
            {
                throw new Exception($"Empty history at {Time}");
            }
            _successCount++;
        }

        public override void OnEndOfAlgorithm()
        {
            if (_successCount < 49)
            {
                throw new Exception($"Scheduled Event did not assert history call as many times as expected: {_successCount}/49");
            }
        }

        /// <summary>
        /// Event - v3.0 DATA EVENT HANDLER: (Pattern) Basic template for user to override for receiving all subscription data in a single event
        /// </summary>
        /// <param name="slice">The current slice of data keyed by symbol string</param>
        public override void OnData(Slice slice)
        {
            if (!Portfolio.Invested)
            {
                foreach (var chain in slice.FutureChains)
                {
                    foreach (var contract in chain.Value)
                    {
                        Log($"{contract.Symbol.Value}," +
                            $"Bid={contract.BidPrice.ToStringInvariant()} " +
                            $"Ask={contract.AskPrice.ToStringInvariant()} " +
                            $"Last={contract.LastPrice.ToStringInvariant()} " +
                            $"OI={contract.OpenInterest.ToStringInvariant()}"
                        );
                    }
                }
            }
        }
        public override void OnSecuritiesChanged(SecurityChanges changes)
        {
            foreach (var change in changes.AddedSecurities)
            {
                var history = History(change.Symbol, 10, Resolution.Minute);

                foreach (var data in history.OrderByDescending(x => x.Time).Take(3))
                {
                    Log("History: " + data.Symbol.Value + ": " + data.Time + " > " + data.Close);
                }
            }
        }

        /// <summary>
        /// Order fill event handler. On an order fill update the resulting information is passed to this method.
        /// </summary>
        /// <param name="orderEvent">Order event details containing details of the evemts</param>
        /// <remarks>This method can be called asynchronously and so should only be used by seasoned C# experts. Ensure you use proper locks on thread-unsafe objects</remarks>
        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            Log(orderEvent.ToString());
        }

        /// <summary>
        /// This is used by the regression test system to indicate if the open source Lean repository has the required data to run this algorithm.
        /// </summary>
        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp, Language.Python };

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Trades", "0"},
            {"Average Win", "0%"},
            {"Average Loss", "0%"},
            {"Compounding Annual Return", "0%"},
            {"Drawdown", "0%"},
            {"Expectancy", "0"},
            {"Net Profit", "0%"},
            {"Sharpe Ratio", "0"},
            {"Probabilistic Sharpe Ratio", "0%"},
            {"Loss Rate", "0%"},
            {"Win Rate", "0%"},
            {"Profit-Loss Ratio", "0"},
            {"Alpha", "0"},
            {"Beta", "0"},
            {"Annual Standard Deviation", "0"},
            {"Annual Variance", "0"},
            {"Information Ratio", "0"},
            {"Tracking Error", "0"},
            {"Treynor Ratio", "0"},
            {"Total Fees", "$0.00"},
            {"Estimated Strategy Capacity", "$0"},
            {"Lowest Capacity Asset", ""},
            {"Fitness Score", "0"},
            {"Kelly Criterion Estimate", "0"},
            {"Kelly Criterion Probability Value", "0"},
            {"Sortino Ratio", "79228162514264337593543950335"},
            {"Return Over Maximum Drawdown", "79228162514264337593543950335"},
            {"Portfolio Turnover", "0"},
            {"Total Insights Generated", "0"},
            {"Total Insights Closed", "0"},
            {"Total Insights Analysis Completed", "0"},
            {"Long Insight Count", "0"},
            {"Short Insight Count", "0"},
            {"Long/Short Ratio", "100%"},
            {"Estimated Monthly Alpha Value", "$0"},
            {"Total Accumulated Estimated Alpha Value", "$0"},
            {"Mean Population Estimated Insight Value", "$0"},
            {"Mean Population Direction", "0%"},
            {"Mean Population Magnitude", "0%"},
            {"Rolling Averaged Population Direction", "0%"},
            {"Rolling Averaged Population Magnitude", "0%"},
            {"OrderListHash", "d41d8cd98f00b204e9800998ecf8427e"}
        };
    }
}
