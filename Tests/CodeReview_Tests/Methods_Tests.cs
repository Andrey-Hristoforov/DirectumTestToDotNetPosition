using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CodeReview;

namespace Tests.CodeReview_Tests
{
    [TestFixture]
    internal class Methods_Tests
    {
        private static readonly object[] TestCases =
        {
            new int[]{100, -100, 100, 101 },
            new int[]{100, -100, 100, -101 },
            new int[]{100000, -100, 100, 0 },
            new int[]{100, -100, 100, 1 },
            new int[]{100, -100, 100, -1 },
        };

        [Test, TestCaseSource("TestCases")]
        public void Func1_LongNotSortedRandomArr_EqualToSorted(int arrSize, int bottomBound, int topBound, int newKey)
        {
            var randomNotSortedArr = GenerateListOfPairs(arrSize, bottomBound, topBound).ToArray();
            var expectedReslut = randomNotSortedArr
                .Append(new KeyValuePair<int, string>(newKey, "new pair"))
                .OrderBy(e => e.Key)
                .ToArray();

            CodeReview.Methods.Func1(ref randomNotSortedArr, newKey, "new pair");

            bool equivalence = randomNotSortedArr
                .Select((elem, i) => elem.ToString() == expectedReslut[i].ToString())
                .All(res => res == true);
            Assert.True(equivalence, "Sequences were not equal");
        }


        [Test, TestCaseSource("TestCases")]
        public void SafetyRefactored_LongNotSortedRandomArr_EqualToSorted(int arrSize, int bottomBound, int topBound, int newKey)
        {
            var randomNotSortedArr = GenerateListOfPairs(arrSize, bottomBound, topBound).ToArray();
            var expectedReslut = randomNotSortedArr
                .Append(new KeyValuePair<int, string>(newKey, "new pair"))
                .OrderBy(e => e.Key)
                .ToArray();

            CodeReview.Methods.SafetyRefactored(ref randomNotSortedArr, newKey, "new pair");

            bool equivalence = randomNotSortedArr
                .Select((elem, i) => elem.ToString() == expectedReslut[i].ToString())
                .All(res => res == true);
            Assert.True(equivalence, "Sequences were not equal");
        }


        [Test, TestCaseSource("TestCases")]
        public void AddToSortedArray_LongSortedRandomArr_EqualToSorted(int arrSize, int bottomBound, int topBound, int newKey)
        {
            var randomSortedArr = GenerateListOfPairs(arrSize, bottomBound, topBound)
                .OrderBy(e => e.Key)
                .ToArray();
            var expectedReslut = randomSortedArr
                .Append(new KeyValuePair<int, string>(newKey, "new pair"))
                .OrderBy(e => e.Key)
                .ToArray();

            CodeReview.Methods.AddToSortedArray(ref randomSortedArr, new KeyValuePair<int, string>(newKey, "new pair"));

            bool equivalence = randomSortedArr
                .Select((elem, i) => elem.ToString() == expectedReslut[i].ToString())
                .All(res => res == true);
            Assert.True(equivalence, "Sequences were not equal");
        }


        [Test, TestCaseSource("TestCases")]
        public void AddToSortedAndReturn_LongSortedRandomArr_EqualToSorted(int arrSize, int bottomBound, int topBound, int newKey)
        {
            var randomSortedArr = GenerateListOfPairs(arrSize, bottomBound, topBound)
                .OrderBy(e => e.Key);
            var expectedReslut = randomSortedArr
                .Append(new KeyValuePair<int, string>(newKey, "new pair"))
                .OrderBy(e => e.Key)
                .ToArray();

            bool equivalence = randomSortedArr
                .AddToSortedAndReturn(new KeyValuePair<int, string>(newKey, "new pair")) // Act is here
                .Select((elem, i) => elem.ToString() == expectedReslut[i].ToString())
                .All(res => res == true);
            Assert.True(equivalence, "Sequences were not equal");
        }

        private static List<KeyValuePair<int, string>> GenerateListOfPairs(int size, int bottom, int top)
        {
            Random rnd = new Random();
            var result = new List<KeyValuePair<int, string>>(size);
            for (int i = 0; i < size; i++)
            {
                int randValue = rnd.Next(bottom, top);
                result.Add(new KeyValuePair<int, string>(randValue, randValue.ToString()));
            }
            return result;
        }
    }
}
