﻿using Melanchall.DryWetMidi.Common;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Melanchall.DryWetMidi.Smf.Interaction
{
    internal static class MusicalTimeParser
    {
        #region Nested types

        internal enum ParsingResult
        {
            Parsed,

            InputStringIsNullOrWhiteSpace,
            NotMatched,
            BarsIsOutOfRange,
            BeatsIsOutOfRange,
            FractionNotMatched,
            FractionNumeratorIsOutOfRange,
            FractionDenominatorIsOutOfRange,
        }

        #endregion

        #region Constants

        private const string BarsGroupName = "B";
        private const string BeatsGroupName = "b";
        private const string FractionGroupName = "f";

        private static readonly string BarsGroup = $@"(?<{BarsGroupName}>\d+)";
        private static readonly string BeatsGroup = $@"(?<{BeatsGroupName}>\d+)";
        private static readonly string FractionGroup = $@"(?<{FractionGroupName}>.+)";

        private static readonly string Divider = Regex.Escape(".");

        private static readonly string[] Patterns = new[]
        {
            $"{BarsGroup}{Divider}{BeatsGroup}{Divider}{FractionGroup}",
            $"{BarsGroup}{Divider}{BeatsGroup}",
            $"{FractionGroup}",
        };

        #endregion

        #region Methods

        internal static ParsingResult TryParse(string input, out MusicalTime time)
        {
            time = null;

            if (string.IsNullOrWhiteSpace(input))
                return ParsingResult.InputStringIsNullOrWhiteSpace;

            input = input.Trim();

            var match = Patterns.Select(p => Regex.Match(input, $"^{p}$")).FirstOrDefault(m => m.Success);
            if (match == null)
                return ParsingResult.NotMatched;

            // Parse bars, beats

            if (!ParseBarsBeats(match, BarsGroupName, out var bars))
                return ParsingResult.BarsIsOutOfRange;

            if (!ParseBarsBeats(match, BeatsGroupName, out var beats))
                return ParsingResult.BeatsIsOutOfRange;

            // Parse fraction

            var fraction = Fraction.ZeroFraction;
            var fractionGroup = match.Groups[FractionGroupName];
            if (fractionGroup.Success)
            {
                switch (FractionParser.TryParse(fractionGroup.Value, out fraction))
                {
                    case FractionParser.ParsingResult.NotMatched:
                        return ParsingResult.FractionNotMatched;

                    case FractionParser.ParsingResult.NumeratorIsOutOfRange:
                        return ParsingResult.FractionNumeratorIsOutOfRange;

                    case FractionParser.ParsingResult.DenominatorIsOutOfRange:
                        return ParsingResult.FractionDenominatorIsOutOfRange;
                }
            }

            // Succesfully parsed

            time = new MusicalTime(bars, beats, fraction);
            return ParsingResult.Parsed;
        }

        internal static Exception GetException(ParsingResult parsingResult, string inputStringParameterName)
        {
            switch (parsingResult)
            {
                case ParsingResult.InputStringIsNullOrWhiteSpace:
                    return new ArgumentException("Input string is null or contains white-spaces only.", inputStringParameterName);

                case ParsingResult.NotMatched:
                    return new FormatException("Input string has invalid musical time format.");

                case ParsingResult.BarsIsOutOfRange:
                    return new FormatException("Bars number is out of range.");

                case ParsingResult.BeatsIsOutOfRange:
                    return new FormatException("Beats number is out of range.");

                case ParsingResult.FractionNotMatched:
                    return new FormatException("Input string has invalid fraction format.");

                case ParsingResult.FractionNumeratorIsOutOfRange:
                    return new FormatException("Fraction's numerator is out of range.");

                case ParsingResult.FractionDenominatorIsOutOfRange:
                    return new FormatException("Fraction's denominator is out of range.");
            }

            return null;
        }

        private static bool ParseBarsBeats(Match match, string groupName, out int value)
        {
            value = 0;

            var group = match.Groups[groupName];
            return !group.Success || int.TryParse(group.Value, out value);
        }

        #endregion
    }
}