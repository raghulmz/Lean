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

using System.Collections.Generic;
using QuantConnect.Algorithm.Framework.Portfolio;
using QuantConnect.Data;
using QuantConnect.Interfaces;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// Regression algorithm testing GH feature 3790, using SetHoldings with a collection of targets
    /// which will be ordered by margin impact before being executed, with the objective of avoiding any
    /// margin errors
    /// </summary>
    public class SetHoldingsMultipleTargetsRegressionAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        private Symbol _spy;
        private Symbol _ibm;

        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        public override void Initialize()
        {
            SetStartDate(2013, 10, 07);
            SetEndDate(2013, 10, 11);

            // use leverage 1 so we test the margin impact ordering
            _spy = AddEquity("SPY", Resolution.Minute, Market.USA, false, 1).Symbol;
            _ibm = AddEquity("IBM", Resolution.Minute, Market.USA, false, 1).Symbol;
        }

        /// <summary>
        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// </summary>
        /// <param name="data">Slice object keyed by symbol containing the stock data</param>
        public override void OnData(Slice data)
        {
            if (!Portfolio.Invested)
            {
                SetHoldings(new List<PortfolioTarget> { new PortfolioTarget(_spy, 0.8m), new PortfolioTarget(_ibm, 0.2m) });
            }
            else
            {
                SetHoldings(new List<PortfolioTarget> { new PortfolioTarget(_ibm, 0.8m), new PortfolioTarget(_spy, 0.2m) });
            }
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
            {"Total Trades", "9"},
            {"Average Win", "0%"},
            {"Average Loss", "-0.02%"},
            {"Compounding Annual Return", "355.022%"},
            {"Drawdown", "2.300%"},
            {"Expectancy", "-1"},
            {"Net Profit", "1.956%"},
            {"Sharpe Ratio", "11.814"},
            {"Probabilistic Sharpe Ratio", "65.598%"},
            {"Loss Rate", "100%"},
            {"Win Rate", "0%"},
            {"Profit-Loss Ratio", "0"},
            {"Alpha", "0.966"},
            {"Beta", "0.994"},
            {"Annual Standard Deviation", "0.249"},
            {"Annual Variance", "0.062"},
            {"Information Ratio", "8.399"},
            {"Tracking Error", "0.114"},
            {"Treynor Ratio", "2.955"},
            {"Total Fees", "$13.02"},
            {"Estimated Strategy Capacity", "$2900000.00"},
            {"Lowest Capacity Asset", "IBM R735QTJ8XC9X"},
            {"Fitness Score", "0.55"},
            {"Kelly Criterion Estimate", "0"},
            {"Kelly Criterion Probability Value", "0"},
            {"Sortino Ratio", "18.793"},
            {"Return Over Maximum Drawdown", "125.928"},
            {"Portfolio Turnover", "0.552"},
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
            {"OrderListHash", "172c86753e33aca918e877479a05c211"}
        };
    }
}
