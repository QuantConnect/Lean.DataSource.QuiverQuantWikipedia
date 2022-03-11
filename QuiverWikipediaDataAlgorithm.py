# QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
# Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

from AlgorithmImports import *
from QuantConnect.DataSource import *

### <summary>
### Quiver Quantitative is a provider of alternative data.
### This algorithm shows how to consume the 'QuiverWikipedia'
### </summary>
class QuiverWikipediaDataAlgorithm(QCAlgorithm):
    def Initialize(self):
        self.SetStartDate(2021, 10, 7)
        self.SetEndDate(2021, 10, 11)
        self.SetCash(100000)

        symbol = self.AddEquity("SPY", Resolution.Daily).Symbol
        wikiSymbol = self.AddData(QuiverWikipedia, symbol).Symbol
        history = self.History(QuiverWikipedia, wikiSymbol, 60, Resolution.Daily)

        self.Debug(f"We got {len(history)} items from our history request");

    def OnData(self, data):
        points = data.Get(QuiverWikipedia)
        for point in points.Values:
            if point.WeekPercentChange != None and point.WeekPercentChange > 5:
                self.SetHoldings(point.Symbol.Underlying, 1)

            else:
                self.Liquidate(point.Symbol.Underlying)
